using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace BlueFramework.Common.CSV
{


    /// <summary>
    /// CSV分页解析
    /// </summary>
    public class CsvPagesParser : CsvFileParser
    {
        /// <summary>
        /// CSV文件分页读取事件
        /// </summary>
        /// <param name="pageTable">分页的数据表格</param>
        /// <param name="e">分页事件参数</param>
        public delegate void PaggingHandle(DataTable pageTable,CsvPaggingEventArg e);

        /// <summary>
        /// 分页事件
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event PaggingHandle OnPaggingEvent;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize = 10000000;

        /// <summary>
        /// CSV分页解析
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        public CsvPagesParser(int pageSize)
        {
            this.PageSize = pageSize;
            OnPaggingEvent = null;
        }

        /// <summary>
        /// 分页读取文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Parse(string filePath)
        {
            StreamReader streamReader = LoadFile(filePath);

            string[] cells = null;
            string line = streamReader.ReadLine();

            // 初始化表格
            DataTable table = new DataTable();
            if (!string.IsNullOrEmpty(line))
            {
                cells = SplitLine(line);
                for (int i = 0; i < cells.Length; i++)
                {
                    DataColumn dc = new DataColumn(cells[i]);
                    table.Columns.Add(dc);
                }
            }

            // 分页读取文件
            int rowCount = 0;
            while (streamReader.BaseStream.CanRead && (line = streamReader.ReadLine()) != null)
            {
                cells = SplitLine(line);
                table.Rows.Add(cells);
                rowCount++;
                if (rowCount >= PageSize)
                {
                    if (OnPaggingEvent != null)
                    {
                        OnPaggingEvent(table,new CsvPaggingEventArg(rowCount,PageSize) );
                        table.Clear();
                        rowCount = 0;
                    }
                }
            }
        }
    }
}
