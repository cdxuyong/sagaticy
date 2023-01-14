using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBackService
{
    public class MyJob: IJob
    {
        public bool Running = false;

        public Task Execute(IJobExecutionContext context)
        {
            if (Running) return Task.CompletedTask;
            // 锁定执行
            Running = true;
            // 启动任务
            return Task.Run(() =>
            {
                try
                {
                    BackData();
                }
                catch (Exception ex)
                {
                    var logger = BlueFramework.Common.Logger.LoggerFactory.CreateDefault();
                    logger.Error("job exec throw a ex : "+ex.Message);
                    logger.Error(ex.StackTrace); 
                }
                finally
                {
                    Running = false;
                }
            });
        }

        private void BackData()
        {
            var svrName = Configer.Current.ServiceNames[0];
            IDbBack dbBack = new SqlServerDbBackImpl(svrName, Configer.Current.FilePaths.ToArray());
            dbBack.StopService(string.Empty);
            dbBack.BackFiles(Configer.Current.BackDirectory, null);
            var succ = dbBack.StartService(string.Empty);

        }
    }
}
