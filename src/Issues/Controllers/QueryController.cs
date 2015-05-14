using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;
using Issues.Data;
using Issues.Models;

namespace Issues.Controllers
{
    [Route("query")]
    public class QueryController : Controller
    {
        private IRepositories _repositories;
        private IIssues _issues;

        public QueryController(IRepositories repositories, IIssues issues)
        {
            _repositories = repositories;
            _issues = issues;
        }

        [HttpGet("repos")]
        public async Task<IList<RepoDto>> Repos()
        {
            return (await _repositories.Get()).Select(s => new RepoDto { Name = s }).ToList();
        }

        [HttpGet("issues/{repo}/{milestone}")]
        public async Task<IList<IssueDto>> Index(string repo, string milestone)
        {
            return await _issues.Get(repo, milestone);
        }
    }
}