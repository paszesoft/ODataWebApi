using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataWebAPI.Data;
using ODataWebAPI.Models;
var builder = WebApplication.CreateBuilder(args);
// Add EF Core with in-memory database (for demo purposes)
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));
// Add controllers + OData
builder.Services.AddControllers()
    .AddOData(opt => opt
        .AddRouteComponents("odata", GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .SetMaxTop(100)
        .Count()
    );
var app = builder.Build();
// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Products.AddRange(
        new Product { Id = 1, Name = "Laptop", Price = 1200 },
        new Product { Id = 2, Name = "Phone", Price = 800 },
        new Product { Id = 3, Name = "Tablet", Price = 450 }
    );
    db.SaveChanges();
}
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Product>("Products");
    return builder.GetEdmModel();
}
