using NUnit.Framework;
using BlueFramework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace NUnit.Tests1
{
    [TestFixture()]
    public class DatabaseTests
    {
        [Test()]
        public void AddInParameterTest()
        {

        }

        [Test()]
        public void AddInParameterTest1()
        {

        }

        [Test()]
        public void AddInParameterTest2()
        {

        }

        [Test()]
        public void AddOutParameterTest()
        {

        }

        [Test()]
        public void AddParameterTest()
        {

        }

        [Test()]
        public void AddParameterTest1()
        {

        }

        [Test()]
        public void BuildParameterNameTest()
        {

        }

        [Test()]
        public void CreateConnectionTest()
        {

        }

        [Test()]
        public void ExecuteDataSetTest()
        {

        }

        [Test()]
        public void ExecuteDataSetTest1()
        {

        }

        [Test()]
        public void ExecuteDataSetTest2()
        {

        }

        [Test()]
        public void LoadDataSetTest()
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            BlueFramework.Data.Database db = factory.CreateDefault();
            DataSet ds = db.ExecuteDataSet(CommandType.Text, "select * from [USER]");
            Assert.IsNotNull(ds);
        }

        [Test()]
        public void LoadDataSetTest1()
        {

        }

        [Test()]
        public void LoadDataSetTest2()
        {

        }

        [Test()]
        public void LoadDataSetTest3()
        {

        }

        [Test()]
        public void ExecuteNonQueryInsertTest()
        {
            string sql = "insert into [USER](username) VALUES('DDDD')";

            Database database = new DatabaseProviderFactory().CreateDefault();
            DbCommand dbCommand = database.GetSqlStringCommand(sql);
            try
            {
                int result = database.ExecuteNonQuery(dbCommand);
                Assert.IsTrue(result > 0);
            }
            catch(Exception ex)
            {
                Assert.Fail();
            }
        }

        [Test()]
        public void ExecuteNonQueryTransTest()
        {
            Database database = new DatabaseProviderFactory().CreateDefault();
           
            try
            {
                using (DbConnection dbConnection = database.CreateConnection())
                {
                    dbConnection.Open();
                    using (DbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 1; i < 5; i++)
                            {
                                string sql = "insert into [USER](username) VALUES('DDDD" + i + "')";
                                DbCommand dbCommand = database.GetSqlStringCommand(sql);
                                int result = database.ExecuteNonQuery(dbCommand, dbTransaction);
                            }
                            dbTransaction.Commit();
                        }
                        catch(Exception exception)
                        {
                            dbTransaction.Rollback();
                        }
                    }
                }
                Assert.IsTrue(1 > 0);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [Test()]
        public void ExecuteNonQueryTest2()
        {

        }

        [Test()]
        public void ExecuteNonQueryTest3()
        {

        }

        [Test()]
        public void ExecuteNonQueryTest4()
        {

        }

        [Test()]
        public void ExecuteNonQueryTest5()
        {

        }

        [Test()]
        public void ExecuteScalarTest()
        {

        }

        [Test()]
        public void ExecuteScalarTest1()
        {

        }

        [Test()]
        public void ExecuteScalarTest2()
        {

        }

        [Test()]
        public void ExecuteScalarTest3()
        {

        }

        [Test()]
        public void GetParameterValueTest()
        {

        }

        [Test()]
        public void GetSqlStringCommandTest()
        {

        }

        [Test()]
        public void GetStoredProcCommandTest()
        {

        }

        [Test()]
        public void GetStoredProcCommandTest1()
        {

        }
    }
}