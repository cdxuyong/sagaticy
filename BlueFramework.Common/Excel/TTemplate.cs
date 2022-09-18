using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Excel
{
    public class TTemplate
    {
        private List<TSheet> sheets;

        public List<TSheet> Sheets { get => sheets; set => sheets = value; }

        public TTemplate()
        {
            sheets = new List<TSheet>();
        }
    }
}
