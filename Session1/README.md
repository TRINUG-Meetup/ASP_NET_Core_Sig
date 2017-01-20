This is a File -> New Project... in Visual Studio 2015 (specifically a .NET Core Web App, using the Web App template, No Auth, No Azure)

After that, I dropped in ChoreRepository.cs

I modified Startup.cs, adding .AddSingleton line as shown below:
```
    public void ConfigureServices(IServiceCollection services)
    {
	// ....
	services.AddSingleton<ChoreApp.ChoreRepository, ChoreApp.ChoreRepository>(); //added this line
    }
```	
I then modified the HomeController.cs, relevant changes below:
```
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
```

I then modified Views/Home/Index.html, I deleted the content in the file and then added the following:
```
@{
   ViewData["Title"] = "Home Page";
}

<ul>
@foreach (var u in (List<ChoreApp.Models.User>)ViewData["users"])
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
```
The reason for UserViewModel is that the ChoreRepository works with a User class that is immutable by default and MVC cannot bind to that immutable class so a view model specific class must be created.

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

As we go farther into the SIG series, we can append logging, mocking, proper dependency injection, replace the ChoreRepository with one that uses EF core, etc.
Since this repository already works on Linux, examples on Linux and with docker containers would be easy to demo as well.

Everything is placed in a single .cs file, so that attendees of the SIG can replicate all the steps listed above easily, without having to manage and place 9 .cs files.
