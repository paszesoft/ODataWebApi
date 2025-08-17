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