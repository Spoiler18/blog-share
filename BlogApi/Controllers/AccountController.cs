using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRegisterLoginService _registerLoginService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger, IRegisterLoginService registerLoginService)
        {
            _accountService = accountService;
            _logger = logger;
            _registerLoginService = registerLoginService;
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginDetail loginDetail)
        {
            return Ok(await _registerLoginService.UserLogin(loginDetail));
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(AdminResetPasswordModel adminResetPasswordModel)
        {
            return Ok(await _registerLoginService.ResetPasswordAsync(adminResetPasswordModel));
        }

        [AllowAnonymous]
        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            return Ok(await _registerLoginService.ForgotPassword(forgotPassword));
        }

        [Route("GetAllUser")]
        [HttpPost,Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _accountService.GetAllUsers());
        }

        [Route("GetSingleUser")]
        [HttpPost, Authorize]
        public async Task<IActionResult> GetSingleUser(int? userId)
        {
            return Ok(await _accountService.GetSingleUser(userId));
        }
    }
}
