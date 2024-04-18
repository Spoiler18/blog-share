using BlogApi.Models;
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

        [Route("AddBlog")]
        [HttpGet,Authorize]
        public async Task<IActionResult> AddBlogAsync(BlogApplications blog)
        {
            return Ok(await _blogService.AddBlogAsync(blog));
        }

        [Route("EditBlog")]
        [HttpGet,Authorize]
        public async Task<IActionResult> EditBlogAsync(BlogApplications blog)
        {
            return Ok(await _blogService.EditBlogAsync(blog));
        }

        [Route("DeleteBlog")]
        [HttpGet, Authorize]
        public async Task<IActionResult> DeleteBlogAsync(int? blog)
        {
            return Ok(await _blogService.DeleteBlogAsync(blog));
        }
    }
}
