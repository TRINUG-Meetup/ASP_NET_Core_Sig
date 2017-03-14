using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IHostingEnvironment Env { get; private set; }
        public IConfiguration Config { get; private set; }

        public HomeController(IHostingEnvironment env, IConfiguration config)
        {
            Env = env;
            Config = config;
        }

        public IActionResult Index()
        {
            ViewData["Env"] = Env.EnvironmentName;
            ViewData["SampleKey"] = Config["SampleKey"];
            ViewData["NestedKey"] = Config.GetSection("SampleSection")["NestedKey"];
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
