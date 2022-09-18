using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Common.Excel
{
    public class TSheet
    {
        private string title;
        private string name;
        private THead head;

        public string Title { get => title; set => title = value; }
        public THead Head { get => head; set => head = value; }
        public string Name { get => name; set => name = value; }

        public TSheet()
        {
            head = new THead();
            title = "sheet1";
        }
    }
}
