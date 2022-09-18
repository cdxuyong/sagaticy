using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace BlueFramework.Data.SQLite
{
    public class SQLiteDatabase:Database
    {
        public SQLiteDatabase(string connectionString, DbProviderFactory dbProviderFactory)
            : base(connectionString, dbProviderFactory)
        {
        }

        public override string BuildParameterName(string name)
        {
            return "@" + name;
        }
    }
}
