using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueFramework.Blood.DataAccess
{
    public class CommandParameter
    {
        private string _parameterName;
        private object _parameterValue;
        private Type _parameterType;

        public string ParameterName { get => _parameterName; set => _parameterName = value; }
        public object ParameterValue { get => _parameterValue; set => _parameterValue = value; }
        public Type ParameterType { get => _parameterType; set => _parameterType = value; }

        public CommandParameter()
        {

        }

        public CommandParameter(string parameterName, object parameterValue)
        {
            Type inputType = null;
            if (parameterValue != null)
            {
                inputType = parameterValue.GetType();
            }
            initialize(parameterName, parameterValue, inputType);
        }
        public CommandParameter(string parameterName, object parameterValue,Type type)
        {
            initialize(parameterName, parameterValue, type);
        }

        void initialize(string parameterName, object parameterValue,Type type)
        {
            _parameterName = parameterName;
            _parameterValue = parameterValue;
            _parameterType = type;
        }
    }
}
