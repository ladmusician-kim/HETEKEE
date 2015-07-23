using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace haetack.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        public ActionResult Index()
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }

            string strUserAgent = Request.UserAgent.ToString().ToLower();
            if (Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                strUserAgent.Contains("palm"))
            {
                return RedirectToAction("Mobile");
            }

            return RedirectToAction("App");
        }
        public ActionResult App()
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }
             string strUserAgent = Request.UserAgent.ToString().ToLower();
             if (Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                 strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                 strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                 strUserAgent.Contains("palm"))
             {
                 return RedirectToAction("Mobile");
             }
            return View();
        }
        public ActionResult CF()
        {
            if (Request.Browser.Type.ToUpper().Contains("IE") || Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER")) // replace with your check
            {
                if (Request.Browser.MajorVersion < 9)
                {
                    return RedirectToAction("Index", "Prepare");
                }
            }
            return View();
        }

        public ActionResult Mobile()
        {
             string strUserAgent = Request.UserAgent.ToString().ToLower();
            if (Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                strUserAgent.Contains("palm"))
            {
                return View();
            };

            return RedirectToAction("App");
        }
    }
}
