﻿using BlogApi.Models;
using BlogApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class BlogService : IBlogService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlogReactionService _blogReactionService;
        private readonly ICommentService _commentService;
        private readonly IConfiguration _configuration;
        public BlogService(BlogContext blogContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IBlogReactionService blogReactionService, ICommentService commentService)
        {
            _httpContextAccessor = httpContextAccessor;
            _blogContext = blogContext;
            _blogReactionService = blogReactionService;
            _commentService = commentService;
            _configuration = configuration;
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
                        join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                        where blog.IsDeleted != true
                        select new DetailedBlogApplications
                        {
                            BlogId = blog.BlogId,
                            BlogTitle = blog.BlogTitle,
                            BlogDescription = blog.BlogDescription,
                            CreatedOn = blog.CreatedOn,
                            FullName = user.FirstName + " " + user.LastName,
                            UserId = blog.UserId,
                        }).ToList();
            }
            else
            {
                data = (from blog in _blogContext.BlogApplication
                        join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                        where blog.IsDeleted != true && blog.UserId == CommonService.GetUserId(_httpContextAccessor.HttpContext)
                        select new DetailedBlogApplications
                        {
                            BlogId = blog.BlogId,
                            BlogTitle = blog.BlogTitle,
                            BlogDescription = blog.BlogDescription,
                            CreatedOn = blog.CreatedOn,
                            FullName = user.FirstName + " " + user.LastName,
                            UserId = blog.UserId,
                        }).ToList();
            }


            foreach (var blog in data)
            {
                var images = await _blogContext.BlogImages
                    .Where(img => img.BlogId == blog.BlogId && img.IsDeleted != true)
                    .ToListAsync();

                List<BlogImageDetailed> detailedImages = images.Select(img => new BlogImageDetailed
                {
                    ImageId = img.ImageId,
                    BlogId = img.BlogId,
                    ImagePath = img.ImagePath,
                }).ToList();

                Parallel.ForEach(detailedImages, img =>
                {
                    img.ImageBytes = ReadLocalImageAsByteArray(img.ImagePath).Result;
                });

                blog.BlogImages = detailedImages;
            }

            return data.ToList();
        }

        public async Task<List<Notifications>> GetUserNotifications(int? userId)
        {
            var notifications = new List<Notifications>();

            var loginTime = await _blogContext.UserLogs.Where(item => item.UserId == userId).OrderByDescending(item => item.LoginTime).Select(item => item.LoginTime)
                .FirstOrDefaultAsync();

            var userBlogs = await _blogContext.BlogApplication.Where(item => item.UserId == userId && item.IsDeleted != true).Select(item => item.BlogId).ToListAsync();

            var reactions = await (from reaction in _blogContext.BlogReactions
                                   join user in _blogContext.UserDetail on reaction.UserId equals user.UserId
                                   where userBlogs.Contains((int)reaction.BlogId) && reaction.CreatedOn >= loginTime
                                   select new Notifications
                                   {
                                       BlogId = reaction.BlogId,
                                       UserId = reaction.UserId,
                                       UserFullName = user.FirstName + " " + user.LastName,
                                       Message = user.FirstName + " " + user.LastName + " Reacted With '" + ((Reactions)reaction.UserReaction).ToString() + "'",
                                   }).ToListAsync();

            var comments = await (from reaction in _blogContext.BlogComments
                                  join user in _blogContext.UserDetail on reaction.UserId equals user.UserId
                                  where userBlogs.Contains((int)reaction.BlogId) && reaction.CreatedOn >= loginTime
                                  select new Notifications
                                  {
                                      BlogId = reaction.BlogId,
                                      UserId = reaction.UserId,
                                      UserFullName = user.FirstName + " " + user.LastName,
                                      Message = user.FirstName + " " + user.LastName + " Commented '" + reaction.UserComment + "'",
                                  }).ToListAsync();

            notifications.AddRange(reactions);
            notifications.AddRange(comments);

            return notifications;
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
                        CreatedOn = blog.CreatedOn,
                        FullName = user.FirstName + " " + user.LastName,
                        UserId = blog.UserId,
                    }).ToList();

            foreach (var blog in data)
            {
                var images = await _blogContext.BlogImages
                    .Where(img => img.BlogId == blog.BlogId && img.IsDeleted != true)
                    .ToListAsync();

                List<BlogImageDetailed> detailedImages = images.Select(img => new BlogImageDetailed
                {
                    ImageId = img.ImageId,
                    BlogId = img.BlogId,
                    ImagePath = img.ImagePath,
                }).ToList();

                Parallel.ForEach(detailedImages, img =>
                {
                    img.ImageBytes = ReadLocalImageAsByteArray(img.ImagePath).Result;
                });

                blog.BlogImages = detailedImages;

                var blogReaction = await _blogReactionService.GetBlogReaction(blog.BlogId);
                var blogComment = await _commentService.GetBlogComments(blog.BlogId);

                var upvotes = blogReaction.Where(item => item.UserReaction == 1).Count();
                var downvotes = blogReaction.Where(item => item.UserReaction == 2).Count();
                var comments = blogComment.Count();

                blog.Popularity = 2 * upvotes - downvotes + comments;
            }

            return data.ToList();
        }

        public async Task<byte[]> ReadLocalImageAsByteArray(string imagePath)
        {
            try
            {

                byte[] imageData = await File.ReadAllBytesAsync(imagePath);
                return imageData;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during reading
                Console.WriteLine($"Error reading image file: {ex.Message}");
                return null;
            }
        }

        public async Task<DetailedBlogApplications> GetBlogDetail(int? id)
        {
            var blogReaction = await _blogReactionService.GetBlogReaction(id);
            var blogComment = await _commentService.GetBlogComments(id);

            var blogDetail = (from blog in _blogContext.BlogApplication
                              join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                              where blog.BlogId == id
                              select new
                              {
                                  Blog = blog,
                                  User = user
                              }).FirstOrDefault();

            if (blogDetail != null)
            {
                var images = await _blogContext.BlogImages
                    .Where(img => img.BlogId == blogDetail.Blog.BlogId && img.IsDeleted != true)
                    .ToListAsync();

                // Convert BlogImage entities to BlogImageDetailed entities
                List<BlogImageDetailed> detailedImages = images.Select(img => new BlogImageDetailed
                {
                    ImageId = img.ImageId,
                    BlogId = img.BlogId,
                    ImageBytes = img.ImagePath != null ? ReadLocalImageAsByteArray(img.ImagePath).Result : null,
                }).ToList();

                DetailedBlogApplications detailedBlog = new DetailedBlogApplications
                {
                    BlogId = blogDetail.Blog.BlogId,
                    BlogTitle = blogDetail.Blog.BlogTitle,
                    BlogDescription = blogDetail.Blog.BlogDescription,
                    FullName = $"{blogDetail.User.FirstName} {blogDetail.User.LastName}",
                    UserId = blogDetail.Blog.UserId,
                    BlogImages = detailedImages
                };

                detailedBlog.BlogReactions = blogReaction;
                detailedBlog.BlogComments = blogComment;

                return detailedBlog;
            }
            return new DetailedBlogApplications();
        }

        public async Task<string[]> SaveImagesAsync(int blogId, List<byte[]> images)
        {
            var _imageFolderPath = _configuration["ImagePath"];
            List<string> imageUrls = new List<string>();

            // Create a directory for the blog if it doesn't exist
            string blogFolderPath = Path.Combine(_imageFolderPath, $"blog_{blogId}");
            Directory.CreateDirectory(blogFolderPath);

            // Save each image
            for (int i = 0; i < images.Count; i++)
            {
                string imagePath = Path.Combine(blogFolderPath, $"{blogId}_image_{i}.jpg");
                await File.WriteAllBytesAsync(imagePath, images[i]);
                imageUrls.Add(imagePath);
            }

            return imageUrls.ToArray();
        }

        public async Task<ResponseModel> AddBlogAsync(AddEditBlogApplications blogApplications)
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

                    List<string> imageUrls = new List<string>();

                    var _imageFolderPath = _configuration["ImagePath"];

                    if (blogApplications.BlogImages != null)
                    {
                        foreach (var image in blogApplications.BlogImages)
                        {
                            string imageName = $"{blog.BlogId}_image_{Guid.NewGuid()}.jpg";
                            string imagePath = Path.Combine(_imageFolderPath, imageName);

                            try
                            {
                                await File.WriteAllBytesAsync(imagePath, image.ImageBytes);
                                imageUrls.Add(imagePath);
                            }
                            catch (Exception ex)
                            {
                                // Handle or log the exception
                                Console.WriteLine($"Error saving image: {ex.Message}");
                            }
                        }

                        // Create BlogImage entities for database insertion
                        List<BlogImage> blogImages = imageUrls.Select(url => new BlogImage
                        {
                            BlogId = blog.BlogId,
                            ImagePath = url
                        }).ToList();

                        try
                        {
                            // Add BlogImage entities to the database
                            _blogContext.BlogImages.AddRange(blogImages);
                            await _blogContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            // Handle or log the exception
                            Console.WriteLine($"Error saving BlogImages to the database: {ex.Message}");
                        }
                    }
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

        public async Task<ResponseModel> EditBlogAsync(AddEditBlogApplications blogApplications)
        {
            ResponseModel response = new ResponseModel();
            var _imageFolderPath = _configuration["ImagePath"];

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

                        List<BlogImage> existingImages = await _blogContext.BlogImages.Where(item => item.IsDeleted != true && item.BlogId == blogApplications.BlogId).ToListAsync();
                        foreach (var existingImage in existingImages.ToList())
                        {
                            // Check if the existing image is not present in the new images
                            if (!blogApplications.BlogImages.Any(newImage => newImage.ImageId == existingImage.ImageId))
                            {
                                // Mark the image as deleted in the database
                                existingImage.IsDeleted = true;
                                _blogContext.BlogImages.Update(existingImage);
                            }
                        }

                        // Add new images to the database
                        foreach (var newImage in blogApplications.BlogImages)
                        {
                            // Check if the new image is not present in the existing images
                            if (!existingImages.Any(existingImage => existingImage.ImageId == newImage.ImageId))
                            {
                                string imageName = $"{blogApplications.BlogId}_image_{Guid.NewGuid()}.jpg";
                                string imagePath = Path.Combine(_imageFolderPath, imageName);
                                await File.WriteAllBytesAsync(imagePath, newImage.ImageBytes);

                                // Add the new image to the database
                                _blogContext.BlogImages.Add(new BlogImage
                                {
                                    BlogId = blogApplications.BlogId,
                                    ImagePath = imagePath,
                                });
                            }
                        }
                        await _blogContext.SaveChangesAsync();
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

        // top 10 most popular blogs
        public async Task<List<DetailedBlogApplications>> GetTopBlogs(bool Alltime,DateTime? fromDate, DateTime? toDate)
        {
            var blogs = await GetBlogsListForBlogs();
            if(Alltime == true)
            {
                return blogs.OrderBy(item => item.Popularity).Take(10).ToList();
            }
            else
            {
                return blogs.Where(item => item.CreatedOn >= fromDate && item.CreatedOn <= toDate).OrderBy(item => item.Popularity).Take(10).ToList();
            }
        }

        // get top bloggers 
        public async Task<List<PopularUsers>> GetTopBloggers(bool Alltime, DateTime? fromDate, DateTime? toDate)
        {
            var blogs = await GetBlogsListForBlogs();

            if (Alltime == true)
            {
                blogs = blogs.OrderBy(item => item.Popularity).Take(10).ToList();
            }
            else
            {
                blogs = blogs.Where(item => item.CreatedOn >= fromDate && item.CreatedOn <= toDate).OrderBy(item => item.Popularity).Take(10).ToList();
            }

            var userIds = blogs.Select(item => item.UserId).Distinct().ToList();

            var popularUsers = blogs.GroupBy(item => item.UserId).Select(group => new PopularUsers
            {
                UserId = group.Key,
                TotalPopularity = group.Sum(item => item.Popularity),
                UserName = group.First().FullName,
            }).ToList();

            return popularUsers.OrderBy(item => item.TotalPopularity).ToList();
        }

        // blogs added in a time frame or all time
        public async Task<List<DetailedBlogApplications>> GetBlogSummary(bool Alltime, DateTime? fromDate, DateTime? toDate)
        {
            var blogs = await GetBlogsListForBlogs();
            if (Alltime == true)
            {
                return blogs.ToList();
            }
            else
            {
                return blogs.Where(item => item.CreatedOn >= fromDate && item.CreatedOn <= toDate).ToList();
            }
        }

        // get the comments , reactions of blogs in a time frame
        public async Task<DetailedBlogApplications> GetBlogDetailPerodically(int? blogId,bool Alltime, DateTime? fromDate, DateTime? toDate)
        {
            DetailedBlogApplications data = new DetailedBlogApplications();

            data = (from blog in _blogContext.BlogApplication
                    join user in _blogContext.UserDetail on blog.UserId equals user.UserId
                    where blog.IsDeleted != true && blog.BlogId == blogId
                    select new DetailedBlogApplications
                    {
                        BlogId = blog.BlogId,
                        BlogTitle = blog.BlogTitle,
                        BlogDescription = blog.BlogDescription,
                        CreatedOn = blog.CreatedOn,
                        FullName = user.FirstName + " " + user.LastName,
                        UserId = blog.UserId,
                    }).FirstOrDefault();

            var blogReaction = await _blogReactionService.GetBlogReaction(data.BlogId);
            var blogComment = await _commentService.GetBlogComments(data.BlogId);

            if (Alltime == true)
            {
            }
            else
            {
                blogReaction = blogReaction.Where(item => item.CreatedOn.Value.Date >= fromDate.Value.Date && toDate.Value.Date >= item.CreatedOn.Value.Date).ToList();
                blogComment = blogComment.Where(item => item.CreatedOn.Value.Date >= fromDate.Value.Date && toDate.Value.Date >= item.CreatedOn.Value.Date).ToList();
            }

            data.BlogReactions = blogReaction;
            data.BlogComments = blogComment;

            return data;
        }
    }
}
