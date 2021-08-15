using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebTask.Models;

namespace WebTask.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Collect> collects { get; set; }
        public DbSet<Item> items { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}