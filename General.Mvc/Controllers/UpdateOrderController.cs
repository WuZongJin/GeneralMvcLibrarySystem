using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using General.Services;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Controllers
{
    [Route("UpdateOrder")]
    public class UpdateOrderController : Controller
    {
        private IUpdateLibrarySeatStateService updateLibrarySeatStateService;
        DateTime time;
        public UpdateOrderController(IUpdateLibrarySeatStateService updateLibrarySeatStateService)
        {
            this.updateLibrarySeatStateService = updateLibrarySeatStateService;
            time = DateTime.Now;
        }

        [HttpGet]
        [Route("",Name = "UpdateOrder")]
        public IActionResult Index()
        {
            if(time.AddSeconds(60) > DateTime.Now)
            {
                time = DateTime.Now;
                updateLibrarySeatStateService.Run();
            }
            return Content("update");
        }
    }
}