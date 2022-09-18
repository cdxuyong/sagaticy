using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Excel
{
    public class THead
    {
        private List<TRow> rows;

        public List<TRow> Rows { get => rows; set => rows = value; }

        public THead()
        {
            rows = new List<TRow>();
        }
    }
}
