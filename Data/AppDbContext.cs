using Microsoft.EntityFrameworkCore;
using ODataWebApi.Models;
namespace ODataWebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Furniture" }
        );
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 1200, CategoryId = 1 },
            new Product { Id = 2, Name = "Phone", Price = 800, CategoryId = 1 },
            new Product { Id = 3, Name = "Desk", Price = 300, CategoryId = 2 }
        );
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, ProductId = 1, Date = DateTime.UtcNow.AddDays(-1) },
            new Order { Id = 2, ProductId = 2, Date = DateTime.UtcNow }
        );
    }
}