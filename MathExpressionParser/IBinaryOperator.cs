using System.Linq.Expressions;

namespace MathExpressionParser
{
    internal interface IBinaryOperator : IOperator
    {
        Expression GetExpression(Expression left, Expression right);
    }
}