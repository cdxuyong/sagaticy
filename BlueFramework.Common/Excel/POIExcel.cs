using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;
using System.Threading;

using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;

namespace BlueFramework.Common.Excel
{
    public class POIExcel : IExcel
    {
        private IWorkbook workbook;
        private List<string> sheetNames;

        public POIExcel()
        {

        }

        private void LoadFile(string filePath)
        {
            var prevCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = WorkbookFactory.Create(fs);
            }
            sheetNames = getSheetNames();
        }

        private void LoadStream(Stream stream)
        {
            workbook = WorkbookFactory.Create(stream);
            stream.Close();
            sheetNames = getSheetNames();
        }

        private List<string> getSheetNames()
        {
            var count = workbook.NumberOfSheets;
            List<string> names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                names.Add(workbook.GetSheetName(i));
            }
            return names;
        }

        public DataSet Read(string filePath)
        {
            LoadFile(filePath);
            var ds = new DataSet();
            foreach (var sheetName in this.sheetNames)
            {
                DataTable dt = Read(sheetName, 1);
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataSet Read(Stream stream)
        {
            return Read(stream,1);
        }

        public DataSet Read(Stream stream, int rownum)
        {
            LoadStream(stream);
            var ds = new DataSet();
            foreach (var sheetName in this.sheetNames)
            {
                DataTable dt = Read(sheetName, rownum);
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public DataTable Read(string filePath, int sheetIndex, int dataIndex)
        {
            LoadFile(filePath);
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            return Read(sheet, dataIndex);
        }

        private DataTable Read(string sheetName,int dataIndex)
        {
            ISheet sheet = workbook.GetSheet(sheetName);
            return Read(sheet, dataIndex);
        }

        private DataTable Read(ISheet sheet,int dataIndex)
        {
            List<string> columns = readColumns(sheet, dataIndex - 1);
            DataTable dt = new DataTable(sheet.SheetName);
            int i = 1;
            foreach (string columnName in columns)
            {
                string colName = columnName;
                if (dt.Columns.Contains(columnName))
                    colName = columnName + "_" + i;
                dt.Columns.Add(colName);
                i++;
            }
            fillTable(sheet, dt, dataIndex);
            return dt;
        }

        private List<string> readColumns(ISheet sheet,int lastRow)
        {
            if (lastRow == -1)
            {
                return readDefaultColumns(sheet);
            }
            // get all header cells
            List<ICell> cells = new List<ICell>();
            for(int i = 0; i <= lastRow; i++)
            {
                var row = sheet.GetRow(i);
                foreach(var cell in row.Cells)
                {
                    if (cell.CellType != CellType.Blank)
                    {
                        cells.Add(cell);
                    }
                }
            }
            // compare last row cells
            int m = 0;
            List<ICell> realCells = new List<ICell>();
            while (true)
            {
                bool find = false;
                foreach(var cell in cells)
                {
                    if (cell.ColumnIndex == m)
                    {
                        find = true;
                        if (realCells.Count == m + 1)
                        {
                            if (realCells[m].RowIndex < cell.RowIndex)
                                realCells[m] = cell;
                        }
                        else
                            realCells.Add(cell);
                    }
                }
                m++;
                if (!find)
                    break;
            }
            // change to columns
            List<string> columns = new List<string>();
            foreach(ICell cell in realCells)
            {
                object o = getCellValue(cell);
                columns.Add(o.ToString());
            }
            return columns;
        }
        private List<string> readDefaultColumns(ISheet sheet)
        {
            var row = sheet.GetRow(0);
            List<string> columns = new List<string>();
            for (int i = 0; i < row.Cells.Count; i++)
            {
                string column = "F_" + i;
                columns.Add(column);
            }
            return columns;
        }
        
        private void fillTable(ISheet sheet,DataTable dt,int startRow)
        {
            for(int i = startRow; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                DataRow dr = dt.NewRow();
                foreach(var cell in row.Cells)
                {
                    if (dt.Columns.Count <= cell.ColumnIndex)
                        continue;
                    dr[cell.ColumnIndex] = getCellValue(cell);
                    
                }
                dt.Rows.Add(dr);
            }
        }

        private object getCellValue(ICell cell)
        {
            if (cell == null) return null;
            object o;
            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                        o = cell.DateCellValue;
                    else
                        o = cell.NumericCellValue;
                    break;
                default:
                    o = cell.StringCellValue;
                    break;
            }
            return o;
        }

        public void Write(Stream stream, DataSet ds, ExcelExtendType extendType=ExcelExtendType.XLSX)
        {
            TemplateConfig config = new TemplateConfig();
            config.Load(ds);
            TTemplate template = config.GetTemplate();
            Write(stream, template, ds,extendType);
        }


        public void Write(Stream stream, TTemplate template, DataSet ds, ExcelExtendType extendType = ExcelExtendType.XLSX)
        {

            if(extendType==ExcelExtendType.XLS)
                workbook = new HSSFWorkbook();
            else
                workbook = new XSSFWorkbook();
            foreach(TSheet tSheet in template.Sheets)
            {
                ISheet sheet = workbook.CreateSheet(tSheet.Title);
                string tableName = string.IsNullOrEmpty(tSheet.Name) ? tSheet.Title : tSheet.Name;
                fillSheet(sheet, tSheet, ds.Tables[tableName]);
            }
           
            workbook.Write(stream);
        }

        private void fillSheet(ISheet sheet, TSheet tSheet,DataTable dt)
        {
            calculate(tSheet);
            // fill header
            int rownum = 0;
            List<TCell> tCells = new List<TCell>();
            foreach(TRow tRow in tSheet.Head.Rows)
            {
                IRow row = sheet.CreateRow(rownum);
                rownum++;
                foreach(TCell tCell in tRow.Cells)
                {
                    tCells.Add(tCell);
                    ICell cell = row.CreateCell(tCell.ColumnIndex);
                    cell.SetCellValue(tCell.Caption);
                }
            }
            // create region
            creatCellRanges(sheet, tCells);
            // get columns
            int m = 0;
            List<TCell> realTCells = new List<TCell>();
            while (true)
            {
                bool find = false;
                foreach (var tCell in tCells)
                {
                    if (tCell.ColumnIndex == m)
                    {
                        find = true;
                        if (realTCells.Count == m + 1)
                        {
                            if (realTCells[m].RowIndex < tCell.RowIndex)
                                realTCells[m] = tCell;
                        }
                        else
                            realTCells.Add(tCell);
                    }
                }
                m++;
                if (!find)
                    break;
            }
            // fill sheet
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                IRow row = sheet.CreateRow(rownum + i);
                foreach(TCell tCell in realTCells)
                {
                    ICell cell = row.CreateCell(tCell.ColumnIndex);
                    object o = dr[tCell.Name];
                    cell.SetCellValue(o.ToString());
                }
            }
            // set column
            setColumnWidth(sheet, tCells);
        }

        private void setColumnWidth(ISheet sheet, List<TCell> tCells)
        {
            sheet.DefaultColumnWidth = 8;
            foreach(TCell cell in tCells)
            {
                if(cell.Width>0)
                {
                    sheet.SetColumnWidth(cell.ColumnIndex, cell.Width);
                }
            }

        }

        private void creatCellRanges(ISheet sheet, List<TCell> tCells)
        {
            foreach(TCell tCell in tCells)
            {
                bool enable = tCell.ColSpan > 1 || tCell.RowSpan > 1 ? true : false;
                if (!enable) continue;
                int firstRow = tCell.RowIndex;
                int lastRow = tCell.RowIndex + tCell.RowSpan-1;
                int firstCol = tCell.ColumnIndex;
                int lastCol = tCell.ColumnIndex + tCell.ColSpan - 1;
                CellRangeAddress region = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
                sheet.AddMergedRegion(region);
            }
        }

        private void calculate(TSheet tSheet)
        {

        }

        public void Write(Stream stream, string templatePath, DataSet ds, ExcelExtendType extendType = ExcelExtendType.XLSX)
        {
            TemplateConfig config = new TemplateConfig();
            config.Load(templatePath);
            TTemplate template = config.GetTemplate();
            Write(stream, template, ds);
        }

        public bool WriteFile(string outputFilePath, Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            try
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    fileStream.Write(buffer, 0, buffer.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WriteFile(string outputFilePath, DataSet ds, TTemplate template)
        {
            POIStream stream = new POIStream();
            stream.AllowClose = false;
            ExcelExtendType extendType = outputFilePath.IndexOf(".xlsx") > 0 ? ExcelExtendType.XLSX : ExcelExtendType.XLS;
            if (template == null)
                Write(stream, ds, extendType);
            else
                Write(stream, template, ds, extendType);
            bool result = WriteFile(outputFilePath, stream);
            return result;
        }


        /// <summary>
        /// WriteFile
        /// </summary>
        /// <param name="outputFilePath"></param>
        /// <param name="ds"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public bool WriteFile(string outputFilePath, DataSet ds, string templateName)
        {
            POIStream stream = new POIStream();
            stream.AllowClose = false;
            ExcelExtendType extendType = outputFilePath.IndexOf(".xlsx") > 0 ? ExcelExtendType.XLSX : ExcelExtendType.XLS;
            TemplateConfig templateConfig = new TemplateConfig();
            templateConfig.Load(templateName);
            TTemplate template = templateConfig.GetTemplate();
            if (template == null)
                Write(stream, ds, extendType);
            else
                Write(stream, template, ds, extendType);
            bool result = WriteFile(outputFilePath, stream);
            return result;
        }
    }
}
