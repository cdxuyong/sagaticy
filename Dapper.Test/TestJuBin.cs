using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;
using System.Net;
using System.IO.MemoryMappedFiles;

namespace Dapper.Test
{
    public class TestJuBin
    {
        public class MyObj1
        {
            public Dictionary<string, object> list = new Dictionary<string, object>();

            /// <summary>
            /// /ddd
            /// </summary>
            public string Name { get; set; }

            public MyObj1()
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("AAAA");
                DataRow dr =  dt.NewRow();
                list.Add("AAAAAAA", dr);

                
            }


            public void openHttp()
            {
                try
                {
                    HttpWebRequest request = WebRequest.CreateHttp("http://www.baidu.com");

                    // Set some reasonable limits on resources used by this request
                    request.MaximumAutomaticRedirections = 4;
                    request.MaximumResponseHeadersLength = 4;
                    // Set credentials to use for this request.
                    request.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                    Console.WriteLine(readStream.ReadToEnd());

                    response.Close();
                    readStream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private static Dictionary<string, MyObj1> _list = new Dictionary<string, MyObj1>();
        static  AutoResetEvent  _autoRestEvent = new AutoResetEvent(true);

        public static void Test()
        {
            ThreadPool.SetMinThreads(50, 50);
            while (true)
            {
                CreateThreads();
                Thread.Sleep(100);
                try
                {
                    var keys = _list.Keys.ToArray();
                    if (System.DateTime.Now.Second == 59)
                    {
                        for (int i = 0; i < keys.Length; i++)
                            _list.Remove(keys[i]);
                    }


                }
                catch
                {

                }
            }
        } 

        private static void CreateThreads()
        {
            for(int i = 0; i < 100; i++)
            {
                string key = Guid.NewGuid().ToString();
                ThreadPool.QueueUserWorkItem(CreateThread,key);
            }
        }

        private static void CreateThread(object state)
        {
            _autoRestEvent.WaitOne();
            MyObj1 m1 = new MyObj1();
            string key = (string)state;
            try
            {

                _list.Add(key, m1);
                m1.openHttp();

            }
            catch
            {

            }
            finally
            {
                _autoRestEvent.Set();
            }
        }
    }
}
