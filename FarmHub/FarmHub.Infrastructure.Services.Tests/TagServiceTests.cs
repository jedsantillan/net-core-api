using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Data.Models;
using FarmHub.Domain.Services.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class GetEntitiesForTagsAsyncWithExistingTags : DbContextTestBase
    {
        private TagService _tagService;

        public GetEntitiesForTagsAsyncWithExistingTags()
        {
            var existingTags = new List<Tag>()
            {
                new Tag("FRUITS"),
                new Tag("Frozen")
            };
            _context.Tags.AddRangeAsync(existingTags);
            _context.SaveChanges();

            _tagService = new TagService(Mock.Of<ILogger<TagService>>(), _context);
        }

        [Fact]
        public async Task WhenAllTagsAreNotInTheDatabase_ThenShouldReturnTheNewTagsWithoutIds()
        {
            var tags = new[] {"Vegetables", "Root Crops"};
            var entities = await _tagService.GetEntitiesForTagsAsync(tags);

            entities.Should().HaveCount(tags.Length);
            entities.Should().OnlyContain(t => t.Id == 0);
            entities.Should().OnlyContain(e => tags.Contains(e.Name, StringComparer.CurrentCultureIgnoreCase));
        }
        
        [Fact]
        public async Task WhenOneTagIsInTheDatabase_ThenShouldReturnTheNewTagsWithoutIdsExceptOne()
        {
            var tags = new[] {"Vegetables", "Root Crops", "Fruits"};
            var entities = await _tagService.GetEntitiesForTagsAsync(tags);

            entities.Should().HaveCount(tags.Length);
            entities.Where(t => t.Id == 0).Should().HaveCount(2);
            entities.Where(t => t.Id != 0).Should().HaveCount(1);
            entities.Should().OnlyContain(e => tags.Contains(e.Name, StringComparer.CurrentCultureIgnoreCase));
        }
        
        [Fact]
        public async Task WhenAllTagsAreNotInTheDatabaseAndTheyAreTheSameWord_ThenShouldNotReturnDuplicates()
        {
            var tags = new[] {"Vegetables", "VeGETablES", "VEGETABLES"};
            var entities = await _tagService.GetEntitiesForTagsAsync(tags);

            entities.Should().HaveCount(1);
            entities.Should().OnlyContain(t => t.Id == 0);
            entities.Should().OnlyContain(e => tags.Contains(e.Name, StringComparer.CurrentCultureIgnoreCase));
        }
    }
}