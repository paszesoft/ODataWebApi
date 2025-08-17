
⭐ Commit 1
An Initial Simple OData ASP.NET Core Web API project using
Microsoft.AspNetCore.OData to expose a Read-Only OData endpoint.

✅ Project Overview

We'll build an API to manage a list of Product entities with OData query support ($filter, $orderby, etc.).

1. Create the Project and install required NuGet packages

- Open in VS Code your dotnet web api base folder, e.g.: 
    windows: c:\code\dotnet\webapi
    macos: /Users/{you}/code/dotnet/webapi
- Open the VS Code Terminal (control + `) and execute this command:
    dotnet new webapi -n ODataWebAPI
- Open in VS Code new project folder, e.g.: 
    windows: c:\code\dotnet\webapi\ODataWebAPI
    macos: /Users/{you}/code/dotnet/webapi/ODataWebAPI
- Execute these commands to install the required NuGet packages:
    dotnet add package Microsoft.AspNetCore.Odata
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.EntityFrameworkCore.InMemory

2. Define the Product Model

📁 Create a new file: 
Models/Product.cs

namespace ODataExample.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}

3. Define the DbContext

📁 Create a new file: 
Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using ODataExample.Models;

namespace ODataExample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}

4. Configure OData and Services

📄 Replace content of Program.cs with:

using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataExample.Data;
using ODataExample.Models;

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

5. Create the OData Controller

📁 Create a new file: 
Controllers/ProductsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataExample.Data;
using ODataExample.Models;

namespace ODataExample.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Products);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == key);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}

6. Run the App
dotnet run
… Now listening on: http://localhost:5198 …
should appear in the Terminal
Clicking it should result in Error code: 404 Not Found
since the root enpoint is not defined.

7. Try OData Queries

Visit these clickable URLs in your browser or Postman:

    • GET http://localhost:5198/odata/Products
    • GET http://localhost:5198/odata/Products?$filter=Price%20gt%20500
    • GET http://localhost:5198/odata/Products?$orderby=Name%20desc
    • GET http://localhost:5198/odata/Products?$select=Name,Price
    • GET http://localhost:5198/odata/Products?$top=2

✅ Summary

This app supports full OData query options for the Products endpoint.

TODO: ⭐ Commit 2