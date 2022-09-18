using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BlueFramework.Blood.EntityFramework
{
    public class ObjectProvider
    {
        static ObjectCollection objectCollection = new ObjectCollection();


        public static object MakeEntity(string assembly, string classType)
        {
            object o = objectCollection[classType];
            if (o == null)
            {
                o = BlueFramework.Common.ObjectHelper.ReflectObject(assembly, classType);
                EntityFramework.BaseObject entity = (BaseObject)o;
                objectCollection.AddObject(classType, entity);
            }
            return o;
        }
       
    }
}
