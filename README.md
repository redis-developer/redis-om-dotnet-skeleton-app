# Redis OM .NET Skeleton ASP.NET Core App

Welcome to the Redis OM .NET Skeleton App, this app is designed to provide you a jumping off point to add Redis OM .NET to your ASP.NET Core app, and get you up and running with some basic CRUD operations. For a detailed explanation of how everything in the app works, read through the [App walkthrough](https://github.com/redis-developer/redis-om-dotnet-skeleton-app/wiki/App-walkthrough).

## Prerequisites

* [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* A means of running Redis, this might be a [Redis Cloud Account](https://app.redislabs.com/), or even just [Docker desktop](https://www.docker.com/products/docker-desktop)

## Run the App

### Spin up Redis

You'll want to spin up Redis before running the app, to do so you can use the [Redis Cloud](https://app.redislabs.com/), or you can just run `docker run -p 6379:6379 redislabs/redismod`

### Configure the App

If you are running in Docker, or you're running Redis locally you probably don't need to do anything here. If you are running anywhere else you'll just need to update the `REDIS_CONNECTION_STRING` field in `appsettings.json` to an [appropriately formatted Redis URI](https://developer.redis.com/develop/dotnet/redis-om-dotnet/connecting-to-redis).

### Run it

Now just run `dotnet run` to run the app, this will start the app up on `https://localhost:7090`.

### Interact with the API

You can interact with the API either directly through the [Swagger interface](https://localhost:7090/swagger/index.html).
