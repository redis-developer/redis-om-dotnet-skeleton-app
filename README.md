# Redis OM .NET Skeleton ASP.NET Core App

Welcome to the Redis OM .NET Skeleton App, this app is designed to provide you a jumping off point to add Redis OM .NET to your ASP.NET Core app, and get you up and running with some basic CRUD operations.

## How it works

### Dependency Inject the RedisConnectionProvider

In Program.cs, we pull the connection string out of the configuration and inject a new `RedisConnectionProvider` into the service container:

```csharp
var redisConnectionString = builder.Configuration["REDIS_CONNECTION_STRING"] ?? "redis://localhost:6379";
builder.Services.AddSingleton(new RedisConnectionProvider(redisConnectionString));
```

### Build our Person model

In order to model and index data with Redis OM, we just need to create a model class and decorate it with a couple of attributes. We created a `Person` class in `Person.cs`. To declare that the class was an object we wanted to be able to model as a document to be stored in redis, we decorated it with the `Document` attribute.

```csharp
[Document(Prefixes = new []{"Person"})]
public class Person
```

We add the Person prefix to override the default prefix of the type, which would be the fully qualified class name.

#### Index Fields

We'll add a few fields to our `Person` class to make it searchable. Anything that we want to do a general equality, or range search on we'll mark with `Indexed`, any string fields we want to do a full text search on we'll mark with `Searchable`

```csharp
[RedisIdField] [Indexed]public string? Id { get; set; }
[Indexed] public string FirstName { get; set; }
[Indexed] public string LastName { get; set; }
[Indexed] public int Age { get; set; }
[Searchable] public string PersonalStatement { get; set; }
[Indexed] public GeoLoc HomeLoc { get; set; }
```

### Build the API controller

We set up a simple CRUD API controller for our model in `Controllers/PeopleController.cs`

#### Pull a RedisCollection out of the provider

Next, in the PeopleController, we'll need to pull out the `RedisConnectionProvider`, and pull a `RedisCollection<Person>` out of the provider as well:

```csharp
[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    
    private readonly RedisCollection<Person> _people;
    private readonly RedisConnectionProvider _provider;
    public PeopleController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _people = (RedisCollection<Person>)provider.RedisCollection<Person>();
    }
```

#### Add a Person

To add a person, we have a `POST` method that calls `InsertAsync` on the `RedisCollection<Person>`:

```csharp
[HttpPost]
public async Task<Person> AddPerson([FromBody] Person person)
{
    await _people.InsertAsync(person);
    return person;
}
```

`InsertAsync` will create a ULID for the Person and add the whole person object to Redis.

#### Filter by Age

We have a `GET` method that takes a minimum and maximum age and returns all the people within that age range, it's as simple as executing a trivial LINQ statement:

```csharp
[HttpGet("filterAge")]
public IList<Person> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
{
    
    return _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
}
```

#### Filter by Name

We also have a `GET` method to query by first and last name:

```csharp
[HttpGet("filterName")]
public IList<Person> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
{
    return _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
}
```

#### Filter by Location 

And a method to filter by location:

```csharp
[HttpGet("filterGeo")]
public IList<Person> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius, [FromQuery] string unit)
{
    return _people.GeoFilter(x => x.HomeLoc, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToList();
}
```

#### Full Text Search

We'll also add a route that will let us do full text search on our `PersonalStatement` property in the `Person` class. Matching against a full-text search is as easy as doing a comparison: `==`

```csharp
[HttpGet("fullText")]
public IList<Person> FilterByPersonalStatement([FromQuery] string text){
    return _people.Where(x => x.PersonalStatement == text).ToList();
}
```

#### Updating

Next, we'll do some light updating, to update an object in Redis OM, you'll first enumerate the object in the Redis collection, make the desired changes, then call `Save` on the collection:

```csharp
[HttpPatch("updateAge/{id}")]
public IActionResult UpdateAge([FromRoute] string id, [FromBody] int newAge)
{
    foreach (var person in _people.Where(x => x.Id == id))
    {
        person.Age = newAge;
    }
    _people.Save();
    return Accepted();
}
```

#### Deleting

To delete something from Redis using Redis.OM, you'll need the key's name, to get the key's name, take the Id and concatanate it to the end of the string `<PREFIX>:`, you'll remember earlier that we made the prefix `Person`:

```csharp
[HttpDelete("{id}")]
public IActionResult DeletePerson([FromRoute] string id)
{
    _provider.Connection.Unlink($"Person:{id}");
    return NoContent();
}
```

## Run the App

### Spin up Redis

You'll want to spin up Redis before running the app, to do so you can use the [Redis Cloud](https://app.redislabs.com/), or you can just run `docker run -p 6379:6379 redislabs/redismod`

### Configure the App

If you are running in docker, or you're running Redis locally you probably don't need to do anything here, if you are running anywhere else you'll just need to update the `REDIS_CONNECTION_STRING` field in `appsettings.json` to an [appropriately formatted Redis URI](https://developer.redis.com/develop/dotnet/redis-om-dotnet/connecting-to-redis)

### Run it

Now just run `dotnet run` to run the app, this will start the app up on `https://localhost:7090`.

### Interact with the API

You can interact with the API either directly through the [Swagger interface](https://localhost:7090/swagger/index.html)