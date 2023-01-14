using BlueFramework.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DataBackService
{
    public class SqlServerDbBackImpl : IDbBack
    {

        string _serviceName;
        string[] _dbFiles;
        ILogger _logger;

        public SqlServerDbBackImpl(string name,string[] dbFiles)
        {
            _serviceName = name;
            _dbFiles = dbFiles; 
            _logger = LoggerFactory.CreateDefault();
        }

        public bool BackFiles(string backDir, string[] files)
        {
            if(files == null)
            {
                files = _dbFiles;
            }
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var path = Path.Combine(backDir, date);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach(var file in files)
            {
                var fileInfo = new FileInfo(file);
                var destFilePath = Path.Combine(path, fileInfo.Name);
                try
                {
                    fileInfo.CopyTo(destFilePath);
                    _logger.Info($"{file} back to {destFilePath}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"copy file to {destFilePath} : {ex.Message}");
                }
            }
            return true;
        }

        public bool StartService(string serviceName)
        {
            _logger.Info($"starting service {serviceName}");
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = _serviceName;
            }
            // 启动服务
            try
            {
                var svr = GetService(serviceName);
                if(svr != null)
                {
                    svr.Start();
                }
            }
            catch(Exception ex)
            {
                _logger.Info($"start {serviceName} throw ex : {ex.Message}");
                return false;
            }
            // 监听状态
            int i = 0;
            bool started = false;
            while (i < 600)
            {
                var svr = GetService(serviceName);
                if (svr.Status == ServiceControllerStatus.Running)
                {
                    started = true;
                    break;
                }
                i++;
                System.Threading.Thread.Sleep(1000);
            }
            _logger.Info($"started {serviceName} :{started}");
            return started?true:false;
        }

        public bool StopService(string serviceName)
        {
            _logger.Info($"stopping service {serviceName}");
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = _serviceName;
            }
            // 停止服务
            try
            {
                var svr = GetService(serviceName);
                if (svr != null)
                {
                    if (svr.Status == ServiceControllerStatus.Stopped)
                    {
                        _logger.Info($"{serviceName} unstart");
                        return true;
                    }
                    if (svr.CanStop)
                        svr.Stop();
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // 未找到指定的服务
                    _logger.Info($"can't find {serviceName}");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Info($"stop {serviceName} throw ex:{ex.Message}");
                return false;
            }
            // 监听服务状态
            int i = 0;
            bool stoped = false;
            while (i < 600)
            {
                var svr = GetService(serviceName);
                if (svr.Status == ServiceControllerStatus.Stopped)
                {
                    stoped = true;
                    break;
                }
                i++;
                System.Threading.Thread.Sleep(1000);
            }
            _logger.Info($"stopped {serviceName} : {stoped}");
            return stoped ? true : false;
        }

        private ServiceController GetService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var svr = services.FirstOrDefault(x => x.ServiceName.Equals(serviceName));
            return svr;
        }
    }
}
