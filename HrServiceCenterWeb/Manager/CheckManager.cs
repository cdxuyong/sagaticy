using BlueFramework.Blood;
using BlueFramework.User;
using BlueFramework.User.Models;
using HrServiceCenterWeb.DataAccess;
using HrServiceCenterWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using BlueFramework.Data;
using BlueFramework.Blood.Config;

namespace HrServiceCenterWeb.Manager
{
    public class CheckManager
    {
        public List<CheckInfo> QueryImportorCheckList(string query)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<CheckInfo> list = context.SelectList<CheckInfo>("hr.check.findChecks", query);
            return list;
        }

        private Dictionary<string, int> getItemCardId()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<EmployeeInfo> list = context.SelectList<EmployeeInfo>("hr.employee.findEmployeesCardId", null);
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (EmployeeInfo ei in list)
            {
                string key = string.Format("{0}.{1}", ei.CardId.ToLower(), ei.PersonName.Trim());
                if (!dic.ContainsKey(key))
                    dic.Add(key, ei.PersonId);
            }
            return dic;
        }

        /// <summary>
        /// 每日考勤导入
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        /// <param name="outmsg"></param>
        /// <returns></returns>
        public bool ImportDayChecks(DataTable dt, string fileName, ref string outmsg)
        {
            Dictionary<string, int> cardIds = getItemCardId();
            int rowIndex = 0;
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    //判断是否已经入库
                    List<CheckInfo> list = context.SelectList<CheckInfo>("hr.check.findChecks", fileName);
                    if (list.Count > 0)
                    {
                        outmsg += "文件：" + fileName + "已上传！";
                        return false;
                    }
                    //入库导入主表
                    CheckInfo ii = new CheckInfo();
                    ii.Title = fileName;
                    ii.CreatorId = UserContext.CurrentUser.UserId;
                    ii.CreateTime = DateTime.Now.ToShortDateString();
                    ii.ImportType = 1;
                    context.Save<CheckInfo>("hr.check.insertCheck", ii);
                    int passRows = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        rowIndex++;
                        //入库导入详细表
                        CheckDayInfo idi = new CheckDayInfo();
                        idi.ImportId = ii.ImportId;
                        idi.PersonName = row["姓名"].ToString();
                        idi.CheckDate = DateTime.Parse( row["考勤日期"].ToString());
                        idi.StartTime = DateTime.Parse( row["上班打卡"].ToString().Trim());
                        idi.EndTime = DateTime.Parse(row["下班打卡"].ToString());
                        idi.WorkHours = decimal.Parse(row["有效工时"].ToString());
                        idi.CheckAdress = row["考勤地点"].ToString();
                        idi.Demo = row["备注"].ToString();
                        string cardId = row["身份证号码"].ToString().ToLower() + "." + idi.PersonName;
                        if (cardIds.ContainsKey(cardId))
                        {
                            idi.PersonId = cardIds[cardId];
                            passRows++;
                        }
                        else
                        {
                            //未找到人员，忽略该行
                            outmsg += string.Format("第{0}行 {1} 人员的身份证号码和姓名不匹配， ", rowIndex, idi.PersonName);
                            continue;
                        }
                        context.Save<CheckDayInfo>("hr.check.insertCheckDay", idi);
                    }
                    context.Commit();
                    outmsg = string.Format("Excel中一共{0}行，导入成功{1}行。{2}", dt.Rows.Count, passRows, outmsg);
                    return true;
                }
                catch (Exception ex)
                {
                    outmsg += $"服务器内部错误，请联系管理员【行号：{rowIndex+1}】：" + ex.Message;
                    context.Rollback();
                    return false;
                }
            }
        }

        public bool DeleteDayChecks(int importId,ref string msg)
        {
            return false;
        }

        // 查询考勤详情
        public List<CheckDayInfo> QueryCheckDayList(int checkId)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<CheckDayInfo> list = context.SelectList<CheckDayInfo>("hr.check.findDayChecks", checkId);
            return list;
        }


        public List<CheckDayInfo> QueryCheckDayList(string personName,string cmpName)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            BlueFramework.Blood.DataAccess.CommandParameter[] ps = {
                new BlueFramework.Blood.DataAccess.CommandParameter("pName",personName),
                new BlueFramework.Blood.DataAccess.CommandParameter("cmpName",cmpName)
            };
            List<CheckDayInfo> list = context.SelectList<CheckDayInfo>("hr.check.findDayChecksByKeys",ps);
            return list;
        }

        #region month
        /// <summary>
        /// 获取单位月报统计
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<CheckMonthSumVO> QueryMonthSummaryList(string query)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<CheckMonthSumVO> list = context.SelectList<CheckMonthSumVO>("hr.check.sumMonthChecks", query);
            list.ForEach(x =>
            {
                x.CheckMonth = x.Month.ToString("yyyy-MM");
            });
            return list;
        }
        /// <summary>
        /// 查询单位月报明细
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<CheckMonthInfo> QueryCheckMonthList(string companyName,string month)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            BlueFramework.Blood.DataAccess.CommandParameter[] ps = { 
                new BlueFramework.Blood.DataAccess.CommandParameter("pName",companyName),
                new BlueFramework.Blood.DataAccess.CommandParameter("pDate",month+"-01")
            };
            List<CheckMonthInfo> list = context.SelectList<CheckMonthInfo>("hr.check.findMonthDetail",ps);
            return list;
        }
        public bool ImportMonthChecks(DataTable dt, string fileName, ref string outmsg)
        {
            Dictionary<string, int> cardIds = getItemCardId();
            int rowIndex = 0;
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    //判断是否已经入库
                    List<CheckInfo> list = context.SelectList<CheckInfo>("hr.check.findMonthChecks", fileName);
                    if (list.Count > 0)
                    {
                        outmsg += "文件：" + fileName + "已上传！";
                        return false;
                    }
                    //入库导入主表
                    CheckInfo ii = new CheckInfo();
                    ii.Title = fileName;
                    ii.CreatorId = UserContext.CurrentUser.UserId;
                    ii.CreateTime = DateTime.Now.ToShortDateString();
                    ii.ImportType = 2;
                    context.Save<CheckInfo>("hr.check.insertCheck", ii);
                    int passRows = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        rowIndex++;
                        //入库导入详细表
                        CheckMonthInfo idi = new CheckMonthInfo();
                        idi.ImportId = ii.ImportId;
                        idi.PersonName = row["姓名"].ToString();
                        idi.CheckMonth = DateTime.Parse(row["考勤月份"].ToString());
                        idi.CheckMonth = new DateTime(idi.CheckMonth.Year, idi.CheckMonth.Month, 1);
                        idi.CheckDays = int.Parse(row["实际打卡天数"].ToString().Trim());
                        idi.LateDays = int.Parse(row["迟到天数"].ToString());
                        idi.LostDays = int.Parse(row["缺勤天数"].ToString());
                        string cardId = row["身份证号码"].ToString().ToLower() + "." + idi.PersonName;
                        if (cardIds.ContainsKey(cardId))
                        {
                            idi.PersonId = cardIds[cardId];
                            passRows++;
                        }
                        else
                        {
                            //未找到人员，忽略该行
                            outmsg += string.Format("第{0}行 {1} 人员的身份证号码和姓名不匹配， ", rowIndex, idi.PersonName);
                            continue;
                        }
                        context.Save<CheckMonthInfo>("hr.check.insertCheckMonth", idi);
                    }
                    context.Commit();
                    outmsg = string.Format("Excel中一共{0}行，导入成功{1}行。{2}", dt.Rows.Count, passRows, outmsg);
                    return true;
                }
                catch (Exception ex)
                {
                    outmsg += $"服务器内部错误，请联系管理员【行号：{rowIndex + 1}】：" + ex.Message;
                    context.Rollback();
                    return false;
                }
            }
        }
        #endregion
    }
}