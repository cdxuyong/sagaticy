using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.XGrid.Models
{
    public class ConvergeColumn:Column
    {
        private string formula;

        public string Formula { get => formula; set => formula = value; }
    }
}
