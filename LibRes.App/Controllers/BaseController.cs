using LibRes.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Controllers
{
    /// <summary>
    /// TODO: Document why are we using this.
    /// </summary>
    public class BaseController : Controller
    {
        protected BaseController()
            : this(new LibResDbContext())
        {
         
        }

        protected BaseController(LibResDbContext data)
        {
            this.Context = data;
        }

        protected LibResDbContext Context { get; private set; }
    }
}
