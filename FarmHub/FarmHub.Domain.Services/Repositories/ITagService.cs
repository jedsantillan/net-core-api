using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Application.Services.Repositories
{
    public interface ITagService : IGenericRepository<Tag>
    {
        Task<Tag[]> GetEntitiesForTagsAsync(IEnumerable<string> modelTags);
    }
}