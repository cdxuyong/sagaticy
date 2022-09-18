using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlueFramework.XGrid.Models;

namespace BlueFramework.XGrid.DataAccess
{
    public interface ISqlMaker
    {
        string makeQuerySql(Template template);

        string makeConvergeSql(Template template);
    }
}
