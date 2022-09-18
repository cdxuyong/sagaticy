using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.XGrid.Models
{
    public class Parameter
    {
        private string parameterName;
        private object parameterValue;
        private string parameterType;

        public string ParameterName { get => parameterName; set => parameterName = value; }
        public object ParameterValue { get => parameterValue; set => parameterValue = value; }
        public string ParameterType { get => parameterType; set => parameterType = value; }
    }
}
