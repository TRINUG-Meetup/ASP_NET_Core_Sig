using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChoreApp;

namespace SimpleAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        public ChoreRepository Repo { get; private set; }

        public HomeController(ChoreRepository repo)
        {
            Repo = repo;
        }
        public IActionResult Index()
        {
            ViewData["users"] = Repo.GetAllUsers();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
