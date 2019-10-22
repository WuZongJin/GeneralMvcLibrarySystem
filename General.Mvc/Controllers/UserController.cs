using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Security.Admin;
using General.Services;
using General.Services.LibrarySeat;
using General.Services.SysUser;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        public ISysUserService _sysUserService;
        public ILibrarySeatService _librarySeatService;
        public IOrderDetailService _orderDetailService;
        public IAdminAuthService _adminAuthService;

        public UserController(ISysUserService sysUserService, ILibrarySeatService librarySeatService, IOrderDetailService orderDetailService, IAdminAuthService adminAuthService)
        {
            _sysUserService = sysUserService;
            _librarySeatService = librarySeatService;
            _adminAuthService = adminAuthService;
            _orderDetailService = orderDetailService;
        }



        [Route("",Name ="userInfoIndex")]
        public IActionResult Index()
        {
            if (_adminAuthService.getCurrentUser() == null)
            {
                Redirect(Url.RouteUrl("publicLogin"));
            }
            var user = _adminAuthService.getCurrentUser();
            ViewBag.CurrentOrder = _orderDetailService.userCurrentOrder(user.Id);
            if (ViewBag.CurrentOrder != null)
            {
                OrderDetail orderDetail = ViewBag.CurrentOrder;
                ViewBag.OrderSeat = _librarySeatService.GetById(orderDetail.LibrarySeatId);
            }
            return View(user);
        }

        [Route("currentOrder",Name="userCurrentOrder")]
        public IActionResult CurrentOrder()
        {
            if (_adminAuthService.getCurrentUser() == null)
            {
                Redirect(Url.RouteUrl("publicLogin"));
            }
            var user = _adminAuthService.getCurrentUser();
            ViewBag.CurrentOrder = _orderDetailService.userCurrentOrder(user.Id);
            if (ViewBag.CurrentOrder != null)
            {
                OrderDetail orderDetail = ViewBag.CurrentOrder;
                ViewBag.OrderSeat = _librarySeatService.GetById(orderDetail.LibrarySeatId);
            }
            return View();
        }

        [HttpPost]
        [Route("reOrder",Name = "reOrder")]
        public IActionResult reOrder(Guid orderId)
        {
            var user = _adminAuthService.getCurrentUser();
            
            var result = _librarySeatService.RepeatOrder(orderId, user.Id);

            return Json(new { Status = false, Message = result.message });
        }

        [Route("changePassword",Name="changePassword")]
        public IActionResult ChangePassword()
        {
            if (_adminAuthService.getCurrentUser() == null)
            {
                Redirect(Url.RouteUrl("publicLogin"));
            }
            var user = _adminAuthService.getCurrentUser();
            ViewData["userId"] = user.Id;
            return View();
        }


        [HttpPost]
        [Route("changePasswordPost", Name = "changePasswordPost")]
        public IActionResult ChangePassword(string password)
        {
            if (_adminAuthService.getCurrentUser() == null)
            {
                Redirect(Url.RouteUrl("publicLogin"));
            }
            var user = _adminAuthService.getCurrentUser();
            user.Password = EncryptorHelper.GetMD5(password + user.Salt);
            _sysUserService.updateSysUser(user);
            

            return Json(new { status=true,Message="密码修改成功！"});
        }


    }
}