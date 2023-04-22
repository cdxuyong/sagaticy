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
        // POST: BaseCode/GetPositions/
        [HttpPost]
        public ActionResult AddPositions(string name)
        {
            // 增加至数据库
            string message = string.Empty;
            BaseCodeProvider.Current.AddPosition(name,out message);
            var data = new
            {
                status = 200,
                message = string.IsNullOrEmpty(message) ? "添加成功，请从列表中选择！" : message
            };
            return Json(data);

        }

        public ActionResult CodeManage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAllCodes(string query)
        {
            List<BaseCodeInfo> codes = BaseCodeProvider.Current.GetCodes(query);
            JsonResult json = Json(codes);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult GetCode(int id)
        {
            var code = BaseCodeProvider.Current.GetCodes().FirstOrDefault(x=>x.Id==id);
            JsonResult json = Json(code);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult NewCode(int parentId)
        {
            var code = BaseCodeProvider.Current.NewCode(parentId);
            JsonResult json = Json(code);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpPost]
        public ActionResult SaveCode(BaseCodeInfo codeInfo)
        {
            var succ = BaseCodeProvider.Current.SaveBaseCode(codeInfo);
            var data = new
            {
                code = 200,
                success = succ
            };
            JsonResult json = Json(data);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
