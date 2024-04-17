using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class BlogService : IBlogService
    {
        private readonly BlogContext _blogContext;
        public BlogService(BlogContext blogContext) 
        {
            _blogContext = blogContext;
        }
        public async Task<List<BlogApplications>> GetBlogsList()
        {
            var data = await _blogContext.BlogApplication.ToListAsync();
            return data;
        }
    }
}
