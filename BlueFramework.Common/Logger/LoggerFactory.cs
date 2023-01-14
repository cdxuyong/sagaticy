using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Logger
{
    public enum LoggerType
    {
        Log4Net = 1,
        NLog = 2
    }
    /// <summary>
    /// logger facoty
    /// </summary>
    public class LoggerFactory
    {

        public static LoggerType LoggerType { get; set; } = LoggerType.Log4Net;

        /// <summary>
        /// create logger instance by default config
        /// </summary>
        /// <returns></returns>
        public static ILogger CreateDefault()
        {
            ILogger logger = null;
            switch (LoggerType)
            {
                case LoggerType.NLog:
                    logger = new NlogWrapper();
                    break;
                default:
                    logger = new Log4Wrapper();
                    break;
            }
            return logger;
        }
    }
}
