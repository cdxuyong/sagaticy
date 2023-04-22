using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HrServiceCenterWeb.Models;
using BlueFramework.Blood;

namespace HrServiceCenterWeb.Manager
{
    public class SystemManager
    {
       
        public bool ExecCommands(string cmds,out string message)
        {
            bool result = false;
            BlueFramework.Data.Database database = new BlueFramework.Data.DatabaseProviderFactory().CreateDefault();
            try
            {
                bool isGet = cmds.StartsWith("get:");
                if (isGet)
                {
                    cmds = cmds.Substring(4);
                }
                if (!isGet)
                {
                    var rows = database.ExecuteNonQuery(System.Data.CommandType.Text, cmds);
                    message = cmds;
                    message += "\r\n请不要重复执行脚本，当前执行结果 = "+rows;
                }
                else
                {
                    var ds = database.ExecuteDataSet(System.Data.CommandType.Text, cmds);
                    message = Newtonsoft.Json.JsonConvert.SerializeObject(ds); 
                }

                result = true;
            }
            catch (Exception ex)
            {
                message = "执行出现错误：" + ex.Message;
            }
            return result;
        }
        
    }
}