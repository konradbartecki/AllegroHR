using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;
using Me.Bartecki.Allegro.Infrastructure.Model;
using Optional;
using System;
using System.Threading.Tasks;

namespace Me.Bartecki.Allegro.Domain.Services.Decorators
{
    public class RoundedStatisticsDecorator : IRepoStatisticsService
    {
        private readonly IRepoStatisticsService _innerService;

        public RoundedStatisticsDecorator(IRepoStatisticsService innerService)
        {
            _innerService = innerService;
        }
        public async Task<Option<UserStatistics, AllegroApiException>> GetRepositoryStatisticsAsync(string username)
        {
            var option = await _innerService.GetRepositoryStatisticsAsync(username);
            option.MatchSome(Transform);
            return option;
        }

        private void Transform(UserStatistics result)
        {
            result.AverageForks = Math.Round(result.AverageForks, 2);
            result.AverageSize = Math.Round(result.AverageSize, 2);
            result.AverageStargazers = Math.Round(result.AverageStargazers, 2);
            result.AverageWatchers = Math.Round(result.AverageWatchers, 2);
        }
    }
}
