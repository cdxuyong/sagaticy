using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BlueFramework.Blood;
using HrServiceCenterWeb.Models;
using BlueFramework.Blood.Config;
using BlueFramework.Blood.DataAccess;

namespace HrServiceCenterWeb.Manager
{
    public class EmployeeManager
    {
        private int ContractWarnningDays
        {
            get
            {
                string day = System.Configuration.ConfigurationManager.AppSettings.Get("ContractWarningDay");
                return string.IsNullOrEmpty(day)?30:int.Parse(day);
            }
        }
        private int RetireWarnningDays
        {
            get
            {
                string day = System.Configuration.ConfigurationManager.AppSettings.Get("RetireWarningDay");
                return string.IsNullOrEmpty(day) ? 30 : int.Parse(day);
            }
        }

        public List<PositionInfo> GetPositions()
        {
            EntityContext context = Session.CreateContext();
            List<PositionInfo> list = context.SelectList<PositionInfo>("hr.position.findPositions", null);
            return list;
        }

        public List<CompanyInfo> GetCompanies(string query)
        {
            EntityContext context = Session.CreateContext();
            CommandParameter[] dbParms = new CommandParameter[2];
            dbParms[0] = new CommandParameter("value", query);
            if (BlueFramework.User.UserContext.CurrentUser.IsCompanyUser)
            {
                dbParms[1] = new CommandParameter("where", $" and t.COMPANY_ID=" + BlueFramework.User.UserContext.CurrentUser.CompanyId);
            }
            else
            {
                dbParms[1] = new CommandParameter("where", "");
            }
            List<CompanyInfo> list = context.SelectList<CompanyInfo>("hr.company.findCompanys", dbParms);
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
                var olds = context.SelectList<CompanyInfo>("hr.company.findCompanyByName", companyInfo.Name);
                if(olds != null && olds.Count > 0)
                {
                    if(companyInfo.Name.Equals(olds[0].Name) && !olds[0].CompanyId.Equals(companyInfo.CompanyId))
                    {
                        return null;
                    }
                }
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
                    BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Warn(ex.Message);
                    context.Rollback();
                    companyInfo = null;
                }
            }
            return companyInfo;
        }

        public int GetMaxPersonCode()
        {
            int max = new DataAccess.EmployeeAccess().GetMaxPersonCode();
            return max + 1;
        }

        public bool SaveRecharge(CompanyAccountRecordInfo accountRecordInfo)
        {
            bool pass = true;
            
            accountRecordInfo.CreateTime = DateTime.Now;

            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    CompanyAccountInfo accountInfo = new CompanyAccountInfo()
                    {
                        CompanyId = accountRecordInfo.CompanyId,
                        AccountId = accountRecordInfo.AccountId,
                        AccountBalance = accountRecordInfo.AccountBalance+accountRecordInfo.Money
                    };
                    context.Save<CompanyAccountInfo>("hr.company.updateCompanyAccount", accountInfo);
                    context.Save<CompanyAccountRecordInfo>("hr.company.insertCompanyAccountDetail", accountRecordInfo);
                    context.Commit();
                }
                catch
                {
                    context.Rollback();
                    pass = false;
                }
            }
            return pass;
        }

        public bool ImportRecharges(DataTable dataTable, out string message)
        {
            bool pass = true;
            message = string.Empty;
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    foreach(DataRow row in dataTable.Rows)
                    {
                        var o = new CompanyAccountBO
                        {
                            Money = decimal.Parse(row["账户余额"].ToString()),
                            CompanyName= row["单位名称"].ToString()
                        };
                        context.Save<CompanyAccountBO>("hr.company.updateCompanyBalanceByCompayName", o);
                    }
                    context.Commit();
                }
                catch(Exception e)
                {
                    message = e.Message;
                    context.Rollback();
                    pass = false;
                }
            }
            return pass;
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

        public EmployeeInfo GetEmployee(int personId)
        {
            EmployeeInfo employeeInfo = null;
            using (EntityContext context = new EntityContext())
            {
                employeeInfo = context.Selete<EmployeeInfo>("hr.employee.findEmployee", personId);
            }
            return employeeInfo;
        }

        public EmployeeInfo GetEmployee(string cardid)
        {
            EmployeeInfo employeeInfo = null;
            using (EntityContext context = new EntityContext())
            {
                employeeInfo = context.Selete<EmployeeInfo>("hr.employee.findEmployeeByCardid", cardid);
            }
            return employeeInfo;
        }

        public List<EmployeeInfo> GetEmployees(int companyId)
        {
            EmployeeInfo employee = new EmployeeInfo();
            employee.CompanyId = companyId;
            return GetEmployees(employee);
        }

        public List<EmployeeInfo> GetEmployees(EmployeeInfo employee)
        {
            List<EmployeeInfo> list = null;
            using (EntityContext context = new EntityContext())
            {
                list = context.SelectListByTemplate<EmployeeInfo>("hr.employee.findEmployees", employee);
            }
            return list;
        }

        public List<EmployeeInfo> GetContractBeEndingEmployees()
        {
            List<EmployeeInfo> list = null;
            using (EntityContext context = new EntityContext())
            {

                string value = System.DateTime.Now.AddDays(ContractWarnningDays).ToString("yyyy-MM-dd");
                list = context.SelectList<EmployeeInfo>("hr.employee.findContractBeEndingEmployees", value);
            }
            foreach(EmployeeInfo employee in list)
            {
                DateTime contractDate = DateTime.Parse(employee.ContractTime);
                employee.ContractDays = int.Parse(Math.Round(contractDate.Subtract(DateTime.Now).TotalDays,0).ToString());
            }
            return list;
        }

        public List<EmployeeInfo> GetRetireBeEndingEmployees()
        {
            List<EmployeeInfo> list = null;
            using (EntityContext context = new EntityContext())
            {

                string value = System.DateTime.Now.AddDays(RetireWarnningDays).ToString("yyyy-MM-dd");
                list = context.SelectList<EmployeeInfo>("hr.employee.findRetireBeEndingEmployees", value);
            }
            foreach (EmployeeInfo employee in list)
            {
                DateTime dateTime = DateTime.Parse(employee.RetireTime);
                employee.RetireDays = int.Parse(Math.Round(dateTime.Subtract(DateTime.Now).TotalDays, 0).ToString());
            }
            return list;
        }

        public DataSet GetEmployees()
        {
            EntityConfig config = ConfigManagent.Configs["hr.employee.exportPersons"];
            BlueFramework.Data.Database database = new BlueFramework.Data.DatabaseProviderFactory().CreateDefault();
            string sql = config.Sql;
            DataSet ds = database.ExecuteDataSet(CommandType.Text, sql);
            return ds;
        }

        public DataSet GetSimpleEmployees()
        {
            EntityConfig config = ConfigManagent.Configs["hr.employee.exportSimplePersons"];
            BlueFramework.Data.Database database = new BlueFramework.Data.DatabaseProviderFactory().CreateDefault();
            string sql = config.Sql;
            DataSet ds = database.ExecuteDataSet(CommandType.Text, sql);
            return ds;
        }

        public bool SaveEmployee(EmployeeInfo employeeInfo)
        {
            bool pass;
            using (EntityContext context = Session.CreateContext())
            {
                if (employeeInfo.PersonId == 0)
                {
                    employeeInfo.CreateTime = DateTime.Now.ToString();
                    employeeInfo.Creator = BlueFramework.User.UserContext.CurrentUser.UserId;
                    pass = context.Save<EmployeeInfo>("hr.employee.insertEmployee", employeeInfo);

                }
                else
                    pass = context.Save<EmployeeInfo>("hr.employee.updateEmployee", employeeInfo);
            }

            return pass;
        }

        public bool DeleteEmployee(int personId)
        {
            bool pass;
            using (EntityContext context = Session.CreateContext())
            {
                int result = context.Selete<int>("hr.employee.isUsed", personId);
                if (result == 1)
                    pass = false;
                else
                    pass = context.Delete("hr.employee.deleteEmployee", personId);
            }

            return pass;
        }

        public bool ImportEmployees(DataTable dataTable, out string message)
        {
            message = string.Empty;
            #region validate
            var cmpList = new Dictionary<string, CompanyInfo>();
            try
            {
                cmpList = GetCompanies(string.Empty).ToDictionary(o => o.Name, o => o);
            }
            catch (Exception ex)
            {
                message = $"单位名称存在重复，请删除重复记录，原因详情：{ex.Message}";
                return false;
            }
            var posList = new Dictionary<string,PositionInfo>();
            try
            {
                posList = BaseCodeProvider.Current.GetPositions().ToDictionary(o => o.PositionName, o => o);
            }
            catch (Exception ex)
            {
                message = $"岗位名称存在重复，请删除重复记录，原因详情：{ex.Message}";
                return false;
            }
            var eduList = new Dictionary<string, BaseCodeInfo>();
            try
            {
                eduList = BaseCodeProvider.Current.GetEduciationCodes().ToDictionary(o => o.Text, o => o);
            }
            catch (Exception ex)
            {
                message = $"学历名称存在重复，请删除重复记录，原因详情：{ex.Message}";
                return false;
            }
            dataTable.Columns.Add("cmpid", Type.GetType("System.Int32"));
            dataTable.Columns.Add("posid", Type.GetType("System.Int32"));
            dataTable.Columns.Add("eduid", Type.GetType("System.Int32"));
            bool pass = true;
            message = string.Empty;
            for(int i=0;i<dataTable.Rows.Count;i++)
            {
                DataRow row = dataTable.Rows[i];
                try
                {
                    var cmp = row["单位"].ToString();
                    row["cmpid"] = cmpList[cmp].CompanyId;
                }
                catch 
                { 
                    pass = false;
                    message += $"{i+1}行单位有误，";
                }
                try
                {
                    var pos = row["岗位"].ToString();
                    row["posid"] = posList[pos].PositionId;
                }
                catch
                {
                    pass = false;
                    message += $"{i + 1}行岗位有误，";
                }
                try
                {
                    var edu = row["学历"].ToString();
                    row["eduid"] = int.Parse( eduList[edu].ObjectValue );
                }
                catch
                {
                    pass = false;
                    message += $"{i + 1}行学历有误，";
                }
            }
            #endregion
            #region repleace tab
            //foreach(DataRow row in dataTable.Rows)
            //{
            //    foreach(DataColumn col in dataTable.Columns)
            //    {
            //        if(row[col.ColumnName] != null)
            //            row[col.ColumnName] = row[col.ColumnName].ToString().Replace("\r\n", " ");
            //    }
            //}
            #endregion
            if (!pass) return false;
            bool hasFee = dataTable.Columns.Contains("服务费") ? true : false ;
            string bankName = dataTable.Columns.Contains("银行名称") ? "银行名称" : "银行卡";
            string bankCard = dataTable.Columns.Contains("银行卡号") ? "银行卡号" : "银行";
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                string carid = row["身份证"].ToString();
                var person = GetEmployee(carid);
                if (person == null)
                {
                    person = new EmployeeInfo();
                    person.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                    person.Creator = BlueFramework.User.UserContext.CurrentUser.UserId;
                    person.ServiceFee = 100;
                }
                person.PersonName = row["姓名"].ToString();
                person.CardId = carid;
                person.Sex = row["性别"].ToString()=="男"? "男" : "女";
                try
                {
                    person.Birthday = DateTime.Parse(row["出生日期"].ToString()).ToString("yyyy-MM-dd");
                }
                catch { message += $"{i + 1}行日期有误，";pass = false; }
                person.Polity = row["政治面貌"].ToString();
                person.Nation = row["民族"].ToString();
                person.Phone = row["手机"].ToString();
                person.Address = row["地址"].ToString();
                person.ContactsPerson = row["联系人"].ToString();
                person.ContactsPhone = row["联系人电话"].ToString();
                person.School = row["学校"].ToString();
                person.Specialty = row["专业"].ToString();
                person.CompanyId = int.Parse( row["cmpid"].ToString() );
                person.PositionId = int.Parse( row["posid"].ToString() );
                person.Degree = int.Parse(row["eduid"].ToString());
                person.BankName = row[bankName].ToString();
                person.BankCard = row[bankCard].ToString();
                person.BankCode = row["银行行号"].ToString();
                person.EmployType = row["任职受雇从业类型"].ToString();
                person.EmployDate = row["任职受雇从业日期"].ToString();
                try
                {
                    person.JoinWorkTime = DateTime.Parse(row["参工时间"].ToString()).ToString("yyyy-MM-dd");
                }
                catch { message += $"{i + 1}行参工时间有误，"; pass = false; }
                
                person.ContractNO = row["合同编号"].ToString();
                try
                {
                    person.ContractTime = DateTime.Parse(row["合同到期日期"].ToString()).ToString("yyyy-MM-dd");
                }
                catch { message += $"{i + 1}行合同到期日期有误，"; pass = false; }
                
                person.AgreementNO = row["协议编号"].ToString();
                try
                {
                    person.ServiceFee = hasFee ? int.Parse(row["服务费"].ToString()) : 0;
                }
                catch 
                {
                    message += $"{i + 1}行服务费非数字，"; 
                    pass = false;
                }
                try
                {
                    person.PersonCode = int.Parse(row["编号"].ToString());
                }
                catch { message += $"{i + 1}行编号有误，"; pass = false; }

                if (pass)
                    pass = SaveEmployee(person);
                if (!pass)
                {
                    message += $"{i + 1}行{person.PersonName}未能入库，";
                    return pass;
                }
            }
            return pass;
        }
    }
}