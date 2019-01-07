using LibRes.App.Data;
using Microsoft.AspNetCore.Mvc;

namespace LibRes.App.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Base controller used to wrap the original ASP.NET controller class.
    ///     We use it to 'inject' the database context so it can be used across classes.
    /// </summary>
    public class BaseController : Controller
    {
        protected BaseController()
            : this(new LibResDbContext())
        {
        }

        protected BaseController(LibResDbContext data)
        {
            Context = data;
        }

        protected LibResDbContext Context { get; }
    }
}