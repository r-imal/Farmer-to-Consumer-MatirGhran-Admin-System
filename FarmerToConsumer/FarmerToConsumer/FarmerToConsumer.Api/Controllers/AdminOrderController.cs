using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOrderController(OrderRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = repo.GetAll();
            return Ok(result);
        }

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromQuery] int orderId, [FromQuery] string? status)
        {
            if (orderId <= 0)
            {
                return BadRequest("Order id is required.");
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Status is required. Example: Pending, Accepted, Delivered, Cancelled.");
            }

            var result = repo.UpdateStatus(orderId, status);
            return Ok(result);
        }
    }
}
