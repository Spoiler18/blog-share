using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReactionCommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IBlogReactionService _blogReactionService;
        private readonly ICommentReactionService _commentReactionService;
        private readonly ILogger<ReactionCommentController> _logger;
        public ReactionCommentController(ICommentService commentService, IBlogReactionService blogReactionService, ICommentReactionService commentReactionService, ILogger<ReactionCommentController> logger)
        {
            _logger = logger;
            _commentService = commentService;
            _blogReactionService = blogReactionService;
            _commentReactionService = commentReactionService;
        }

        [Route("AddBlogReaction")]
        [HttpPost, Authorize]
        public async Task<IActionResult> AddBlogReactionAsync(BlogReaction blog)
        {
            return Ok(await _blogReactionService.AddBlogReaction(blog));
        }

        [Route("EditBlogReaction")]
        [HttpPost, Authorize]
        public async Task<IActionResult> EditBlogReactionAsync(BlogReaction blog)
        {
            return Ok(await _blogReactionService.EditBlogReaction(blog));
        }

        [Route("DeleteBlogReaction/{blog}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBlogReactionAsync(int? blog)
        {
            return Ok(await _blogReactionService.DeleteBlogReaction(blog));
        }

        [Route("AddBlogComment")]
        [HttpPost, Authorize]
        public async Task<IActionResult> AddBlogCommentAsync(BlogComment blog)
        {
            return Ok(await _commentService.AddBlogComment(blog));
        }

        [Route("EditBlogComment")]
        [HttpPost, Authorize]
        public async Task<IActionResult> EditBlogCommentAsync(BlogComment blog)
        {
            return Ok(await _commentService.EditBlogComment(blog));
        }

        [Route("DeleteBlogComment/{blog}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBlogCommentAsync(int? blog)
        {
            return Ok(await _commentService.DeleteBlogComment(blog));
        }

        [Route("AddBlogCommentReaction")]
        [HttpPost, Authorize]
        public async Task<IActionResult> AddBlogCommentReactionAsync(CommentReaction blog)
        {
            return Ok(await _commentReactionService.AddCommentReaction(blog));
        }

        [Route("EditBlogCommentReaction")]
        [HttpPost, Authorize]
        public async Task<IActionResult> EditBlogCommentReactionAsync(CommentReaction blog)
        {
            return Ok(await _commentReactionService.EditBlogCommentReaction(blog));
        }

        [Route("DeleteBlogCommentReaction/{blog}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBlogCommentReactionAsync(int? blog)
        {
            return Ok(await _commentReactionService.DeleteBlogComment(blog));
        }
    }
}
