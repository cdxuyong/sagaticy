using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace NUnit.Tests1
{
    [TestFixture]
    public class DyncTests
    {
        [Test]
        public void TestMethod()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
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

            string exp = "99+88/33";
            object result = dt.Compute(exp, "");

            Assert.IsNull(result);
        }
    }
}
