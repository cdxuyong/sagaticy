using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;

namespace BlueFramework.Common.Excel
{
    public class TemplateConfig
    {
        private string filePath;
        private TTemplate template;

        public TemplateConfig()
        {
            template = new TTemplate();
            template.Sheets = new List<TSheet>();
        }

        public void Load(string filePath)
        {
            this.filePath = filePath;
            XmlDocument doc = new XmlDocument();
            doc.Load(this.filePath);
            XmlNodeList nodeList = doc.SelectNodes("dataSet/table");
            foreach(XmlNode node in nodeList)
            {
                TSheet sheet = CreateSheet(node);
                template.Sheets.Add(sheet);
            }

        }

        private TSheet CreateSheet(XmlNode tableNode)
        {
            TSheet sheet = new TSheet();
            sheet.Head = new THead();
            if (tableNode.Attributes["name"] != null)
                sheet.Name = tableNode.Attributes["name"].Value;
            XmlNodeList nodeList = tableNode.SelectNodes("thead/tr");
            for(int i = 0; i < nodeList.Count; i++)
            {
                XmlNode tr = nodeList[i];
                TRow row = new TRow();
                for(int j=0;j<tr.ChildNodes.Count;j++)
                {
                    XmlNode th = tr.ChildNodes[j];
                    TCell cell = new TCell();
                    cell.RowIndex = i;
                    cell.Name = th.Attributes["field"] == null ? string.Empty : th.Attributes["field"].Value;
                    cell.Caption = th.InnerText;
                    if (th.Attributes["colspan"] != null)
                        cell.ColSpan = int.Parse(th.Attributes["colspan"].Value);
                    if (th.Attributes["rowspan"] != null)
                        cell.RowSpan = int.Parse(th.Attributes["rowspan"].Value);
                    cell.ColumnIndex = j;
                    row.Cells.Add(cell);
                }
                sheet.Head.Rows.Add(row);
            }
            CalculateCells(sheet);
            return sheet;
        }

        private void CalculateCells(TSheet sheet)
        {
            // 跨列
            for(int i = 0; i < sheet.Head.Rows.Count; i++)
            {
                var row = sheet.Head.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var cell = row.Cells[j];
                    if (cell.ColSpan > 1 ) //跨列
                    {
                        for (int m = j + 1; m < row.Cells.Count; m++)
                            row.Cells[m].ColumnIndex += cell.ColSpan - 1;
                    }
                }
            }
            // 跨行
            for (int i = 0; i < sheet.Head.Rows.Count; i++)
            {
                var row = sheet.Head.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var cell = row.Cells[j];
                    if (cell.RowSpan < 2) continue;
                    // 如果跨行，计算单元格跨列
                    // 循环计算下一行的单元格跨列
                    for(int ii = i+1; ii < sheet.Head.Rows.Count; ii++)
                    {
                        var row2 = sheet.Head.Rows[ii];
                        for(int jj = 0; jj < row2.Cells.Count; jj++)
                        {
                            var cell2 = row2.Cells[jj];
                            if (cell2.ColumnIndex >= cell.ColumnIndex)
                                cell2.ColumnIndex += cell.ColSpan;
                        }
                    }
                }
            }
        }

        private int GetColumnIndex(TSheet sheet,TCell cell)
        {
            TCell preCell = GetPreCell(sheet, cell);
            return 0;
        }

        private TCell GetPreCell(TSheet sheet,TCell cell)
        {
            var row = sheet.Head.Rows[cell.RowIndex];
            for(int i = 0; i < row.Cells.Count; i++)
            {
                if (cell == row.Cells[i])
                    return row.Cells[i - 1];
            }
            return null;
        }

        public void Load(DataSet ds)
        {
            foreach(DataTable dt in ds.Tables)
            {
                TSheet sheet = new TSheet();
                sheet.Title = dt.TableName;
                sheet.Name = sheet.Title;
                sheet.Head = new THead();
                TRow row = new TRow();
                sheet.Head.Rows.Add(row);
                int i = 0;
                foreach(DataColumn column in dt.Columns)
                {
                    TCell cell = new TCell();
                    cell.RowIndex = 0;
                    cell.ColumnIndex = i;
                    cell.Name = column.ColumnName;
                    cell.Caption = string.IsNullOrEmpty(column.Caption)?column.ColumnName:column.Caption;
                    row.Cells.Add(cell);
                    i++;
                }
                this.template.Sheets.Add(sheet);
            }
        }

        public TTemplate GetTemplate()
        {
            return this.template;
        }
    }
}
