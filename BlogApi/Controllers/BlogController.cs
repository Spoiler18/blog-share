using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;
        public BlogController(IBlogService blogService, ILogger<BlogController> logger) 
        {
            _blogService = blogService;
            _logger = logger;
        }

        [Route("GetBlogsList")]
        [HttpGet]
        public async Task<IActionResult> GetBlogsList()
        {
            return Ok(await _blogService.GetBlogsList());
        }
    }
}
