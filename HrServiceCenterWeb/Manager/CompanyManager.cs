using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BlueFramework.Blood;
using HrServiceCenterWeb.Models;
using BlueFramework.Blood.Config;

namespace HrServiceCenterWeb.Manager
{
    public class CompanyManager
    {

        public List<PositionInfo> GetPositions()
        {
            EntityContext context = Session.CreateContext();
            List<PositionInfo> list = context.SelectList<PositionInfo>("hr.position.findPositions", null);
            return list;
        }

        public List<CompanyInfo> GetCompanies(string query)
        {
            EntityContext context = Session.CreateContext();
            List<CompanyInfo> list = context.SelectList<CompanyInfo>("hr.company.findCompanys", query);
            return list;
        }

        public List<CompanyInfo> GetPayCompanies()
        {
            EntityContext context = Session.CreateContext();
            List<CompanyInfo> list = context.SelectList<CompanyInfo>("hr.company.getPayCompanys", null);
            return list;
        }

        public CompanyInfo GetCompany(int companyId)
        {
            EntityContext context = Session.CreateContext();
            CompanyInfo companyInfo = context.Selete<CompanyInfo>("hr.company.findCompanyById", companyId);
            companyInfo.Positions = context.SelectList<CompanyPositionSetInfo>("hr.company.findPositions", companyId);
            return companyInfo;
        }

        public CompanyInfo SaveCompany(CompanyInfo companyInfo)
        {

            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();

                    if (companyInfo.CompanyId > 0)
                        context.Save<CompanyInfo>("hr.company.updateCompany", companyInfo);
                    else
                    {
                        context.Save<CompanyInfo>("hr.company.insertCompany", companyInfo);
                        CompanyAccountInfo account = new CompanyAccountInfo()
                        {
                            CompanyId = companyInfo.CompanyId
                        };
                        context.Save<CompanyAccountInfo>("hr.company.insertAccount", account);
                    }

                    context.Commit();
                }
                catch (Exception ex)
                {
                    ex = null;
                    context.Rollback();
                    companyInfo = null;
                }
            }
            return companyInfo;
        }

        public bool DeleteCompany(int companyId)
        {
            bool pass = true;
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    context.Delete("hr.company.deleteCompanyAccount", companyId);
                    context.Delete("hr.company.deleteCompany", companyId);
                    context.Commit();
                }
                catch (Exception ex)
                {
                    ex = null;
                    context.Rollback();
                    pass = false;
                }
            }
            return pass;
        }

        public List<CompanyPositionSetInfo> GetPositonSets(int companyId)
        {
            List<CompanyPositionSetInfo> list = null;
            using (EntityContext context = new EntityContext())
            {
                list = context.SelectList<CompanyPositionSetInfo>("hr.company.findPositions", companyId);
            }
            return list;
        }

        public bool SavePosition(CompanyPositionSetInfo positionSetInfo)
        {
            bool pass = true;
            using (EntityContext context = new EntityContext())
            {
                try
                {
                    context.BeginTransaction();
                    context.Delete<CompanyPositionSetInfo>("hr.company.deletePositions", positionSetInfo);
                    context.Save< CompanyPositionSetInfo>("hr.company.insertPosition", positionSetInfo);
                    context.Commit();
                }
                catch
                {
                    pass = false;
                    context.Rollback();
                }
            }
            return pass;
        }

        public bool DeletePostion(CompanyPositionSetInfo positionSetInfo)
        {
            using (EntityContext context = new EntityContext())
            {
                context.Delete<CompanyPositionSetInfo>("hr.company.deletePositions", positionSetInfo);
            }
            return true;
        }



        public bool ImportAccountDetail(DataTable dataTable, string fileName, out string message)
        {
            bool pass = true;
            message = string.Empty;
            //判断是否已经入库
            var list = QueryAccountImport(fileName);
            if (list.Count > 0)
            {
                message = "文件：" + fileName + "已上传！";
                return false;
            }

            var cmpList = GetCompanies("");
            Dictionary<string, CompanyInfo> cmps = new Dictionary<string, CompanyInfo>();
            cmpList.ForEach(x =>
            {
                if (!cmps.ContainsKey(x.Name) )
                {
                    cmps.Add(x.Name, x);
                }
            });

            int i = 0;
            int passRows = 0;
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();


                    foreach (DataRow row in dataTable.Rows)
                    {
                        i++;
                        CompanyAccountRecordInfo ari = new CompanyAccountRecordInfo();
                        var cmpName = row["用工单位"].ToString();
                        ari.PayDate = BlueFramework.Common.Converts.Convert2Date(row["结算月份"].ToString());
                        ari.ItemName = row["结算项目"].ToString();
                        ari.Money = decimal.Parse(row["结算金额"].ToString());
                        ari.BillDate = DateTime.Parse(row["开票日期"].ToString());
                        ari.EntryDate = DateTime.Parse(row["到账日期"].ToString());
                        ari.OptDate = DateTime.Parse(row["操作日期"].ToString());
                        ari.PersonName = row["操作人"].ToString();
                        ari.Remark = row["备注"].ToString();
                        ari.CreateTime = DateTime.Now;
                        ari.Creator = BlueFramework.User.UserContext.CurrentUser.UserId;
                        ari.ImportName = fileName;
                        if (cmps.ContainsKey(cmpName))
                        {
                            ari.CompanyId = cmps[cmpName].CompanyId;
                            ari.AccountId = cmps[cmpName].AccountId;
                            passRows++;
                        }
                        else
                        {
                            //未找到人员，忽略该行
                            message += string.Format("第{0}行 {1} 单位不匹配， ", i, cmpName);
                            continue;
                        }
                        context.Save<CompanyAccountRecordInfo>("hr.company.insertCompanyAccountDetail", ari);
                    }
                    context.Commit();
                }
                catch (Exception e)
                {
                    message = e.Message;
                    context.Rollback();
                    pass = false;
                }
            }
            return pass;
        }

        /// <summary>
        /// 查询导入表明细
        /// </summary>
        /// <param name="importName"></param>
        /// <returns></returns>
        public List<CompanyAccountRecordInfo> QueryAccountDetail(string importName)
        {
            List<CompanyAccountRecordInfo> list = null;
            using (EntityContext context = new EntityContext())
            {
                list = context.SelectList<CompanyAccountRecordInfo>("hr.company.queryAccountDetail", importName);
            }
            return list;
        }

        /// <summary>
        /// 查询导入列表
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public List<CompanyAccountImportVO> QueryAccountImport(string q)
        {
            List<CompanyAccountImportVO> list = null;
            using (EntityContext context = new EntityContext())
            {
                list = context.SelectList<CompanyAccountImportVO>("hr.company.queryAccountImport",q);
            }
            return list;
        }

        public List<CompanyAccountImportVO> QueryAccountMonth(string q)
        {
            List<CompanyAccountImportVO> list = null;
            using (EntityContext context = new EntityContext())
            {
                list = context.SelectList<CompanyAccountImportVO>("hr.company.queryAccountMonth", q);
            }
            return list;
        }
    }
}