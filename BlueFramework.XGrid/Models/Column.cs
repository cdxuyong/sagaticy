using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.XGrid.Models
{
    public class Column
    {
        private string name;
        private string caption;

        public string Name { get => name; set => name = value; }
        public string Caption { get => caption; set => caption = value; }
    }
}
