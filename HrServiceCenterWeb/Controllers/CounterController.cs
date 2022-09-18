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
    public class CounterController : BaseController
    {
        // GET: BaseCode/GetPositions/
        [HttpGet]
        public ActionResult GetEmployeeCount()
        {
            ReportManager manager = new ReportManager();
            List<CounterBO> list = manager.GetBarChartData();
            CounterVO vo = fillCounter(list);
            JsonResult json = Json(vo);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult GetPositionCounts()
        {
            ReportManager manager = new ReportManager();
            List<CountetBase> list = manager.GetPositionCounts();
            JsonResult json = Json(list);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult GetDegreeCounts()
        {
            ReportManager manager = new ReportManager();
            List<CountetBase> list = manager.GetDegreeCounts();
            JsonResult json = Json(list);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult GetEmployeeCounts()
        {
            ReportManager manager = new ReportManager();
            List<CounterBO> list = manager.GetBarChartData();
            CounterVO vo = fillCounter(list);
            JsonResult json = Json(vo);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpGet]
        public ActionResult GetPayCounts()
        {
            ReportManager manager = new ReportManager();
            List<CounterVO> list = manager.GetLineChartData();
            var dataSource = new
            {
                success = list.Count>=3?true:false,
                series = list
            };
            JsonResult json = Json(dataSource);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        private CounterVO fillCounter(List<CounterBO> list)
        {
            CounterVO vo = new CounterVO();
            vo.Data = new decimal[list.Count];
            vo.DataAxis = new string[list.Count];
            for(int i = 0; i < list.Count; i++)
            {
                CounterBO bo = list[i];
                vo.Data[i] = bo.Data;
                vo.DataAxis[i] = bo.DataAxis;
            }

            return vo;
        }
    }
}
