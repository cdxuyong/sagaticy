using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.EntityFramework
{
    public class ObjectCollection
    {
        private Dictionary<string, BaseObject> objects = new Dictionary<string, BaseObject>();

        public BaseObject this[string classType]
        {
            get
            {
                return null;
            }
        }

        public void AddObject(string classType, BaseObject o)
        {
            if (!objects.ContainsKey(classType))
            {
                objects.Add(classType, o);
            }
        }
    }
}
