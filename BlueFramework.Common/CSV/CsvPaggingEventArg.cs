using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.CSV
{
    /// <summary>
    /// CSV分页事件参数
    /// </summary>
    public class CsvPaggingEventArg:EventArgs
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 开始行号
        /// </summary>
        public int BeginIndex { get; set; }

        /// <summary>
        /// CSV分页事件参数
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="pageSize"></param>
        public CsvPaggingEventArg(int beginIndex,int pageSize)
        {
            BeginIndex = beginIndex;
            PageSize = pageSize;
        }
    }
}
