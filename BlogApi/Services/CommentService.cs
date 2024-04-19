using BlogApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class CommentService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommentService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
        }

        public async Task<List<DetailedComment>> GetBlogComments(int? blogId)
        {
            var data = (from comments in _blogContext.Comments
                        join user in _blogContext.UserDetail
                        on comments.UserId equals user.UserId
                        where comments.BlogId == blogId && comments.IsDeleted != true
                        select new DetailedComment
                        {
                            CommentId = comments.CommentId,
                            UserId = comments.UserId,
                            BlogId = comments.BlogId,
                            UserComment = comments.UserComment,
                            ReplyToCommentId = comments.ReplyToCommentId,
                            UserFirstName = user.FirstName,
                            UserLastName = user.LastName,
                        }).ToList();

            return data;
        }

        public async Task<ResponseModel> AddBlogComment(BlogComment comment)
        {
            ResponseModel response = new ResponseModel();

            if (comment.UserComment != null)
            {
                BlogComment newComment = new BlogComment
                {
                    UserId = comment.UserId,
                    BlogId = comment.BlogId,
                    UserComment = comment.UserComment,
                    ReplyToCommentId = comment.ReplyToCommentId,
                    CreatedOn = DateTime.Now,
                    CreatedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext),
                };

                try
                {
                    _blogContext.Comments.Add(newComment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Added Successfully!!!";

                }
                catch (Exception ex)
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Something Went Wrong!!!";
                }
            }
            else
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Empty Comment!!!";
            }

            return response;
        }

        public async Task<ResponseModel> EditBlogComment(BlogComment comment)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogComment? editComment = await _blogContext.Comments.Where(item => item.CommentId == comment.CommentId && item.IsDeleted != true).FirstOrDefaultAsync();

                if (editComment != null)
                {
                    editComment.UserComment = comment.UserComment;
                    editComment.ModifiedOn = DateTime.Now;
                    editComment.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);

                    _blogContext.Comments.Update(editComment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Edited Successfully!!!";
                }
                else
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Comment Not Found!!!";
                }
            }
            catch (Exception ex)
            {
                response.isError = true;
                response.isSuccess = false;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }

        public async Task<ResponseModel> DeleteBlogComment(int? commentId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogComment? comment = await _blogContext.Comments.Where(x => x.CommentId == commentId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (comment != null)
                {
                    comment.IsDeleted = true;
                    comment.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    comment.ModifiedOn = DateTime.UtcNow;

                    _blogContext.Comments.Update(comment);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Deleted Successfully!!!";
                }
                else
                {
                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Comment Has Already Been Deleted!!!";
                }
            }
            catch
            {
                response.isError = false;
                response.isSuccess = true;
                response.message = "Something Went Wrong!!!";
            }

            return response;
        }
    }
}
