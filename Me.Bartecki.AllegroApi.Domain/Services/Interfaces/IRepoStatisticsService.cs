using System.Threading.Tasks;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Infrastructure.Model;
using Optional;

namespace Me.Bartecki.Allegro.Domain.Services.Interfaces
{
    public interface IRepoStatisticsService
    {
        Task<Option<UserStatistics, AllegroApiException>> GetRepositoryStatisticsAsync(string username);
    }
}