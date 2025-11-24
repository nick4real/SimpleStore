using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Entities;

namespace SimpleStore.IdentityServer.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
