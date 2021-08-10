using Microsoft.AspNetCore.Mvc;
using WebTask.Models;
using WebTask.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICollectData collectData;

        public HomeController(ICollectData collectData)
        {
            this.collectData = collectData;
        }

        public IActionResult Index()
        {
            return View(collectData.GetCollect());
        }

        public IActionResult Personal()
        {
            return View(collectData.GetCollect());
        }

        [HttpGet]
        public IActionResult AddCollect()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCollect(Collect collect)
        {

            if (ModelState.IsValid && collect.NameCollection != null)
            {
                collect.NameAuthor = User.Identity.Name;
                collectData.AddCollect(collect);
            }
            //collectData.AddCollect(collect);

            return RedirectToAction("Personal", "Home");
        }
    }
}
