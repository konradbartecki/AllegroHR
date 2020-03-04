using Me.Bartecki.Allegro.Api.Services;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Me.Bartecki.Allegro.Api.Controllers
{
    [Route("/[controller]/")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IRepoStatisticsService _repoStatisticsService;
        private readonly ErrorCodeMapper _errorMapper;

        //In the future if we will get too many injected services here, we could use MediatR to avoid that "constructor explosion"
        public RepositoriesController(IRepoStatisticsService repoStatisticsService, ErrorCodeMapper errorMapper)
        {
            _repoStatisticsService = repoStatisticsService;
            _errorMapper = errorMapper;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserStatistics>> GetRepositoriesStatistics(string username)
        {
            var response = await _repoStatisticsService.GetRepositoryStatisticsAsync(username);
            return response.Match(
                statistics => Ok(statistics),
                error => _errorMapper.GetUserFriendlyError(error));

        }
    }
}