FROM microsoft/dotnet:1.0-sdk-projectjson
WORKDIR /app

COPY . .
RUN dotnet restore

CMD dotnet run
