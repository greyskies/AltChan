using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AltChan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "AltChans web site";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "AltChans contact info";

            return View();
        }
    }
}