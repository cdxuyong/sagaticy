using System;
using System.Threading;
using System.Data.Common;

namespace BlueFramework.Data
{
    public class DatabaseConnectionWrapper:IDisposable
    {
        private int refCount;
        public DbConnection Connection { get; private set; }

        public DatabaseConnectionWrapper(DbConnection connection)
        {
            Connection = connection;
            refCount = 1;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                int count = Interlocked.Decrement(ref refCount);
                if (count == 0)
                {
                    Connection.Dispose();
                    Connection = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        public DatabaseConnectionWrapper AddRef()
        {
            Interlocked.Increment(ref refCount);
            return this;
        }
    }
}
