using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace BlueFramework.Common
{
    public class ApplicationUtils
    {
        public static string GetApplicationPath()
        {
            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            return path;
        }
    }
}
