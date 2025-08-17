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
