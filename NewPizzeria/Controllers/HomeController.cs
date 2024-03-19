using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewPizzeria.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult BackOffice()
        {
            ViewBag.Title = "Back Office";

            return View();
        }
    }
}
