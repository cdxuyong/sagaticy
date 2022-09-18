using System;
using System.Collections.Generic;
using System.Linq;
using HrServiceCenterWeb.Models;
using System.Web;
using System.Web.Mvc;
using HrServiceCenterWeb.Manager;
using BlueFramework.Common.Excel;
using System.Data;
using BlueFramework.Common;

namespace HrServiceCenterWeb.Controllers
{
    public class CompanyController : BaseController
    {


        // GET: /Company/CompanyList
        public ActionResult CompanyList()
        {
            return View();
        }


        // Company/GetCompanyList?query=
        public ActionResult GetCompanyList(string query)
        {
            List<CompanyInfo> list = new Manager.EmployeeManager().GetCompanies(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        // Company/QueryCompanyList?q=
        public ActionResult QueryCompanyList(string q)
        {
            if (String.IsNullOrEmpty(q))
            {
                return null;
            }
            List<CompanyInfo> list = new Manager.EmployeeManager().GetCompanies(q);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        public ActionResult CompanyPage()
        {
            ViewBag.CompanyId = int.Parse(this.HttpContext.Request.QueryString["id"]);
            return View();
        }

        // Company/GetCompany?id=
        public ActionResult GetCompany(int id)
        {
            CompanyInfo company = new Manager.EmployeeManager().GetCompany(id);
            JsonResult jsonResult = Json(company,JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // Company/GetCompany?id=
        [HttpPost]
        public ActionResult SaveCompany(CompanyInfo companyInfo)
        {
            CompanyInfo ci = new Manager.EmployeeManager().SaveCompany(companyInfo);
            Object result = new
            {
                success = ci==null?false:true,
                data = ci==null?0:ci.CompanyId
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult SaveRecharge(int companyId,decimal money)
        {
            EmployeeManager manager = new EmployeeManager();
            CompanyInfo company = manager.GetCompany(companyId);
            CompanyAccountRecordInfo recordInfo = new CompanyAccountRecordInfo()
            {
                AccountId = company.AccountId,
                CompanyId = companyId,
                Creator = BlueFramework.User.UserContext.CurrentUser.UserId,
                Money = money,
                AccountBalance = company.AccountBalance
            };
            bool pass = manager.SaveRecharge(recordInfo);
            Object result = new
            {
                success = pass,
                data = pass?"充值成功！":"充值失败！"
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult DeleteCompany(int id)
        {
            bool pass = new Manager.EmployeeManager().DeleteCompany(id);
            Object result = new
            {
                success = pass,
                data = pass?"删除成功": "删除失败，数据可能已经被引用。"
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // Company/SavePosition
        [HttpPost]
        public ActionResult SavePosition(CompanyPositionSetInfo positionSetInfo)
        {
            bool pass = new Manager.EmployeeManager().SavePosition(positionSetInfo);
            Object result = new
            {
                success = pass,
                data = pass?"成功":"失败"
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult DeletePosition(CompanyPositionSetInfo positionSetInfo)
        {
            bool pass = new Manager.EmployeeManager().DeletePostion(positionSetInfo);
            Object result = new
            {
                success = pass,
                data = pass ? "成功" : "失败"
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult Export()
        {

            List<CompanyInfo> list = new Manager.EmployeeManager().GetCompanies(string.Empty);
            IExcel excel = ExcelFactory.CreateDefault();
            DataTable dataTable = Converts.ListToDataTable<CompanyInfo>(list);
            dataTable.Columns["Name"].Caption = "单位名称";
            dataTable.Columns["Code"].Caption = "组织机构代码";
            dataTable.Columns["Representative"].Caption = "法人";
            dataTable.Columns["AccountBalance"].Caption = "账户余额";
            dataTable.Columns.Remove("CompanyId");
            dataTable.Columns.Remove("AccountId");
            dataTable.Columns.Remove("Positions");
            dataTable.Columns.Remove("Remark");
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            POIStream stream = new POIStream();
            stream.AllowClose = false;
            excel.Write(stream, dataSet, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode("单位信息", System.Text.Encoding.UTF8)));
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

        public ActionResult Import()
        {
            if (Request.Files.Count == 0)
            {
                Object failResult = new
                {
                    success = false,
                    data = ""
                };
                JsonResult failJsonResult = Json(failResult, JsonRequestBehavior.AllowGet);
                return failJsonResult;
            }

            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = excel.Read(Request.Files[0].InputStream);
            DataTable dt = ds.Tables[0];
            string message;
            bool pass = new EmployeeManager().ImportRecharges(dt,out message);
            Object result = new
            {
                success = pass,
                data = message
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}
