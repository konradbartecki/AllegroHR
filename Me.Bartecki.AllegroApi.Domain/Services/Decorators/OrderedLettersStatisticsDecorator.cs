using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;

namespace Me.Bartecki.Allegro.Domain.Services.Decorators
{
    public class OrderedLettersStatisticsDecorator : IRepoStatisticsService
    {
        private readonly IRepoStatisticsService _innerService;

        public OrderedLettersStatisticsDecorator(IRepoStatisticsService innerService)
        {
            _innerService = innerService;
        }

        public async Task<UserStatistics> GetRepositoryStatisticsAsync(string username)
        {
            var input = await _innerService.GetRepositoryStatisticsAsync(username);
            input.Letters = input.Letters
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
            return input;
        }
    }
}
