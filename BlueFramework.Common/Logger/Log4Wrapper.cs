using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net.Config;

namespace BlueFramework.Common.Logger
{
    public class Log4Wrapper:ILogger
    {
        private const string DEFAULT_CONFIG_FILE = "log4net.config";

        public void Init()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + DEFAULT_CONFIG_FILE;
            path = System.IO.Path.GetFullPath(path);
            var logCfg = new FileInfo(path);
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

        /// <summary>
        /// write information
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
            log.Info(message);
        }

        /// <summary>
        /// write error message
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
            log.Error(message);
        }

        /// <summary>
        /// write warnning message
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
            log.Warn(message);
        }

        /// <summary>
        /// write debugger message
        /// </summary>
        /// <param name="meeeage"></param>
        public void Debugger(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
            log.Debug(message);

        }
    }
}
