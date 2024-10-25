using DotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Login> Logins { get; set; }
    }
}
