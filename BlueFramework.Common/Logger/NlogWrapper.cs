using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BlueFramework.Common.Logger
{
    public class NlogWrapper : ILogger
    {
        private static NLog.Logger _logger = null;

        public void Init()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger(); 
        }
        /// <summary>
        /// write information
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        /// write error message
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            _logger.Error(message);
        }

        /// <summary>
        /// write warnning message
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        /// <summary>
        /// write debugger message
        /// </summary>
        /// <param name="meeeage"></param>
        public void Debugger(string message)
        {
            _logger.Debug(message);

        }
    }
}
