using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly BlogContext _blogContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(BlogContext blogContext, IHttpContextAccessor httpContextAccessor)
        {
            _blogContext = blogContext;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<ResponseModel> EditUserAsync(UserDetails user)
        {
            var EmailCollision = await _blogContext.UserDetail.Where(item => item.Email.ToLower() == user.Email.ToLower() && item.IsDeleted != true && item.UserId != user.UserId).FirstOrDefaultAsync();
            var response = new ResponseModel(); 
            try
            {
                if (EmailCollision == null)
                {
                    var User = await _blogContext.UserDetail.Where(item => item.IsDeleted != true && item.UserId == user.UserId).FirstOrDefaultAsync();

                    User.FirstName = user.FirstName; 
                    User.LastName = user.LastName; 
                    User.Email = user.Email;
                    User.DateOfBirth = user.DateOfBirth;
                    User.Gender = user.Gender;
                    User.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    User.ModifiedOn = DateTime.Now;

                    _blogContext.UserDetail.Update(User);
                    await _blogContext.SaveChangesAsync();

                    response.isSuccess = true;
                    response.isError = false;
                    response.message = "";
                }
                else
                {
                    response.isSuccess = false;
                    response.isError = true;
                    response.message = "Email Already Exists";
                }
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.isError = true;
                response.message = "Something Went Wrong";
            }

            return response;
        }

        public async Task<ResponseModel> DeactivateUserAsync(int? UserId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                UserDetails? user = await _blogContext.UserDetail.Where(x => x.UserId == UserId && x.IsDeleted != true).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.IsDeactivated = true;
                    user.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    user.ModifiedOn = DateTime.UtcNow;

                    _blogContext.UserDetail.Update(user);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "User Deactivated Successfully!!!";
                }
                else
                {
                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Something Went Wrong!!!";
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

        public async Task<ResponseModel> ReactivateUserAsync(int? UserId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                UserDetails? user = await _blogContext.UserDetail.Where(x => x.UserId == UserId && x.IsDeactivated == true).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.IsDeactivated = false;
                    user.ModifiedBy = CommonService.GetUserId(_httpContextAccessor.HttpContext);
                    user.ModifiedOn = DateTime.UtcNow;

                    _blogContext.UserDetail.Update(user);
                    await _blogContext.SaveChangesAsync();

                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "User Reactivated Successfully!!!";
                }
                else
                {
                    response.isError = false;
                    response.isSuccess = true;
                    response.message = "Something Went Wrong!!!";
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
