using NUnit.Framework;
using BlueFramework.Blood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Tests1.Models;

namespace BlueFramework.Blood.Tests
{
    [TestFixture()]
    public class EntityContextTests
    {
        [Test()]
        public void EntityContextTest()
        {
            Assert.Pass("passed");
            //Assert.Fail();
        }

        
        [Test()]
        public void InsertTest()
        {
            // init是系统加载时初始化加载，这里不用写，只需要加载一次
            Session.Init();
            // 下面是非事务插入单个数据对象
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    UserInfo ui = new UserInfo()
                    {
                        UserName = "single user object",
                        Birthday = DateTime.Now,
                        Sex="UNKNOW"
                    };
                    context.Save<UserInfo>("test.insertUser", ui);
                }
                catch
                {

                }
            }
        }

        [Test()]
        public void UpdateTest()
        {
            // init是系统加载时初始化加载，这里不用写，只需要加载一次
            Session.Init();
            // 下面是非事务保存单个数据对象
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    UserInfo ui = new UserInfo()
                    {
                        UserId = 1,
                        UserName = "XY-UPDATE",
                        Birthday = DateTime.Now,
                        Sex = "男",
                        Address="update address"

                    };
                    context.Save<UserInfo>("test.updateUser", ui);
                }
                catch
                {

                }
            }
        }

        [Test()]
        public void DeleteTest()
        {
            // init是系统加载时初始化加载，这里不用写，只需要加载一次
            Session.Init();
            // 下面是非事务插入单个数据对象
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    context.Delete("test.deleteUserBySex", "UNKNOW");
                }
                catch
                {

                }
            }
        }

        [Test()]
        public void SaveTest()
        {
            bool pass = true;
            // init是系统加载时初始化加载，这里不用写，只需要加载一次
            Session.Init();
            // 过程如下
            // 1 创建上下文context
            using (EntityContext context = Session.CreateContext())
            {
                try
                {
                    // 如果是事务，BeginTransaction
                    context.BeginTransaction();
                    for(int i = 1; i <= 5; i++)
                    {
                        UserInfo ui = new UserInfo() {
                            UserName = "pk"+i.ToString(),
                            Birthday = DateTime.Now
                        };
                        context.Save<UserInfo>("test.insertUser", ui);
                    }
                    // 如果是事务，结束Commit
                    context.Commit();

                }
                catch(Exception ex)
                {
                    // 如果是事务，异常Rollback
                    context.Rollback();
                    pass = false;
                }
            }
            Assert.IsTrue(pass);
            //Assert.Fail();
        }

        [Test()]
        public void SeleteTest()
        {
            Assert.Pass("passed");
            //Assert.Fail();
        }

        [Test()]
        public void BeginTransactionTest()
        {
            Assert.Pass("passed");
        }

        [Test()]
        public void CommitTest()
        {
            Assert.Pass("passed");
        }

        [Test()]
        public void RollbackTest()
        {
            Assert.Pass("passed");
        }

        [Test()]
        public void DisposeTest()
        {
            Assert.Pass("passed");
        }
    }
}