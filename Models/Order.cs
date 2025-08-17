namespace ODataWebApi.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}