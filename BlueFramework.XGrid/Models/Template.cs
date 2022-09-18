using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BlueFramework.XGrid.Models
{
    public class Template
    {
        private string templateName;
        private List<Column> columns;
        private List<Parameter> parameters;
        private List<ConditionColumn> conditions;
        private List<SortColumn> sortColumns;
        private List<Column> groupColumns;
        private List<ConvergeColumn> convergeColums;
        private string commandBodyText;

        public List<Column> Columns { get => columns; set => columns = value; }
        public List<Parameter> Parameters { get => parameters; set => parameters = value; }
        public List<ConditionColumn> Conditions { get => conditions; set => conditions = value; }
        public List<SortColumn> SortColumns { get => sortColumns; set => sortColumns = value; }
        public List<Column> GroupColumns { get => groupColumns; set => groupColumns = value; }
        public string CommandBodyText { get => commandBodyText; set => commandBodyText = value; }
        public string TemplateName { get => templateName; set => templateName = value; }
        public List<ConvergeColumn> ConvergeColums { get => convergeColums; set => convergeColums = value; }
    }
}
