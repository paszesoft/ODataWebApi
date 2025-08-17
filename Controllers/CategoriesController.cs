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
