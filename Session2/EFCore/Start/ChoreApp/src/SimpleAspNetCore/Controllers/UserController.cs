﻿using Microsoft.AspNetCore.Mvc;
using SimpleAspNetCore.Models;
using ChoreApp.Contracts;

namespace SimpleAspNetCore.Controllers
{
    public class UserController
    {
        private readonly IChoreRepository Repo;

        public UserController(IChoreRepository repo)
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
