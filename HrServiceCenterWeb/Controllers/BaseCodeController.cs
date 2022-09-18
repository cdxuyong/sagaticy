using System;
using System.Collections.Generic;
using System.Linq;
using HrServiceCenterWeb.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using HrServiceCenterWeb.Manager;


namespace HrServiceCenterWeb.Controllers
{
    public class BaseCodeController : BaseController
    {
        // GET: BaseCode/GetSexCodes/
        [HttpGet]
        public ActionResult GetSexCodes()
        {
            List<BaseCodeInfo> codes = BaseCodeProvider.Current.GetSexCodes();
            JsonResult json = Json(codes);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        // GET: BaseCode/GetEduciationCodes/
        [HttpGet]
        public ActionResult GetEduciationCodes()
        {
            List<BaseCodeInfo> codes = BaseCodeProvider.Current.GetEduciationCodes();
            JsonResult json = Json(codes);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        // GET: BaseCode/GetPositions/
        [HttpGet]
        public ActionResult GetPositions()
        {
            List<PositionInfo> list = BaseCodeProvider.Current.GetPositions();
            JsonResult json = Json(list);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
