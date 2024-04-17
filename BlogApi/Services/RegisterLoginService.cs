using BlogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogApi.Services
{
    public class RegisterLoginService : IRegisterLoginService
    {
        private readonly BlogContext _blogContext;
        private readonly IConfiguration _config;
        public RegisterLoginService(BlogContext blogContext, IConfiguration config)
        {
            _blogContext = blogContext;
            _config = config;
        }

        public async Task<ResponseModel> UserRegistration(UserDetails RegistrationData)
        {
            var EmailCollision = await _blogContext.UserDetail.Where(item => item.Email.ToLower() == RegistrationData.Email.ToLower() && item.IsDeleted != true).FirstOrDefaultAsync();
            var response = new ResponseModel();

            try
            {
                if (EmailCollision == null)
                {
                    var User = new UserDetails
                    {
                        FirstName = RegistrationData.FirstName,
                        LastName = RegistrationData.LastName,
                        Email = RegistrationData.Email,
                        Gender = RegistrationData.Gender,
                        IsAdmin = RegistrationData.IsAdmin,
                        DateOfBirth = RegistrationData.DateOfBirth,
                        ContactNumber = RegistrationData.ContactNumber,
                        SaltKey = Guid.NewGuid(),
                    };

                    User.Password = CreatePasswordHash(RegistrationData.Password, User.SaltKey);

                    _blogContext.UserDetail.Add(User);
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

        public async Task<SessionDetails> UserLogin(LoginDetail LoginData)
        {
            SessionDetails sd = new SessionDetails();
            var user = await _blogContext.UserDetail.Where(x => x.Email.ToLower() == LoginData.Email.ToLower().Trim() && x.IsDeleted != true).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.Password == CreatePasswordHash(LoginData.Password, user.SaltKey))
                {
                    string token = GenerateToken(user);
                    sd.userId = user.UserId;
                    sd.tokenId = token;
                    sd.firstName = user?.FirstName ?? "";
                    sd.lastName = user?.LastName ?? "";
                    sd.userRoleId = user.IsAdmin;
                }
                else
                {
                    sd.message = "Password do not match!!!";
                    sd.userId = user.UserId;
                    sd.tokenId = "";
                }
            }
            else
            {
                sd.userId = 0;
                sd.message = "User Not Found !!!";
                sd.tokenId = "";
            }

            return sd;
        }

        public static string CreatePasswordHash(string plainPassword, Guid guidSaltKey)
        {
            var guidSaltedPassword = string.Concat(plainPassword, guidSaltKey);
            return CryptoService.CreatePasswordHash(guidSaltedPassword);
        }

        private string GenerateToken(UserDetails user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:WebTokenSecret").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: creds,
                    expires: DateTime.Now.AddHours(2));
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;

            }
            catch (Exception ex)
            {
                throw;
            }
            return null;

        }
    }
}
