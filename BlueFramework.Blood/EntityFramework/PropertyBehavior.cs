using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BlueFramework.Blood.EntityFramework
{
    /// <summary>
    /// the behavior of object's <see cref="System.Reflection.PropertyInfo"/>
    /// </summary>
    public class PropertyBehavior
    {
        public PropertyInfo Property { get; set; }

        public BehaviorType Behavior { get; set; }

        public PropertyBehavior(PropertyInfo property)
        {
            this.Property = property;
            this.Behavior = BehaviorType.Default;
        }
    }
}
