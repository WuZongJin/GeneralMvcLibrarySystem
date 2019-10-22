using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.LibrarySeat;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/libraryseats")]
    public class LibrarySeatController : AdminPermissionController
    {
        private ILibrarySeatService _librarySeatService;

        public LibrarySeatController(ILibrarySeatService librarySeatService)
        {
            _librarySeatService = librarySeatService;
        }


        [Function("图书馆座位",true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 10)]
        [Route("", Name = "librarySeatIndex")]
        public IActionResult Index(LibrarySeatSearchArgs args, int page = 1, int size = 20)
        {
            var dataSource = _librarySeatService.searchSeat(args, page, size);
            var pageList = dataSource.toDataSourceResult<Entities.LibrarySeat,LibrarySeatSearchArgs>("librarySeatIndex",args);
            return View(pageList);
        }

        [HttpGet]
        [Route("edit",Name="editLibrarySeat")]
        [Function("编辑图书馆座位",false, FatherResource = "General.Mvc.Areas.Admin.Controllers.LibrarySeatController.Index")]
        public IActionResult EditLibrarySeat(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("librarySeatIndex");
            if(id != null)
            {
                var model = _librarySeatService.GetById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        [Route("edit")]
        public ActionResult EditLibrarySeat(Entities.LibrarySeat model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("librarySeatIndex");
            if (!ModelState.IsValid)
                return View(model);
            if(model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                _librarySeatService.InsertLibrarySeat(model);
            }
            else
            {
                _librarySeatService.UpdateLibrarySeat(model);
            }


            return Redirect(ViewBag.ReturnUrl);

        }
    }
}