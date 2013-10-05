using System.Linq.Expressions;

namespace MathExpressionParser
{
    internal sealed class DivideOperator : IBinaryOperator
    {
        public int Precedence
        {
            get { return 9; }
        }

        public string Operator
        {
            get { return "/"; }
        }

        public Expression GetExpression(Expression left, Expression right)
        {
            return Expression.Divide(left, right);
        }
    }
}