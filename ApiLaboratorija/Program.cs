var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// for ui endpoint testin
app.UseSwagger();
app.UseSwaggerUI();

// mock db table
var users = new List<User>
{
    new (1, "Petras", 30),
    new (2, "Juozas", 25),
    new (3, "Orestas", 28)
};

var api = app.MapGroup("/api");

api.MapGet("/health", () => "All good and running.")
    .WithName("Health");

#region Users
api.MapGet("/users", () => users);
api.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
        return Results.NotFound();
    
    return Results.Ok(user);
});
api.MapPost("/users", (UserDto body) =>
{
    if (string.IsNullOrWhiteSpace(body.Name) || body.Age is null or < 0 or > 100)
        return Results.BadRequest();
    
    var user = new User(users.Count + 1, body.Name, body.Age.Value);
    users.Add(user);
    return Results.Ok(user);
});
api.MapPut("/users/{id}", (int id, UserDto body) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null)
        return Results.NotFound();
    
    if (!string.IsNullOrWhiteSpace(body.Name))
        user.Name = body.Name;
    if (body.Age is > 0 and < 100)
        user.Age = body.Age.Value;
    
    return Results.Ok(user);
});
api.MapDelete("/users/{id}", (int id) => users.RemoveAll(u => u.Id == id));
#endregion

app.Run();

class User(int id, string name, int age)
{
    public int Id { get; } = id;
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;
}
record UserDto(string? Name, int? Age);