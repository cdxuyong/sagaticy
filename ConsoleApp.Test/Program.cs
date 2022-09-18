using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Test
{
    class Program
    {
        static string queuepath = @".\private$\myQueue";


        static void Main(string[] args)
        {
            async_test();

            Console.WriteLine("end");
            Console.ReadLine();
        }

        static async void async_test()
        {
            string x = await async_test_read();
            Console.WriteLine(x);
            Console.WriteLine("async_test end");
        }
        static async Task<string> async_test_read()
        {
            Console.WriteLine("start...");
            string xy = string.Empty;
            await Task.Run(() => {
                Thread.Sleep(2000);
                xy = "just do it";
            });


            return xy;
            
        }


        static void initMsMQ()
        {
            Task task = new Task(createFactory);
            task.Start();

            Task taskConsume = new Task(createConsume);
            taskConsume.Start();

            Console.WriteLine("write message");
            while (true)
            {
                string line = Console.ReadLine();
                if ("exit" == line)
                    break;

                MessageQueue mq = new MessageQueue(queuepath);
                System.Messaging.Message myMessage = new System.Messaging.Message();
                myMessage.Body = line;
                myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(System.String) });
                mq.Send(line);

            }
        }

        static void createFactory()
        {
            if (!MessageQueue.Exists(queuepath))
            {
                MessageQueue.Create(queuepath);
            }

        }

        static void createConsume()
        {
            MessageQueue mq = new MessageQueue(queuepath);
            mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(System.String) });
            while (true)
            {
                Message message = mq.Receive();
                if (message != null)
                {
                    string body = (string)message.Body;
                    Console.WriteLine("the bodys = " + body.ToString());
                }
            }

        }
    }
}
