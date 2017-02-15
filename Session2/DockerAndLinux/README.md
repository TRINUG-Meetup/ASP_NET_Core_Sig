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

You can create containers locally or you can run containers directly from Docker Hub. Let's look at that.

## Run our sample app from last session as Docker image

I took our source code from last session, found in the Session1/ folder and created a Dockerfile for it. The Dockerfile uses a Linux image as a base. If you look in the folder I also have a Dockerfile.windows, that is demo'd later and uses a Windows OS as the base image. Once I built the image, I then pushed it to Docker Hub. Anyone can create their own repo on Docker Hub and push public images. You can also push private images, but there are limits to how many free private images you can have.

To run our sample from last session, simply run:

```
docker run -p 8080:5000 kstreith/simple-aspnetcore:session1
```

This downloads the image and any other required base images and then starts the container. The ASP.Net Core app running in the container is bound to port 5000. We need to bind that port to one on the actual host, that is what -p does. The 8080 is the port on the host, the 5000 is the port inside the container. You can then open your browser to *http://localhost:8080/* and see our app running.

If you press Ctrl+C in the command prompt, you can still can back to the browser, refresh the page and see that the container is still running. You can verify that using *docker ps* command. If you go back to the browser, feel free to add some users. Then go the command prompt and do:

```
docker stop [container id/container name]
```

You see the container id and name in the output of *docker ps*. This stops the container. Now if you go back to the browser and refresh the page, it won't refresh because the container isn't running. You can restart that same container using:

```
docker start [container id/container name]
```

This is an important distinction, *docker run* always creates a brand-new container from the requested image and then runs that brand-new container. You can actually run the same image in two different containers side-by-side, let's do that with:

```
docker run -d -p 8082:5000 kstreith/simple-aspnetcore:session1
```
 The -d runs this is daemon mode, which means we immediately return to the command prompt even though the container is still running in the background. At this point you have two instances of the sample app running, one on http://localhost:8080/ with custom added users, another on http://localhost:8082/ with only the default hard-coded users. Go ahead and access both in the browser, they have different users because they are different independent containers both based upon the same original image. They have different data because our ChoreRepository stores data in-memory and then it persists to the local filesystem in the App_Data/ folder.

## Multiple versions of the same image

Let's quickly look at how we can version and tag images. Let's run a console app I put up on Docker Hub, with the following command:

```
docker run kstreith/hello-dotnet
```

You will see the output, *Hello TRINUG Meetup!*. I actually have two versions of this image, a 1.0 and a 2.0. I have tagged the 2.0 version as latest, if you don't specify a tag after the :, Docker assumes it is latest. Try running the 1.0 version of this image with:

```
docker run kstreith/hello-dotnet:1.0
```

You will see the output as, *Hello World*, which was the initial version of this image. You can also specifically request the 2.0 version using:

```
docker run kstreith/hello-dotnet:2.0
```

If you go back to our earlier example of running our sample ASP.Net Core app, you'll remember it was :session1, so *session1* was the version. Docker doesn't specifically have versions it is really just tags and the default tag is *latest*. So simply keep the *latest* tag always pointed at the most recent version of your image, but feel free to use whatever tagging conventions you want to use.

## Run Wordpress install using Docker

You can really run any console or server application in a Docker container. This includes any console or server app technology that runs on Linux, including things like PHP, MariaDB (variant of MySQL), etc. To me this is a giant advantage of Docker images and containers. An operations team for an organization only needs how to start, stop and connect Docker containers. There is less concern for how to install technology stacks, that is only needed by the creator of the image. So, for this example we want to run a Wordpress installation. That requires PHP and MariaDB (MySQL variant) and then installing Wordpress on top. I don't know how to install any of those three things. However, there are images on Docker Hub, one that includes PHP & Wordpress, another that includes MariaDB. Even better, Docker provides a tool to start multiple containers at the same time and then wire those containers together as a unit. This tool is Docker compose. If you go the official Wordpress repo on DockerHub at: https://hub.docker.com/_/wordpress/ and then scroll down you see details on Docker compose. You simply copy that into a docker-compose.yml file. I've already done that in the Wordpress/ subfolder of this repository.

You then simply open a command prompt to that folder and execute:

```
docker-compose up
```

This creates brand-new containers the first time, one for Wordpress, one for MariaDB and then will wire them up. The creators of each image respectively dealt with how to install of those technologies into the respective images and then put those images on DockerHub.

Once running, you can go to http://localhost:8080/ and see a newly created Wordpress installation. The port 8080 is found in the docker-compose.yml if you want to use a different port. You can stop the containers, leaving their data intact using Ctrl+C.
If you want to run without blocking the command prompt, use daemon mode:
```
docker-compose up -d
```

In order to stop the containers but keep the data in the containers, use:

```
docker-compose stop
```

In order to delete the containers and basically start with a fresh Wordpress install again, use:

```
docker-compose down
```

But notice, I didn't really have to understand how to install PHP, how to install Wordpress, how to install MariaDB. I just had to wire those containers up, in my case I even copied that from the Wordpress page on Docker Hub.

## Running container in Azure

Azure has just recently added an App Service on Linux, it's currently in preview. This new service allows you to point to a Docker image and Azure will handle allocating the resources and pulling the image from the repository (either Docker Hub or a private repository) and starting the container. So, let's demo that in Azure and run our Sample ASP.NET Core app from last session in Azure using the image I've put on Docker Hub.

The Docker Hub image name is 'kstreith/simple-aspnetcore:session1'. Once the App Service is running, you need to go into Application Settings and add a setting for PORT with a value of 5000 and then restart the App Service. This is because the service doesn't know which port inside the container to map to port 80 on the App Service. We inform the service that our container interally uses port 5000 by setting the environment variable named PORT to a value of 5000.

## What about Windows containers?

I have created Windows container versions of the Hello, World console app and our sample ASP.Net Core app from Session1. First, you must switch Docker over to running Windows containers. Right-click on the Docker icon in the system tray and select "Switch to Windows containers..." in the menu. Once Docker has switch over, you can use the same command prompt you were using before.

Let's run our hello world image using:

```
docker run kstreith/hello-dotnet-nanoserver
```

You should see the Hello, World message.

Let's run our sample ASP.Net Core app from last session using:

```
docker run -d kstreith/simple-aspnetcore:session1-nanoserver
```

Notice I didn't use -p to map ports that because currently this feature is waiting on updates to the Windows OS networking stack that is still forthcoming. You need to query Docker to get the internal ip address the container is running under, do that using:

```
docker inspect --format '{{ .NetworkSettings.Networks.nat.IPAddress }}' [container id/container name]
```

This will give you the ip address the container is using, then open the browser to http://[ip]:5000/

This limitation in the Windows network stack will be addressed in a future Windows release.

I created those images using the Dockerfile.windows you find in the Session1/ and Session2/ folder. If you compare with the Dockfile for Linux the only difference is the base image referenced in the FROM command in the Dockerfile, otherwise it is the same Dockerfile.

