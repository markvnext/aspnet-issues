using Microsoft.AspNet.Mvc;
using Issues.Data;

namespace Issues.Controllers
{
    [Route("milestone")]
    public class MilestoneController : Controller
    {
        private IIssues _issues;

        public MilestoneController(IIssues issues)
        {
            _issues = issues;
        }

        [HttpGet("{milestone}")]
        public IActionResult Index(string milestone)
        {
            return View();
        }
    }
}