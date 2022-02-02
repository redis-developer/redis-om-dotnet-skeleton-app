using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
namespace Redis.OM.Skeleton.Controllers;

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

    [HttpPost]
    public async Task<Person> AddPerson([FromBody] Person person)
    {
        await _people.InsertAsync(person);
        return person;
    }

    [HttpGet("filterAge")]
    public IList<Person> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
    {
        
        return _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
    }

    [HttpGet("filterGeo")]
    public IList<Person> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius, [FromQuery] string unit)
    {
        return _people.GeoFilter(x => x.HomeLoc, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToList();
    }

    [HttpGet("filterName")]
    public IList<Person> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
    {
        return _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
    }

    [HttpGet("fullText")]
    public IList<Person> FilterByPersonalStatement([FromQuery] string text){
        return _people.Where(x => x.PersonalStatement == text).ToList();
    }

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

    [HttpDelete("{id}")]
    public IActionResult DeletePerson([FromRoute] string id)
    {
        _provider.Connection.Unlink($"Person:{id}");
        return NoContent();
    }
}