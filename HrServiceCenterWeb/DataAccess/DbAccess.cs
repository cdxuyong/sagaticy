using BlueFramework.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HrServiceCenterWeb.DataAccess
{
    public class DbAccess
    {
        public int GetMax(string tableName,string fieldName)
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database dataBase = dbFactory.CreateDefault();
            string sql = $"select max({fieldName}) maxCode from {tableName}";
            object result = dataBase.ExecuteScalar(CommandType.Text, sql);
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.ToString()))
                    return 0;

            }
            else
            {
                return 0;
            }
            return int.Parse(result.ToString());
        }
    }
}