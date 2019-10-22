using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Framework.Controllers;
using General.Services.SysUser;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Controllers
{
    [Route("register")]
    public class RegisterController : BaseContoller
    {
        private ISysUserService _sysUserService;

        public RegisterController(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        [Route("", Name = "publicRegisterIndex")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("register",Name="publicRegister")]
        public IActionResult register(Entities.RegisterModel model)
        {
            if (_sysUserService.existAccount(model.Account))
            {
                AjaxData.Status = false;
                AjaxData.Message = "该账号已存在";
                return Json(AjaxData);
            }

            if (_sysUserService.existEmail(model.Email)) 
            {
                AjaxData.Status = false;
                AjaxData.Message = "该邮箱已被注册";
                return Json(AjaxData);
            }

            if (_sysUserService.existMobilePhone(model.MobilePhone)) 
            {
                AjaxData.Status = false;
                AjaxData.Message = "该手机号码已被注册";
                return Json(AjaxData);
            }


            Entities.SysUser user = new Entities.SysUser();
            user.Account = model.Account;
            user.Name = model.Name;
            user.Email = model.Email;
            user.MobilePhone = model.MobilePhone;

            user.Id = Guid.NewGuid();
            user.CreationTime = DateTime.Now;
            user.Salt = EncryptorHelper.CreateSaltKey();
            user.Account = user.Account.Trim();
            user.Enabled = true;
            user.IsAdmin = false;
            user.Password = EncryptorHelper.GetMD5(model.Password + user.Salt);
            user.Creator = user.Id;
            _sysUserService.insertSysUser(user);

            AjaxData.Status = true;
            AjaxData.Message = "注册成功";

            return Json(AjaxData);
            
        }
    }
}