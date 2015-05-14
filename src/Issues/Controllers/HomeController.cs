using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Issues.Data;

namespace Issues.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMilestones _milestones;

        public HomeController(IMilestones milestones)
        {
            _milestones = milestones;
        }

        public async Task<IActionResult> Index()
        {
            var milestones = await _milestones.Get();
            return View(milestones);
        }

        // public async Task<IActionResult> Milestone(string id)
        // {

        // }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}