using System.Collections.Generic;
using System.Threading.Tasks;
using Me.Bartecki.Allegro.Infrastructure.Model;

namespace Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores
{
    public interface IRepositoryStoreService
    {
        Task<IEnumerable<Repository>> GetUserRepositoriesAsync(string username);
    }
}