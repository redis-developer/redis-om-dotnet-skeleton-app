using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations;
namespace Redis.OM.Skeleton;

[Document(Prefixes = new []{"Person"})]
public class Person
{
    // Id Field, also indexed, marked as nullable to pass validation
    [RedisIdField] [Indexed]public string? Id { get; set; }

    // Indexed for exact text matching
    [Indexed] public string FirstName { get; set; }
    [Indexed] public string LastName { get; set; }

    //Indexed for numeric matches
    [Indexed] public int Age { get; set; }

    //Indexed for Full Text matches
    [Searchable] public string PersonalStatement { get; set; }

    //Indexed for Geo Filtering
    [Indexed] public GeoLoc HomeLoc { get; set; }
}