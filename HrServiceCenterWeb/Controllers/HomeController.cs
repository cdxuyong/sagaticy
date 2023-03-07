using BlueFramework.User;
using HrServiceCenterWeb.Manager;
using HrServiceCenterWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HrServiceCenterWeb.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Version = Publish2Local.Complication.Version;
            if (UserContext.CurrentUser.IsCompanyUser)
                return View("CmpIndex");
            else
                return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}