using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Test.Ioc
{
    public class TestImpl:IService
    {
        IDao myDao;

        public TestImpl(IDao dao)
        {
            //Console.WriteLine("TestImpl Class");
            myDao = dao;
        }
        public void Insert()
        {
            Console.WriteLine("TestImpl Insert.");
            myDao.Insert();
        }
    }
}
