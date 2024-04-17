using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly BlogContext _blogContext;
        public AccountService(BlogContext blogContext) 
        {
            _blogContext = blogContext;
        }

        public async Task<List<UserDetails>> GetAllUsers()
        {
            return await _blogContext.UserDetail.ToListAsync();
        }
        public async Task<UserDetails> GetSingleUser(int? id)
        {
            var User = new UserDetails();
            if(id > 0)
            {
                User = await _blogContext.UserDetail.Where(item => item.UserId == id).FirstOrDefaultAsync();
            }
            return User;
        }

        
    }


}
