using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using BlueFramework.Blood;
using System.Data.Common;
using BlueFramework.User;
using BlueFramework.Blood.DataAccess;

namespace HrServiceCenterWeb.Manager
{
    public class ReportManager
    {
        /// <summary>
        /// 获取岗位分布饼图数据
        /// </summary>
        /// <returns></returns>
        public List<Models.CountetBase> GetPositionCounts()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            CommandParameter[] dbParms = new CommandParameter[1];
            if (UserContext.CurrentUser.IsCompanyUser)
            {
                dbParms[0] = new CommandParameter("where", $" and e.COMPANY_ID={UserContext.CurrentUser.CompanyId} ");
            }
            else
            {
                dbParms[0] = new CommandParameter("where", "");
            }
            List<Models.CountetBase> list = context.SelectList<Models.CountetBase>("hr.chart.positionCount", dbParms);
            return list;
        }

        /// <summary>
        /// 获取学历分布饼图数据
        /// </summary>
        /// <returns></returns>
        public List<Models.CountetBase> GetDegreeCounts()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            CommandParameter[] dbParms = new CommandParameter[1];
            if (UserContext.CurrentUser.IsCompanyUser)
            {
                dbParms[0] = new CommandParameter("where", $" and e.COMPANY_ID={UserContext.CurrentUser.CompanyId} ");
            }
            else
            {
                dbParms[0] = new CommandParameter("where", "");
            }
            List<Models.CountetBase> list = context.SelectList<Models.CountetBase>("hr.chart.degreeCount", dbParms);
            return list;
        }

        /// <summary>
        /// 获取人员统计柱状图数据
        /// </summary>
        public List<Models.CounterBO> GetBarChartData()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<Models.CounterBO> list = context.SelectList<Models.CounterBO>("hr.chart.employeeCount", null);
            return list;
        }

        /// <summary>
        /// 获取保险，应发工资，实发工资统计折线图数据
        /// </summary>
        /// <returns></returns>
        public List<Models.CounterVO> GetLineChartData()
        {
            List<Models.CounterVO> series = new List<Models.CounterVO>();
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            CommandParameter[] dbParms = new CommandParameter[1];
            if (UserContext.CurrentUser.IsCompanyUser)
            {
                dbParms[0] = new CommandParameter("where", $" and b.COMPANY_ID={UserContext.CurrentUser.CompanyId} ");
            }
            else
            {
                dbParms[0] = new CommandParameter("where", "");
            }
            List<Models.CounterBO> insuranceList = context.SelectList<Models.CounterBO>("hr.chart.insuranceCount", dbParms);
            List<Models.CounterBO> shouldPayList = context.SelectList<Models.CounterBO>("hr.chart.shouldPayCount", dbParms);
            List<Models.CounterBO> personPayList = context.SelectList<Models.CounterBO>("hr.chart.personPayCount", dbParms);
            List<Models.CounterBO> truePayList = context.SelectList<Models.CounterBO>("hr.chart.truePayCount", dbParms);
            List<Models.CounterBO> servicePayList = context.SelectList<Models.CounterBO>("hr.chart.servicePayCount", dbParms);

            Dictionary<string, DateTime> months = new Dictionary<string, DateTime>();
            DateTime startDate = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-01") );
            int i = 0;
            for(i = -12; i <=0; i++)
            {
                DateTime date = startDate.AddMonths(i);
                string month = date.ToString("yyyyMM");
                months.Add(month, date);
            }

            Models.CounterVO s1 = new Models.CounterVO();
            Models.CounterVO s2 = new Models.CounterVO();
            Models.CounterVO s3 = new Models.CounterVO();
            Models.CounterVO s4 = new Models.CounterVO();
            Models.CounterVO s5 = new Models.CounterVO();
            s1.DataAxis = s2.DataAxis = s3.DataAxis = months.Keys.ToArray();
            s1.Data = new decimal[13]; s1.Title = "单位社保公积金部分";
            s2.Data = new decimal[13]; s2.Title = "单位工资部分";
            s3.Data = new decimal[13]; s3.Title = "个人社保部分";
            s4.Data = new decimal[13]; s4.Title = "单位+个人费用总额";
            s5.Data = new decimal[13]; s5.Title = "服务费";

            i = 0;
            foreach (string x in months.Keys)
            {
                string month = months[x].ToString("yyyy-MM-dd");
                foreach (Models.CounterBO o in insuranceList)
                {
                    if (o.DataAxis == month)
                    {
                        s1.Data[i] = o.Data;
                        break;
                    }
                }
                foreach (Models.CounterBO o in shouldPayList)
                {
                    if (o.DataAxis == month)
                    {
                        s2.Data[i] = o.Data;
                        break;
                    }
                }
                foreach (Models.CounterBO o in personPayList)
                {
                    if (o.DataAxis == month)
                    {
                        s3.Data[i] = o.Data;
                        break;
                    }
                }
                foreach (Models.CounterBO o in truePayList)
                {
                    if (o.DataAxis == month)
                    {
                        s4.Data[i] = o.Data;
                        break;
                    }
                }
                foreach (Models.CounterBO o in servicePayList)
                {
                    if (o.DataAxis == month)
                    {
                        s5.Data[i] = o.Data;
                        break;
                    }
                }
                i++;
            }
            series.Add(s1);
            series.Add(s2);
            series.Add(s3);
            series.Add(s4);
            series.Add(s5);
            return series;
        }

        private decimal[] getMonthData(List<Models.CountetBase> list)
        {
            if (list == null)
            {
                return new decimal[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            string[] month = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            decimal[] data = new decimal[12];
            for (int i = 0; i < month.Length; i++)
            {
                Models.CountetBase obj = new Models.CountetBase();
                obj = list.Where(l => l.name == month[i]).FirstOrDefault();
                if (obj != null)
                    data[i] = obj.moneyValue;
                else
                    data[i] = 0;
            }
            return data;
        }

        public DataSet GetWxyjByYear(int year)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.WxyjYear");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            db.AddInParameter(command, "payYear", DbType.String, year.ToString());
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }

        public DataSet GetWxyjByMonth(string date)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.WxyjMonth");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            db.AddInParameter(command, "payDate", DbType.String, date);
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }


        public DataSet GetRybdByYear(int year)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.RybdYear");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            db.AddInParameter(command, "year", DbType.String, year.ToString());
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }

        public DataSet GetRybdByMonth(string date)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.RybdMonth");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            string nextMonth = DateTime.Parse(date).AddMonths(1).ToString("yyyy-MM-dd");
            db.AddInParameter(command, "queryDate", DbType.String, date);
            db.AddInParameter(command, "beginDate", DbType.String, date);
            db.AddInParameter(command, "endDate", DbType.String, nextMonth);
  
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];

            // 填充人员
            DataTable xrzTable = GetRybdXrzPersonsByMonth(date, nextMonth);
            foreach(DataRow dataRow in dt.Rows)
            {
                int cmpId = int.Parse(dataRow["COMPANY_ID"].ToString());
                int pid = int.Parse(dataRow["POSITION_ID"].ToString());
                var array = xrzTable.AsEnumerable().Where(o => o.Field<Int32>("COMPANY_ID") == cmpId && o.Field<Int32>("POSITION_ID") == pid).Select(o => o.Field<string>("PERSON_NAME"));
                //DataRow[] rows = personTable.Select($"COMPANY_ID={cmpId} and POSITION_ID={pid}");
                string names = String.Join(",", array);
                if (!string.IsNullOrEmpty(names))
                {
                    dataRow["xrzName"] = names;
                }
            }
            DataTable lzTable = GetRybdLzPersonsByMonth(date, nextMonth);
            foreach (DataRow dataRow in dt.Rows)
            {
                int cmpId = int.Parse(dataRow["COMPANY_ID"].ToString());
                int pid = int.Parse(dataRow["POSITION_ID"].ToString());
                var array = lzTable.AsEnumerable().Where(o => o.Field<Int32>("COMPANY_ID") == cmpId && o.Field<Int32>("POSITION_ID") == pid).Select(o => o.Field<string>("PERSON_NAME"));
                //DataRow[] rows = personTable.Select($"COMPANY_ID={cmpId} and POSITION_ID={pid}");
                string names = String.Join(",", array);
                if (!string.IsNullOrEmpty(names))
                {
                    dataRow["lzName"] = names;
                }
            }
            // 合并单元格
            string lastCompany = string.Empty;
            foreach (DataRow dataRow in dt.Rows)
            {
                string cmp = dataRow["NAME"].ToString();
                if (cmp.Equals(lastCompany))
                {
                    dataRow["NAME"] = "";
                    dataRow["yfgz"] = 0;
                    dataRow["sfgz"] = 0;
                }
                else
                {
                    lastCompany = cmp;
                }
            }

            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }

        public DataTable GetRybdXrzPersonsByMonth(string beginDate,string endDate)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.RybdXrzPesonsMonth");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            db.AddInParameter(command, "beginDate", DbType.String, beginDate);
            db.AddInParameter(command, "endDate", DbType.String, endDate);

            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        public DataTable GetRybdLzPersonsByMonth(string beginDate, string endDate)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.RybdLzPesonsMonth");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            db.AddInParameter(command, "beginDate", DbType.String, beginDate);
            db.AddInParameter(command, "endDate", DbType.String, endDate);


            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        private void FillRowNum(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            dt.Columns.Add("rownum");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["rownum"] = i + 1;
            }
        }

        private void SumDataSet(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            DataRow row = dt.NewRow();
            foreach(DataColumn column in dt.Columns)
            {

                if (column.DataType != Type.GetType("System.Decimal") && column.DataType != Type.GetType("System.Int32"))
                    continue;
                double total = 0;
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    string rowValue = dt.Rows[i][column.ColumnName].ToString();
                    if(!string.IsNullOrEmpty(rowValue))
                     total += double.Parse(rowValue);
                }
                row[column.ColumnName] = total;
            }
            dt.Rows.Add(row);
        }

        public DataSet GetGwtj()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.Gwtj");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }

        public DataSet GetPersonPayMonthDetail(int year,int month,int companyId)
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.PersonPayMonth");
            DateTime date = new DateTime(year, month, 1);
            sql = sql.Replace("#yyyy-mm-dd#", date.ToString("yyyy-MM-dd"));
            sql = sql.Replace("#yyyymm#", date.ToString("yyyyMM"));
            if (companyId > 0)
            {
                sql = sql.Replace("#where#", " and e.COMPANY_ID="+companyId);
            }
            else
            {
                sql = sql.Replace("#where#", "" );
            }
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            command.CommandTimeout = BlueFramework.Data.Database.CONNECTION_TIME_OUT;
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            FillRowNum(ds);
            SumDataSet(ds);
            return ds;
        }
    }
}