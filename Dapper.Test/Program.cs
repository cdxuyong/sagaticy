using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Diagnostics;

using Unity;
using Unity.Container;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Dapper.Test.Ioc;

namespace Dapper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.LoadConfiguration();
            IService service1 = container.Resolve<IService>();
            IService service2 = container.Resolve<IService>("p1");


            //

            //container.RegisterType<IDao, OracleDao>();
            //container.RegisterType<IDao, MySqlDao>();
            //container.RegisterType<IService, TestImpl>();
            //IService service1 = container.Resolve<IService>();
            //service1.Insert();
        }
    }
}
