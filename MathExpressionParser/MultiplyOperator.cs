using System.Linq.Expressions;

namespace MathExpressionParser
{
    internal sealed class MultiplyOperator : IBinaryOperator
    {
        public int Precedence
        {
            get { return 9; }
        }

        public string Operator
        {
            get { return "*"; }
        }

        public Expression GetExpression(Expression left, Expression right)
        {
            return Expression.Multiply(left, right);
        }
    }
}