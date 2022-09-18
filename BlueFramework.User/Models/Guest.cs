using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.User.Models
{
    public class Guest:IUser
    {
        public string UserName { get; set; }
    }
}
