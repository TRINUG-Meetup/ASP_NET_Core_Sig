# Dependency Injection in .NET Core

## Presentation
This presentation was created for the Triangle .NET User group and is being presented at the March 1st Web SIG.  It provides a brief overview of Dependency Injection / IOC concepts and demonstrates related features available in .NET Core.  

Specifically it shows examples of how the 
Microsoft.Extensions.DependencyInjection container is used in ASP.NET MVC.  In the new version of ASP.NET, the dependency injection container is a first class citizen.  The entire MVC framework is based heavily on the use of interfaces and dependency injection.  This decreases coupling for the framwork and makes it easier to extend and test.  

The presentation was created with RevealJS and may be viewed directly on gh-pages:
[https://davidbwilson.github.io/Core_DI_Presentation](https://davidbwilson.github.io/Core_DI_Presentation)

The content of the presentation is available for pull requests and comments at [https://github.com/davidbwilson/Core_DI_Presentation](https://github.com/davidbwilson/Core_DI_Presentation)

## Working with the example application

Install .NET Core from [https://dot.net](https://dot.net).  Follow the instructions for your Operating System and editor choices.

**Note:** This example application was created with release candidate 3 of the .NET Core SDK and utilizes a .csproj file.  You will need to install rc3 or rc4 of the SDK to work with this project.  

It is based off of the example web application created by the dotnet command line:
```
dotnet new -t web
```

Clone the main repo:
```
git clone https://github.com/TRINUG-Meetup/ASP_NET_Core_Sig.git
```
Navigate to the folder ASP_NET_Core_Sig/Session3/DependencyInjection/ExampleApp/

Open the folder or project in your favorite editor.

Restore packages
```
dotnet restore
```

### Simple Registration Example
This example shows how the CustomerRepository is injected into the CustomerController via the registration found in Startup.cs.

CustomerRepository is constructor injected based on its interface ICustomerRepository.

An alternative implementation of CustomerGlueRepository shows how this would be wired up without dependency injection using the new keyword.

### View injection example
IInstanceService is injected into the ScopeTest/Index.cshtml file via the inject directive:
```
@inject MvcApp.Services.IInstanceService InstanceService;
```
This is a new feature is ASP.NET MVC Core.

### Scope test example
The scope test example demonstrates how the lifetime scope chosen during a registration impacts which instance of an object the container provides.  The scopetest/index.csthml requests an injected IInstanceService and so does the scopetestcontroller.  The scopetest/index.html displays the identifier (guid) created during object construction for both instances of the service.  This clearly demonstrates the instances provided by the container.  The scope can be varied in startup.cs to show how the instances supplied will change based on the registrations.

InstanceService also logs a message when it is constructed and when the GetInstance() method is called.  This allows you to observe the difference between a AddSingleton and AddInstance registrations by viewing the console log.
