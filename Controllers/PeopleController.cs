using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

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

    /// <summary>
    /// Creates an indexed Person object in Redis
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<Person> AddPerson([FromBody] Person person)
    {
        await _people.InsertAsync(person);
        return person;
    }
    
    /// <summary>
    /// Filters People in Redis by their Age 
    /// </summary>
    /// <param name="minAge"></param>
    /// <param name="maxAge"></param>
    /// <returns></returns>
    [HttpGet("filterAge")]
    public IList<Person> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
    {
        
        return _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
    }

    /// <summary>
    /// Draws a circular geofilter around a spot and returns all people in that radius
    /// </summary>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    /// <param name="radius"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    [HttpGet("filterGeo")]
    public IList<Person> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius, [FromQuery] string unit)
    {
        return _people.GeoFilter(x => x.Address!.Location, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToList();
    }

    /// <summary>
    /// Filters people by their first and last name
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    [HttpGet("filterName")]
    public IList<Person> FilterByName([FromQuery] string firstName, [FromQuery] string lastName)
    {
        return _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
    }

    /// <summary>
    /// Performs full text search on a person's personal Statement
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [HttpGet("fullText")]
    public IList<Person> FilterByPersonalStatement([FromQuery] string text){
        return _people.Where(x => x.PersonalStatement == text).ToList();
    }

    /// <summary>
    /// Retrieves a people in a given postal code.
    /// </summary>
    /// <param name="postalCode"></param>
    /// <returns></returns>
    [HttpGet("postalCode")]
    public IList<Person> FilterByPostalCode([FromQuery] string postalCode)
    {
        return _people.Where(x => x.Address!.PostalCode == postalCode).ToList();
    }

    /// <summary>
    /// retrieves people who's street name matches a given street name
    /// </summary>
    /// <param name="streetName"></param>
    /// <returns></returns>
    [HttpGet("streetName")]
    public IList<Person> FilterByStreetName([FromQuery] string streetName)
    {
        return _people.Where(x => x.Address!.StreetName == streetName).ToList();
    }

    /// <summary>
    /// retrieves people who have a given skill
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    [HttpGet("skill")]
    public IList<Person> FilterBySkill([FromQuery] string skill)
    {
        return _people.Where(x => x.Skills.Contains(skill)).ToList();
    }

    /// <summary>
    /// Updates a person at a particular Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newAge"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Deletes a person at the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public IActionResult DeletePerson([FromRoute] string id)
    {
        _provider.Connection.Unlink($"Person:{id}");
        return NoContent();
    }
}