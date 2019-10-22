using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Services;
using General.Services.LibrarySeat;
using General.Services.SysUser;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Controllers
{
    [Route("CheckIn")]
    public class CheckInController : Controller
    {
        private IOrderDetailService _orderDetailService;
        private ISysUserService _sysUserService;
        private ILibrarySeatService _librarySeatService;

        public CheckInController(IOrderDetailService orderDetailService,ISysUserService sysUserService,ILibrarySeatService librarySeatService)
        {
            _orderDetailService = orderDetailService;
            _sysUserService = sysUserService;
            _librarySeatService = librarySeatService;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("Check",Name ="Checkin")]
        public IActionResult Checkin(string account,string code)
        {
            var user = _sysUserService.getByAccount(account);
            if(user == null)
            {
                return Json(new { Status = false, Message = "用户不存在" });
            }
            var order = _orderDetailService.userCurrentOrder(user.Id);
            if(order == null)
            {
                return Json(new { Status = false, Message = "你当前还没有预定座位" });
            }

            if(order.VerificationCode.Trim() != code)
            {
                return Json(new { Status = false, Message = "验证码错误" });
            }

            order.HasCheckIn = true;
            _orderDetailService.UpdateOrderdetail(order);

            var seat = _librarySeatService.GetById(order.LibrarySeatId);
            seat.SeatState = Entities.LibrarySeat.SeatStates.InAvailable;
            _librarySeatService.UpdateLibrarySeat(seat);

            return Json(new { Status = false, Message = "打卡成功" });

        }
    }
}