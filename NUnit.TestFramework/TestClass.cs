using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using BlueFramework.User.Models;

namespace NUnit.Tests1
{
    public class DynamicSample
    {
        public string Name { get; set; }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    [TestFixture]
    public class TestStringClass
    {
        [Test]
        public void TestMethod()
        {
            char[] vs1 = new char[] {'a','b' };
            char[] vs2 = new char[] {'a','b' };

            String a1 = new String(vs1);
            String a2 = new String(vs2);
            bool result = false;
            if (a1 == a2)
            {
                result = true;
            }
            if (a1.Equals(a2))
            {
                result = true;

            }

        }


        [Test]
        public void TestDynamic()
        {
            int i = 1;
            dynamic d1 = i;

            dynamic d2 = new DynamicSample();
            //int a = d2.add
        }

        [Test]
        public void TestDataTableComputer()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            dt.Columns.Add("B");
            dt.Columns.Add("C");

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            dr["A"] = 1;
            dr["B"] = 1;
            dr["C"] = 1;

            string exp = "int.Parse(dr[\"A\"].ToString())+ int.Parse(dr[\"B\"].ToString())";
            object result = dt.Compute(exp, "");

            Assert.IsNull(result);
        }
    }
}
