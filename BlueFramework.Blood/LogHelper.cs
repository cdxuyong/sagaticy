using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueFramework.Common.Logger;

namespace BlueFramework.Blood
{
    public class LogHelper
    {
        public static void Info(string source,string message)
        {
            string content = source + "：" + message;
            LoggerFactory.CreateDefault().Info(content);
        }

        public static void Warn(string source, Exception ex)
        {
            string content = source + "：" + ex.Message;
            LoggerFactory.CreateDefault().Warn(content);
        }

        public static void Debugger(string source, string message)
        {

            string content = source + "：" + message;
            LoggerFactory.CreateDefault().Debugger(content);

        }
    }
}
