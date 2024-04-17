using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IRegisterLoginService
    {
        Task<ResponseModel> UserRegistration(UserDetails RegistrationData);
        Task<SessionDetails> UserLogin(LoginDetail LoginData);
    }
}
