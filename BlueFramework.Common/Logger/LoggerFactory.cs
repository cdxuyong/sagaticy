using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Logger
{
    /// <summary>
    /// logger facoty
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// create logger instance by default config
        /// </summary>
        /// <returns></returns>
        public static ILogger CreateDefault()
        {
            ILogger logger = new Log4Wrapper();
            return logger;
        }
    }
}
