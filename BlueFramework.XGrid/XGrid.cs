using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BlueFramework.Common;
using BlueFramework.XGrid.Config;
using BlueFramework.XGrid.Models;
using BlueFramework.XGrid.DataAccess;

namespace BlueFramework.XGrid
{
    public class XGrid
    {
        private List<Column> columns;
        private DataTable dataSource;

        public List<Column> Columns { get => columns; set => columns = value; }
        public DataTable DataSource { get => dataSource; set => dataSource = value; }

        private string GetConfigPath(string templateName)
        {
            string appPath = ApplicationUtils.GetApplicationPath();
            string configPath = appPath + "/" + templateName + ".xml";
            string realPath = System.IO.Path.GetFullPath(configPath);
            return realPath;
        }

        public void Load(Template queryTemplate)
        {
            // load template
            string configPath = GetConfigPath(queryTemplate.TemplateName);
            TemplateConfig config = new TemplateConfig();
            Template template = config.LoadTemplateByConfig(configPath);
            // make columns
            if(queryTemplate.Parameters!=null)
                template.Parameters.AddRange(queryTemplate.Parameters);
            if(queryTemplate.GroupColumns!=null)
                template.GroupColumns.AddRange(queryTemplate.GroupColumns);
            if(queryTemplate.SortColumns!=null)
                template.SortColumns.AddRange(queryTemplate.SortColumns);
            if(queryTemplate.ConvergeColums!=null)
                template.ConvergeColums.AddRange(queryTemplate.ConvergeColums);

            // query
            QueryAccess dbAccess = new QueryAccess(template);
            dataSource = dbAccess.GetTable();
        }
    }
}
