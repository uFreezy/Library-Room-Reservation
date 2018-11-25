using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibRes.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class RequestsController : Controller
    {

        private readonly LibResDbContext dbContext;

        public RequestsController(LibResDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(dbContext.ReservationModels.ToList());
        }
    }
}