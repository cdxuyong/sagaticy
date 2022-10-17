using System;
using System.Collections.Generic;
using System.Linq;
using HrServiceCenterWeb.Models;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using BlueFramework.Common.CSV;
using System.Xml;
using BlueFramework.Common.Excel;

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
        public ActionResult DayDetail(int importId)
        {
            ViewBag.ImportId = importId;
            return View();
        }

        public ActionResult MonthReporter()
        {
            return View();
        }

        public ActionResult ChecksQuery()
        {
            return View();
        }



        public ActionResult MonthDetail(string cmpName,string date)
        {
            ViewBag.companyName = cmpName;
            ViewBag.queryDate = date;
            return View();
        }

        public ActionResult QueryImportorList(string query)
        {
            List<CheckInfo> list = new Manager.CheckManager().QueryImportorCheckList(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        // 导入
        public ActionResult ImportDay()
        {
            if (Request.Files.Count == 0)
            {
                Object o = new
                {
                    success = false,
                    data = "上传失败，请选择需要上传的EXCEL"
                };
                JsonResult r = Json(o, JsonRequestBehavior.AllowGet);
                return r;

            }

            string message = string.Empty;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = excel.Read(Request.Files[0].InputStream);
            DataTable dt = ds.Tables[0];

            bool pass = new Manager.CheckManager().ImportDayChecks(dt, fileName, ref message);
            Object result = new
            {
                success = pass,
                data = message
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult DeleteDayCheck(int id)
        {
            string msg = string.Empty;
            if (new Manager.CheckManager().DeleteDayChecks(id, ref msg))
            {
                msg = "删除成功！";
            }
            else
            {
                msg = "删除失败！" + msg;
            }
            return Json(msg);
        }

        public ActionResult QueryImportDayList(int checkId)
        {
            List<CheckDayInfo> list = new Manager.CheckManager().QueryCheckDayList(checkId);
            //JsonResult jsonResult = Json(list);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            var result = new
            {
                code = 200,
                data = jsonString
            };
            return Json(result);
            //return jsonResult;
        }
        public ActionResult QueryDayCheckList(string cmpName,string pName)
        {
            List<CheckDayInfo> list = new Manager.CheckManager().QueryCheckDayList(pName, cmpName);
            //JsonResult jsonResult = Json(list);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            var result = new
            {
                code = 200,
                data = jsonString
            };
            return Json(result);
            //return jsonResult;
        }
        #region 月报
        /// <summary>
        /// 获取单位月报
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult QueryMonthSummaryList(string query)
        {
            List<CheckMonthSumVO> list = new Manager.CheckManager().QueryMonthSummaryList(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        /// <summary>
        /// 获取月度明细
        /// </summary>
        /// <param name="cmpName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult QueryMonthDetailList(string cmpName,string month)
        {
            List<CheckMonthInfo> list = new Manager.CheckManager().QueryCheckMonthList(cmpName, month);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            var result = new
            {
                code = 200,
                data = jsonString
            };
            JsonResult jsonResult = Json(result);
            return jsonResult;
        }

        /// <summary>
        /// 导入月报
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportMonth()
        {
            if (Request.Files.Count == 0)
            {
                Object o = new
                {
                    success = false,
                    data = "上传失败，请选择需要上传的EXCEL"
                };
                JsonResult r = Json(o, JsonRequestBehavior.AllowGet);
                return r;

            }

            string message = string.Empty;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = excel.Read(Request.Files[0].InputStream);
            DataTable dt = ds.Tables[0];

            bool pass = new Manager.CheckManager().ImportMonthChecks(dt, fileName, ref message);
            Object result = new
            {
                success = pass,
                data = message
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
        #endregion
    }
}