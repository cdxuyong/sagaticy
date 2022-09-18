using NUnit.Framework;
using BlueFramework.Common.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace BlueFramework.Common.Excel.Tests
{
    [TestFixture()]
    public class POIExcelTests
    {
        [Test()]
        public void POIExcelTest()
        {
            Assert.Pass();
        }

        [Test()]
        public void ReadTest()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t1.xls";
            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = excel.Read(file);
            int realCount = 7;
            int count = ds.Tables[0].Rows.Count;
            Assert.IsTrue(count == realCount);
        }

        [Test()]
        public void ReadTest2()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t2.XLSX";
            IExcel excel = ExcelFactory.CreateDefault();
            DataTable dt = excel.Read(file, 0, 2);
            int realCount = 10;
            int count = dt.Rows.Count;
            Assert.IsTrue(count == realCount);
        }

        [Test()]
        public void WriteXlsTest()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t1.xls";
            IExcel excel = ExcelFactory.CreateDefault();
            DataTable dt = excel.Read(file, 0, 1);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            MemoryStream stream1 = new MemoryStream();
            excel.Write(stream1, ds,ExcelExtendType.XLS);
            string outputFilePath = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/out1.xls";
            bool result = excel.WriteFile(outputFilePath, stream1);

            Assert.IsNotNull(result) ;
        }

        [Test()]
        public void WriteMoreDataTypeTest()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t3.xlsx";
            IExcel excel = ExcelFactory.CreateDefault();
            DataSet ds = excel.Read(file);
            DataTable dt = ds.Tables[0];

            Assert.IsNotNull(ds);
        }

        [Test()]
        public void WriteXlsxTest()
        {

            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t2.xlsx";
            IExcel excel = ExcelFactory.CreateDefault();
            DataTable dt = excel.Read(file, 0, 2);
            
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            TTemplate template = new TTemplate();
            TRow R1 = new TRow();
            TCell A = new TCell("A", "A", 0, 0);A.ColSpan = 2;
            TCell B = new TCell("", "B", 0, 2);
            TCell C = new TCell("C", "C", 0, 3);C.RowSpan = 2;
            TCell D = new TCell("D", "D", 0, 4);D.RowSpan = 2;
            TCell E = new TCell("", "E", 0, 5);E.ColSpan = 2;
            R1.Cells.Add(A);
            R1.Cells.Add(B);
            R1.Cells.Add(C);
            R1.Cells.Add(D);
            R1.Cells.Add(E);

            TRow R2 = new TRow();
            TCell A1 = new TCell("A1", "A1", 1, 0);
            TCell A2 = new TCell("A2", "A2", 1, 1);
            TCell B1 = new TCell("B1", "B1", 1, 2);
            TCell E1 = new TCell("E1", "E1", 1, 5);
            TCell E2 = new TCell("E2", "E2", 1, 6);
            R2.Cells.Add(A1);
            R2.Cells.Add(A2);
            R2.Cells.Add(B1);
            R2.Cells.Add(E1);
            R2.Cells.Add(E2);
            TSheet tsheet = new TSheet();
            tsheet.Name = "Sheet1";
            tsheet.Title = "表格O";
            tsheet.Head.Rows.Add(R1);
            tsheet.Head.Rows.Add(R2);
            template.Sheets.Add(tsheet);

            string outputFilePath = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/out2.xlsx";
            bool result = excel.WriteFile(outputFilePath, ds, template);
            Assert.IsTrue(result);
        }

        [Test()]
        public void WriteXlsxTestByTemplate()
        {

            string file = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/t4.xlsx";
            IExcel excel = ExcelFactory.CreateDefault();
            DataTable dt = excel.Read(file, 0, 1);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            string templateName = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/template.xml";
            string outputFilePath = AppDomain.CurrentDomain.BaseDirectory + "/ExcelTests/out4.xlsx";
            bool result = excel.WriteFile(outputFilePath, ds, templateName);
            Assert.IsTrue(result);
        }

    }
}