using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBackService
{
    interface IDbBack
    {
        bool StopService(string serviceName);

        bool StartService(string serviceName);

        bool BackFiles(string backDir, string[] files);
    }
}
