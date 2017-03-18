This is a File -> New Project... in Visual Studio 2017 (specifically a .NET Core Web App, using the Web App template, No Auth, No Azure). Or you can create a new directory, cd into it and then dotnet new mvc on the command-line to create an new web project.

After that, I dropped in ChoreRepository.cs, you can download it from this repository.

I modified Startup.cs, adding .AddSingleton line as shown below:
```
    public void ConfigureServices(IServiceCollection services)
    {
	// ....
	services.AddSingleton<ChoreApp.ChoreRepository, ChoreApp.ChoreRepository>(); //added this line
    }
```	

I then created a new model, specifically this is a MVC model. The ChoreRepository contains a model, but that would be considered a data access layer model, not the model for MVC (Model-View-Controller). Create a Models/ folder and create a new .cs file called UserViewModel.cs with the following content:
```
using System;

namespace SimpleAspNetCore.Models
{
    public class UserViewModel
    {
        public UserViewModel() { }

        public string Name { get; set; }
    }
}
```

We now want to query the repository for any users and display them on the main page. First, we must update the controller part of MVC, in this case HomeController.cs, see relevant changes below, notice this converts from the data access model to the model we just created for the MVC layer of the app:
```
	public ChoreRepository Repo { get; private set; }

 	public HomeController(ChoreRepository repo)
	{
		Repo = repo;
	}
	public IActionResult Index()
	{
		ViewData["users"] = Repo.GetAllUsers().Select(user => new UserViewModel { Name = user.Name }).ToList();
		return View();
	}
```

We now need to update the View part of MVC, open up Views/Home/Index.html. Delete the content in the file and then add the following:
```
@{
    ViewData["Title"] = "Home Page";
}

<ul>
@foreach (var u in (List<SimpleAspNetCore.Models.UserViewModel>)ViewData["users"])
{
    <li>@u.Name</li>
}
</ul>
```
If you then run the app you will see John and Mary show-up in a list on the home page.
From there, the ability to add users can be added. I added the following to the bottom of Views/Home/Index.html
```
<form action="/user/add" method="post">
    <input type="text" name="Name" />
    <button type="submit">Add</button>
</form> 
```
I then created a UserController.cs with the following content:
```
namespace SimpleAspNetCore.Controllers
{
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
```
Now if you run the app, you can type in a new user name, click Add and it appears immediately in the UI.

If the app is stopped and started, only John and Mary will appear, the persistence is currently in-memory only.

In order to enable persistence, create an empty App_Data\ folder in the same directory where Startup.cs is. Now, if the app is shutdown and restarted users are persisted, data is persisted as .json files in the App_Data\ folder.

The ChoreRepository.cs contains a total of 9 classes:
 * ChoreApp.Exceptions.DataConflictException
 * ChoreApp.Exceptions.DataMissingException
 * ChoreApp.Exceptions.InvalidRequestException
 * ChoreApp.Models.AssignmentSummary
 * ChoreApp.Models.CompleteChorePayload
 * ChoreApp.Models.Chore
 * ChoreApp.Models.User
 * ChoreApp.Models.CompletedChore
 * ChoreApp.ChoreRepository
 
This repository works on dotNet Core and has been tested on Linux. As noted it works in-memory, can optionally persist and does not require a database. It is also thread-safe.
It tracks Users, Chores for those Users and then completion of those chores, displaying current assignments and completions for the current week.

The ChoreRepository has the following potentially useful methods:
 * List<User> GetAllUsers();
 * List<Chore> GetAllChores();
 * List<AssignmentSummary> GetChildAssignmentsThisWeek(int userId);
 * User GetUser(int id);
 * Chore GetChore(int id);
 * void AddUser(User value);
 * void EditUser(int id, User value);
 * void DeleteUser(int id);
 * void AddChore(Chore value);
 * void EditChore(int id, Chore Value);
 * void DeleteChore(int id);
 * void CompleteChore(CompleteChorePayload data);
 * void ClearChoreCompletion(CompleteChorePayload data);
 
	
As we go farther into the SIG series, we can append logging, mocking, proper dependency injection, replace the ChoreRepository with one that uses EF core, etc.
Since this repository already works on Linux, examples on Linux and with docker containers would be easy to demo as well.
