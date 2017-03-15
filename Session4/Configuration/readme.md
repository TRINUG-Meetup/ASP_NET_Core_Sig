Configuration
=============

How was configuration done previously?
--------------------------------------
Previously, configuration was split out among multiple places:
  1. Web.config - some things like exception pages and mime types were configured here. Setting and secrets were likely stored here.
  2. Global.asax.cs - some behaviors, like JSON casing were configured here.
  3. Framework specific set-up in C# - routing, MVC filters were configured here.

The largest problem was that you had to configure it where it was required to be configured. For example, mime type support had to be configured in Web.config. You couldn't configure that in C#. Going forward in ASP.Net Core, configuration is done in code, all configuration is done in code.

Configuration now in ASP.NET Core
---------------------------------
Remember, that in ASP.NET Core, you have a console application with an embedded web server (e.g. Kestrel typically) and so therefore configuration is done within your application. That configuration code can defer to other places, like .json files, environment variables, command-line arguments, but ultimately the configuration is controlled by code. So, Web.config and Global.asax are now both gone. Almost all configuration will be done in Startup.cs, some is done in Program.cs, you'll see that later.

Please note that the embedded web server (e.g. Kestrel) does not handle every HTTP scenario. So, typically you will put Kestrel behind a production quality, hardended web server, such as IIS, Apache, NGINX. Those servers support things like SSL, you would configure SSL specifically for that server and that configuration would reside out ASP.NET Core configuration.

Sample Application
------------------
For this discussion, there is a sample ASP.NET Core application. It is in the WebApp/ folder. This was built using the Visual Studio 2017 release. I removed a number of things from the initial template provided with VS 2017. If you run the application using VS 2017 and IIS Express (e.g. the defaults), you should see a page showing the environment as Development, and a blank value for both the Sample Key and Nested Key values.

Middleware
----------
A significant amount of the behavior that was configured in Web.config before is now done as middleware. You can find a discussion and diagram on the [official microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware). But basically, you have a chained pipeline of middleware components. Each may execute the next item in the pipeline or not. Before executing the next item in the pipeline they generate their own response if needed, after executing the next item in the pipeline they edit and already generated response if needed. MVC is actual implemented as a middleware component and is inserted into the pipeline. Something like exception handling or response compression is another middleware component and would be a sibling to MVC. The Configure method is what builds the middleware pipeline, specifically using the IApplicationBuilder interface.

We can demo this by adding our own middleware inline. Please run the application first so you see it working before adding the code below. Add this code to the **beginning** of the Configure method in Startup.cs.

```
    app.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("Hello, World! Custom middleware here!");
    });
```

If you run the application, you will now see that message, no matter url you attempt to hit. We never call the *next* argument which is the next item in the pipeline, in our example that would be the static files middleware component. In our example app, we now have three middleware: our custom code, static files, MVC.

Now, move that code to the bottom of the Configure method and run the application. Now, you see the application running fine. Only if you enter a URL that isn't accepted by either the static files or the MVC middleware do you see our message. That is because our middleware is now configured last in the pipeline for generating a response.

If you want to experiment further, you can see how a custom middleware can inspect a path and decide whether to generate a response or pass to the next item in the middleware. Remove the previous code and put this at the top of the Configure method.

```
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/Custom"))
        {
            await context.Response.WriteAsync("Hello, World! Custom middleware here!");
            return;
        }
        else
        {
            await next.Invoke();
        }
    });
```

Now you will only see our custom middleware response under a URL like /Custom or /Custom/SubContent even though we are again the first item in the middleware pipeline.

As you see in our sample if you want Kestrel to serve up static files, like .css, .js files, you must enable the static files middleware, that is done with app.UseStaticFiles(). You would typically want it to occur before MVC in the middleware pipeline, so that MVC routing isn't being applied to those files. That middleware by default looks for files in the wwwroot/ folder and if the url being requested can be matched in wwwroot/ it will be returned by the static files middleware and any other middleware will be skipped (e.g. next won't be called).

Another useful middleware component is error handling. That can be enabled using app.UseDeveloperExceptionPage() or app.UseExceptionHandler("/error") along with the Microsoft.AspNetCore.Diagnostics nuget package. You typically want that before anything else in the pipeline that you want to catch errors for. Remember if it is first in the pipeline for generating a response that means it is last in the pipeline for modifying a response. For errors, you want it to be the first in the pipeline, so it can catch any errors generated from anything else later in the pipeline and generate a useful error message for the user. For more details about middleware, see the [official documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware).

Environments
------------
We were just talking about handling exceptions using middleware. The Microsoft.AspNetCore.Diagnostics nuget package provides two methods:
  1. app.UseDeveloperExceptionPage()
  2. app.UseExceptionHandler("/error")

The first option, UseDeveloperExceptionPage, will display full stack trace details and other useful information when any exception occurs later in the middleware pipeline. However only developers should see this, when running in production that would be a security risk to expose that information. The other method UseExceptionHandler("/error") will simply render the given url to the user (e.g. likely a razor page from MVC) that would log the error and simply inform the user to contact an IT helpdesk for more information. So, depending on whether we are running in development or production will determine which middleware component we should add to the pipeline. The Configure method has an argument passed to it, called IHostingEnvironment. That interface has methods such as .IsDevelopment() and .IsProduction(). So, we could add code like this to our Configure method, remember to add it at the top so we catch any exceptions later in the pipeline:

```
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }
```    

Now run the application and navigate to "/Home/ThrowEx". You will see a detailed error page with the full stack trace and details. That is because we are currently running in *Development* mode. You can also tell that because when the page initial loads for "/", it lists the name of the current environment. So, how does IHostingEnvironment know that it is the development environment? Well, the ASP.NET Core implementation of that interface defaults to a production environment, unless there is an environment variable with a name of ASPNETCORE_ENVIRONMENT. If that environment variable exists and has a value of "Development" then the environment is considered development. If that environment variable has a value of "Staging" then staging, if "Production" then production. You can define a custom environment as well, but then the helper methods like .IsDevelopment(), .IsStaging(), .IsProduction() won't work obviously, you would have to use the .EnvironmentName property. New to Visual Studio 2017 is the ability to add/edit environment variables for the given launch configuration. You can find this either by right-clicking on the project for "Properties..." and then going to Debug or by opening up launchSettings.json directly in the Solution Explorer (it's under the Properties node of the Project). Try changing the environment variable value or removing it and then going to the "Home/ThrowEx" to see if you now see the generic error page instead of the detailed error page.

So, to recap you can in the configuration code of Startup.cs, use IHostingEnvironment to determine if the app is running in development, staging or production and then configure the application including middleware correctly. To update the environment you must create an environment variable called ASPNETCORE_ENVIRONMENT with the appropriate value. Visual Studio 2017 now has an easy way to configure environment variables for a given launch configuration or edit launchSettings.json.

Configuration
-------------
Now, you may be wondering about configuration setting values at this point. Where did the <appSettings> and <connectionStrings> sections of Web.config go? Do I hard-code that into the application now? Do I still do transforms? How?

So, as you see the configuration is really done in application code, mostly in Startup.cs. But since it's code it can defer to anywhere else really to get values. As an aside the code can be organized as well, it **doesn't have to be a 2000 line Startup.cs file**.

So, the ASP.NET Core team wrote a configuration library, specifically "Microsoft.Extensions.Configuration" nuget package. You could use it any .NET application. You get an IConfiguration interface or an IConfigurationRoot interface. Your create an instance of that interface using the ConfigurationBuilder instance. You then add any number of configuration sources to the builder and then call .Build() to get back an instance of IConfigurationRoot which could then be down cast to IConfiguration.

Let's look at the simplest possible example of a working configuration. Replace the code in the Startup.cs constructor to be the following:

```
    var hardCodedConfig = new Dictionary<string, string> {
        { "SampleKey", "Value assigned in-memory in Startup.cs" },
        { "SampleSection:NestedKey", "NestedKey value for SampleSection assigned in-memory in Startup.cs" }
    };
    var builder = new ConfigurationBuilder()
        .AddInMemoryCollection(hardCodedConfig);
    Configuration = builder.Build();
```

You can see that I add an in-memory configuration source and provided initial data from a Dictionary.

Now, if you look in the ConfigureServices method, I register the Configuration instance into the built-in dependency injection container for any code that requests an IConfiguration interface, specifically this line in ConfigureServices method:

```
    services.AddSingleton<IConfiguration>(Configuration);
```

Now if you look at HomeController.cs in the Controllers folder, you will see the constructor takes an IConfiguration constructor argument. That is then stored on the class and referenced in the Index controller action method which is how you see the value when running the web application. If you run the web application, you will now the values on the home page.

So, looking at HomeController.cs, IConfiguration is pretty close to what we had before in legacy ASP.NET with ConfigurationManager.AppSettings["SampleKey"]. However, now we are using an interface and have an implementation injected instead of using static variables.

Now, going back to Startup.cs, the nice part is that we can add lots of configuration sources, including custom code. So, let's now add environment variables. Update the Startup.cs constructor to look like this:

```
    var hardCodedConfig = new Dictionary<string, string> {
        { "SampleKey", "Value assigned in-memory in Startup.cs" },
        { "SampleSection:NestedKey", "NestedKey value for SampleSection assigned in-memory in Startup.cs" }
    };
    var builder = new ConfigurationBuilder()
        .AddInMemoryCollection(hardCodedConfig)
        .AddEnvironmentVariables();
    Configuration = builder.Build();
```

All I changed was adding the .AddEnvironmentVariables() line. Now you should be able to update the launch configuration and add a SAMPLEKEY environment variable. Now if you run the app you will see the environment variable value win over the hard-coded value. That is because of the order we used for the configuration sources, .AddInMemoryCollection is first, followed by .AddEnvironmentVariables. If you wanted to set the nested key as an environment variable, the name of the variable would be, SAMPLESECTION:NESTEDKEY. The environment variable names are case-insensitive.

There are a number of provided configuration sources provided by Microsoft, plus you can write your own configuration source. Let's look at adding a json file as a configuration source. Modify the Startup.cs constructor to look like:

```
    var hardCodedConfig = new Dictionary<string, string> {
        { "SampleKey", "Value assigned in-memory in Startup.cs" },
        { "SampleSection:NestedKey", "NestedKey value for SampleSection assigned in-memory in Startup.cs" }
    };
    var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddInMemoryCollection(hardCodedConfig)
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
    Configuration = builder.Build();
```                

Remove the previously set environment variables and create an appsettings.json in the same directory as Startup.cs with the following content:

```
{
  "SampleKey": "Sample value from JSON file",
  "SampleSection": {
    "NestedKey":  "Also from JSON file"
  }
}
```

If you look at the code change to Startup.cs, we made two changes, the call to SetBasePath(env.ContentRootPath) and the .AddJsonFile("appsettings.json").  The SetBasePath is used so that the ConfigurationBuilder knows where on the filesystem to look for files. The AddJsonFile obviously adds the appsettings.json as a configuration source. Run the application now and you should see the values from the JSON file. You can also override those as environment variables in the launchSettings.json, again because of the order we have specified for the configuration sources.

Now, let's talk about settings for each different environment. Let's modify the Startup.cs as shown below:

```
    var hardCodedConfig = new Dictionary<string, string> {
        { "SampleKey", "Value assigned in-memory in Startup.cs" },
        { "SampleSection:NestedKey", "NestedKey value for SampleSection assigned in-memory in Startup.cs" }
    };
    var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddInMemoryCollection(hardCodedConfig)
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
        .AddEnvironmentVariables();
    Configuration = builder.Build();
```

and create an appsettings.Development.json in the same directory as Startup.cs with the following content:

```
{
  "SampleKey": "Development value from JSON file"
}
```

The only change we made this time was adding a .AddJsonFile($"appsettings.{env.EnvironmentName}.json") line to the configuration. This now means we will read from a file like appsettings.Development.json or appsettings.Production.json depending on the environment. And this is done **after** we read from appsettings.json. If the file doesn't exist that is fine, no error is thrown.

Secrets
-------
Now what about secrets such as passwords or connection strings? We could just reference a secrets.json and then make sure we don't check it into source control. This is the general approach that most teams took with legacy ASP.NET. The concern is that the file is on the filesystem and could accidentally be checked into source control. For developers, Microsoft has written a configuration source for user secrets. To enable it, you need to update the .csproj to add three lines:

```
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="1.1.1" />
    <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="1.0.0" />    
  </ItemGroup>
  <PropertyGroup>
    <!-- put your own secret in here -->
    <UserSecretsId>TrinugSampleApp-2679ad45-f497-440f-9ad3-fed98ab1199d</UserSecretsId>  
  </PropertyGroup>
```

This adds a nuget package reference which is the User Secrets configuration source.  We also a command line tool for the dotnet command-line. Lastly, we need to configure a unique ID for this secrets configuration source. This can be anything, but is typically a Guid.

You should then make your Startup.cs constructor look like:

```
    var hardCodedConfig = new Dictionary<string, string> {
        { "SampleKey", "Value assigned in-memory in Startup.cs" },
        { "SampleSection:NestedKey", "NestedKey value for SampleSection assigned in-memory in Startup.cs" }
    };
    var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddInMemoryCollection(hardCodedConfig)
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json");
    if (env.IsDevelopment())
    {
        builder.AddUserSecrets<Startup>();
    }
    builder.AddEnvironmentVariables();
    Configuration = builder.Build();
```            

We added the builder.AddUserSecrets<Startup>() inside the if (env.IsDevelopment()). Now, if you run the application you won't notice any differences. That's because we don't have any secrets loaded into the user secrets store.

Open a command prompt in the same directory as the .csproj file and you can now run:

```
dotnet user-secrets --help
```

To list all configured secrets use:
```
dotnet user-secrets list
```

You can set a value using
```
dotnet user-secrets set [key] [value]
```

Go ahead and use that to set a value for SampleKey using
```
dotnet user-secrets set SampleKey SuperSecretValue
```

Now, if you run the application you will see the value from the user secret store. We've configured the application in Startup.cs to only use this store if the application is in development mode. In production, you would likely set environment variables and the application will read configuration from environment variables last.

If you want to know where the secrets are stored, they are stored clear-text in a file in your user directory. On Windows, this would be C:\Users\[user profile]\AppData\Roaming\Microsoft\UserSecrets\[userSecretsGuid]\secrets.json. The secrets are stored clear-text, but if you were using the secrets.json or secrets.config method in legacy ASP.NET, you were already storing secrets on the filesystem in clear text. The larger point here is that developers don't have a file containing secrets they might accidentally commit or copy around.


Hosting Configuration
---------------------
TBD    
