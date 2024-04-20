using BlogApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using static Azure.Core.HttpHeader;

namespace BlogApi.Services
{
    public class BlogService : IBlogService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlogReactionService _blogReactionService;
        private readonly ICommentService _commentService;
        public BlogService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor, IBlogReactionService blogReactionService, ICommentService commentService)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
            _blogReactionService = blogReactionService;
            _commentService = commentService;
        }
        public async Task<List<DetailedBlogApplications>> GetBlogsListForDashboard()
        {
            bool isAdmin = false;
            try
            {
                isAdmin = await _blogContext.UserDetail.Where(item => item.UserId == CommonService.GetUserId(_httpContextAccessor.HttpContext)).Select(item => item.IsAdmin).FirstOrDefaultAsync();
            }
            catch (Exception ex) { }

            List<DetailedBlogApplications> data = new List<DetailedBlogApplications>();

            if (isAdmin)
            {
                data = (from blog in _blogContext.BlogApplication
                        join user in _blogContext.UserDetail
                        on blog.UserId equals user.UserId
                        where blog.IsDeleted != true
                        select new DetailedBlogApplications
                        {
                            BlogId = blog.BlogId,
                            BlogTitle = blog.BlogTitle,
                            BlogDescription = blog.BlogDescription,
                            FullName = user.FirstName + " " + user.LastName,
                            UserId = blog.UserId,
                        }).ToList();
            }
            else
            {
                data = (from blog in _blogContext.BlogApplication
                        join user in _blogContext.UserDetail
                        on blog.UserId equals user.UserId
                        where blog.IsDeleted != true && blog.UserId == CommonService.GetUserId(_httpContextAccessor.HttpContext)
                        select new DetailedBlogApplications
                        {
                            BlogId = blog.BlogId,
                            BlogTitle = blog.BlogTitle,
                            BlogDescription = blog.BlogDescription,
                            FullName = user.FirstName + " " + user.LastName,
                            UserId = blog.UserId,
                        }).ToList();
            }


            return data.ToList();
        }

        public async Task<List<DetailedBlogApplications>> GetBlogsListForBlogs()
        {
            List<DetailedBlogApplications> data = new List<DetailedBlogApplications>();

            data = (from blog in _blogContext.BlogApplication
                    join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                    where blog.IsDeleted != true
                    select new DetailedBlogApplications
                    {
                        BlogId = blog.BlogId,
                        BlogTitle = blog.BlogTitle,
                        BlogDescription = blog.BlogDescription,
                        FullName = user.FirstName + " " + user.LastName,
                        UserId = blog.UserId,
                    }).ToList();

            return data.ToList();
        }

        public async Task<DetailedBlogApplications> GetBlogDetail(int? id)
        {
            var blogReaction = await _blogReactionService.GetBlogReaction(id);
            var blogComment = await _commentService.GetBlogComments(id);

            var blogDetail = (from blog in _blogContext.BlogApplication
                              join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                              join img in _blogContext.BlogImages.AsEnumerable() on blog.BlogId equals img.BlogId into images
                              where blog.BlogId == id
                              select new DetailedBlogApplications
                              {
                                  BlogId = blog.BlogId,
                                  BlogTitle = blog.BlogTitle,
                                  BlogDescription = blog.BlogDescription,
                                  FullName = user.FirstName + " " + user.LastName,
                                  UserId = blog.UserId,
                                  BlogImages = images.ToList()
                              }).FirstOrDefault();

            blogDetail.BlogReactions = blogReaction;
            blogDetail.BlogComments = blogComment;

            return blogDetail;
        }

        public async Task<ResponseModel> AddBlogAsync(BlogApplications blogApplications)
        {
            ResponseModel response = new ResponseModel();

            BlogApplications blog = new BlogApplications
            {
                UserId = blogApplications.UserId,
                BlogTitle = blogApplications.BlogTitle?.Trim(),
                BlogDescription = blogApplications.BlogDescription?.Trim(),
                CreatedOn = DateTime.Now,
                CreatedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext),
            };

            try
            {
                if (_blogContext.BlogApplication.Any(item => item.BlogTitle == blogApplications.BlogTitle))
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Blog Title Already Exists!!!";
                }
                else
                {
                    _blogContext.BlogApplication.Add(blog);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Blog Added Successfully!!!";
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

        public async Task<ResponseModel> EditBlogAsync(BlogApplications blogApplications)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogApplications? editBlog = await _blogContext.BlogApplication.Where(item => item.BlogId == blogApplications.BlogId && item.IsDeleted != true).FirstOrDefaultAsync();

                if (editBlog != null)
                {
                    if (!_blogContext.BlogApplication.Any(item => item.BlogTitle == blogApplications.BlogTitle && item.BlogId != blogApplications.BlogId && item.IsDeleted != true))
                    {
                        editBlog.BlogTitle = blogApplications.BlogTitle;
                        editBlog.BlogDescription = blogApplications.BlogDescription;
                        editBlog.ModifiedOn = DateTime.Now;
                        editBlog.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);

                        _blogContext.BlogApplication.Update(editBlog);
                        await _blogContext.SaveChangesAsync();

                        response.isError = false;
                        response.isSuccess = true;
                        response.message = "Blog Edited Successfully!!!";
                    }
                    else
                    {
                        response.isError = true;
                        response.isSuccess = false;
                        response.message = "Blog Title Already Exists!!!";
                    }

                }
                else
                {
                    response.isError = true;
                    response.isSuccess = false;
                    response.message = "Blog Not Found!!!";
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

        public async Task<ResponseModel> DeleteBlogAsync(int? blogApplicationId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                BlogApplications? blogApplication = await _blogContext.BlogApplication.Where(x => x.BlogId == blogApplicationId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (blogApplication != null)
                {
                    blogApplication.IsDeleted = true;
                    blogApplication.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    blogApplication.ModifiedOn = DateTime.UtcNow;

                    _blogContext.BlogApplication.Update(blogApplication);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Blog Deleted Successfully!!!";
                }
                else
                {
                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Blog Has Already Been Deleted!!!";
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
