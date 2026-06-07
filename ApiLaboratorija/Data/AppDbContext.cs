using ApiLaboratorija.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiLaboratorija.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();
}