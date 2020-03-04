using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;
using Me.Bartecki.Allegro.Infrastructure.Model;
using Optional;
using System.Linq;
using System.Threading.Tasks;

namespace Me.Bartecki.Allegro.Domain.Services.Decorators
{
    public class OrderedLettersStatisticsDecorator : IRepoStatisticsService
    {
        private readonly IRepoStatisticsService _innerService;

        public OrderedLettersStatisticsDecorator(IRepoStatisticsService innerService)
        {
            _innerService = innerService;
        }

        public async Task<Option<UserStatistics, AllegroApiException>> GetRepositoryStatisticsAsync(string username)
        {
            var input = await _innerService.GetRepositoryStatisticsAsync(username);
            input.Map(stats => stats.Letters = stats.Letters
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value));
            return input;
        }
    }
}
