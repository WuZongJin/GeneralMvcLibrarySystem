using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using General.Mvc.Models;
using General.Entities;
using General.Services.Category;
using General.Core;
using General.Core.Data;
using General.Framework.Controllers;
using General.Services.LibrarySeat;
using General.Services;
using General.Framework.Infrastructure;

namespace General.Mvc.Controllers
{ 
    
    public class HomeController : BaseContoller
    {
        private ILibrarySeatService _librarySeatService;
        private IOrderDetailService _orderDetailService;
        
        private IWorkContext _workContext;
        

        public HomeController(ILibrarySeatService librarySeatService, IOrderDetailService orderDetailService,IWorkContext workContext)
        {
            _librarySeatService = librarySeatService;
            _orderDetailService = orderDetailService;
            _workContext = workContext;
            
        }

        [Route("",Name ="homeIndex")]
        public IActionResult Index()
        {

            return View();
        }
        
      
        [Route("firstFloor",Name ="firstFloor")]
        public IActionResult FirstFloor()
        {
            int floor = 0;
            var data = _librarySeatService.GetLibrarySeatCount(floor);
            var query = _librarySeatService.GetLibrarySeats(floor);

            
            ViewBag.LibrarySeats = query;
            ViewBag.SeatPosition = LibrarySeatData.seatPosition();

            ViewData["avilable"] = data.avilable;
            ViewData["booked"] = data.booked;
            ViewData["inavailable"] = data.inavilable;


            return View();
        }

        [HttpPost]
        [Route("",Name ="SeatSchedule")]
        public ActionResult Shcedule(int seatnumber)
        {
            var user = _workContext.CurrentUser;
            if (_orderDetailService.userCurrentOrder(user.Id)!=null)
            {
                AjaxData.Status = false;
                AjaxData.Message = "预定失败！上一个订单还未结束";
                return Json(AjaxData);
            }

            var data = _librarySeatService.Schedule(seatnumber, user.Id);
            AjaxData.Status = data.state;
            AjaxData.Message = data.message;
            return Json(AjaxData);
        }



        [HttpPost]
        [Route("seatDetail", Name = "seatDetail")]
        public IActionResult seatDetail(int seatnumber)
        {
            var seat = _librarySeatService.GetBySeatNum(seatnumber);
            var order = _orderDetailService.GetBySeatNumberWithOutEnd(seatnumber);
            if(seat == null|| order == null)
            {
                AjaxData.Status = false;
                AjaxData.Message = "出错";
                return Json(AjaxData);
            }

            string seatSate = seat.SeatState == LibrarySeat.SeatStates.Booked ? "已被预定" : "已被坐";

            AjaxData.Status = true;
            AjaxData.Message = $"座位状态: {seatSate}\n 开始时间：{order.CreateTime}\n 结束时间：{order.EndTime}";


            return Json(AjaxData) ;



        }
        public class SeatDetail
        {
            public bool Status { get; set; }
            public DateTime StratTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Message { get; set; }
        }
       
    }
}
