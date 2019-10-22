using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/orderdetail")]
    public class OrderDetailController : AdminPermissionController
    {
        private IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }


        [Function("座位订单",true,"menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 20)]
        [Route("",Name ="OrderDetailIndex")]
        public IActionResult Index(OrderDetailSearchArgs args,int page=1,int size = 20)
        {
            var pageList = _orderDetailService.searchOrder(args, page, size);
            var dataSource = pageList.toDataSourceResult<Entities.OrderDetail,OrderDetailSearchArgs>("OrderDetailIndex",args);
            return View(dataSource);
        }

        [HttpGet]
        [Route("edit",Name ="editOrderDetail")]
        [Function("编辑订单信息",false,FatherResource = "General.Mvc.Areas.Admin.Controllers.OrderDetailController.Index")]
        public IActionResult EditOrderDetail(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("OrderDetailIndex");
            if (id != null)
            {
                var model = _orderDetailService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [Route("edit")]
        public ActionResult EditOrderDetail(Entities.OrderDetail model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("OrderDetailIndex");
            if (!ModelState.IsValid)
                return View(model);

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                _orderDetailService.InsertOrdertail(model);
            }
            else
            {
                _orderDetailService.UpdateOrderdetail(model);
            }


            return Redirect(ViewBag.ReturnUrl);

        }


        [Route("HasEnd",Name ="HasEnd")]
        [Function("设置订单是否结束",false,FatherResource = "General.Mvc.Areas.Admin.Controllers.OrderDetailController.Index")]
        public JsonResult HasEnd(Guid id,bool hasEnd)
        {
            _orderDetailService.hasEnd(id, hasEnd);
            AjaxData.Message = "修改订单成功";
            AjaxData.Status = true;
            return Json(AjaxData);
        }

        [Route("HasCheckIn",Name ="HasCheckIn")]
        [Function("设置订单是否打卡",false,FatherResource = "General.Mvc.Areas.Admin.Controllers.OrderDetailController.Index")]
        public JsonResult HasCheckIn(Guid id,bool hasCheckIn)
        {
            _orderDetailService.hasCheckIn(id, hasCheckIn);
            AjaxData.Message = "修改订单成功";
            AjaxData.Status = true;
            return Json(AjaxData);
        }

    }
}