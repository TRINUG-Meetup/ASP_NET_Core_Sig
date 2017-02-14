# Linux
On Windows, use a virtualization technology. If you have Windows 8 or later professional version, Hyper-V is built-in. Otherwise, use VirtualBox for free.

I demo'd Ubuntu 16.04 and RedHat 7.2 using Hyper-V.

If you want a guided installation, create a RedHat developer account (free) and they will guide you through configuring the virtualization technology and installing RedHat. RedHat is an official partner of Microsoft.

Once you have Linux configured and running with internet access in a VM. You can go to http://dot.net/ and follow instructions to install .NET Core on Linux, there are instructions for RedHat and Ubuntu and for different versions of .NET Core. I installed the 1.0.3 version of .NET Core.

I also installed VS code, https://code.visualstudio.com/ clicking on the appropriate Linux package.

I created a new sample project using the Terminal and the following:

```
mkdir ConsoleTest
dotnet new
dotnet restore
dotnet run
```

This should create Program.cs and project.json and run it (if .NET Core 1.0.3 was installed)

You can then open the folder with project.json in Visual Studio Code using:

```
code .
```

VS Code will prompt to install C# extension. After it's installed, you need to edit launch.json to put the full path to the resulting .dll in. At that point you can use VS Code to build and run the application and to set breakpoints and use the debugger in VS Code. It's a very similiar experience to full Visual Studio on Windows. You also get Intellisense in the VS Code Editor.

# Docker

## Installing Docker
Go to https://www.docker.com/products/overview and download for your platform

I highly recommend installing directly on Windows, but this requires Windows 10 Professional or Enterprise 64-bit and must have the Anniversary update installed
In addition, Hyper-V will be enabled, so please **disable** any other virtualization techology (VirtualBox, VMWare) first before installing Docker. Once installed you can go to the system tray and right-click to find "About Docker...". I recommend having 1.13 or later installed. This version supports both Linux containers and Windows containers. This is also why I recommend running Docker directly on Windows 10, you get a chance to try both technologies.

If you don't have Windows 10, but another version of Windows you can first create a Linux VM as discussed above and then follow instructions for installing Docker on Linux.

### Testing Docker Install

Open command prompt and type
```
docker run hello-world
```

You should see Hello from Docker in the output, it will take a few minutes the first time you run it. It is downloading the required Docker images from Docker Hub. When you run it again later, the images are already cached and it will simply run the image.

## Creating our first .NET Core container on Linux

In the HelloWorldDotNet folder, I've already created a new .NET Core Console application using *dotnet new*, so we have Program.cs and project.json.

We want to create a docker image that contains this program and then once we've built the image, we want to run it.

To create the image, we need a Dockerfile, so we create it with the following contents:

```
FROM microsoft/dotnet:1.0-sdk-projectjson
WORKDIR /app

COPY . .
RUN dotnet restore

CMD dotnet run
```

The first line is the image that we are basing our image on. The one I've chosen has the .NET Core SDK installed, the version that uses project.json and the 1.0 series of the .NET runtime. This image is based on another image, e.g. a Debian Linux variant. As you can see the image we are using is provided and supported by Microsoft. You can find a whole range of .NET Core Docker images on Docker Hub: https://hub.docker.com/r/microsoft/dotnet/ And more images for other Microsoft stacks at: https://hub.docker.com/u/microsoft/

The later lines copy the contents of our current directory into the image. Once copied we then run *dotnet restore* and *dotnet run* inside the image.

To build this image, in the directory you execute:
```
docker build -t hello-netcore .
```

The -t option tags the image with a name, this is more useful than just using the default guid. This will take a few minutes, it has to download the required base images and then execute *dotnet restore* and *dotnet run*.

Once built you can see the images using:

```
docker images
```

To run your image, use:
```
docker run hello-netcore
```

You will see the compilation output and the message 'Hello World'.

The act of running an image created a container, you can see currently running containers with:
```
docker ps
```

You won't see the previous execution because *dotnet run* start and finished. You have to execute:
```
docker ps -a
```
to see all containers, even ones that have already finished running.


