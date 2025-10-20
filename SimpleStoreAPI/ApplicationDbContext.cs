using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleStore.Domain.Models;
using System.Text.RegularExpressions;

namespace SimpleStoreAPI
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
            {
                if (!context.Set<Product>().Any())
                {
                    var product_f = new Faker<Product>()
                        .RuleFor(p => p.Guid, f => Guid.NewGuid())
                        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                        .RuleFor(p => p.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
                        .RuleFor(p => p.LastUpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.UtcNow).ToUniversalTime())
                        .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price(1, 2000)));

                    var products = product_f.Generate(20);

                    await context.Set<Product>().AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            });
        }
    }
}
