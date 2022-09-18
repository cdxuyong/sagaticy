using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Logger
{
    /// <summary>
    /// logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// write information
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// write error message
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// write warnning message
        /// </summary>
        /// <param name="meeeage"></param>
        void Warn(string meeeage);

        /// <summary>
        /// write debugger message
        /// </summary>
        /// <param name="meeeage"></param>
        void Debugger(string meeeage);
    }
}
