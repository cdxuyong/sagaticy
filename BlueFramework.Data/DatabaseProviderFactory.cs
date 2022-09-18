using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;


namespace BlueFramework.Data
{
    /// <summary>
    /// <para>Represents a factory for creating named instances of <see cref="Database"/> objects.</para>
    /// </summary>
    public class DatabaseProviderFactory
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DatabaseProviderFactory"/> class 
        /// with the default configuration source.</para>
        /// </summary>
        public DatabaseProviderFactory()
        {

        }

        public Database CreateDefault()
        {
            return Create("DEFAULT");
        }

        public Database Create(string name)
        {
            string connectiongString = System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
            string providerName = ConfigurationManager.ConnectionStrings[name].ProviderName;
            DbProviderFactory dbProvider = DbProviderFactories.GetFactory(providerName);
            Database database;
            switch (providerName)
            {
                case "Oracle.ManagedDataAccess.Client":
                    database = new Oracle.OracleClientDatabase(connectiongString, dbProvider);
                    break;
                case "System.Data.SqlClient": 
                    database = new SqlServer.SqlClientDatabase(connectiongString, dbProvider);
                    break;
                case "System.Data.SQLite": 
                    database = new SQLite.SQLiteDatabase(connectiongString, dbProvider);
                    break;
                default:
                    database = new GenericDatabase(connectiongString, dbProvider);
                    break;
            }
            return database;
        }
    }
}
