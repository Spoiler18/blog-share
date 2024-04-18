using BlogApi.Models;

namespace BlogApi.Services
{
    public interface IRegisterLoginService
    {
        Task<ResponseModel> UserRegistration(UserDetails RegistrationData);
        Task<SessionDetails> UserLogin(LoginDetail LoginData);
        Task<ResponseModel> ResetPasswordAsync(AdminResetPasswordModel adminResetPasswordModel);
        Task<ResponseModel> ForgotPassword(ForgotPassword? userDetail);
        Task<ResponseModel> ChangePassword(ChangePassword userDetail);
    }
}
