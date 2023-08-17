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
using BlueFramework.User;

namespace HrServiceCenterWeb.Controllers
{
    public class PayController : BaseController
    {

        // 模板列表
        // VIEW: /Pay/TemplateList
        public ActionResult TemplateList()
        {
            return View();
        }

        //获取所有模板列表
        //VIEW: /Pay/GetTemplateList
        public ActionResult GetTemplateList()
        {
            List<Models.TemplateInfo> list = new Manager.PayManager().GetTemplateList();
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        // 模板编辑器
        // VIEW: /Pay/TemplateEditor
        public ActionResult TemplateEditor()
        {
            ViewBag.TemplateId = int.Parse(this.HttpContext.Request.QueryString["id"]);
            return View();
        }

        public ActionResult QueryTemplate(string id)
        {
            int tempId = int.Parse(id);
            Models.TemplateInfo temp = new Manager.PayManager().GetTemplate(tempId);
            JsonResult jsonResult = Json(temp);
            return jsonResult;
        }

        // 获取模板列表树
        // VIEW: /Pay/TemplateEditor
        public ActionResult GetTemplateTree()
        {
            return Json(new Manager.PayManager().GetTree());
        }

        /// <summary>
        /// 获取数据库中存在的模版列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTemplateByTable(string id)
        {
            int tempId = int.Parse(id);
            int[] itemp = new Manager.PayManager().GetTemplateByTable(tempId);
            string[] stemp = itemp.Select(i => i.ToString()).ToArray();
            string strtemp = string.Join(",", stemp);
            return Json(strtemp);
        }

        public ActionResult SaveTemplateForTable(int id, string temps)
        {
            int[] tempArr = null;
            if (!string.IsNullOrEmpty(temps))
            {
                string strTemp = temps;
                string[] strArr = strTemp.Split(',');
                tempArr = new int[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    tempArr[i] = Convert.ToInt32(strArr[i]);
                }
            }
            string msg = string.Empty;
            if (new Manager.PayManager().SaveTemplateForTable(id, tempArr))
            {
                msg = "保存成功";
            }
            else
            {
                msg = "保存失败";
            }
            return Json(msg);
        }

        public ActionResult SaveTemplateMsg(int id, string temps)
        {
            string msg = string.Empty;
            //查询公司模版是否已经存在
            Models.TemplateInfo ti = new Models.TemplateInfo();
            ti = new Manager.PayManager().GetTemplateByCompanyId(id);
            if (ti.TemplateId != 0)
            {
                msg = "该公司已有模版";
                return Json(msg);
            }
            int[] tempArr = null;
            if (!string.IsNullOrEmpty(temps))
            {
                string strTemp = temps;
                string[] strArr = strTemp.Split(',');
                tempArr = new int[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    tempArr[i] = Convert.ToInt32(strArr[i]);
                }
            }
            if (new Manager.PayManager().SaveTemplateMsg(id, tempArr))
            {
                msg = "保存成功";
            }
            else
            {
                msg = "保存失败";
            }
            return Json(msg);
        }

        public ActionResult DeleteTemplate(int id)
        {
            string msg = string.Empty;
            if (new Manager.PayManager().DeleteTemplate(id))
            {
                msg = "删除成功";
            }
            else
            {
                msg = "删除失败";
            }
            return Json(msg);
        }

        // 导入列表
        // VIEW: /Pay/ImportorList
        public ActionResult ImportorList()
        {
            ViewBag.IsAdmin = UserContext.CurrentUser.IsCompanyUser ? false : true;

            return View();
        }

        public ActionResult ImportorPayList()
        {
            return View();
        }
        // PAY detail
        public ActionResult ImportorPayDetail(int importorId)
        {
            var info = new Manager.PayManager().QueryImportorInsuranceInfo(importorId);
            if (info == null)
            {
                return Error("警告：非法请求");
            }
            if (UserContext.CurrentUser.IsCompanyUser && info.CreatorId != UserContext.CurrentUser.UserId)
            {
                return Error("错误提示：无访问权限");
            }
            ViewBag.importorId = importorId;
            ViewBag.importorTitle = info.Title;
            return View();
        }

        //查询保险导入列表
        //VIEW: /Pay/QueryImportorList
        public ActionResult QueryImportorList(string query)
        {
            List<InsuranceInfo> list = new Manager.PayManager().QueryImportorInsuranceList(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        public ActionResult QueryImportorPayList(string query)
        {
            List<InsuranceInfo> list = new Manager.PayManager().QueryImportorPaymentList(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        public ActionResult ImportorPayDetailTable(int importorId)
        {
            DataTable dataTable = new Manager.PayManager().GetImportorDetail(importorId);
            String json = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
            ContentResult jsonResult = Content(json);
            return jsonResult;
        }

        public ActionResult DeleteInsurance(int id)
        {
            string msg = string.Empty;
            try
            {
                if (new Manager.PayManager().DeleteInsurance(id, out msg))
                {
                    msg = "删除成功";
                }
                else
                {
                    msg = string.IsNullOrEmpty(msg) ? "删除失败" : msg;
                }
            }
            catch(Exception ex)
            {
                msg = $"系统错误：{ex.Message}";
            }

            return Json(msg);
        }

        // 导入
        public ActionResult Import()
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
            DataTable dt = null;
            bool pass = true;
            try
            {
                IExcel excel = ExcelFactory.CreateDefault();
                DataSet ds = excel.Read(Request.Files[0].InputStream);
                dt = ds.Tables[0];
            }
            catch(Exception ex)
            {
                message = $"读取EXCEL错误，请检查是否存在公式单元格等问题，错误详情：{ex.Message}";
                pass = false;
            }

            if (pass)
            {
                pass = new Manager.PayManager().ImportInsurance(dt, fileName, ref message);
            }
            Object result = new
            {
                success = pass,
                data = message
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult ImportPay()
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

            string fileName = System.IO.Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
            string message = string.Empty;
            DataTable dt = null;
            bool pass = true;
            try
            {
                IExcel excel = ExcelFactory.CreateDefault();
                DataSet ds = excel.Read(Request.Files[0].InputStream);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                message = $"读取EXCEL错误，请检查是否存在公式单元格等问题，错误详情：{ex.Message}";
                pass = false;
            }

            if (pass)
            {
                pass = new Manager.PayManager().ImportPaymentData(dt, fileName, ref message);
            }
            Object result = new
            {
                success = pass,
                data = message
            };
            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // 导入保险
        // VIEW: /Pay/ImportorEditor
        [Obsolete]
        public ActionResult ImportorEditor()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            string outmsg = string.Empty;
            bool success = false;
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file1 = files[i];
                    Stream stream = file1.InputStream;
                    DataTable dt = new DataTable();
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file1.FileName);
                    string fileType = System.IO.Path.GetExtension(file1.FileName).ToLower();
                    switch (fileType)
                    {
                        case ".csv":
                            CsvFileParser cfp = new CsvFileParser();
                            dt = cfp.TryParse(stream, out outmsg);
                            break;
                        case ".xls":
                        case ".xlsx":
                            success = false;
                            //outmsg = "文件：" + fileName + "的文件格式接口待开发！<br />";
                            IExcel excel = ExcelFactory.CreateDefault();
                            DataSet ds = excel.Read(stream);
                            dt = ds.Tables[0];
                            break;
                        default:
                            success = false;
                            outmsg += "文件：" + fileName + "格式不支持！<br />";
                            break;
                    }
                    success = new Manager.PayManager().ImportInsurance(dt, fileName, ref outmsg);
                }
            }
            else
            {
                outmsg += "未获取到文件，请重试！<br />";
            }
            if (success)
                outmsg = "上传成功！<br />" + outmsg;
            else
                outmsg = "上传失败！<br />" + outmsg;
            return Json(outmsg);
        }

        public ActionResult ImportorDetail(int importId)
        {
            var info = new Manager.PayManager().QueryImportorInsuranceInfo(importId);
            if(info == null)
            {
                return Error("警告：非法请求");
            }
            if (UserContext.CurrentUser.IsCompanyUser && info.CreatorId != UserContext.CurrentUser.UserId)
            {
                return Error("错误提示：无访问权限");
            }

            ViewBag.ImportId = importId;
            ViewBag.importorTitle = info.Title;
            //HttpContext.Request.Headers[""]
            return View();
        }

        public ActionResult ExportInsuranceDetail(int importId)
        {
            var info = new Manager.PayManager().QueryImportorInsuranceInfo(importId);
            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = new Manager.PayManager().ExportInsuranceDetail(importId);
            POIStream stream = new POIStream();
            stream.AllowClose = false;
            excel.Write(stream, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode(info.Title, System.Text.Encoding.UTF8)));
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

        //详细列表
        //VIEW: /Pay/QueryInsuranceDetail
        public ActionResult QueryInsuranceDetail(int importId)
        {
            List<InsuranceDetailInfo> list = new Manager.PayManager().QueryInsuranceDetail(importId);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }



        // 发放列表
        // VIEW: /Pay/PayList
        public ActionResult PayList()
        {
            ViewBag.IsAdmin = UserContext.CurrentUser.IsCompanyUser?false:true;
            return View();
        }

        //查询发放列表
        //VIEW：/Pay/QueryPayList
        public ActionResult QueryPayList(string query)
        {
            List<Payment> list = new Manager.PayManager().QueryPayList(query);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        // 发放编辑器
        // VIEW: /Pay/PayEditor
        public ActionResult PayEditor(int id)
        {
            if (id != 0)
            {
                var info = new Manager.PayManager().GetPayDetailByPayId(id);
                if (info == null)
                {
                    return Error("警告：非法请求");
                }
                if (UserContext.CurrentUser.IsCompanyUser && info.CreatorId != UserContext.CurrentUser.UserId)
                {
                    return Error("错误提示：无访问权限");
                }
            }
            ViewBag.PayId = id;
            ViewBag.PayDate = System.DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01");
            return View();
        }

        //
        //VIEW: Pay/GetPayDetailByPayId
        public ActionResult GetPayDetailByPayId(int id)
        {
            Payment pay = new Manager.PayManager().GetPayDetailByPayId(id);
            return Json(pay);
        }

        public List<PayTableConfig> PayTableInfo
        {
            get
            {
                return null;
            }
        }

        //动态加载表头
        [HttpPost]
        public ActionResult RefreshTable(int id)
        {
            List<PayTableConfig> list = GetTableColumns();
            HashSet<int> temp = new Manager.PayManager().QueryTemplateDetail(id);
            if (temp.Count > 0)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].IsDynamic == true)
                    {
                        if (list[i].Subnode.Count > 0)
                        {
                            for (int n = list[i].Subnode.Count - 1; n >= 0; n--)
                            {
                                if (!temp.Contains(list[i].Subnode[n].ItemId))
                                    list[i].Subnode.RemoveAt(n);
                            }
                        }
                        else
                        {
                            if (!temp.Contains(list[i].ItemId))
                                list.RemoveAt(i);
                        }
                    }
                }
            }
            list = list.Where(i => i.IsLastStage == false && i.Subnode.Count > 0 || i.IsLastStage == true && i.Subnode.Count == 0).ToList();
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        //获取发放表配置
        public List<PayTableConfig> GetTableColumns()
        {
            List<PayTableConfig> list = new List<PayTableConfig>();
            XmlDocument doc = new XmlDocument();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "Setting/Pay/PayTableConfig.xml";
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("Root");
            XmlNodeList xn = root.ChildNodes;
            foreach (XmlNode xmlnode in xn)
            {
                XmlElement xe = (XmlElement)xmlnode;
                PayTableConfig config = new PayTableConfig();
                config.FieldCode = xe.GetAttribute("fieldName");
                config.FieldTitle = xe.GetAttribute("title");
                config.ItemId = int.Parse(xe.GetAttribute("code"));
                config.IsLastStage = bool.Parse(xe.GetAttribute("isLastStage"));
                config.IsEdit = bool.Parse(xe.GetAttribute("isEdit"));
                config.IsShow = bool.Parse(xe.GetAttribute("isShow"));
                config.IsDynamic = bool.Parse(xe.GetAttribute("isDynamic"));
                if (!config.IsLastStage)
                {
                    XmlNodeList child = xmlnode.ChildNodes;
                    foreach (XmlNode node in child)
                    {
                        XmlElement nd = (XmlElement)node;
                        PayTableConfig configs = new PayTableConfig();
                        configs.FieldCode = nd.GetAttribute("fieldName");
                        configs.FieldTitle = nd.GetAttribute("title");
                        configs.ItemId = int.Parse(nd.GetAttribute("code"));
                        configs.IsLastStage = bool.Parse(nd.GetAttribute("isLastStage"));
                        configs.IsEdit = bool.Parse(nd.GetAttribute("isEdit"));
                        configs.IsShow = bool.Parse(nd.GetAttribute("isShow"));
                        configs.IsDynamic = bool.Parse(nd.GetAttribute("isDynamic"));
                        config.Subnode.Add(configs);
                    }
                }
                list.Add(config);
            }
            return list;
        }

        public ActionResult GetPayDetail(int cmpid, int payid)
        {
            List<PayDetailInfo> list = new Manager.PayManager().GetPayDetail(cmpid, payid);
            JsonResult jsonResult = Json(list);
            return jsonResult;
        }

        [HttpPost]
        public ActionResult SavePayDetail(List<PayDetailInfo> list, int cmpid, string tname, string time, string count, int stus)
        {
            string msg = string.Empty;
            if (new Manager.PayManager().SavePayDetail(list, cmpid, tname, time, count, stus, ref msg))
            {
                msg = "发放成功";
            }
            else
            {
                msg = "发放失败！<br />" + msg;
            }
            return Json(msg);
        }
        /// <summary>
        /// 归档
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cmpid"></param>
        /// <param name="tname"></param>
        /// <param name="time"></param>
        /// <param name="count"></param>
        /// <param name="stus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnFilePay(List<PayDetailInfo> list, int cmpid, string tname, string time, string count, int stus)
        {
            string msg = string.Empty;
            if (new Manager.PayManager().SavePayDetail(list, cmpid, tname, time, count, stus, ref msg))
            {
                msg = "归档成功";
            }
            else
            {
                msg = "归档失败！<br />" + msg;
            }
            return Json(msg);
        }

        /// <summary>
        /// 删除发放表
        /// 只能删除未归档的发放表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeletePay(int id)
        {
            string msg = string.Empty;
            if (new Manager.PayManager().DeletePay(id,ref msg))
            {
                msg = "删除成功！";
            }
            else
            {
                msg = "删除失败！" + msg;
            }
            return Json(msg);
        }

        public ActionResult UpdatePayDetail(List<PayDetailInfo> list,int id)
        {
            string msg = string.Empty;
            if (new Manager.PayManager().UpdatePayDetail(list, id))
            {
                msg = "更新成功！";
            }
            else
            {
                msg = "更新失败！";
            }
            return Json(msg);
        }

        // 银行发放
        public ActionResult BankPayment()
        {
            return View();
        }

        /// <summary>
        /// 导出缴存数据
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public ActionResult ExportJiaoCun(string month)
        {
            IExcel excel = ExcelFactory.CreateDefault();
            string payMonth = DateTime.Parse(month).ToString("yyyyMM");
            DataSet ds = new Manager.PayManager().GetJiaoCun(payMonth);
            POIStream stream = new POIStream();
            stream.AllowClose = false;
            excel.Write(stream, ds, ExcelExtendType.XLSX);
            stream.AllowClose = true;
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            HttpResponse context = System.Web.HttpContext.Current.Response;
            try
            {
                context.ContentType = "application/ms-excel";
                context.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode(payMonth+"缴存数据", System.Text.Encoding.UTF8)));
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
