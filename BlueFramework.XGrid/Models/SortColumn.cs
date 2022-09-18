using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.XGrid.Models
{
    public class SortColumn:Column
    {
        private string orderType;

        public string OrderType { get => orderType; set => orderType = value; }
    }
}
