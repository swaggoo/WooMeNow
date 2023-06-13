using Microsoft.EntityFrameworkCore;
using WooMeNow.API.Models;

namespace WooMeNow.API.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
}
