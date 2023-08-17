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
using BlueFramework.Blood.DataAccess;
using System.Globalization;

namespace HrServiceCenterWeb.Manager
{
    public class PayManager
    {

        public List<Models.TemplateInfo> GetTemplateList()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<Models.TemplateInfo> list = context.SelectList<Models.TemplateInfo>("hr.template.findTempList", null);
            return list;
        }

        public DataTable GetImportorDetail(int importId)
        {
            EntityConfig config = ConfigManagent.Configs["hr.payment.queryImportorDetail"];
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            Database database = factory.CreateDefault();
            string sql = config.Sql.Replace("#{value}", importId.ToString());
            DataSet dataSet = database.ExecuteDataSet(CommandType.Text, sql);
            return dataSet.Tables[0];

        }

        public Models.TemplateInfo GetTemplate(int id)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            Models.TemplateInfo temp = context.Selete<Models.TemplateInfo>("hr.template.findTemplateById", id);
            return temp;
        }

        /// <summary>
        /// 根据公司ID找发放模版
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns></returns>
        public Models.TemplateInfo GetTemplateByCompanyId(int id)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            Models.TemplateInfo temp = context.Selete<Models.TemplateInfo>("hr.template.findTemplateByCompanyId", id);
            return temp;
        }

        /// <summary>
        /// 获取模版树结构
        /// </summary>
        /// <returns></returns>
        public List<VEasyUiTree> GetTree()
        {
            List<VEasyUiTree> list = new List<VEasyUiTree>();
            DataTable dt = toDataTable<SalaryItemInfo>(GetTemplateTree());
            DataRow[] drs = dt.Select("ParentId = '0'");
            if (drs.Length > 0)
            {
                foreach (DataRow dr in drs)
                {
                    list.Add(GetTree(dr, dt));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取树形菜单的子菜单
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public VEasyUiTree GetTree(DataRow dr, DataTable dt)
        {
            VEasyUiTree tree = new VEasyUiTree();
            tree.text = dr["Name"].ToString();
            tree.id = dr["ItemId"].ToString();
            DataRow[] drs = dt.Select("ParentId = '" + dr["ItemId"].ToString() + "'");
            if (drs.Length > 0)
            {
                tree.children = new List<VEasyUiTree>();
                foreach (DataRow mdr in drs)
                {
                    //递归子节点
                    tree.children.Add(GetTree(mdr, dt));
                }
            }
            return tree;
        }

        public List<SalaryItemInfo> GetTemplateTree()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<SalaryItemInfo> list = context.SelectList<SalaryItemInfo>("hr.template.findSalaryItem", null);
            return list;
        }

        /// <summary>
        /// 转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        private DataTable toDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        public int[] GetTemplateByTable(int tempId)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<TemplateDetailInfo> detail = context.SelectList<TemplateDetailInfo>("hr.template.getTemplateByTable", tempId);
            if (detail != null && detail.Count > 0)
            {
                int[] str = new int[detail.Count];
                for (int i = 0; i < detail.Count; i++)
                {
                    str[i] = detail[i].ItemId;
                }
                return str;
            }
            else
                return null;
        }

        public bool SaveTemplateForTable(int tempId, int[] tempItems)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    context.Delete("hr.template.deleteTempDetial", tempId);

                    if (tempItems != null)
                    {
                        int[] temp = tempItems.Select(i => i).ToArray();
                        for (int i = 0; i < temp.Length; i++)
                        {
                            TemplateDetailInfo tdi = new TemplateDetailInfo();
                            tdi.TemplateId = tempId;
                            tdi.ItemId = temp[i];
                            context.Save<TemplateDetailInfo>("hr.template.insertTemplateDetail", tdi);
                        }
                    }
                    TemplateInfo ti = new TemplateInfo();
                    ti.UpdateTime = DateTime.Now.ToShortDateString();
                    ti.TemplateId = tempId;
                    context.Save<TemplateInfo>("hr.template.updateTemplate", ti);

                    context.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    LogHelper.Warn("payment.SaveTemplateForTable", ex);
                    return false;
                }
            }
        }

        public bool SaveTemplateMsg(int cmpId, int[] tempItems)
        {
            EmployeeManager em = new EmployeeManager();
            CompanyInfo cmp = em.GetCompany(cmpId);
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    TemplateInfo tp = new TemplateInfo(cmp.CompanyId, cmp.Name + "-薪酬模版", UserContext.CurrentUser.UserId, DateTime.Now.ToShortDateString());
                    context.Save<TemplateInfo>("hr.template.insertTemplate", tp);

                    if (tempItems != null)
                    {
                        int[] temp = tempItems.Select(i => i).ToArray();
                        for (int i = 0; i < temp.Length; i++)
                        {
                            TemplateDetailInfo tdi = new TemplateDetailInfo();
                            tdi.TemplateId = tp.TemplateId;
                            tdi.ItemId = temp[i];
                            context.Save<TemplateDetailInfo>("hr.template.insertTemplateDetail", tdi);
                        }
                    }
                    context.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    LogHelper.Warn("payment.SaveTemplateMsg", ex);
                    return false;
                }
            }
        }

        public bool DeleteTemplate(int tempId)
        {
            try
            {
                EntityContext context = BlueFramework.Blood.Session.CreateContext();
                context.Delete("hr.template.deleteTemp", tempId);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Warn("payment.DeleteTemplate", ex);
                return false;
            }
        }

        /// <summary>
        /// 获取保险导入信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InsuranceInfo QueryImportorInsuranceInfo(int id)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                var data = context.Selete<InsuranceInfo>("hr.insurance.findInsuranceById", id);
                return data;
            }
        }

        public List<InsuranceInfo> QueryImportorInsuranceList(string query)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                CommandParameter[] dbParms = new CommandParameter[2];
                dbParms[0] = new CommandParameter("value", query);
                if (UserContext.CurrentUser.IsCompanyUser)
                {
                    dbParms[1] = new CommandParameter("where", $" and t.CREATOR=" + UserContext.CurrentUser.UserId);
                }
                else
                {
                    dbParms[1] = new CommandParameter("where", "");
                }
                List<InsuranceInfo> list = context.SelectList<InsuranceInfo>("hr.insurance.findInsurance", dbParms);
                return list;
            }

        }

        public List<InsuranceInfo> QueryImportorPaymentList(string query)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                CommandParameter[] dbParms = new CommandParameter[2];
                dbParms[0] = new CommandParameter("value", query);
                if (UserContext.CurrentUser.IsCompanyUser)
                {
                    dbParms[1] = new CommandParameter("where", $" and t.CREATOR=" + UserContext.CurrentUser.UserId);
                }
                else
                {
                    dbParms[1] = new CommandParameter("where", "");
                }
                List<InsuranceInfo> list = context.SelectList<InsuranceInfo>("hr.insurance.findPayments", dbParms);
                return list;
            }
        }

        public bool DeleteInsurance(int id,out string message)
        {
            message = string.Empty;
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                // 判断是否已经被引用
                var dateList = context.SelectList<GenericBO>("hr.insurance.getDateOfImport", id);
                if(dateList.Count > 0)
                {
                    var date = DateTime.MinValue;
                    var dateString = dateList[0].S1;
                    bool datePass = false;

                    if (DateTime.TryParseExact(dateString, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        datePass = true;
                    }
                    else if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        datePass = true;
                    }
                    else if (DateTime.TryParse(dateString,out date))
                    {
                        datePass = true;
                    }
                    if(!datePass)
                    {
                        Console.WriteLine("无法解析日期字符串");
                        LogHelper.Warn("DeleteInsurance", new Exception("无法解析日期字符串"));
                    }

                    var cmds = new CommandParameter[2];
                    cmds[0] = new CommandParameter("value", id);
                    cmds[1] = new CommandParameter("paymonth", date.ToString("yyyy-MM-01"));
                    var lst = context.SelectList<GenericBO>("hr.insurance.isRefrecedByPayment",cmds);
                    var refCount = lst.Count;

                    //var refCount = context.Selete<int>("hr.insurance.isRefrecedByPayment", id);
                    if (refCount > 0)
                    {
                        message = $"当前数据已经被引用：";
                        foreach (var x in lst)
                        {
                            message += $"{x.S1}({x.S2}) ";
                        }
                        message += "，如果要删除请联系管理员删除工资发放表！";
                        return false;
                    }
                }



                // 删除缴存表和明细表
                try
                {
                    context.BeginTransaction();
                    context.Delete("hr.insurance.deleteInsurance", id);

                    context.Delete("hr.insurance.deleteInsuranceDetail", id);
                    context.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    LogHelper.Warn("payment.DeleteInsurance", ex);
                    context.Rollback();
                    message = "系统错误";
                    return false;
                }
            }
        }

        /// <summary>
        /// 保险导入
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        /// <param name="outmsg"></param>
        /// <returns></returns>
        public bool ImportInsurance(DataTable dt, string fileName, ref string outmsg)
        {
            Dictionary<string, int> cardIds = getItemCardId();
            Dictionary<string, int> titles = getItemTitle();

            string colPayMonth = "计划月度";
            string colAccountIndex = "做账期号";
            string colPayIndex = "费款所属期";
            string colPersonName = dt.Columns.Contains("个人姓名")?"个人姓名": "姓名";
            string colIdCard = dt.Columns.Contains("身份证号码") ? "身份证号码" : "证件号";
            string colPayType = dt.Columns.Contains("险种") ? "险种" : "应缴类型";
            string colBase = "缴费基数";
            string colScalCmp = "单位缴费比例";
            string colScalPerson = "个人缴费比例";
            string colPayCmp = dt.Columns.Contains("单位缴费金额") ? "单位缴费金额" : "单位缴费";
            string colPayPerson = dt.Columns.Contains("个人缴费金额") ? "个人缴费金额" : "个人缴费";
            string colMemo = "备注";

            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    //判断是否已经入库
                    List<InsuranceInfo> list = context.SelectList<InsuranceInfo>("hr.insurance.findInsuranceByTitle", fileName);
                    if (list.Count > 0)
                    {
                        outmsg += "文件：" + fileName + "已上传！";
                        return false;
                    }
                    //入库导入主表
                    InsuranceInfo ii = new InsuranceInfo();
                    ii.Title = fileName;
                    ii.CreatorId = UserContext.CurrentUser.UserId;
                    ii.CreateTime = DateTime.Now.ToShortDateString();
                    ii.ImportType = 1;
                    context.Save<InsuranceInfo>("hr.insurance.insertInsurance", ii);
                    int rowIndex = 0;
                    int passRows = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        rowIndex++;
                        //入库导入详细表
                        InsuranceDetailInfo idi = new InsuranceDetailInfo();
                        idi.ImportId = ii.ImportId;
                        idi.PayMonth = row[colPayMonth].ToString();
                        idi.PayIndex = row[colPayIndex].ToString();
                        idi.AccountIndex = row[colAccountIndex].ToString();
                        idi.PersonName = row[colPersonName].ToString().Trim();
                        idi.BaseValue = decimal.Parse(row[colBase].ToString());
                        idi.ScaleCompany = decimal.Parse(row[colScalCmp].ToString());
                        idi.ScalePerson = decimal.Parse(row[colScalPerson].ToString());
                        idi.PersonPayValue = decimal.Parse(row[colPayPerson].ToString());
                        idi.CompanyPayValue= decimal.Parse(row[colPayCmp].ToString());
                        idi.ImportColumnName = row[colPayType].ToString();
                        idi.Memo = row[colMemo].ToString();
                        string cardId = row[colIdCard].ToString().ToLower()+"."+idi.PersonName;
                        if (cardIds.ContainsKey(cardId))
                        {
                            idi.PersonId = cardIds[cardId];
                            passRows++;
                        }
                        else
                        {
                            //未找到人员，忽略该行
                            outmsg += string.Format("第{0}行 {1} 人员的身份证号码和姓名不匹配， ",rowIndex, idi.PersonName ); 
                            continue;
                        }
                        if (titles.ContainsKey(row[colPayType].ToString()))
                        {
                            idi.ItemId = titles[row[colPayType].ToString()];
                        }
                        else
                        {
                            //值错误，不入库
                            outmsg += "第" + dt.Rows.IndexOf(row) + "行险种不匹配！";
                            context.Rollback();
                            return false;
                        }
                        context.Save<InsuranceDetailInfo>("hr.insurance.insertInsuranceDetail", idi);
                    }
                    context.Commit();
                    outmsg = string.Format("Excel中一共{0}行，导入成功{1}行。{2}", dt.Rows.Count,passRows, outmsg);
                    return true;
                }
                catch (Exception ex)
                {
                    outmsg += "服务器内部错误，请联系管理员："+ex.Message;
                    context.Rollback();
                    return false;
                }
            }
        }


        public bool ImportPaymentData(DataTable dt, string fileName, ref string outmsg)
        {
            Dictionary<string, int> cardIds = getItemCardId();
            Dictionary<string, int> titles = getItemTitle();
            Dictionary<int, int> keyValues = new Dictionary<int, int>();
            #region 项目匹配
            for(int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn column = dt.Columns[i];
                if (titles.ContainsKey(column.ColumnName))
                    keyValues.Add(i, titles[column.ColumnName]);
            }
            #endregion

            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    //判断是否已经入库
                    List<InsuranceInfo> list = context.SelectList<InsuranceInfo>("hr.insurance.findInsuranceByTitle", fileName);
                    if (list.Count > 0)
                    {
                        outmsg += "文件：" + fileName + "已上传！";
                        return false;
                    }
                    //入库导入主表
                    InsuranceInfo ii = new InsuranceInfo();
                    ii.Title = fileName;
                    ii.CreatorId = UserContext.CurrentUser.UserId;
                    ii.CreateTime = DateTime.Now.ToShortDateString();
                    ii.ImportType = 2;
                    context.Save<InsuranceInfo>("hr.insurance.insertInsurance", ii);
                    int rowIndex = 0;
                    int passRows = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        rowIndex++;
                        //入库导入详细表
                        InsuranceDetailInfo idi = new InsuranceDetailInfo();
                        idi.ImportId = ii.ImportId;
                        idi.PayMonth = row["发放年月"].ToString();
                        idi.PersonName = row["姓名"].ToString().Trim();
                        idi.ImportType = 2;
                        string cardId = row["身份证"].ToString().ToLower()+"."+idi.PersonName;
                        #region 身份证判断
                        if (cardIds.ContainsKey(cardId))
                        {
                            idi.PersonId = cardIds[cardId];
                            passRows++;
                        }
                        else
                        {
                            //未找到人员，忽略该行
                            outmsg += string.Format("第{0}行{1}人员的身份证号码和姓名不匹配， ", rowIndex, idi.PersonName);
                            continue;
                        }
                        #endregion
                        foreach(var columnindex in keyValues.Keys)
                        {
                            if (row[columnindex] == null || string.IsNullOrEmpty(row[columnindex].ToString()))
                                continue;
                            InsuranceDetailInfo item = idi.Clone();
                            item.ItemId = keyValues[columnindex];
                            item.PersonPayValue = decimal.Parse(row[columnindex].ToString());
                            item.ImportColumnName = dt.Columns[columnindex].Caption;
                            context.Save<InsuranceDetailInfo>("hr.insurance.insertInsuranceDetail", item);
                        }
                    }
                    context.Commit();
                    outmsg = string.Format("Excel一共{0}行，导入成功{1}行。{2}", dt.Rows.Count,passRows, outmsg);
                    return true;
                }
                catch (Exception ex)
                {
                    outmsg += "保存失败，"+ex.Message;
                    context.Rollback();
                    return false;
                }
            }
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

        private Dictionary<string, int> getItemTitle()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<SalaryItemInfo> list = context.SelectList<SalaryItemInfo>("hr.template.findSalaryItemTitle", null);
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add("工伤保险", 204);
            dic.Add("生育保险", 205);
            dic.Add("失业保险", 202);
            dic.Add("基本养老", 201);

            foreach (SalaryItemInfo si in list)
            {
                if (!dic.ContainsKey(si.Name))
                    dic.Add(si.Name, si.ItemId);
            }
            return dic;
        }

        /// <summary>
        /// 匹配非数据库保险列表
        /// 用XML配置
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> getThirdItem()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            XmlDocument doc = new XmlDocument();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "Setting/Insurance/InsuranceConfig.xml";
            doc.Load(path);
            XmlNode menus = doc.SelectSingleNode("Root");
            XmlNodeList xn = menus.ChildNodes;
            foreach (XmlNode xmlNode in xn)
            {
                XmlElement xe = (XmlElement)xmlNode;
                XmlNodeList nodes = xmlNode.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;
                    XmlElement nd = (XmlElement)node;
                    dic.Add(nd.GetAttribute("name"), int.Parse(xe.GetAttribute("code")));
                }
            }
            return dic;
        }
        /// <summary>
        /// 查询指定id的缴存明细
        /// </summary>
        /// <param name="importId"></param>
        /// <returns></returns>
        public List<InsuranceDetailInfo> QueryInsuranceDetail(int importId)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<InsuranceDetailInfo> list = context.SelectList<InsuranceDetailInfo>("hr.insurance.findInsuranceDetailById", importId);
            return list;
        }

        /// <summary>
        /// 导出指定id的缴存明细
        /// </summary>
        /// <param name="importId"></param>
        /// <returns></returns>
        public DataSet ExportInsuranceDetail(int importId)
        {
            EntityConfig config = ConfigManagent.Configs["hr.insurance.exportInsuranceDetailById"];
            BlueFramework.Data.Database database = new BlueFramework.Data.DatabaseProviderFactory().CreateDefault();
            string sql = config.Sql.Replace("#{value}", importId.ToString());
            DataSet ds = database.ExecuteDataSet(CommandType.Text, sql);
            return ds;
        }

        public List<Payment> QueryPayList(string query)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            CommandParameter[] dbParms = new CommandParameter[2];
            dbParms[0] = new CommandParameter("value", query);
            if (UserContext.CurrentUser.IsCompanyUser)
            {
                dbParms[1] = new CommandParameter("where", $" and t.CREATOR=" + UserContext.CurrentUser.UserId);
            }
            else
            {
                dbParms[1] = new CommandParameter("where", "");
            }
            List<Payment> list = context.SelectList<Payment>("hr.pay.findPayList", dbParms);
            return list;
        }

        /// <summary>
        /// 根据发放表ID查询发放表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Payment GetPayDetailByPayId(int id)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            Payment list = context.Selete<Payment>("hr.pay.findPayByPayId", id);
            return list;
        }

        public HashSet<int> QueryTemplateDetail(int id)
        {
            HashSet<int> result = new HashSet<int>();
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<TemplateDetailInfo> list = context.SelectList<TemplateDetailInfo>("hr.template.findTemplateDetailByCompanyId", id);
            foreach (TemplateDetailInfo temp in list)
            {
                result.Add(temp.ItemId);
            }
            return result;
        }

        public List<PayDetailInfo> GetPayDetail(int id, int payid)
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<PayDetailInfo> list = context.SelectList<PayDetailInfo>("hr.pay.findPayDefaultDetail", id);
            Dictionary<int, Dictionary<string, decimal>> dic = GetPersionInsuranceDic();
            Dictionary<int, Dictionary<string, decimal>> payDetailDic = getPayDetailDic(payid);
            foreach (PayDetailInfo info in list)
            {
                //填充保险
                if (dic.ContainsKey(info.PersonId))
                {
                    foreach (KeyValuePair<string, decimal> kv in dic[info.PersonId])
                    {
                        Type type = info.GetType();
                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(kv.Key);
                        propertyInfo.SetValue(info, kv.Value);
                    }
                }
                //填充发放表金额
                if (payDetailDic.ContainsKey(info.PersonId))
                {
                    foreach (KeyValuePair<string, decimal> kv in payDetailDic[info.PersonId])
                    {
                        Type type = info.GetType();
                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(kv.Key);
                        propertyInfo.SetValue(info, kv.Value);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 键：人员ID
        /// 值：（键：项目名，值：金额）
        /// </summary>
        /// <param name="payId">发放ID</param>
        /// <returns></returns>
        private Dictionary<int, Dictionary<string, decimal>> getPayDetailDic(int payId)
        {
            Dictionary<int, Dictionary<string, decimal>> dic = new Dictionary<int, Dictionary<string, decimal>>();
            if (payId == 0)
                return dic;
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<PayValueInfo> list = context.SelectList<PayValueInfo>("hr.pay.findPayDetailByPayId", payId);
            Dictionary<int, string> cfgdic = GetItemConfigDic();
            foreach (PayValueInfo info in list)
            {
                if (!dic.ContainsKey(info.PersonId))
                {
                    Dictionary<string, decimal> infodic = new Dictionary<string, decimal>();
                    infodic.Add(cfgdic[info.ItemId], info.PayValue);
                    dic.Add(info.PersonId, infodic);
                }
                else
                {
                    dic[info.PersonId].Add(cfgdic[info.ItemId], info.PayValue);
                }
            }
            return dic;
        }

        public Dictionary<int, Dictionary<string, decimal>> GetPersionInsuranceDic()
        {
            EntityContext context = BlueFramework.Blood.Session.CreateContext();
            List<InsuranceDetailInfo> list = context.SelectList<InsuranceDetailInfo>("hr.insurance.findInsuranceDetailByNewMonth", null);
            Dictionary<int, string> ItemConfigDic = GetItemConfigDic();
            Dictionary<int, Dictionary<string, decimal>> dic = new Dictionary<int, Dictionary<string, decimal>>();
            foreach (InsuranceDetailInfo info in list)
            {
                if (!dic.ContainsKey(info.PersonId))
                {
                    Dictionary<string, decimal> sdic = new Dictionary<string, decimal>();
                    if (ItemConfigDic.ContainsKey(info.ItemId))
                    {
                        sdic.Add(ItemConfigDic[info.ItemId], info.PersonPayValue);
                        dic.Add(info.PersonId, sdic);
                    }
                }
                else
                {
                    if (ItemConfigDic.ContainsKey(info.ItemId))
                    {
                        dic[info.PersonId].Add(ItemConfigDic[info.ItemId], info.PersonPayValue);
                    }
                }
            }
            return dic;
        }

        public Dictionary<int, string> GetItemConfigDic()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            XmlDocument doc = new XmlDocument();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "Setting/Pay/PayTableConfig.xml";
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("Root");
            XmlNodeList xn = root.ChildNodes;
            foreach (XmlNode xmlnode in xn)
            {
                XmlElement xe = (XmlElement)xmlnode;
                if (xe.GetAttribute("isDynamic") == "true")
                {
                    dic.Add(int.Parse(xe.GetAttribute("code")), xe.GetAttribute("fieldName"));
                }
                if (xe.GetAttribute("isLastStage") == "false")
                {
                    XmlNodeList child = xmlnode.ChildNodes;
                    foreach (XmlNode node in child)
                    {
                        XmlElement nd = (XmlElement)node;
                        dic.Add(int.Parse(nd.GetAttribute("code")), nd.GetAttribute("fieldName"));
                    }
                }
            }
            return dic;
        }

        public bool SavePayDetail(List<PayDetailInfo> list, int cmpid, string tname, string time, string count, int status, ref string msg)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();
                    Models.TemplateInfo temp = context.Selete<Models.TemplateInfo>("hr.template.findTemplateIdByCompanyId", cmpid);
                    Payment pl = new Payment();
                    pl.CompanyId = cmpid;
                    pl.TemplateId = temp.TemplateId;
                    pl.PayTitle = tname;
                    pl.PayMonth = time;
                    pl.CreatorId = UserContext.CurrentUser.UserId;
                    pl.CreateTime = DateTime.Now;
                    pl.Status = status;
                    if (status == 0)//新建发放表重复校验
                    {
                        Payment payinfo = context.SelectListByTemplate<Payment>("hr.pay.findPay", pl)[0];
                        if (payinfo.PayId != 0)
                        {
                            msg += "该公司该月份已创建发放表！<br />";
                            context.Rollback();
                            return false;
                        }
                        context.Save<Payment>("hr.pay.insertPayTable", pl);
                    }
                    Dictionary<string, int> dic = GetIdConfigDic();
                    foreach (PayDetailInfo info in list)
                    {
                        foreach (System.Reflection.PropertyInfo p in info.GetType().GetProperties())
                        {
                            if (dic.ContainsKey(p.Name))
                            {
                                PayValueInfo pvi = new PayValueInfo();
                                pvi.PayId = pl.PayId;
                                pvi.ItemId = dic[p.Name];
                                pvi.PersonId = info.PersonId;
                                pvi.PayValue = decimal.Parse(p.GetValue(info).ToString());
                                context.Save<PayValueInfo>("hr.pay.insertPayTableDetail", pvi);
                            }
                        }
                    }
                    if (status == 2)//归档
                    {
                        CompanyAccountInfo cai = new CompanyAccountInfo();
                        cai.AccountBalance = decimal.Parse(count);
                        cai.CompanyId = cmpid;
                        context.Save<CompanyAccountInfo>("hr.company.updateCompanyBalance", cai);
                    }
                    context.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    msg += "服务器内部错误，请联系管理员！<br />";
                    context.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新发放表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cmpid"></param>
        /// <param name="tname"></param>
        /// <param name="time"></param>
        /// <param name="count"></param>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdatePayDetail(List<PayDetailInfo> list, int payid)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();

                    context.Delete("hr.pay.deletePayDetail", payid);

                    Dictionary<string, int> dic = GetIdConfigDic();
                    foreach (PayDetailInfo info in list)
                    {
                        foreach (System.Reflection.PropertyInfo p in info.GetType().GetProperties())
                        {
                            if (dic.ContainsKey(p.Name))
                            {
                                PayValueInfo pvi = new PayValueInfo();
                                pvi.PayId = payid;
                                pvi.ItemId = dic[p.Name];
                                pvi.PersonId = info.PersonId;
                                pvi.PayValue = decimal.Parse(p.GetValue(info).ToString());
                                context.Save<PayValueInfo>("hr.pay.insertPayTableDetail", pvi);
                            }
                        }
                    }
                    context.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    return false;
                }
            }
        }

        public Dictionary<string, int> GetIdConfigDic()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            XmlDocument doc = new XmlDocument();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "Setting/Pay/PayTableConfig.xml";
            doc.Load(path);
            XmlNode root = doc.SelectSingleNode("Root");
            XmlNodeList xn = root.ChildNodes;
            foreach (XmlNode xmlnode in xn)
            {
                XmlElement xe = (XmlElement)xmlnode;
                if (xe.GetAttribute("isDynamic") == "true")
                {
                    dic.Add(xe.GetAttribute("fieldName"), int.Parse(xe.GetAttribute("code")));
                }
                if (xe.GetAttribute("isLastStage") == "false")
                {
                    XmlNodeList child = xmlnode.ChildNodes;
                    foreach (XmlNode node in child)
                    {
                        XmlElement nd = (XmlElement)node;
                        dic.Add(nd.GetAttribute("fieldName"), int.Parse(nd.GetAttribute("code")));
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 删除发放表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePay(int id, ref string msg)
        {
            using (EntityContext context = BlueFramework.Blood.Session.CreateContext())
            {
                try
                {
                    context.BeginTransaction();

                    Payment payinfo = context.Selete<Payment>("hr.pay.findPayByPayId", id);
                    if (payinfo.Status == 2)
                    {
                        msg += "该发放表已归档，不能删除！";
                        return false;
                    }
                    context.Delete("hr.payment.deletePay", id);
                    context.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    context.Rollback();
                    return false;
                }
            }
        }

        public DataSet GetJiaoCun(string month)
        {
            EntityConfig config = ConfigManagent.Configs["hr.pay.exportjiaocun"];
            BlueFramework.Data.Database database = new BlueFramework.Data.DatabaseProviderFactory().CreateDefault();
            string sql = config.Sql.Replace("#{PayMonth}", month);
            DataSet ds = database.ExecuteDataSet(CommandType.Text, sql);
            return ds;
        }


    }
}