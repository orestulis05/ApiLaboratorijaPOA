var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // kazkas jei tik developmente reiks
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () => "All good and running.")
    .WithName("Health");

app.Run();
