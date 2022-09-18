using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BlueFramework.Data;

namespace HrServiceCenterWeb.DataAccess
{
    public class EmployeeAccess
    {
        public DataTable GetEmployees()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database dataBase = dbFactory.CreateDefault();
            string sql = "select * from HR_EMPLOYEE";
            DataSet ds = dataBase.ExecuteDataSet(CommandType.Text, sql);
            if (ds != null)
                return ds.Tables[0];
            else
                return null;
        }

        public int GetMaxPersonCode()
        {
            DatabaseProviderFactory dbFactory = new DatabaseProviderFactory();
            Database dataBase = dbFactory.CreateDefault();
            string sql = "select max(person_code) maxCode from HR_EMPLOYEE";
            object result = dataBase.ExecuteScalar(CommandType.Text, sql);
            return int.Parse(result.ToString());

        }
    }
}