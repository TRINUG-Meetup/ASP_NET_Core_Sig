Pre-reqs
--------
This session assumes you have read the corresponding chapters of the book and have some familiarity with C# and ASP.Net MVC development. If not, Microsoft Virtual Academy has a number of free courses with videos and associated sample code:
  - [Introduction to ASP.Net Core](https://mva.microsoft.com/en-US/training-courses/introduction-to-aspnet-core-10-16841?l=JWZaodE6C_5706218965)
  - [Intermediate ASP.Net Core](https://mva.microsoft.com/en-US/training-courses/intermediate-aspnet-core-10-16964?l=Kvl35KmJD_4306218965)
  - [ASP.Net Core 1.0 Cross-Platform](https://mva.microsoft.com/en-US/training-courses/intermediate-aspnet-core-10-16964?l=Kvl35KmJD_4306218965)

Hello, World
------------
To start, you can use DotNet Core, which does not require Visual Studio to be installed.

Go to http://dot.net/ and click on "Download", then click on ".Net Core". You can then click on the .NET Core 1.0.3 SDK - Installer, either the Windows x86 or Windows x64 link, depending on whether you are running Windows 32-bit or Windows 64-bit. Once downloaded, click to run the installer. This is all you will require.

If you want to use Visual Studio, scroll down and click on the "Windows" step by step instructions. This will prompt you to install Visual Studio 2015, then install Visual Studio 2015 Update 3, then download and install an extension for Visual Studio. That is a multiple hour process, I recommend just downloading the SDK installer and installing it.

To create a new console app, do the following on the command-line:
```
mkdir ConsoleTest
cd ConsoleTest
dotnet new
dotnet restore
dotnet run
```

For documentation on the dotnet command-line, either type:
```
dotnet --help
```
or for details on a command, the name of the command and then --help:
```
dotnet new --help
```

For complete documentation on the dotnet command-line, go to: https://docs.microsoft.com/en-us/dotnet/articles/core/

To create a sample web application, do the following on the command-line:
```
mkdir WebTest
cd WebTest
dotnet new -t Web
dotnet restore
dotnet run
```

Then open a web browser and go to http://localhost:5000/ to see the web app running.

For documentation on ASP.Net itself, see: https://docs.microsoft.com/en-us/aspnet/core/

At this point, you could switch over to the SimpleAspNetCoreApp-README.md to see how to create a simple ASP.Net Core application that does a little more than the starter applicatino does.

Multiple versions of .Net Core, project.json and .csproj
--------------------------------------------
You can have multiple versions of the .Net Core SDK installed on the same machine. This was a problem with full .NET Framework, but is fully supported with .Net Core. On Windows if you want to see what versions you have installed, go to C:\Program Files\dotnet\sdk folder or C:\Program Files (x86)\dotnet\sdk folder. You will have a subfolder there for each version of the SDK that you have installed. I want to point out that the SDK also bundles the .Net Core runtime. The Runtime itself has both a long-term supported 1.0.X line and a newer 1.1.X line. The runtime is production quality. The SDK which is what contains the build system and commands like dotnet new, dotnet run is still in preview. That is why you see the words preview in the sdk/ folder even though the runtime itself is production quality.

The sdks which contain the words, preview2 utilize project.json as the build system. This is what when you download the SDKs currently for .NET Core 1.0.3 or 1.1.0, or if you use Visual Studio 2015.

The sdks which contain the words, preview4 or rc3, have removed project.json and replaced with .csproj format. You would currently get this SDK if you installed VS 2017 RC.

The SDKs are not compatible, the ones that use project.json have no idea how to build dotnet new projects with .csproj files and vice-versa. You can check which version of the SDK you are using by running the following command in the directory where your project is:
```
dotnet --info
```

The two SDKs being incompatible also means a project created in VS 2015 will prompt you to be upgraded in VS 2017 RC. A project created in VS 2017 RC cannot be opened in VS 2015.

project.json is being removed and replaced with .csproj. If you download the .NET Core 1.0.3 you will end up using project.json. So, how will migration happen? The new SDKs that use .csproj add a new command to the dotnet command-line called dotnet migrate. This command will automatically upgrade a project.json into a .csproj file.

When you utilize the dotnet command-line by default you will use the latest SDK on your system, e.g. the latest version in your C:\Program Files\dotnet\sdk\ folder. If you want to control which version of the SDK that is used, simply create a global.json file, similiar to this:
```
{
  "sdk": {
    "version": "1.0.0-preview2-003156"
  }
}
```

What happens when run the dotnet command is it will look in the current directory for a global.json. If it cannot find one, it'll look in the parent directory for global.json, and then that parent, etc. until it reaches the root of the drive. If it finds a global.json, it will use the sdk/version setting and try and locate that in the C:\Program Files\dotnet\sdk folder. If it locates that sdk, it will use that SDK. This allows you install multiple SDKs on a single system but still decide which SDK is used for which project.

NET Standard
------------
At this point there are multiple .NET runtimes: Full .NET Framework (e.g. the original), .NET Core (cross platform to Windows, Mac, Linux), Xamarin runtime for supporting Android/iOs, Mono runtime for supporting Linux, UWP Runtime (for Universal Windows Apps), Unity runtime for targeting devices that Unity game engine supports, etc.

If building a library these runtimes were typically supported either by creating multiple assemblies for the multiple runtimes or by creating portable class libraries that were portable across multiple runtimes.

There is a new effort to make supporting multiple runtimes easier, .NET Standard. .NET Standard is a specification, e.g. a document and some associated shim assemblies. Each runtime will then support certain versions of .NET Standard. So, now you can target a given .NET Standard, compile once and your compiled assembly will work on any runtime that supports that version of .NET Standard.

You can find more details here:
https://github.com/dotnet/standard

To see the actual spec, e.g. the APIs available in a given version of .NET Standard, you can go the respective page:
  * https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.0_ref.md
  * https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.4_ref.md
  
By remember, .NET Standard is only a spec. A given runtime, e.g. .NET Core or Full Framework actually decides if it implements the spec or not. You don't have to target the spec, you can still target a particular runtime.

Further details can also be found at: https://docs.microsoft.com/en-us/dotnet/articles/core/tutorials/libraries
