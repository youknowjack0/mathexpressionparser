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
using System.Reflection;

namespace Langman.MathExpressionParser
{
    public class ExpressionParser<TResult>
    {
        

        private readonly IBinaryOperator[] _allOperators;
        private readonly ParserContext _context;
        private readonly char[] _operatorChars;
        private readonly Dictionary<string, IBinaryOperator> _operatorDictionary = new Dictionary<string, IBinaryOperator>();
        private bool _allowBoolConstants = false;

        protected ParamDescriptor[] Params { get; private set; }

        internal ExpressionParser(Type[] allowableTypes, ParserContext context = null, ParamDescriptor[] @params = null)
        {

            if (@params != null && context != null)
            {
                if (@params.Any(x => context.StringFunctions.ContainsKey(x.Token)))
                    throw new ArgumentException("parameter has same name as a string function");
            }

            _allOperators = GetAllOperatorsByReflection(allowableTypes);

            Params = @params;

            if (allowableTypes.Contains(typeof (bool)))
                _allowBoolConstants = true;

            _operatorChars = _allOperators
                                    .SelectMany(x => x.Operator.ToCharArray())
                                    .Distinct()
                                    .ToArray();

            foreach (var item in _allOperators)
                _operatorDictionary.Add(item.Operator, item);

            Array.Sort(_operatorChars);

            _context = context ?? new ParserContext();
        }

        /// <summary>
        /// todo allow override
        /// </summary>
        private IBinaryOperator[] GetAllOperatorsByReflection(Type[] allowableTypes)
        {
            return Assembly.GetAssembly(this.GetType())
                            .GetTypes()
                            .Where(p => typeof (IBinaryOperator).IsAssignableFrom (p) && p.IsClass && !p.IsAbstract)
                            .Select(Activator.CreateInstance)
                            .Cast<IBinaryOperator>()
                            .ToArray();
        }


        public Func<TResult> Parse(string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var exp = ParseInternal(expression, 0, expression.Length);
            if (exp.Type != typeof (TResult))
                throw new ExpressionParseException(
                    String.Format("Not a {0} expression", typeof (TResult).Name), -1, "");
            return Expression
                .Lambda<Func<TResult>>(exp)
                .Compile();
        }
        
        public Expression ParseToExpression(string expression, object[] @params = null)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            var exp = ParseInternal(expression, 0, expression.Length);
            return exp;
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
                        operandStack.Push(ProcessStacks(operandStack, operatorStack, @operator.Precedence));
                    operatorStack.Push(@operator);
                }
                else
                    break;

                if (!TryReadOperand(e, ref o1, o2, out operand))
                    throw new ExpressionParseException(string.Format("Expected an operand at character {0} on the right side of {1}", o1, @operator.Operator), o1, "");
                operandStack.Push(operand);
            }

            return ProcessStacks(operandStack, operatorStack, int.MaxValue);
        }

        private Expression ProcessStacks(Stack<Expression> operandStack, Stack<IBinaryOperator> operatorStack, int nextOperatorPrecedence)
        {
            if (operandStack.Count == 0 && operatorStack.Count == 0)
                return Expression.Constant(0d);

            if (operandStack.Count - operatorStack.Count != 1)
                throw new InvalidOperationException();



            Expression operand1 = operandStack.Pop();

            while (operatorStack.Count > 0 && operatorStack.Peek().Precedence <= nextOperatorPrecedence)
            {
                Expression operand2 = operandStack.Pop();
                IBinaryOperator @operator = operatorStack.Pop();                
                try
                {
                    operand1 = @operator.GetExpression(operand2, operand1);
                }
                catch (InvalidOperationException ex)
                {
                    throw new ExpressionParseException(ex.Message, -1, @operator.Operator, ex);
                }
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
                operand = ParseVariableOrFunction(s, ref o1, o2);
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

        private Expression ParseVariableOrFunction(string s, ref int o1, int o2)
        {
            string name = ParseAlphaNumericToken(s, ref o1, o2);

            StringFunction func;
            ParamDescriptor pdc;
            if (_allowBoolConstants && string.Equals(name, "true", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(true);
            }
            else if (_allowBoolConstants && string.Equals(name, "false", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(false);
            }
            else if (_context.StringFunctions.TryGetValue(name, out func))
            {
                if (func == null)
                    throw new InvalidOperationException("A null function was provided");

                string token = ParseGroupToken(s, ref o1, o2);

                //validate token
                if(func.Validator != null && !func.Validator(token))
                    throw new ExpressionParseException(string.Format("An invalid function token '{0}' was provided for function '{1}", token,func.FunctionName), o1, token);

                Expression<Func<double>> expr = () => func.Func(token);
                return Expression.Invoke(expr, null);
            }
            else if ((pdc = Params.FirstOrDefault(x => x.Token.Equals(name, x.Comparison))) != null)
            {
                Skip(".",s, ref o1, o2);
                return pdc.Resolve(ParseAlphaNumericToken(s, ref o1, o2));
            }
            else
            {
                throw new ExpressionParseException(string.Format("Function name \"{0}\" not recognized",name),o1,name);
            }
        }

        private string ParseAlphaNumericToken(string s, ref int o1, int o2)
        {
            int i;
            for (i = o1; i < o2 && (char.IsLetterOrDigit(s[i]) || s[i] == '_'); i++) ;

            string name = s.Substring(o1, i - o1);
            o1 = i;
            return name;
        }

        private void Skip(string skipstring, string s, ref int o1, int o2)
        {
            int original = o1;

            int i;
            for (i = 0; o1 < o2 && i < skipstring.Length; o1++, i++)
            {
                if(!_context.CurrentStringComparer.Equals(skipstring.Substring(i,1), s.Substring(o1,1)))
                    throw new ExpressionParseException("Expected token '" + s + "' at position '" + original + "'", original, ".");
            }

            if(i != s.Length - 1)
                throw new ExpressionParseException("Expected token '" + s + "' at position '" + original + "'", original, ".");
        }

        private string ParseGroupToken(string s, ref int o1, int o2)
        {
            SkipWhitespace(s, ref o1, o2);

            if(s[o1] != '(')
                throw new ExpressionParseException(string.Format("Expected '(' at {0}",o1),o1,"");

            o1++;
            int close = FindGroupEnd(s, o1, o2);
            string token = s.Substring(o1, close - o1).Trim();

            o1 = close + 1;

            return token;
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

            SkipWhitespace(s, ref o1, o2);

            if (o1 == o2)
            {
                @operator = null;
                return false;
            }

            string token;
            int i;
            for (i = o1; i < o2 && IsOperatorCharacter(s[i]); i++);

            int len;
            while((len = i - o1) > 0)
            {
                token = s.Substring(o1, len);
                if (_operatorDictionary.TryGetValue(token, out @operator))
                {
                    o1 = i;
                    return true;
                }
                i--;
            }

            throw new ExpressionParseException(string.Format("Unrecognized operator  {0} at position {1}", s[o1], o1), o1, s[o1].ToString());
        }

        private bool IsOperatorCharacter(char c)
        {
            if (Array.BinarySearch(_operatorChars, c) >= 0)
                return true;
            return false;
        }


        private static void SkipWhitespace(string s, ref int o1, int o2)
        {
            if (o1 > o2)
                throw new InvalidOperationException();


            for (; o1 < o2 && Char.IsWhiteSpace(s[o1]); o1++) ;
        }
    }

    public class ExpressionParser<T1, TResult> : ExpressionParser<TResult>
    {
        private ParamDescriptor<T1> _param1;

        internal ExpressionParser(Type[] allowableTypes, ParamDescriptor<T1> p1, ParserContext context = null) 
            : base(allowableTypes, context, new ParamDescriptor[]{p1})
        {
            if(p1 == null)
                throw new ArgumentNullException("p1");
        }

        public Func<T1, TResult> Parse(string expression, T1 param1)
        {
            Expression exp = base.ParseToExpression(expression, new object[] {param1});
            return Expression.Lambda<Func<T1, TResult>>(exp).Compile();
        }
    }

    public class ExpressionParser<T1, T2, TResult> : ExpressionParser<TResult>
    {
        internal ExpressionParser(Type[] allowableTypes, ParamDescriptor<T1> p1, ParamDescriptor<T2> p2, ParserContext context = null)
            : base(allowableTypes, context, new ParamDescriptor[]{p1,p2})
        {
            if (p1 == null)
                throw new ArgumentNullException("p1");
            if (p2 == null)
                throw new ArgumentNullException("p2");
        }

        public Func<T1, T2, TResult> Parse(string expression, T1 param1, T2 param2)
        {
            Expression exp = base.ParseToExpression(expression, new object[] { param1 , param2 });
            return Expression.Lambda<Func<T1, T2, TResult>>(exp).Compile();
        }
    }
}