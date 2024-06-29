using DPC.Importor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<C4EventInfo> c4EventInfos = new List<C4EventInfo>();
            for(int i = 0; i < 10000; i++)
            {
                // 循环随机填充C4EventInfo对象
                C4EventInfo c4EventInfo = new C4EventInfo();
                c4EventInfo.EventName = "EventName" + i;
                c4EventInfo.Path = "\\202406120907_大秦线_下行_下庄-茶坞_阳原站\\北辛堡-延庆\\K253008_49" + i;
                c4EventInfo.SamplledTime = DateTime.Now.ToString();
                c4EventInfos.Add(c4EventInfo);

            }
            // 文件保存至D盘根目录，名称为Event.json
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(c4EventInfos);
            System.IO.File.WriteAllText("D:\\Event.json", json);
        }
    }
}
