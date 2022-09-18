using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueFramework.XGrid.Models;

namespace BlueFramework.XGrid.DataAccess
{
    public class SqlMakeAdapter
    {
        private ISqlMaker SqlMaker;
        public string makeSql(Template template)
        {
            bool isConverge = template.GroupColumns != null && template.GroupColumns.Count > 0 ? true : false;
            if (isConverge)
            {
                SqlMaker = new ConvergeSqlMaker();
                return SqlMaker.makeConvergeSql(template);
            }
            else
            {
                SqlMaker = new QuerySqlMaker();
                return SqlMaker.makeQuerySql(template);
            }
        }
    }
}
