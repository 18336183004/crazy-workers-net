using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.Cryptions;
using CW.Web.IServices;
using CW.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CW.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ResponseModel> LoginAccount(LoginAccountModel model)
        {
            model.Password = Md5Helper.ToMd5(model.Password);
            return await _loginService.LoginAccount(model);
        }

        public IActionResult Login()
        {
            return View();
        }


    }
}
