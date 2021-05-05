using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class GenericRepositoryTests
    {
        [Fact]
        public async Task Insert_NewHarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("insert_harvestperiod");

            // Arrange
            var obj = new HarvestPeriod(name: "Fake Name")
            {
                Inventories = new List<Inventory>()
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);

                // Act
                await repo.InsertAsync(obj);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Single(context.HarvestPeriods, h => h.Id == obj.Id);
            }
        }

        [Fact]
        public async Task InsertRange_NewHarvestPeriods_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("insertrange_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Inventories = new List<Inventory>()
                },
                new HarvestPeriod(name: "Fake Name2")
                {
                    Inventories = new List<Inventory>()
                }
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);

                // Act
                await repo.InsertRangeAsync(objs);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Equal(2, context.HarvestPeriods.Count());
            }
        }


        [Fact]
        public async Task UpdateAsync_NewHarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("update_harvestperiod");

            // Arrange
            var obj = new HarvestPeriod(name: "Fake Name")
            {
                Inventories = new List<Inventory>()
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await repo.InsertAsync(obj);
            }

            // Act
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                obj.Name = "Updated Name";
                await repo.Update(obj);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var fetch = context.HarvestPeriods.Single(h => h.Id == obj.Id);
                Assert.Equal("Updated Name", fetch.Name);
            }
        }


        [Fact]
        public async Task DeleteAsync_HarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("delete_harvestperiod");

            // Arrange
            var obj = new HarvestPeriod(name: "Fake Name")
            {
                Inventories = new List<Inventory>()
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await repo.InsertAsync(obj);
            }

            // Act
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await repo.Delete(obj);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var fetch = context.HarvestPeriods.SingleOrDefault(h => h.Id == obj.Id);
                Assert.Null(fetch);
            }
        }

        [Fact]
        public async Task DeleteByIdAsync_HarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("deletebyid_harvestperiod");

            // Arrange
            var obj = new HarvestPeriod(name: "Fake Name")
            {
                Inventories = new List<Inventory>()
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await repo.InsertAsync(obj);
            }

            // Act
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await repo.DeleteById(obj.Id);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var fetch = context.HarvestPeriods.SingleOrDefault(h => h.Id == obj.Id);
                Assert.Null(fetch);
            }
        }

        [Fact]
        public async Task GetAllAsync_HarvestPeriods_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("getall_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Inventories = new List<Inventory>()
                },
                new HarvestPeriod(name: "Fake Name 2")
                {
                    Inventories = new List<Inventory>()
                },
            };

            List<HarvestPeriod> allHps;
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await context.AddRangeAsync(objs);
                await context.SaveChangesAsync();

                // Act
                allHps = repo.GetAll().ToList();
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Collection(allHps, hp => Assert.True(context.HarvestPeriods.Contains(hp)),
                    hp => Assert.True(context.HarvestPeriods.Contains(hp)));
            }
        }

        [Fact]
        public async Task GetByIdAsync_HarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("getbyid_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name"),
                new HarvestPeriod(name: "Fake Name2"),
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                await context.AddRangeAsync(objs);
                await context.SaveChangesAsync();
            }

            HarvestPeriod hp;
            // Act
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                hp = await repo.GetByIdAsync(objs.First().Id);
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Equal(JsonConvert.SerializeObject(hp), JsonConvert.SerializeObject(objs.First()));
            }
        }


        [Fact]
        public async Task GetWhereAsync_HarvestPeriod_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("getwhere_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name"),
                new HarvestPeriod(name: "Fake Name2"),
            };

            await using (var context = new CatalogDbContext(dbOptions))
            {
                await context.AddRangeAsync(objs);
                await context.SaveChangesAsync();
            }

            List<HarvestPeriod> hps;
            // Act
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                hps = await repo.GetWhere(o => o.Name.StartsWith("Fake Name")).ToListAsync();
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Collection(hps,
                    hp => Assert.Contains(objs, o => JsonConvert.SerializeObject(o) == JsonConvert.SerializeObject(hp)),
                    hp => Assert.Contains(objs,
                        o => JsonConvert.SerializeObject(o) == JsonConvert.SerializeObject(hp)));
            }
        }

        [Fact]
        public async Task CountAllAsync_HarvestPeriods_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("countall_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Inventories = new List<Inventory>()
                },
                new HarvestPeriod(name: "Fake Name 2")
                {
                    Inventories = new List<Inventory>()
                }
            };

            int count = 0;
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);

                // Act
                count = await repo.CountAllAsync();
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Equal(context.HarvestPeriods.Count(), count);
            }
        }


        [Fact]
        public async Task CountWhereAsync_HarvestPeriods_Success()
        {
            var dbOptions = TestUtils.GetInMemoryDbOptions<CatalogDbContext>("countwhere_harvestperiod");

            // Arrange
            var objs = new List<HarvestPeriod>
            {
                new HarvestPeriod(name: "Fake Name")
                {
                    Inventories = new List<Inventory>()
                },
                new HarvestPeriod(name: "Fake Name 2")
                {
                    Inventories = new List<Inventory>()
                }
            };

            int count = 0;
            await using (var context = new CatalogDbContext(dbOptions))
            {
                var repo = new GenericRepository<HarvestPeriod, CatalogDbContext>(context);
                await context.AddRangeAsync(objs);
                await context.SaveChangesAsync();
                // Act
                count = await repo.CountWhereAsync(hp => hp.Name == "Fake Name");
            }

            // Assert
            await using (var context = new CatalogDbContext(dbOptions))
            {
                Assert.Equal(1, count);
            }
        }
    }
}