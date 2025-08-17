using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataWebAPI.Data;
using ODataWebAPI.Models;
namespace ODataWebAPI.Controllers
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
