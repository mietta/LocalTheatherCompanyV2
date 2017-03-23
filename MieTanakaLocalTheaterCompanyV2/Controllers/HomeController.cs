using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: returns HomeIndex Home/contact view
namespace MieTanakaLocalTheaterCompanyV2.Controllers
{
    public class HomeController : Controller

        //returns Home/Index view
    {   [AllowAnonymous]// allow this method accessible to unautonrized users
        public ActionResult Index()
        {
            return View();
        }
        
/*        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
*/
        //rerurns Home/Contact view
        [AllowAnonymous] // allow this method accessible to unautonrized users
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}