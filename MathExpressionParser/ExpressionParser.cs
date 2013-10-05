using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MathExpressionParser
{
    public class ExpressionParser
    {
        private readonly IBinaryOperator[] _allOperators;
        private readonly ParserContext _context;


        public ExpressionParser(ParserContext context = null)
        {
            _allOperators = AppDomain.CurrentDomain.GetAssemblies()
                                     .SelectMany(s => s.GetTypes())
                                     .Where(p => typeof (IBinaryOperator).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                                     .Select(Activator.CreateInstance)
                                     .Cast<IBinaryOperator>()
                                     .ToArray();

            _context = context ?? new ParserContext();
        }

        public Func<double> Parse(string mathExpression)
        {
            var exp = ParseInternal(mathExpression, 0, mathExpression.Length);
            Debug.WriteLine(mathExpression);
            Debug.WriteLine(exp.ToString());
            return Expression
                .Lambda<Func<double>>(exp)
                .Compile();
        }

        private Expression ParseInternal(string e, int o1, int o2)
        {
            var operandStack = new Stack<Expression>();
            var operatorStack = new Stack<IBinaryOperator>();

            IBinaryOperator @operator;
            Expression operand;

            if (TryReadOperand(e, ref o1, o2, out operand))
                operandStack.Push(operand);

            for (;;)
            {
                if (TryReadOperator(e, ref o1, o2, out @operator))
                {
                    if (operatorStack.Count > 0 && @operator.Precedence >= operatorStack.Peek().Precedence)
                        operandStack.Push(ProcessStacks(operandStack, operatorStack));
                    operatorStack.Push(@operator);
                }
                else
                    break;

                if (!TryReadOperand(e, ref o1, o2, out operand))
                    throw new InvalidOperationException();
                operandStack.Push(operand);
            }

            return ProcessStacks(operandStack, operatorStack);
        }

        private Expression ProcessStacks(Stack<Expression> operandStack, Stack<IBinaryOperator> operatorStack)
        {
            if (operandStack.Count == 0 && operatorStack.Count == 0)
                return Expression.Constant(0d);

            if (operandStack.Count - operatorStack.Count != 1)
                throw new InvalidOperationException();



            Expression operand1 = operandStack.Pop();

            while (operatorStack.Count > 0)
            {
                Expression operand2 = operandStack.Pop();
                IBinaryOperator @operator = operatorStack.Pop();
                operand1 = @operator.GetExpression(operand2, operand1);
            }

            return operand1;
        }

        private bool TryReadOperand(string s, ref int o1, int o2, out Expression operand)
        {
            SkipWhitespace(s, ref o1, o2);
            if (o1 == o2)
            {
                operand = null;
                return false;
            }

            if (char.IsLetter(s[o1]) || s[o1] == '_')
            {
                operand = ParseVariable(s, ref o1, o2);
                return true;
            }

            if (char.IsNumber(s[o1]) || s[o1] == '-' || s[o1] == _context.DecimalSeparator)
            {
                operand = ParseConstant(s, ref o1, o2);
                return true;
            }

            if (s[o1] == '(')
            {
                operand = ParseGroup(s, ref o1, o2);
                return true;
            }

            throw new InvalidOperationException();
        }

        private Expression ParseGroup(string s, ref int o1, int o2)
        {
            throw new NotImplementedException();
        }

        private Expression ParseVariable(string s, ref int o1, int o2)
        {
            int i;
            for (i = o1; i < o2 && char.IsLetterOrDigit(s[o1]) || s[01] == '_'; i++) ;
            o1 = i;
            return Expression.Variable(typeof (double), s.Substring(o1, i - o1));
        }

        private Expression ParseConstant(string s, ref int o1, int o2)
        {
            int i;
            i = o1;
            if (s[o1] == '-')
                i++;
            for (; i < o2 && (char.IsDigit(s[i]) || s[i] == _context.DecimalSeparator); i++) ;
            double d;
            string sub = s.Substring(o1, i - o1);
            if (!double.TryParse(sub, out d))
            {
                throw new InvalidOperationException("can't parse number " + sub);
            }
            o1 = i;
            return Expression.Constant(d);
        }

        private bool TryReadOperator(string s, ref int o1, int o2, out IBinaryOperator @operator)
        {
            //todo: trie or something
            //this is obviously terrible

            SkipWhitespace(s, ref o1, o2);

            if (o1 == o2)
            {
                @operator = null;
                return false;
            }

            foreach (IBinaryOperator op in _allOperators)
            {
                if (o2 - o1 < op.Operator.Length)
                    continue;

                if (s.Substring(o1, op.Operator.Length) == op.Operator)
                {
                    @operator = op;
                    o1 += op.Operator.Length;
                    return true;
                }
            }

            throw new InvalidOperationException();
        }


        private static void SkipWhitespace(string s, ref int o1, int o2)
        {
            if (o1 > o2)
                throw new InvalidOperationException();


            for (; o1 < o2 && Char.IsWhiteSpace(s[o1]); o1++) ;
        }
    }
}