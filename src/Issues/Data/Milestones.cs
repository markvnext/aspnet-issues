using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;
using Octokit;

namespace Issues.Data
{
    public interface IMilestones
    {
        Task<IList<string>> Get();
    }

    public class Milestones : IMilestones
    {
        private readonly IGitHubProvider _gitHubProvider;
        private readonly IMemoryCache _cache;
        private readonly IRepositories _repositories;

        public Milestones(IMemoryCache cache, IRepositories repositories, IGitHubProvider gitHubProvider)
        {
            _cache = cache;
            _repositories = repositories;
            _gitHubProvider = gitHubProvider;
        }

        public async Task<IList<string>> Get()
        {
            List<string> milestones;
            if (!_cache.TryGetValue("Milestones", out milestones))
            {
                var repos = await _repositories.Get();
                milestones = new List<string>();
                var client = _gitHubProvider.GetClient();

                foreach (var repo in repos)
                {
                    var req = new MilestoneRequest
                    {
                        State = ItemState.Open
                    };
                    var ms = await client.Issue.Milestone.GetAllForRepository("aspnet", repo, req);
                    milestones.AddRange(ms.Select(m => m.Title));
                }

                milestones = milestones.Distinct().ToList();
                _cache.Set("Milestones", milestones);
            }
            return milestones;
        }
    }
}