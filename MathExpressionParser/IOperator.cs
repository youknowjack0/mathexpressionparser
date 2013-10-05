namespace MathExpressionParser
{
    internal interface IOperator
    {
        int Precedence { get; }
        string Operator { get; }
    }
}
