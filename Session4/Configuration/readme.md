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

```
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="1.1.1" />    

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="1.0.0" />
  </ItemGroup>  
    
   <UserSecretsId>TrinugSampleApp-2679ad45-f497-440f-9ad3-fed98ab1199d</UserSecretsId>
    
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
    
            //app.Use(async (context, next) =>
            //{
            //    //if (context.Request.Path.StartsWithSegments("/Home")) {
            //    //    await next.Invoke();
            //    //    return;
            //    //}
            //    await context.Response.WriteAsync("Hello, World! Custom middleware here!");
            //});
    
            var builder = new ConfigurationBuilder().AddEnvironmentVariables("ASPNETCORE_");
    
                .UseUrls("http://localhost:8000")
                .UseConfiguration(builder.Build())
    
```    
