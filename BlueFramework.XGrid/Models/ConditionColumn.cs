using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.XGrid.Models
{
    public class ConditionColumn:Column
    {
        private string condition;
        private string columnValue;

        public string Condition { get => condition; set => condition = value; }
        public string ColumnValue { get => columnValue; set => columnValue = value; }
    }
}
