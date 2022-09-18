using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Test.Ioc
{
    public class OracleDao:IDao
    {

        public OracleDao()
        {
            string id = Guid.NewGuid().ToString();
            Console.WriteLine("OracleDao Class : "+id);
        }

        public void Insert()
        {
            Console.WriteLine("Oracle Insert");
        }
    }
}
