using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Controllers;
using General.Framework.Security.Admin;
using General.Services.SysUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace General.Mvc.Controllers
{
    [Route("login")]
    public class LoginController : BaseContoller
    {
        private const string R_KEY = "R_KEY";
        private ISysUserService _sysUserService;
        private IMemoryCache _memoryCache;
        private IAdminAuthService _authenticationService;

        public LoginController(ISysUserService sysUserService,
            IAdminAuthService authenticationService,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._sysUserService = sysUserService;
            this._authenticationService = authenticationService;
        }



        [Route("",Name ="publicLogin")]
        public IActionResult Index()
        {
            string r = EncryptorHelper.GetMD5(Guid.NewGuid().ToString());
            HttpContext.Session.SetString(R_KEY, r);
            LoginModel loginModel = new LoginModel() { R = r };
            return View(loginModel);
        }

        [HttpPost]
        [Route("")]
        public IActionResult LoginIndex(LoginModel model)
        {
            string r = HttpContext.Session.GetString(R_KEY);
            r = r ?? "";
            if (!ModelState.IsValid)
            {
                AjaxData.Message = "请输入用户账号和密码";
                return Json(AjaxData);
            }
            var result = _sysUserService.validateUser(model.Account, model.Password, r);
            AjaxData.Status = result.Item1;
            AjaxData.Message = result.Item2;
            if (result.Item1)
            {
                _authenticationService.signIn(result.Item3, result.Item4.Name);
            }
            return Json(AjaxData);
        }

        [Route("publicgetSalt", Name = "publicgetSalt")]
        public IActionResult getSalt(string account)
        {
            var user = _sysUserService.getByAccount(account);
            return Content(user?.Salt);
        }

        [Route("publicSignout", Name ="publicSignout")]
        public IActionResult SignOut()
        {
            _authenticationService.signOut();
            return RedirectToRoute("publicLogin");
        }


    }
}