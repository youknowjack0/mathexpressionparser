/*
Copyright 2013 Jack Langman
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using MathExpressionParser;

namespace Langman.MathExpressionParser
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
            if (mathExpression == null) throw new ArgumentNullException("mathExpression");
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
                    throw new ExpressionParseException(string.Format("Expected an operand at character {0} on the right side of {1}", o1, @operator.Operator), o1, "");
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

            throw new ExpressionParseException(string.Format("Character {0} at position {1} is not a valid operand starting character", s[o1], o1), o1, s[o1].ToString());
        }

        private Expression ParseGroup(string s, ref int o1, int o2)
        {
            o1++;
            int close = FindGroupEnd(s, o1, o2);
            var result = ParseInternal(s, o1, close);
            o1 = close+1;
            return result;
        }

        private int FindGroupEnd(string s, int o1, int o2)
        {
            int depth = 1;
            for (int i = o1; i < o2; i++)
            {
                if (s[i] == '(')
                    depth++;
                else if (s[i] == ')')
                {
                    if (--depth == 0)
                    {
                        return i;
                    }
                    
                }
            }
            throw new ExpressionParseException("Closing ')' expected for '(' at position " + (o1 - 1), o1 - 1, "(");
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
                throw new ExpressionParseException(string.Format("Error parsing number \"{0}\" at position {1} ",sub,s[o1]), o1, sub);
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

            throw new ExpressionParseException(string.Format("Unrecognized operator \"{0}\" at position {1}",s[o1], o1), o1,s[o1].ToString());
        }


        private static void SkipWhitespace(string s, ref int o1, int o2)
        {
            if (o1 > o2)
                throw new InvalidOperationException();


            for (; o1 < o2 && Char.IsWhiteSpace(s[o1]); o1++) ;
        }
    }
}