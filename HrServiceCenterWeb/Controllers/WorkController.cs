using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HrServiceCenterWeb.Controllers
{
    public class WorkController : BaseController
    {
        // GET: Work
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Checks()
        {
            return View();
        }
    }
}