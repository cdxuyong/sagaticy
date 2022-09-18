using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using BlueFramework.XGrid.Models;
using BlueFramework.Data;

namespace BlueFramework.XGrid.DataAccess
{
    public class QueryAccess
    {
        private Database database;
        private Template template;

        public QueryAccess(Template template)
        {
            this.template = template;
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            database = factory.CreateDefault();
        }

        public DataTable GetTable()
        {
            //string sql = makeSql();
            DbCommand dbCommand = makeCommand();
            DataSet ds = database.ExecuteDataSet(dbCommand);
            return ds.Tables[0];
        }

        private DbCommand makeCommand()
        {
            string sql = makeSql();
            DbCommand command = database.GetSqlStringCommand(null);

            return command;
        }

        private string makeSql()
        {
            StringBuilder stringBuilder = new StringBuilder();
            // columns 

            // groups 


            return null;
        }

    }
}
