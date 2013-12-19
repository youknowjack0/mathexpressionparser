using System;

namespace Langman.MathExpressionParser
{
    public class StringFunction
    {
        private readonly string _functionName;
        private readonly Func<string, double> _func;
        private readonly Func<string, bool> _validator;

        public StringFunction(string functionName, Func<string, double> func, Func<string, bool> validator = null)
        {
            _functionName = functionName;
            _func = func;
            _validator = validator;
        }

        public string FunctionName
        {
            get { return _functionName; }
        }

        public Func<string, double> Func
        {
            get { return _func; }
        }

        public Func<string, bool> Validator
        {
            get { return _validator; }
        }
    }
}