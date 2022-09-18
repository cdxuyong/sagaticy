using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlueFramework.Common
{
    /// <summary>
    /// 对象辅助类
    /// 提供对象和类的常用方法
    /// </summary>
    public class ObjectHelper
    {
        /// <summary>
        /// 通过序列化和反序列化深度克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Clone<T>()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;
            return (T)formatter.Deserialize(memoryStream);
        }


        public static object ReflectObject(string assemblyName, string objectType)
        {
            string message;
            object o = ReflectObject(assemblyName, objectType,null,out message);
            return o;
        }

        public static object ReflectObject(string assemblyName,string objectType,object[] arguments,out string message)
        {
            message = string.Empty;
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                Type type = assembly.GetType(objectType);
                object o = System.Activator.CreateInstance(type, arguments);
                return o;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }

        }

    }


}
