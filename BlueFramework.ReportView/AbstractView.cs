using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BlueFramework.ReportView
{
    public abstract class AbstractView
    {
        public abstract string Render();

        public DataTable LoadDataTable()
        {
            return null;
        }

    }
}
