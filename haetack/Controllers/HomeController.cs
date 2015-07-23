using haetack.Views.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace haetack.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }
            else { }

            return View();
        }
    }
}
