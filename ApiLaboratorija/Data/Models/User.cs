namespace ApiLaboratorija.Data.Models;

public class User(string name, int age)
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;
}

public record UserDto(string? Name, int? Age);