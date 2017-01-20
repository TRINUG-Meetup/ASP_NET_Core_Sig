using ChoreApp;
using ChoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAspNetCore.Controllers
{
    public class UserViewModel
    {
        public UserViewModel() { }
        
        public string Name { get; set; }
    }
    public class UserController
    {
        private readonly ChoreRepository Repo;

        public UserController(ChoreRepository repo)
        {
            Repo = repo;
        }

        [HttpPost]
        public IActionResult Add(UserViewModel user)
        {
            Repo.AddUser(new ChoreApp.Models.User(-1, user.Name));
            return new RedirectToActionResult("Index", "Home", null);
        }
    }
}
