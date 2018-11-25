using System.Linq;
using LibRes.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Controllers
{
    public class RequestsController : Controller
    {
        private readonly LibResDbContext _dbContext;

        public RequestsController(LibResDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(_dbContext.ReservationModels.ToList());
        }
    }
}