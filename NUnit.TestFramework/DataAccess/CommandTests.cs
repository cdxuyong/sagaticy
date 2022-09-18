using NUnit.Framework;
using BlueFramework.Blood.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueFramework.Blood.Config;
using NUnit.Tests1.Models;

namespace BlueFramework.Blood.DataAccess.Tests
{
    [TestFixture()]
    public class CommandTests
    {
        [Test()]
        public void SelectTest()
        {
            Command command = new Command();
            EntityConfig config = new EntityConfig();
            config.Id = "test.findUserById";
            config.Sql = "SELECT ID USERID,USERNAME,BIRTHDAY FROM [USER] WHERE id=#{value}";
            UserInfo ui = command.Select<UserInfo>(config, 1);
            if (ui == null)
                Assert.Fail();

            config.Sql = "SELECT ID USERID,USERNAME,BIRTHDAY FROM [USER] WHERE id=${value}";
            ui = command.Select<UserInfo>(config, 1);
            if (ui == null)
                Assert.Fail();

            Assert.Pass();
        }

        [Test()]
        public void LoadEntityTest()
        {
            Assert.Pass();
        }

        [Test()]
        public void LoadEntitiesTest()
        {
            Assert.Pass();
        }

        [Test()]
        public void CommandTest()
        {
            Assert.Pass();
        }

        [Test()]
        public void LoadEntityTest1()
        {
            Assert.Pass();
        }

        [Test()]
        public void LoadEntitiesTest1()
        {
            Assert.Pass();
        }

        [Test()]
        public void SelectTest1()
        {
            Assert.Pass();
        }

        [Test()]
        public void SelectTest2()
        {
            Assert.Pass();
        }

        [Test()]
        public void InsertTest()
        {
            UserInfo userInfo = new UserInfo()
            {
                UserName = "XY",
                Birthday = DateTime.Now
            };
            InsertConfig config = new InsertConfig
            {
                Id = "test.insertUser",
                Sql = "insert into [user](username,birthday,address) values(#{UserName},#{Birthday},#{Address})",
                KeyProperty = "UserId",
                KeyMadeOrder = IdentityMadeOrder.Inserting,
                KeyPropertySql = "select @UserId=@@IDENTITY"

            };
            Command command = new Command();
            UserInfo o = command.Insert<UserInfo>(config, userInfo);
            Assert.IsNotNull(o);
        }

        [Test()]
        public void SelectTest3()
        {
            Command command = new Command();
            EntityConfig config = new EntityConfig();
            config.Id = "test.findUsers";
            config.Sql = "SELECT ID USERID,USERNAME,BIRTHDAY FROM [USER] WHERE SEX LIKE '%${value}%'";
            //config.Sql = "SELECT ID USERID,USERNAME,BIRTHDAY FROM [USER] WHERE SEX=#{value}";
            CommandParameter[] parameters =  {
                new CommandParameter() { ParameterName="value",ParameterValue="S",ParameterType=typeof(System.String) }
            };

            List<UserInfo> US = command.SelectList<UserInfo>(config, parameters);

            Assert.IsNotNull(US);
        }

        [Test()]
        public void InsertTest1()
        {
            UserInfo userInfo = new UserInfo()
            {
                UserId = 0,
                UserName = "jim",
                Birthday = DateTime.Now
            };
            InsertConfig config = new InsertConfig
            {
                Id = "test.insertUser",
                Sql = "insert into [user](username,birthday,address) values(#{UserName},#{Birthday},#{Address})",
                KeyProperty= "UserId",
                KeyMadeOrder = IdentityMadeOrder.Inserting,
                KeyPropertySql= "select @UserId=@@IDENTITY"

            };
            Command command = new Command();
            UserInfo o = command.Insert<UserInfo>(config, userInfo);
            Assert.IsNotNull(o);
        }

        [Test()]
        public void TransactionTest()
        {
            InsertConfig config = new InsertConfig
            {
                Id = "test.insertUser",
                Sql = "insert into [user](username,birthday,address) values(#{UserName},#{Birthday},#{Address})",
                KeyProperty = "UserId",
                KeyMadeOrder = IdentityMadeOrder.Inserting,
                KeyPropertySql = "select @UserId=@@IDENTITY"

            };
            UpdateConfig updateConfig = new UpdateConfig
            {
                Id = "test.insertUser",
                Sql = "update [user] set username=#{UserName} where id=#{UserId}",

            };
            DeleteConfig deleteConfig = new DeleteConfig
            {
                Id = "test.deleteUser",
                Sql = "delete from [user] where id=#{value}",

            };
            int insertCount = 0;
            int beginId = 0;
            using (var command = new Command())
            {
                try
                {
                    command.BeginTransaction();
                    for (int i = 0; i < 10; i++)
                    {
                        UserInfo userInfo = new UserInfo()
                        {
                            UserId = 0,
                            UserName = "jim"+i.ToString(),
                            Birthday = DateTime.Now
                        };
                        UserInfo o = command.Insert<UserInfo>(config, userInfo);
                        insertCount++;
                        if (o != null && i==0)
                        {
                            beginId = o.UserId;
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        UserInfo userInfo = new UserInfo()
                        {
                            UserId = i+50,
                            UserName = "jim" + i.ToString(),
                            Birthday = DateTime.Now
                        };
                        command.Update<UserInfo>(updateConfig, userInfo);
                    }

                    for(int i = beginId; i <= beginId+ 5; i++)
                    {
                        command.Delete(deleteConfig, i);
                    }

                    command.CommitTransaction();
                }
                catch(Exception ex)
                {
                    command.RollbackTransaction();
                }
            }
            Assert.AreEqual(insertCount, 10);

        }
    }
}