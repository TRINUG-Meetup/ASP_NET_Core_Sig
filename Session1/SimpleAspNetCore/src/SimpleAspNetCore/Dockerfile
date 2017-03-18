FROM microsoft/dotnet:1.0-sdk
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet build

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT dotnet run