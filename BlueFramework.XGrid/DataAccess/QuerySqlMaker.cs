using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueFramework.XGrid.Models;

namespace BlueFramework.XGrid.DataAccess
{
    public class QuerySqlMaker : ISqlMaker
    {
        StringBuilder stringBuilder = new StringBuilder();

        public string makeConvergeSql(Template template)
        {
            // nothing
            return string.Empty;
        }

        public string makeQuerySql(Template template)
        {
            makeHeader(template);
            return string.Empty;
        }

        private void makeHeader(Template template)
        {
            if (template.Columns != null && template.Columns.Count > 0)
            {
                stringBuilder.Append("select ");
                for (int i = 0; i < template.Columns.Count; i++)
                {
                    Column column = template.Columns[i];
                    if (i == 0)
                        stringBuilder.Append(column.Name);
                    else
                        stringBuilder.Append("," + column.Name);
                }
                stringBuilder.Append(" from (");
                stringBuilder.Append(template.CommandBodyText);
                stringBuilder.Append(")");
            }
            else
            {
                stringBuilder.Append("select *");
                stringBuilder.Append(" from (");
                stringBuilder.Append(template.CommandBodyText);
                stringBuilder.Append(")");
            }
        }
    }
}
