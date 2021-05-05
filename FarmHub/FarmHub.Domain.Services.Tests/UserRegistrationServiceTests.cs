using System;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Infrastructure.Services;
using FarmHub.Domain.Services.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.Application.Services.Tests
{
    public class UserRegistrationServiceTests
    {
        [Fact]
        public async void CreateUserAsync_WhenAllPrametersArePassed_ThenShouldReturnCreatedUser()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var customerService = new CustomerService(Mock.Of<ILogger<Customer>>(), arrangeDbContext);
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService,
                Mock.Of<ILogger<UserRegistrationService>>());
            
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";
            const string password = "Abcd1234!";

            var result = await regService.CreateUserAsync(email,
                userName,
                password);

            result.Errors.Should().BeNullOrEmpty();
            userManager.Verify(m => m.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()), Times.Once());
        }


        [Fact]
        public async void CreateUserAsync_WhenAllUserManagerThrowsError_ThenShouldReturnErrors()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError()
                    {
                        Code = "PasswordError",
                        Description = "Password does not meet criteria"
                    },
                }));

            var customerService = new CustomerService(Mock.Of<ILogger<Customer>>(), arrangeDbContext);
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService,
                Mock.Of<ILogger<UserRegistrationService>>());
            
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";
            const string password = "Abcd1234!";

            var result = await regService.CreateUserAsync(email,
                userName,
                password);

            result.Errors.Should().NotBeNullOrEmpty();
            userManager.Verify(m => m.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void CreateCustomerAndUserAsync_WhenAllParametersArePassed_ThenShouldCreateCustomer()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var customerService = new CustomerService(Mock.Of<ILogger<Customer>>(), arrangeDbContext);
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService,
                Mock.Of<ILogger<UserRegistrationService>>());
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";

            var customer = new Customer
            {
                ContactEmail = email,
                UserName = userName,
                Gender = Gender.Male,
                FirstName = "Ha?",
                LastName = "Hakdog"
            };
            
            var errors = await regService.CreateCustomerAndUserAsync(email,
                userName,
                "Abcd1234!",
                customer);

            errors.Errors.Should().BeNullOrEmpty();

            var assertDbContext = new CatalogDbContext(dbOptions);
            var expectedCustomer = assertDbContext.Customers.FirstOrDefault(c => c.ContactEmail == email);
            expectedCustomer.Should().NotBeNull();
        }

        [Fact]
        public async void
            CreateCustomerAndUserAsync_WhenErrorSavingPasswordForAuth_ShouldReturnErrorsAndShouldNotCreateBoth()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>())).ReturnsAsync(
                IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError()
                    {
                        Code = "PasswordError",
                        Description = "Password does not meet criteria"
                    },
                }));

            var customerService = new CustomerService(Mock.Of<ILogger<Customer>>(), arrangeDbContext);
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService,
                Mock.Of<ILogger<UserRegistrationService>>());
            var customer = new Mock<Customer>();
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";

            customer.Object.ContactEmail = email;
            customer.Object.UserName = userName;

            var errors = await regService.CreateCustomerAndUserAsync(email,
                userName,
                "Abcd1234!",
                customer.Object);

            errors.Should().NotBeNull();

            var assertDbContext = new CatalogDbContext(dbOptions);
            var expectedCustomer = assertDbContext.Customers.FirstOrDefault(c => c.ContactEmail == email);
            expectedCustomer.Should().BeNull();
        }


        [Fact]
        public void CreateCustomerAndUserAsync_WhenErrorCreatingCustomer_ShouldThrowException()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.CreateAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var customerService = new Mock<ICustomerService>();
            customerService.Setup(c => c.InsertAsync(It.IsAny<Customer>())).Throws(new Exception());
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService.Object,
                Mock.Of<ILogger<UserRegistrationService>>());
            var customer = new Mock<Customer>();
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";

            customer.Object.ContactEmail = null;
            customer.Object.UserName = null;

            Func<Task> act = async () => await regService.CreateCustomerAndUserAsync(email,
                userName,
                "Abcd1234!",
                customer.Object);

            act.Should().Throw<Exception>();

            var assertDbContext = new CatalogDbContext(dbOptions);
            var expectedCustomer = assertDbContext.Customers.FirstOrDefault(c => c.ContactEmail == email);
            expectedCustomer.Should().BeNull();
        }

        [Fact]
        public async void UpdateCustomerAndUserAsync_WhenPasswordsDoNotMatch_ShouldReturnAnErrorAndDontCreateBoth()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(Guid.NewGuid().ToString());
            var arrangeDbContext = new CatalogDbContext(dbOptions);

            var userManager = AspNetCoreIdentityMockHelpers.MockUserManager<AuthUser>();
            userManager.Setup(u => u.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync(IdentityResult.Success);

            userManager.Setup(u => u.ChangePasswordAsync(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError()
                    {
                        Code = "PasswordError",
                        Description = "Passwords do not match"
                    },
                }));

            var customerService = new CustomerService(Mock.Of<ILogger<Customer>>(), arrangeDbContext);
            var regService = new UserRegistrationService(arrangeDbContext, userManager.Object, customerService,
                Mock.Of<ILogger<UserRegistrationService>>());
            var customer = new Mock<Customer>();
            const string email = "andrei.delacruz@gmail.com";
            const string userName = "adelacruz";

            customer.Object.ContactEmail = email;
            customer.Object.UserName = userName;

            var identityUser = new Mock<AuthUser>();

            var errors = await regService.UpdateCustomerAndUserAsync(customer.Object,
                identityUser.Object,
                "Abcd1234!",
                "1234Abcd!");

            errors.Should().NotBeNull();
        }
    }
}