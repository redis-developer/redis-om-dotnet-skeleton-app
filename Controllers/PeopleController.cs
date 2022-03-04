using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IRedisCollection<Person> _people;

    public PeopleController(RedisConnectionProvider provider)
    {
        _people = provider.RedisCollection<Person>();
    }
}