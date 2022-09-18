using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace BlueFramework.Data.Oracle
{
    public class OracleClientDatabase : Database
    {
        public OracleClientDatabase(string connectionString, DbProviderFactory dbProviderFactory)
            : base(connectionString, dbProviderFactory)
        {
        }

        public override string BuildParameterName(string name)
        {
            return ":" + name;
        }
    }
}
