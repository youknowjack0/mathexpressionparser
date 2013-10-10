using System;

namespace Langman.MathExpressionParser
{
    public class ExpressionParseException : Exception
    {        
        public int Character { get; set; }
        public string Token { get; set; }

        public ExpressionParseException(string message, int character, string token) : base(message)
        {
            Character = character;
            Token = token;
        }

        public ExpressionParseException(string message, int character, string token, Exception innerException)
            : base(message, innerException)
        {
            Character = character;
            Token = token;
        }
    }
}