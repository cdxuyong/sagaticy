using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.EntityFramework
{
    public abstract class BaseObject:Object,ICloneable
    {
        /// <summary>
        /// 对象浅克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
