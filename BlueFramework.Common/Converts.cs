using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data;

namespace BlueFramework.Common
{
    public static class Converts
    {
        public static DataTable ListToDataTable<T>(this List<T> list,Dictionary<string,string> fields=null)
        {
            Type t = typeof(T);
            PropertyInfo[] ps = t.GetProperties();
            DataTable dataTable = new DataTable();
            // add columns
            for (int i = 0; i < ps.Length; i++)
            {
                PropertyInfo p = ps[i];
                string field = p.Name;
                if(fields==null)
                    dataTable.Columns.Add(field);
                else
                {
                    string caption;
                    if( fields.TryGetValue(field, out caption) )
                    {
                        DataColumn column = new DataColumn(field);
                        column.Caption = caption;
                        dataTable.Columns.Add(column);
                    }
                    else
                    {
                        ps[i] = null;
                    }
                }
            }
            // add rows
            foreach (T obj in list)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);
                for (int i = 0; i < ps.Length; i++)
                {
                    PropertyInfo pi = ps[i];
                    if (pi == null) continue;
                    object value = pi.GetValue(obj, null);
                    row[pi.Name] = value;
                }
            }
            return dataTable;
        }
    }
}
