using NUnit.Framework;
using BlueFramework.Blood.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.Config.Tests
{
    [TestFixture()]
    public class ConfigManagentTests
    {
        [Test()]
        public void InitTest()
        {
            string path = @"E:\GIT\BlueFramework\HrServiceCenterWeb\sqlMappers";
            ConfigManagent.Init(path);
            int count = ConfigManagent.Configs.Count;
            Assert.IsTrue(count > 0 ? true : false);
        }
    }
}