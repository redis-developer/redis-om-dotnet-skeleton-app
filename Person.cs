using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations;
namespace Redis.OM.Skeleton;

[Document(Prefixes = new []{"Person"})]
public class Person
{
    [RedisIdField] [Indexed]public string? Id { get; set; }
    [Indexed] public string FirstName { get; set; }
    [Indexed] public string LastName { get; set; }
    [Indexed] public int Age { get; set; }
    [Searchable] public string PersonalStatement { get; set; }
    [Indexed] public GeoLoc HomeLoc { get; set; }
}