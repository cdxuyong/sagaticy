using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Test
{
    public static class MyObject
    {
        public static void Doit(this IMyObject myObject, string name)
        {

        }

        public static int ToInt(this String s)
        {
            return int.Parse(s);
        }
    }
}
