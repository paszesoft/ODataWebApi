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

namespace ODataWebApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }
    }
}

3. Define the DbContext

📁 Create a new file: 
Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using ODataWebApi.Models;

namespace ODataWebApi.Data
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
using ODataWebApi.Data;
using ODataWebApi.Models;

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
using ODataWebApi.Data;
using ODataWebApi.Models;

namespace ODataWebApi.Controllers
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

⭐ Commit 2
OData-enabled ASP.NET Core Web API Full Crud Relational example that includes:
✅ Features:
    • OData support with filtering, sorting, projection, and paging
    • Entity relationships: Product, Category, and Order
    • CRUD operations (Create, Read, Update, Delete)
    • Proper use of NuGet packages (including SqlServer, InMemory, etc.)
    • Redirect: http://localhost:5198/ to: http://localhost:5198/odata/Products
    • Testing using the new MyRequest.http test file and the REST Client Extension

✅ STEP 0: Create the Project if from sratch
dotnet new webapi -n ODataWebAPI

cd ODataWebAPI (Or just open the created folder in VS CODE)

✅ STEP 1: Install Required NuGet Packages, if from scratch:

dotnet add package Microsoft.AspNetCore.OData
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.InMemory

✅ STEP 2: Define Models

📁 Replace file: 
Models/Product.cs

namespace ODataWebApi.Models;
public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Order>? Orders { get; set; }
}

📁 Create a new file: 
Models/Category.cs

namespace ODataWebApi.Models;
public class Category
{
    public int Id { get; set; }
    public string Name? { get; set; }
    public ICollection<Product>? Products { get; set; }
}

📁 Create a new file: 
Models/Order.cs

namespace ODataWebApi.Models;
public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}

✅ STEP 3: Setup DbContext

📁 Replace file: 
Data/AppDbContext.cs

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

✅ STEP 4: Configure Application

📄 Replace file: 
Program.cs

using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataWebApi.Data;
using ODataWebApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ODataDb"));
builder.Services.AddControllers().AddOData(opt => opt
    .AddRouteComponents("odata", GetEdmModel())
    .Select()
    .Expand()
    .Filter()
    .OrderBy()
    .Count()
    .SetMaxTop(100)
);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}
app.UseRouting();
app.UseAuthorization();
// Redirect: http://localhost:5198/ to: http://localhost:5198/odata/Products
app.MapGet("/", context =>
{
    context.Response.Redirect("/odata/Products");
    return Task.CompletedTask;
});
app.MapControllers();
app.Run();
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Category>("Categories");
    builder.EntitySet<Order>("Orders");
    return builder.GetEdmModel();
}

✅ STEP 5: Create/Update OData Controllers

📁 Create a new file: 
Controllers/CategoriesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataWebApi.Data;
using ODataWebApi.Models;

namespace ODataWebApi.Controllers;

public class CategoriesController : ODataController
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public IActionResult Get() => Ok(_context.Categories);

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == key);
        return category == null ? NotFound() : Ok(category);
    }

    public IActionResult Post([FromBody] Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChanges();
        return Created(category);
    }

    public IActionResult Put(int key, [FromBody] Category updated)
    {
        if (key != updated.Id) return BadRequest();
        _context.Entry(updated).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();
        return Updated(updated);
    }

    public IActionResult Patch(int key, [FromBody] Delta<Category> patch)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == key);
        if (category == null) return NotFound();

        patch.Patch(category);
        _context.SaveChanges();
        return Updated(category);
    }

    public IActionResult Delete(int key)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == key);
        if (category == null) return NotFound();

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return NoContent();
    }
}

📁 Create a new file: 
Controllers/OrdersController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataWebApi.Data;
using ODataWebApi.Models;
namespace ODataWebApi.Controllers;

public class OrdersController : ODataController
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public IActionResult Get() => Ok(_context.Orders);

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == key);
        return order == null ? NotFound() : Ok(order);
    }

    public IActionResult Post([FromBody] Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return Created(order);
    }

    public IActionResult Put(int key, [FromBody] Order updated)
    {
        if (key != updated.Id) return BadRequest();
        _context.Entry(updated).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();
        return Updated(updated);
    }

    public IActionResult Patch(int key, [FromBody] Delta<Order> patch)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == key);
        if (order == null) return NotFound();

        patch.Patch(order);
        _context.SaveChanges();
        return Updated(order);
    }

    public IActionResult Delete(int key)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == key);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        _context.SaveChanges();
        return NoContent();
    }
}

📁 Replace file: 
Controllers/ProductsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataWebApi.Data;
using ODataWebApi.Models;

namespace ODataWebApi.Controllers;

public class ProductsController : ODataController
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [EnableQuery]
    public IActionResult Get() => Ok(_context.Products);

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == key);
        return product == null ? NotFound() : Ok(product);
    }

    public IActionResult Post([FromBody] Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return Created(product);
    }

    public IActionResult Put(int key, [FromBody] Product updated)
    {
        if (key != updated.Id) return BadRequest();
        _context.Entry(updated).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();
        return Updated(updated);
    }

    public IActionResult Patch(int key, [FromBody] Delta<Product> patch)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == key);
        if (product == null) return NotFound();

        patch.Patch(product);
        _context.SaveChanges();
        return Updated(product);
    }

    public IActionResult Delete(int key)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == key);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        _context.SaveChanges();
        return NoContent();
    }
}

✅ STEP 6: Run the App

dotnet run

… Now listening on: http://localhost:5198 …
should appear in the Terminal

Clicking it should now redirect to:
http://localhost:5198/odata/Products

✅ STEP 7: Test the OData Endpoints using the new test file:
MyRequest.http
You may need to install a VS Code Extension to use it:
- Search for "REST Client" by Huachao Mao, and click "Install."

📁 Create a new file: 
MyRequest.http

### Should redirect to /odata/Products
GET http://localhost:5198/
### 
GET http://localhost:5198/odata/Products
###
GET http://localhost:5198/odata/Categories
###
GET http://localhost:5198/odata/Orders
###
GET http://localhost:5198/odata/Products?$filter=Price%20gt%20500
###
GET http://localhost:5198/odata/Products?$orderby=Name%20desc
###
GET http://localhost:5198/odata/Products?$select=Name,Price
###
GET http://localhost:5198/odata/Products?$top=2
###
GET http://localhost:5198/odata/Products?$expand=Category
###
GET http://localhost:5198/odata/Orders?$expand=Product
###
GET http://localhost:5198/odata/Orders?$expand=Product($expand=Category)
###

### POST Category
POST http://localhost:5198/odata/Categories
Content-Type: application/json

{
  "Id": 3,
  "Name": "Accessory"
}

###

### PUT Category
PUT http://localhost:5198/odata/Categories/3
Content-Type: application/json

{
  "Id": 3,
  "Name": "Accessories"
}

###

### DELETE Category
DELETE http://localhost:5198/odata/Categories/3
###

### POST Product
POST http://localhost:5198/odata/Products
Content-Type: application/json

{
  "Id": 4,
  "Name": "Lamp",
  "Price": 12,
  "CategoryId": 1
}

###

### PUT Product
PUT http://localhost:5198/odata/Products/4
Content-Type: application/json

{
  "Id": 4,
  "Name": "Desk Lamp",
  "Price": 15,
  "CategoryId": 3
}

###

### DELETE Product
DELETE http://localhost:5198/odata/Products/4
###

### POST Order
POST http://localhost:5198/odata/Orders
Content-Type: application/json

{
  "Id": 3,
  "ProductId": 4,
  "Date": "2025-08-17T00:00:00Z"
}

###

### PUT Order
PUT http://localhost:5198/odata/Orders/3
Content-Type: application/json

{
  "Id": 3,
  "ProductId": 4,
  "Date": "2025-08-18T00:00:00Z"
}

###

### DELETE Order
DELETE http://localhost:5198/odata/Orders/3
###

TODO: ⭐ Commit 3