using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Excel
{
    /// <summary>
    /// 表头行
    /// </summary>
    public class TRow
    {
        private List<TCell> cells;

        public List<TCell> Cells { get => cells; set => cells = value; }

        public TRow()
        {
            cells = new List<TCell>();
        }
    }
}
