using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Excel
{
    /// <summary>
    /// 单元格
    /// </summary>
    public class TCell
    {
        private string name;
        private int rowIndex= -1;
        private int columnIndex = -1;
        private int colSpan = 1;
        private int rowSpan = 1;
        private int width=0;
        private string caption;

        public string Name { get => name; set => name = value; }
        public int ColSpan { get => colSpan; set => colSpan = value; }
        public int RowSpan { get => rowSpan; set => rowSpan = value; }
        public int Width { get => width; set => width = value; }
        public string Caption { get => caption; set => caption = value; }
        public int RowIndex { get => rowIndex; set => rowIndex = value; }
        public int ColumnIndex { get => columnIndex; set => columnIndex = value; }

        public TCell()
        {

        }

        public TCell(String name,string caption,int rowIndex,int columnIndex)
        {
            this.name = name;
            this.caption = caption;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }
    }
}
