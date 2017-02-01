This session assumes you have read the corresponding chapters of the book and have some familiarity with C# and ASP.Net MVC development. If not, Microsoft Virtual Academy has a number of free courses with videos and associated sample code:
  - [Introduction to ASP.Net Core](https://mva.microsoft.com/en-US/training-courses/introduction-to-aspnet-core-10-16841?l=JWZaodE6C_5706218965)
  - [Intermediate ASP.Net Core](https://mva.microsoft.com/en-US/training-courses/intermediate-aspnet-core-10-16964?l=Kvl35KmJD_4306218965)
  - [ASP.Net Core 1.0 Cross-Platform](https://mva.microsoft.com/en-US/training-courses/intermediate-aspnet-core-10-16964?l=Kvl35KmJD_4306218965)

To start, you can use DotNet Core, which does not require Visual Studio to be installed.

Go to http://dot.net/ and click on "Download", then click on ".Net Core". You can then click on the .NET Core 1.0.3 SDK - Installer, either the Windows x86 or Windows x64 link, depending on whether you are running Windows 32-bit or Windows 64-bit. Once downloaded, click to run the installer. This is all you will require.

If you want to use Visual Studio, scoll down and click on the "Windows" step by step instructions. This will prompt you to install Visual Studio 2015, then install Visual Studio 2015 Update 3, then download and install an extension for Visual Studio. That is a multiple hour process, I recommend just downloading the SDK installer and installing it.

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
