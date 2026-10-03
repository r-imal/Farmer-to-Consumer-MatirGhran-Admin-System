using FarmerToConsumer.Entities;
using FarmerToConsumer.Repos;
using Microsoft.AspNetCore.Mvc;

namespace FarmerToConsumer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerController(FarmerRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = repo.GetAll();
            return Ok(result);
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetById(int id)
        {
            var result = repo.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Save(Farmer model)
        {
            var result = repo.Save(model);
            return Ok(result);
        }
    }
}
