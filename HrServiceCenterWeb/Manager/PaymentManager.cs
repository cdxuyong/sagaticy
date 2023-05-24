using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using HrServiceCenterWeb.Models;
using BlueFramework.Blood;
using BlueFramework.Blood.DataAccess;

namespace HrServiceCenterWeb.Manager
{
    public class PaymentManager
    {
        private const decimal MANAGE_PAYVALUE = 100;

        public bool UpdatePayment(Payment payment)
        {
            EntityContext context = Session.CreateContext();
            bool pass = context.Save<Payment>("hr.payment.savePayment", payment);
            return pass;
        }

        public bool SubmitPayment(int paymentId,out string message)
        {
            Payment payment = LoadPayment(paymentId);
            payment.Status = 2;

            int detailID = new DataAccess.DbAccess().GetMax("HR_CO_ACCOUNT_RECORD", "id");
            using(EntityContext context = Session.CreateContext())
            {
                try
                {
                    CompanyInfo companyInfo = context.Selete<CompanyInfo>("hr.company.findCompanyById", payment.CompanyId);

                    CompanyAccountRecordInfo accountRecordInfo = new CompanyAccountRecordInfo();
                    accountRecordInfo.Id = detailID + 1;
                    accountRecordInfo.CompanyId = payment.CompanyId;
                    accountRecordInfo.AccountId = companyInfo.AccountId;
                    accountRecordInfo.AccountBalance = companyInfo.AccountBalance;
                    accountRecordInfo.Money = -1 * payment.Total;
                    accountRecordInfo.CreateTime = DateTime.Now;
                    accountRecordInfo.PayDate = DateTime.Parse(payment.PayMonth);
                    accountRecordInfo.ItemName = "应扣总额";
                    accountRecordInfo.EntryDate = DateTime.Now;
                    accountRecordInfo.OptDate = DateTime.Now;
                    accountRecordInfo.BillDate = DateTime.Now;
                    accountRecordInfo.PersonName = BlueFramework.User.UserContext.Current.UserName;

                    context.BeginTransaction();
                    CompanyAccountInfo accountInfo = new CompanyAccountInfo()
                    {
                        CompanyId = accountRecordInfo.CompanyId,
                        AccountId = accountRecordInfo.AccountId,
                        AccountBalance = accountRecordInfo.AccountBalance + accountRecordInfo.Money
                    };
                    context.Save<CompanyAccountInfo>("hr.company.updateCompanyAccount", accountInfo);
                    context.Save<CompanyAccountRecordInfo>("hr.company.insertCompanyAccountDetail", accountRecordInfo);
                    context.Save<Payment>("hr.payment.submitPayment", payment);
                    context.Commit();

                    message = string.Format("本次从单位账户扣款{0}元，账户余额{1}元。", payment.Total, accountInfo.AccountBalance);

                    return true;
                }
                catch(Exception e)
                {
                    message = e.Message;
                    context.Rollback();
                    return false;
                }
            }
        }

        public bool CancelPayment(int paymentId, out string message)
        {
            Payment payment = LoadPayment(paymentId);
            payment.Status = 0;

            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    CompanyInfo companyInfo = context.Selete<CompanyInfo>("hr.company.findCompanyById", payment.CompanyId);



                    CompanyAccountRecordInfo accountRecordInfo = new CompanyAccountRecordInfo();
                    accountRecordInfo.CompanyId = payment.CompanyId;
                    accountRecordInfo.AccountId = companyInfo == null ? 0 : companyInfo.AccountId;
                    accountRecordInfo.AccountBalance = companyInfo == null ? 0 : companyInfo.AccountBalance;
                    accountRecordInfo.Money = payment.Total*-1;
                    accountRecordInfo.CreateTime = DateTime.Now;
                    accountRecordInfo.PayDate = DateTime.Parse(payment.PayMonth);
                    accountRecordInfo.ItemName = "撤销总额";
                    accountRecordInfo.EntryDate = DateTime.Now;
                    accountRecordInfo.OptDate = DateTime.Now;
                    accountRecordInfo.BillDate = DateTime.Now;
                    accountRecordInfo.PersonName = BlueFramework.User.UserContext.Current.UserName;
                    context.BeginTransaction();
                    CompanyAccountInfo accountInfo = new CompanyAccountInfo()
                    {
                        CompanyId = accountRecordInfo.CompanyId,
                        AccountId = accountRecordInfo.AccountId,
                        AccountBalance = accountRecordInfo.AccountBalance - accountRecordInfo.Money
                    };
                    context.Save<CompanyAccountInfo>("hr.company.updateCompanyAccount", accountInfo);
                    context.Save<CompanyAccountRecordInfo>("hr.company.insertCompanyAccountDetail", accountRecordInfo);
                    context.Save<Payment>("hr.payment.submitPayment", payment);
                    context.Commit();

                    message = string.Format("本次从单位账户返还{0}元，账户余额{1}元。", payment.Total, accountInfo.AccountBalance);

                    return true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    context.Rollback();
                    return false;
                }
            }
        }

        public void CreatePayment(Payment payment)
        {
            EntityContext context = Session.CreateContext();
            TemplateInfo template = context.Selete<TemplateInfo>("hr.payment.findDefaultTemplate", 0);
            List<PayItemDO> items = context.SelectList<PayItemDO>("hr.payment.findTemplateItems", template.TemplateId);
            //List<PayObjectDO> objects = context.SelectList<PayObjectDO>("hr.payment.findCompanyPersons", payment.CompanyId);
            CommandParameter[] parameters =
            {
                new CommandParameter("CompanyId",payment.CompanyId),
                new CommandParameter("PayMonth",DateTime.Parse(payment.PayMonth).ToString("yyyyMM") )
            };
            List<PayObjectDO> objects = context.SelectList<PayObjectDO>("hr.payment.findCompanyPersons", parameters);
            List<PayValueInfo> payValues = context.SelectList<PayValueInfo>("hr.payment.findCompanyPersonsValue", parameters);
            #region 派遣服务费
            foreach (PayObjectDO o in objects)
            {
                PayValueInfo payValue = new PayValueInfo()
                {
                    ItemId = 6,
                    PersonId = o.ObjectId,
                    PayValue = o.ServiceFee
                };
                payValues.Add(payValue);
            }
            #endregion
            using (context)
            {
                try
                {
                    context.BeginTransaction();
                    payment.TemplateId = template.TemplateId;
                    context.Save<Payment>("hr.payment.insertPayment", payment);
                    foreach(PayItemDO item in items)
                    {
                        item.PaymentId = payment.PayId;
                        context.Save<PayItemDO>("hr.payment.insertPayment.items", item);
                    }
                    foreach (PayObjectDO payObject in objects)
                    {
                        payObject.PaymentId = payment.PayId;
                        context.Save<PayObjectDO>("hr.payment.insertPayment.object", payObject);
                    }
                    foreach (PayValueInfo payValue in payValues)
                    {
                        payValue.PayId = payment.PayId;
                        context.Save<PayValueInfo>("hr.payment.insertPayment.detail", payValue);
                    }

                    context.Commit();
                }
                catch (Exception ex)
                {
                    BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Info(ex.Message);
                    context.Rollback();
                }
            }

        }

        public Payment LoadPayment(int paymentId)
        {
            EntityContext context = Session.CreateContext();
            Payment payment = context.Selete<Payment>("hr.payment.findPayment", paymentId) ;
            //payment.PayMonth = DateTime.Parse(payment.PayMonth).ToString("yyyy-M-d");
            List<PayItemDO> items = context.SelectList<PayItemDO>("hr.payment.findPaymentItems", paymentId);
            List<PayObjectDO> objects = context.SelectList<PayObjectDO>("hr.payment.findPaymentPersons", paymentId);
            List<PayValueInfo> payValues = context.SelectList<PayValueInfo>("hr.payment.findPaymentValue", paymentId);
            payment.Items = items;
            payment.Objects = objects;
            DataTable dt = new DataTable();
            dt.Columns.Add("PersonId");
            dt.Columns.Add("PersonName");
            dt.Columns.Add("PersonCode");
            dt.Columns.Add("CardID");
            dt.Columns.Add("Position");
            dt.Columns.Add("Memo");
            dt.Columns.Add("Bank");
            int fixColumn = 7; //固定列，如果增加列，手动增加fixColumn
            foreach (PayItemDO item in items)
            {
                if (item.IsLeaf == false) continue;
                item.ItemName = "f_" + item.ItemId;
                DataColumn column = new DataColumn(item.ItemName, Type.GetType("System.Decimal"));
                column.DefaultValue = 0;
                dt.Columns.Add(column);
                
            }

            Dictionary<int, DataRow> rows = new Dictionary<int, DataRow>();
            foreach(PayObjectDO o in objects)
            {
                DataRow dr = dt.NewRow();
                dr["PersonId"] = o.ObjectId;
                dr["PersonName"] = o.ObjectName;
                dr["PersonCode"] = o.ObjectCode;
                dr["CardID"] = o.CardID;
                dr["Position"] = o.Position;
                dr["Memo"] = o.Memo;
                dr["Bank"] = o.Bank;
                dt.Rows.Add(dr);
                rows.Add(o.ObjectId, dr);
            }
            // fill values by dictionary
            foreach(PayValueInfo pv in payValues)
            {
                DataRow dr = rows[pv.PersonId];
                string columnName = "f_" + pv.ItemId;
                if (dt.Columns.Contains(columnName))
                {
                    dr[columnName] = pv.PayValue;
                }
            }
 
            // TODO:calculate by formula but this is fix code now
            #region calculate columns
            foreach (DataRow dr in dt.Rows)
            {
                decimal yfgz = 0;
                foreach(var i in items)
                {
                    if(i.ParentId == 1 && i.ItemId != 199)
                    {
                        yfgz += decimal.Parse(dr[i.ItemName].ToString());
                    }
                }
                decimal gryk = 0;
                foreach (var i in items)
                {
                    if (i.ParentId == 2 && i.ItemId != 299)
                    {
                        gryk += decimal.Parse(dr[i.ItemName].ToString());
                    }
                }
                dr["f_199"] = yfgz;//应发工资
                dr["f_299"] = gryk;//个人应扣
                decimal realPayValue = yfgz - gryk;
                dr["f_3"] = realPayValue<0 ? 0: realPayValue;
                if (dt.Columns.Contains("f_7"))
                {
                    dr["f_7"] = decimal.Parse(dr["f_199"].ToString()) + decimal.Parse(dr["f_4"].ToString()) + decimal.Parse(dr["f_5"].ToString()) + decimal.Parse(dr["f_6"].ToString());
                    decimal realPaytotal = decimal.Parse(dr["f_199"].ToString()) + decimal.Parse(dr["f_4"].ToString()) + decimal.Parse(dr["f_5"].ToString());
                    payment.Total += realPaytotal;
                }
                else
                {
                    payment.Total += realPayValue;
                }

            }
            #endregion

            payment.DataSource = dt;

            payment.Sheet = new PayDetailVO();
            payment.Sheet.rows = dt;
            payment.Sheet.total = dt.Rows.Count;
            payment.Sheet.footer = new List<JObject>();
            JObject joSum = new JObject();
            joSum.Add("PersonCode", "合计");
            payment.Sheet.footer.Add(joSum);
            for(int i = fixColumn; i < dt.Columns.Count; i++)
            {
                if ("System.String".Equals(dt.Columns[i].DataType.ToString())) continue;
                string columnName = dt.Columns[i].ColumnName;
                string exp = string.Format("sum({0})", columnName);
                object sum = dt.Compute(exp, "");
                joSum.Add(columnName, sum.ToString());

            }

            return payment;
        }

        public bool ImportPayment(int paymentId,DataTable dt,out string message)
        {
            message = string.Empty;
            EntityContext context = Session.CreateContext();
            List<PayItemDO> items = context.SelectList<PayItemDO>("hr.payment.findPaymentItems", paymentId);
            List<PayObjectDO> objects = context.SelectList<PayObjectDO>("hr.payment.findPaymentPersons", paymentId);
            List<PayValueInfo> payValues = new List<PayValueInfo>();
            Dictionary<string, PayItemDO> dicItems = new Dictionary<string, PayItemDO>();
            foreach(PayItemDO pi in items)
            {
                if (pi.Editable==1)
                    dicItems.Add(pi.ItemCaption, pi);
            }
            int rownum = 1;
            foreach(DataRow dr in dt.Rows)
            {
                int objectId = int.Parse(dr["ID"].ToString());
                foreach(DataColumn column in dt.Columns)
                {
                    bool exist = dicItems.ContainsKey(column.ColumnName);
                    if (!exist) continue;
                    string cellValue = dr[column.ColumnName].ToString();
                    if (string.IsNullOrEmpty(cellValue)) continue;
                    PayItemDO item = dicItems[column.ColumnName];
                    PayValueInfo pv = new PayValueInfo();
                    pv.PayId = paymentId;
                    pv.PersonId = objectId;
                    pv.ItemId = item.ItemId;
                    decimal payValue = 0;
                    bool success = decimal.TryParse(cellValue, out payValue);
                    if (success)
                        pv.PayValue = payValue;
                    else
                        message = message + string.Format("第{0}行{1}数据异常.",rownum, column.ColumnName);
                    payValues.Add(pv);
                    rownum++;
                }
            }

            using (context)
            {
                try
                {
                    context.BeginTransaction();
                    // 删除可编辑的列数据
                    foreach(var item in dicItems.Values)
                    {
                        PayValueInfo payValue = new PayValueInfo()
                        {
                            PayId = paymentId,
                            ItemId = item.ItemId
                        };
                        context.Delete<PayValueInfo>("hr.payment.insertPayment.delete", payValue);
                    }
                    // 写入所有数据
                    foreach (PayValueInfo payValue in payValues)
                    {
                        context.Save<PayValueInfo>("hr.payment.insertPayment.detail", payValue);
                    }

                    context.Commit();
                }
                catch (Exception ex)
                {
                    BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Info(ex.Message);
                    message += ex.Message;
                    context.Rollback();
                }
            }
            if (string.IsNullOrEmpty(message))
                return true;
            else
                return false;
        }

        public DataSet ExportBankPayment(string payMonth)
        {
            payMonth = DateTime.Parse(payMonth).ToString("yyyy-MM-dd");
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.payBank");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            db.AddInParameter(command, "payMonth", DbType.String, payMonth);
            //DataTable dt = db.ExecuteDataSet(command).Tables[0];
            DataSet ds = db.ExecuteDataSet(command);
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = i + 1;
            }
            return ds;
        }

        public DataSet ExportDetail(string payMonth)
        {
            payMonth = DateTime.Parse(payMonth).ToString("yyyy-MM-dd");
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Setting/sql/hr.sql.xml";
            string sql = BlueFramework.Common.XmlUtils.GetInnerText(filePath, "hr.paydetail");
            BlueFramework.Data.DatabaseProviderFactory factory = new BlueFramework.Data.DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DbCommand command = db.GetSqlStringCommand(sql);
            db.AddInParameter(command, "payMonth", DbType.String, payMonth);
            DataSet ds = db.ExecuteDataSet(command);
            return ds;
        }

    }
}