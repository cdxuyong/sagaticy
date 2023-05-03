using System;
using System.Collections.Generic;
using System.Linq;
using HrServiceCenterWeb.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using HrServiceCenterWeb.Manager;
using System.Data;
using System.Web.UI.WebControls;
using BlueFramework.Common.Excel;

namespace HrServiceCenterWeb.Controllers
{
    public class ReportsController : BaseController
    {
        public ActionResult WxyjYear()
        {
            return View();
        }

        public ActionResult WxyjMonth()
        {
            return View();
        }
        public ActionResult RybdYear()
        {
            return View();
        }
        public ActionResult RybdMonth()
        {
            return View();
        }
        public ActionResult Gwtj()
        {
            return View();
        }
        public ActionResult PersonPayMonth()
        {
            return View();
        }
        // GET: Reports/GetYears/
        [HttpPost]
        public ActionResult GetYears()
        {
            int start = 2018;
            int end = DateTime.Now.Year + 2;
            List<object> list = new List<object>();
            for(int i = start; i < end; i++)
            {
                var o = new 
                {
                    id = i,
                    text = i+"年度"
                };
                list.Add(o);
            }
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        // GET: Reports/GetWxyjByYear/
        [HttpGet]
        public ActionResult GetWxyjByYear(int year)
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetWxyjByYear(year);
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);
         
            return contentResult;
        }
        // GET: Reports/GetWxyjByYear/
        public ActionResult DownWxyjByYear(int year)
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetWxyjByYear(year);
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\wxyjYear.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("单位五险一金统计", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }
        
        // GET: Reports/GetWxyjByMonth/
        [HttpGet]
        public ActionResult GetWxyjByMonth(int year,int month)
        {
            string date = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetWxyjByMonth(date);
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);

            return contentResult;
        }
        // GET: Reports/DownWxyjByMonth/
        public ActionResult DownWxyjByMonth(int year, int month)
        {
            string date = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetWxyjByMonth(date);
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\wxyjYear.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("单位五险一金统计", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }

        // GET: Reports/GetRybdByYear/
        [HttpGet]
        public ActionResult GetRybdByYear(int year)
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetRybdByYear(year);
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);

            return contentResult;
        }

        // GET: Reports/GetWxyjByYear/
        public ActionResult DownRybdByYear(int year)
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetRybdByYear(year);
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\rybdYear.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("用工单位各岗位人员变动、工资统计表", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }

        // GET: Reports/GetWxyjByMonth/
        [HttpGet]
        public ActionResult GetRybdByMonth(int year, int month)
        {
            string date = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetRybdByMonth(date);
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);

            return contentResult;
        }

        // GET: Reports/GetWxyjByYear/
        public ActionResult DownRybdByMonth(int year, int month)
        {
            ReportManager manager = new ReportManager();
            string date = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            DataSet ds = manager.GetRybdByMonth(date);
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\rybdMonth.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("用工单位各岗位人员变动、工资统计表（月度）", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }

        [HttpGet]
        public ActionResult GetGwtj()
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetGwtj();
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);

            return contentResult;
        }

        public ActionResult DownGwtj()
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetGwtj();
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\gwtj.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("单位五险一金统计", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }


        [HttpGet]
        public ActionResult GetPersonPayMonthDetail()
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetPersonPayMonthDetail();
            DataTable dt = ds.Tables[0];
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            ContentResult contentResult = Content(json);

            return contentResult;
        }

        public ActionResult DownPersonPayMonthDetail()
        {
            ReportManager manager = new ReportManager();
            DataSet ds = manager.GetPersonPayMonthDetail();
            ds.Tables[0].TableName = "sheet1";

            POIStream stream = new POIStream();
            IExcel excel = ExcelFactory.CreateDefault();
            stream.AllowClose = false;
            //excel.Write(stream, ds, ExcelExtendType.XLSX);
            string templateName = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\Reports\gwtj.xml";
            excel.Write(stream, templateName, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("单位五险一金统计", System.Text.Encoding.UTF8)));
                context.BinaryWrite(buffer);
                context.Flush();
                context.End();
            }
            catch (Exception ex)
            {
                context.ContentType = "text/plain";
                context.Write(ex.Message);
            }
            return null;
        }

    }
}
