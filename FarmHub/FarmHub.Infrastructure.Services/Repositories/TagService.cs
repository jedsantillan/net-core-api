using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class TagService : GenericRepository<Tag, CatalogDbContext>, ITagService
    {
        private ILogger<TagService> _logger;

        public TagService(ILogger<TagService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<Tag[]> GetEntitiesForTagsAsync(IEnumerable<string> modelTags)
        {
            modelTags = modelTags.Distinct(StringComparer.CurrentCultureIgnoreCase);
            var existingTags = await _dbContext.Tags
                .Where(t => modelTags.Select(t => t.ToLower()).Contains(t.Name.ToLower()))
                .ToListAsync();

            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();
            var newTags = modelTags.Where(t => !existingTagNames.Contains(t, StringComparer.CurrentCultureIgnoreCase))
                .Select(nt => new Tag(nt.ToUpper()));

            return existingTags.Union(newTags).ToArray();
        }
    }
}