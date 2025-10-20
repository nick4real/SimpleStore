using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleStore.Domain.Models;

namespace SimpleStoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("{_guid}")]
        public async Task<IActionResult> Get(Guid _guid)
        {
            var product = await _dbContext.Products.FindAsync(_guid);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _dbContext.Products.ToListAsync();
            if (list == null)
                return NotFound();
            return Ok(list);
        }
    }
}
