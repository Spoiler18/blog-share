﻿using BlogApi.Models;
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

        [Route("GetBlogsListForDashboard")]
        [HttpGet,Authorize]
        public async Task<IActionResult> GetBlogsListForDashboard()
        {
            return Ok(await _blogService.GetBlogsListForDashboard());
        }

        [Route("GetBlogsListForBlogs")]
        [HttpGet]
        public async Task<IActionResult> GetBlogsListForBlogs()
        {
            return Ok(await _blogService.GetBlogsListForBlogs());
        }

        [Route("GetBlogDetail/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetBlogDetail(int? id)
        {
            return Ok(await _blogService.GetBlogDetail(id));
        }

        [Route("AddBlog")]
        [HttpPost,Authorize]
        public async Task<IActionResult> AddBlogAsync(AddEditBlogApplications blog)
        {
            return Ok(await _blogService.AddBlogAsync(blog));
        }

        [Route("EditBlog")]
        [HttpPost,Authorize]
        public async Task<IActionResult> EditBlogAsync(AddEditBlogApplications blog)
        {
            return Ok(await _blogService.EditBlogAsync(blog));
        }

        [Route("DeleteBlog/{blog}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBlogAsync(int? blog)
        {
            return Ok(await _blogService.DeleteBlogAsync(blog));
        }

        [Route("GetUserNotifications/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(int? userId)
        {
            return Ok(await _blogService.GetUserNotifications(userId));
        }

        [Route("GetTopBlogs/{AllTime}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetTopBlogs(bool alltime, DateTime? fromDate, DateTime? toDate)
        {
            return Ok(await _blogService.GetTopBlogs(alltime,fromDate,toDate)) ;
        }

        [Route("GetTopBloggers/{AllTime}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetTopBloggers(bool alltime, DateTime? fromDate, DateTime? toDate)
        {
            return Ok(await _blogService.GetTopBloggers(alltime, fromDate, toDate));
        }

        [Route("GetBlogSummary/{AllTime}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetBlogSummary(bool alltime, DateTime? fromDate, DateTime? toDate)
        {
            return Ok(await _blogService.GetBlogSummary(alltime, fromDate, toDate));
        }

        [Route("GetBlogDetailPeriodically/{blogId}/{AllTime}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetBlogDetailPeriodically(int? blogId,bool alltime, DateTime? fromDate, DateTime? toDate)
        {
            return Ok(await _blogService.GetBlogDetailPerodically(blogId,alltime, fromDate, toDate));
        }
    }
}
