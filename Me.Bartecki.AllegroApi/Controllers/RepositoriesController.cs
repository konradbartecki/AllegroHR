using Me.Bartecki.Allegro.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;

namespace Me.Bartecki.Allegro.Api.Controllers
{
    [Route("/[controller]/")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IRepoStatisticsService _repoStatisticsService;

        //In the future if we will get too many injected services here, we could use MediatR to avoid that "constructor explosion"
        public RepositoriesController(IRepoStatisticsService repoStatisticsService)
        {
            _repoStatisticsService = repoStatisticsService;
        }

        [HttpGet("{username}")]
        public async Task<UserStatistics> GetRepositoriesStatistics(string username)
        {
            return await _repoStatisticsService.GetRepositoryStatisticsAsync(username);
        }
    }
}