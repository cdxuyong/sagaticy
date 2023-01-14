using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataBackService
{
    internal class Program
    {
        static Task _job = null;
        static bool _cacelled = false;
        static System.Threading.Thread _mainThread = null;
        static void Main(string[] args)
        {
            // 取消事件
            Console.CancelKeyPress += Console_CancelKeyPress;

            BlueFramework.Common.Logger.LoggerFactory.LoggerType = BlueFramework.Common.Logger.LoggerType.NLog;
            BlueFramework.Common.Logger.LoggerFactory.CreateDefault().Init();
            // load config
            Configer.Init();
            var config = Configer.Current;
            var logger = BlueFramework.Common.Logger.LoggerFactory.CreateDefault();
            logger.Info("service load config");
            logger.Info($"settings : {config.BackDirectory},{config.Corn}");
            _job = Start();
            _job.Wait();
            logger.Info("service start");
            _mainThread = new Thread(new ThreadStart(MainStart));
            _mainThread.Start();

        }
        private static void MainStart()
        {
            while (!_cacelled)
            {
                try
                {
                    Thread.Sleep(1000*60);
                }
                catch 
                {

                }
            }
        }
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _cacelled = true;
            //throw new NotImplementedException();
            var logger = BlueFramework.Common.Logger.LoggerFactory.CreateDefault();
            logger.Info("service cancel");
            _job.Dispose();
            logger.Info($"{_mainThread.Name} is {_mainThread.ThreadState}");
            logger.Info("service stop");
            try
            {
                _mainThread.Abort();
            }
            catch
            {
            }
        }

        static async Task Start()
        {
            string corn = Configer.Current.Corn;
            if (string.IsNullOrEmpty(corn))
            {
                corn = "0 20 00 ? * * ";
            }
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();
                await scheduler.Start();
                IJobDetail scanFileJob = JobBuilder.Create<MyJob>()
                    .WithIdentity("backFileJob", "group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger", "group1")
                 .WithCronSchedule(corn)
                     .Build();
                await scheduler.ScheduleJob(scanFileJob, trigger);
            }
            catch (Exception ex)
            {
                var logger = BlueFramework.Common.Logger.LoggerFactory.CreateDefault();
                logger.Error(ex.Message);
            }
        }
    }
}
