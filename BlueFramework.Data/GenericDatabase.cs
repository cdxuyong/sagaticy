using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace BlueFramework.Data
{
    public class GenericDatabase : Database
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDatabase"/> class with a connection string and 
        /// a provider factory.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dbProviderFactory">The provider factory.</param>
        public GenericDatabase(string connectionString, DbProviderFactory dbProviderFactory)
            : base(connectionString, dbProviderFactory)
        {
        }

    }
}
