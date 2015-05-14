using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;

namespace Issues.Data
{
    public interface IRepositories
    {
        Task<IList<string>> Get();
    }

    public class Repositories : IRepositories
    {
        private readonly IMemoryCache _cache;
        private readonly IGitHubProvider _gitHubProvider;

        public Repositories(IMemoryCache cache, IGitHubProvider gitHubProvider)
        {
            _cache = cache;
            _gitHubProvider = gitHubProvider;
        }

	    public async Task<IList<string>> Get()
        {
            IList<string> repositories;

            if (!_cache.TryGetValue("Repositories", out repositories))
            {
                var client = _gitHubProvider.GetClient();
                var repos = await client.Repository.GetAllForOrg("aspnet");
                repositories = repos.Select(repo => repo.Name).ToList();
                _cache.Set("Repositories", repositories);
            }

            return repositories;
        }
    }
}