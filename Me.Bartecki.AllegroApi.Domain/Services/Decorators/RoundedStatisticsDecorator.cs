using System;
using System.Threading.Tasks;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;

namespace Me.Bartecki.Allegro.Domain.Services.Decorators
{
    public class RoundedStatisticsDecorator : IRepoStatisticsService
    {
        private readonly IRepoStatisticsService _innerService;

        public RoundedStatisticsDecorator(IRepoStatisticsService innerService)
        {
            _innerService = innerService;
        }
        public async Task<UserStatistics> GetRepositoryStatisticsAsync(string username)
        {
            var result = await _innerService.GetRepositoryStatisticsAsync(username);
            result.AverageForks = Math.Round(result.AverageForks, 2);
            result.AverageSize = Math.Round(result.AverageSize, 2);
            result.AverageStargazers = Math.Round(result.AverageStargazers, 2);
            result.AverageWatchers = Math.Round(result.AverageWatchers, 2);
            return result;
        }
    }
}
