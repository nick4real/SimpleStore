using Bogus;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Models;

namespace SimpleStore.API.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<PictureLink> Pictures { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
            {
                Random rnd = new Random();

                if (!context.Set<PictureLink>().Any())
                {
                    var picture_f = new Faker<PictureLink>()
                        .RuleFor(p => p.Id, f => Guid.NewGuid())
                        .RuleFor(p => p.Url, f => f.Image.PicsumUrl(width: 640, height: 480));

                    var pictures = picture_f.Generate(40);

                    await context.Set<PictureLink>().AddRangeAsync(pictures);
                    await context.SaveChangesAsync();
                }

                if (!context.Set<Product>().Any())
                {
                    var pictures = context.Set<PictureLink>().ToList();

                    var product_f = new Faker<Product>()
                        .RuleFor(p => p.Guid, f => Guid.NewGuid())
                        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                        .RuleFor(p => p.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
                        .RuleFor(p => p.LastUpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.UtcNow).ToUniversalTime())
                        .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(1, 2000)))
                        .RuleFor(p => p.Pictures, f => new List<PictureLink>
                        {
                            pictures[rnd.Next(pictures.Count)],
                            pictures[rnd.Next(pictures.Count)],
                            pictures[rnd.Next(pictures.Count)]
                        });

                    var products = product_f.Generate(20);

                    await context.Set<Product>().AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            });
        }
    }
}
