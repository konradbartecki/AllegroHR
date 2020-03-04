using Me.Bartecki.Allegro.Infrastructure.Model;
using Optional;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores
{
    public interface IRepositoryStoreService
    {
        Task<Option<IEnumerable<Repository>, AllegroApiException>> GetUserRepositoriesAsync(string username);
    }
}