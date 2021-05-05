using AutoMapper;
using FarmHub.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Adyen;
using FarmHub.Application.Services;
using FarmHub.Application.Services.Email;
using FarmHub.Application.Services.Infrastructure;
using FarmHub.Application.Services.Repositories;
using FarmHub.Application.Services.Services;
using FarmHub.Application.Services.Services.Interface;
using FarmHub.Data.Models;
using FarmHub.Domain.Services;
using FarmHub.Domain.Services.Email;
using FarmHub.Domain.Services.Repositories;
using FarmHub.Domain.Services.ShopifyReportGenerator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Environment = Adyen.Model.Enum.Environment;

namespace FarmHub.API
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CatalogConnectionString"),
                    b => b.MigrationsAssembly("FarmHub.Data")));

            services.AddIdentityCore<AuthUser>()
                .AddRoles<AuthRole>()
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<AuthUser, AuthRole>>()
                .AddEntityFrameworkStores<CatalogDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.Configure<AzureStorageConfig>(Configuration.GetSection(nameof(AzureStorageConfig)));
            services.Configure<GoogleRecaptchaConfig>(Configuration.GetSection(nameof(GoogleRecaptchaConfig)));
            services.Configure<AdyenConfig>(Configuration.GetSection(nameof(AdyenConfig)));
            services.Configure<RedisCacheOptions>(Configuration.GetSection($"CacheConfiguration:{nameof(RedisCacheOptions)}"));
            services.Configure<CacheConfiguration>(Configuration.GetSection(nameof(CacheConfiguration)));
            services.Configure<EmailOptions>(Configuration.GetSection(nameof(EmailOptions)));
            services.Configure<UrlOptions>(Configuration.GetSection(nameof(UrlOptions)));
            services.Configure<SendGridOptions>(Configuration.GetSection(nameof(SendGridOptions)));
            services.Configure<SendGridOptions>(settings =>
            {
                settings.Templates = new Dictionary<string, string>();
                Configuration.GetSection("SendGrid:Templates").Bind(settings.Templates);
            });

            RegisterDataServices(services);
            RegisterDomainServices(services);
            RegisterThirdPartyServices(services);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["CacheConfiguration:RedisCacheOptions:Configuration"];
                options.InstanceName = Configuration["CacheConfiguration:RedisCacheOptions:InstanceName"];
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200");
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // automapper configuration

            // services.AddAuthentication("Bearer")
            //     .AddJwtBearer("Bearer", options =>
            //     {
            //         options.Authority = Configuration["IdentityUrl"];
            //         options.RequireHttpsMetadata = false;
            //         options.Audience = "farmhub.api";
            //     });
            services.AddAuthentication();
            RegisterSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmHub.API");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void RegisterDataServices(IServiceCollection services)
        {
            services.AddScoped<IHarvestPeriodService, HarvestPeriodService>();
            services.AddScoped<IFarmerAssociationService, FarmerAssociationService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IFarmerService, FarmerService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPortionService, PortionService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IShippingAddressService, ShippingAddressService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAdyenPaymentService, AdyenPaymentService>();
            services.AddScoped<IShopifyExportService, ShopifyExportService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<ICarouselService, CarouselService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IConfirmationService, ConfirmationService>();
            services.AddScoped<IEmailMessageFactory, SendGridMessageFactory>();
            services.AddScoped<ICarouselService, CarouselService>();
            services.AddScoped<ITagService, TagService>();
            

            // Singletons
            // because cache should be not instantiated every request
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IGoogleRecaptchaService, GoogleRecaptchaService>();
            services.AddSingleton<IReportGeneratorFactory, ReportGeneratorFactory>();
        }

        private static void RegisterDomainServices(IServiceCollection services)
        {
            services.AddScoped<IConfirmationEmailService<Customer>, AccountConfirmationService>();
            services.AddScoped<IConfirmationEmailService<Order>, OrderConfirmationService>();
        }

        private static void RegisterSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FarmHub.API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }


        private void RegisterThirdPartyServices(IServiceCollection services)
        {
            // Adyen
            services.AddSingleton(new Client(Configuration["AdyenConfig:ApiKey"],
                Enum.Parse<Environment>(Configuration["AdyenConfig:Environment"])));
            
            // Shopify
            services.AddSingleton(new ShopifySharp.OrderService(Configuration["ShopifyConfig:ShopifyUrl"],
                Configuration["ShopifyConfig:AccessToken"]));
            services.AddSingleton(new ShopifySharp.ProductService(Configuration["ShopifyConfig:ShopifyUrl"],
                Configuration["ShopifyConfig:AccessToken"]));
        }
    }
}