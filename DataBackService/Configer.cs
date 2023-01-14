using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DataBackService
{
    internal class Configer
    {
        private List<string> _filePaths = new List<string>();
        public List<string> FilePaths
        {
            get
            {
                return _filePaths;
            }
        }
        private string _backDirectory;
        public string BackDirectory
        {
            get
            {
                return _backDirectory;
            }
        }

        private string _corn;
        public string Corn
        {
            get
            {
                return _corn;
            }
        }
        private List<string> _serviceNames = new List<string>();
        public List<string> ServiceNames
        {
            get
            {
                return _serviceNames;
            }
        }

        static Configer _Configer;
        public static Configer Current
        {
            get
            {
                return _Configer;
            }
        }

        public static void Init()
        {
            _Configer = new Configer();
            var appSettings = ConfigurationManager.AppSettings;
            var keys = appSettings.AllKeys;
            foreach (var key in keys)
            {
                var value = appSettings[key];
                if (key.StartsWith("services"))
                {
                    _Configer._serviceNames.Add(value);
                }
                if (key.StartsWith("files"))
                {
                    _Configer._filePaths.Add(value);
                }
                if ("backdir".Equals(key))
                {
                    _Configer._backDirectory = value;
                }
                if ("corn".Equals(key))
                {
                    _Configer._corn = value;
                }
            }
        }
    }
}
