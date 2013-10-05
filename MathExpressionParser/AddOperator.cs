using System.Linq.Expressions;

namespace MathExpressionParser
{
    internal sealed class AddOperator : IBinaryOperator
    {
        public int Precedence
        {
            get { return 10; }
        }

        public string Operator
        {
            get { return "+"; }
        }

        public Expression GetExpression(Expression left, Expression right)
        {
            return Expression.Add(left, right);
        }
    }
}