using System.Threading.Tasks;
using Me.Bartecki.Allegro.Domain.Model;

namespace Me.Bartecki.Allegro.Domain.Services.Interfaces
{
    public interface IRepoStatisticsService
    {
        Task<UserStatistics> GetRepositoryStatisticsAsync(string username);
    }
}