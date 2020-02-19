using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Me.Bartecki.Allegro.Infrastructure.Model;
using Me.Bartecki.Allegro.Infrastructure.Services;

namespace Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores.GitHub
{

    public partial class GitHubIntegrationService : IRepositoryStoreService
    {
        private const string GITHUB_URL = "https://api.github.com/graphql";
        private readonly GraphQLHttpClient _client;
        private readonly IEmbeddedResourceService _embeddedResourceService;

        public GitHubIntegrationService(HttpClient client, IEmbeddedResourceService embeddedResourceService)
        {
            _client = client.AsGraphQLClient(GITHUB_URL);
            _embeddedResourceService = embeddedResourceService;
        }

        private async Task<IEnumerable<GitHubRepository>> GetAllUserRepositoriesAsync(string username)
        {
            string query = _embeddedResourceService.GetResource("Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores.GitHub.GitHubQuery.graphql");
            bool isNextPageAvailable = false;
            string nextPageId = null;

            List<GitHubRepository> repositories = new List<GitHubRepository>();
            do
            {
                var request = new GraphQLHttpRequest(query,
                    variables: new
                    {
                        username = username,
                        nextCursorId = nextPageId
                    });
                var response = await _client.SendQueryAsync<RootResponse>(request);
                if (response.Errors?.Any() == true)
                {
                    string message = response.Errors.First().Message;
                    throw new Exception(message);
                }

                repositories.AddRange(response.Data.User.Repositories.Nodes);

                //Handle pagination
                var pageInfo = response.Data.User.Repositories.PageInfo;
                isNextPageAvailable = pageInfo.HasNextPage;
                if (isNextPageAvailable)
                {
                    nextPageId = pageInfo.EndCursor;
                }
            } while (isNextPageAvailable);

            return repositories;
        }

        private Repository Convert(GitHubRepository source)
        {
            var dest = new Repository();
            dest.Name = source.Name;
            dest.Size = source.DiskUsage;
            dest.Stargazers = source.Stargazers.TotalCount;
            dest.Watchers = source.Watchers.TotalCount;
            dest.Forks = source.ForkCount;
            return dest;
        }

        public async Task<IEnumerable<Repository>> GetUserRepositoriesAsync(string username)
        {
            var allRepos = await GetAllUserRepositoriesAsync(username);
            return allRepos.Select(Convert);
        }
    }
}
