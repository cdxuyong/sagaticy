using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Test.Ioc
{
    public class MySqlDao : IDao
    {

        public MySqlDao()
        {
            string id = Guid.NewGuid().ToString();
            Console.WriteLine("MySqlDao Class : " + id);
        }

        public void Insert()
        {
            Console.WriteLine("MySql Insert");
        }
    }
}
