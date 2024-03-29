﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using General.Services.SysUser;
using General.Entities;
using General.Framework.Datatable;
using General.Core.Librs;
using General.Framework.Filters;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/users")]
    public class UserController : AdminPermissionController
    {
        private ISysUserService _sysUserService;

        public UserController(ISysUserService sysUserService)
        {
            this._sysUserService = sysUserService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        [Route("", Name = "userIndex")]
        public IActionResult UserIndex(SysUserSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysUserService.searchUser(arg, page, size);
            var dataSource = pageList.toDataSourceResult<Entities.SysUser, SysUserSearchArg>("userIndex", arg);
            return View(dataSource);
        }
         
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("edit", Name = "editUser")]
        [Function("编辑系统用户", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public IActionResult EditUser(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex");
            if (id != null)
            {
                var model = _sysUserService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        } 
        [HttpPost]
        [Route("edit")]
        public ActionResult EditUser(Entities.SysUser model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.MobilePhone))
                model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
            model.Name = model.Name.Trim();

            

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.Salt = EncryptorHelper.CreateSaltKey();
                model.Account = StringUitls.toDBC(model.Account.Trim());
                model.Enabled = true;
                model.IsAdmin = false;
                model.Password = EncryptorHelper.GetMD5(model.Account + model.Salt);
                model.Creator = WorkContext.CurrentUser.Id;
                _sysUserService.insertSysUser(model);
            }
            else
            {
                var user = _sysUserService.getById(model.Id);
                user.Name = model.Name;
                user.Email = model.Email;
                user.MobilePhone = model.MobilePhone;
                user.Sex = model.Sex;
                user.ModifiedTime = DateTime.Now;
                user.Modifier = WorkContext.CurrentUser.Id;
                _sysUserService.updateSysUser(user);
            }
            return Redirect(ViewBag.ReturnUrl);
        }


        /// <summary>
        /// 设置启用与禁用账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Route("enabled", Name = "enabled")]
        [Function("设置启用与禁用账号", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult Enabled(Guid id, bool enabled)
        {
            _sysUserService.enabled(id, enabled, WorkContext.CurrentUser.Id);
            AjaxData.Message = "启用禁用设置完成";
            AjaxData.Status = true;
            return Json(AjaxData);
        }


        /// <summary>
        /// 设置登录锁解锁与锁定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Route("loginLock", Name = "loginLock")]
        [Function("设置登录锁解锁与锁定", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult LoginLock(Guid id, bool loginLock)
        {
            _sysUserService.loginLock(id, loginLock, WorkContext.CurrentUser.Id);
            AjaxData.Message = "登录锁状态设置完成";
            AjaxData.Status = true;
            return Json(AjaxData);
        }

        [Route("scheduleLock",Name ="scheduleLock")]
        [Function("设置预定解锁和锁定",false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult ScheduleLock(Guid id, bool scheduleLock)
        {
            _sysUserService.scheduleLock(id, scheduleLock, WorkContext.CurrentUser.Id);
            AjaxData.Message = "预定锁状态设置完成";
            AjaxData.Status = true;
            return Json(AjaxData);
        }
    


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete/{id}", Name = "deleteUser")]
        [Function("删除用户", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult DeleteUser(Guid id)
        {
            _sysUserService.deleteUser(id, WorkContext.CurrentUser.Id);
            AjaxData.Status = true;
            AjaxData.Message = "删除完成";
            return Json(AjaxData);
        }

        /// <summary>
        /// 远程验证账号是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [Route("existAccount", Name = "remoteAccount")]
        [PermissionActionFilter(true)]
        public JsonResult RemoteAccount(string account)
        {
            account = account.Trim();
            return Json(!_sysUserService.existAccount(account));
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("resetPwd/{id}", Name = "resetPassword")]
        [Function("重置用户密码", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult ResetPassword(Guid id)
        {
            _sysUserService.resetPassword(id, WorkContext.CurrentUser.Id);
            AjaxData.Status = true;
            AjaxData.Message = "用户密码已重置为原始密码";
            return Json(AjaxData);
        }
    }
}