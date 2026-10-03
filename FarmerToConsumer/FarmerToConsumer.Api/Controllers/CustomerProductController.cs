using FarmerToConsumer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FarmerToConsumer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerProductController(FtcDbContext context) : ControllerBase
    {
        [HttpGet("GetByID/{id}")]
        public IActionResult GetById(int id)
        {
            var stock = context.ProductStocks
                .Include(e => e.Product)
                .ThenInclude(e => e!.Farmer)
                .AsNoTracking()
                .FirstOrDefault(e => e.ID == id && e.Quantity > 0);
            if (stock == null)
            {
                return NotFound();
            }
            var product = new
            {
                id = stock.ID,
                productId = stock.ProductId,
                name = stock.Product?.Name ?? "",
                category = stock.Product?.Category ?? "",
                farmer = stock.Product?.Farmer?.Name ?? "",
                price = stock.Price,
                quantity = stock.Quantity
            };
            return Ok(product);
        }
    }
}
