using ApiLaboratorija.Data;
using ApiLaboratorija.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        Console.WriteLine("Migrating database...");
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Migration complete. Database is up to date.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex}");
        throw;
    }
}

// for ui endpoint testin
app.UseSwagger();
app.UseSwaggerUI();

var api = app.MapGroup("/api");

api.MapGet("/health", () => "All good and running.")
    .WithName("Health");

#region Users
api.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());
api.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    return user is null 
        ? Results.NotFound() 
        : Results.Ok(user);
});

api.MapPost("/users", async (UserDto body, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(body.Name) || body.Age is null or < 0 or > 100)
        return Results.BadRequest();
   
    var user = new User(body.Name, body.Age.Value);
    await db.Users.AddAsync(user);
    
    await db.SaveChangesAsync();
    return Results.Ok(user);
});
api.MapPut("/users/{id}", async (int id, UserDto body, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    if (user is null)
        return Results.NotFound();
    
    if (!string.IsNullOrWhiteSpace(body.Name))
        user.Name = body.Name;
    if (body.Age is > 0 and < 100)
        user.Age = body.Age.Value;
    
    await db.SaveChangesAsync();
    return Results.Ok(user);
});
api.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    if (user is null)
        return Results.NotFound();
    
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
#endregion

app.Run();
