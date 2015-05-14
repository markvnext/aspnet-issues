using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Issues.Models;
using Octokit;
using Newtonsoft.Json.Linq;
using Microsoft.Framework.Caching.Memory;

namespace Issues.Data
{
    public interface IIssues
    {
        Task<IList<IssueDto>> Get(string repo, string milestone);
    }

    public class Issues : IIssues
    {
        private static IList<IssueDto> Empty = new IssueDto[0];
        private readonly ILogger _logger;
        private readonly IGitHubProvider _gitHubProvider;
        private readonly IMemoryCache _cache;

        public Issues(IMemoryCache cache, ILoggerFactory loggerFactory, IGitHubProvider gitHubProvider)
        {
            _cache = cache;
            _logger = loggerFactory.CreateLogger<Issues>();
            _gitHubProvider = gitHubProvider;
        }

        public async Task<IList<IssueDto>> Get(string repo, string milestone)
        {
            IList<IssueDto> list;
            if (!_cache.TryGetValue($"Issues/{repo}/{milestone}", out list))
            {
                list = await GetImpl(repo, milestone);
                _cache.Set($"Issues/{repo}/{milestone}", list);
            }
            return list;
        }

        public async Task<IList<IssueDto>> GetImpl(string repo, string milestone)
        {
            try
            {
                var client = _gitHubProvider.GetClient();

                var repoMilestones = await client.Issue.Milestone.GetAllForRepository("aspnet", repo);

                Milestone repoMilestone;

                if (milestone.StartsWith("*"))
                {
                    repoMilestone = repoMilestones.FirstOrDefault(rm => rm.Title.EndsWith(milestone.Substring(1)));
                }
                else
                {
                    repoMilestone = repoMilestones.FirstOrDefault(rm => rm.Title == milestone);
                }

                if (repoMilestone == null) return Empty;

                var request = new RepositoryIssueRequest
                {
                    Filter = IssueFilter.All,
                    Milestone = repoMilestone.Number.ToString()
                };
                request.Labels.Add("bug");
                var issues = await client.Issue.GetAllForRepository("aspnet", repo, request);
                return issues.Select(i => new IssueDto
                {
                    Repo = repo,
                    Title = i.Title,
                    HtmlUrl = i.HtmlUrl.ToString()
                })
                .ToList();
            }
            catch (ApiValidationException ex)
            {
                var data = JObject.FromObject(ex.HttpResponse).ToString();
                _logger.LogError($"Getting {repo}/{milestone} errored {data}");
                return Empty;
            }
        }
    }
}