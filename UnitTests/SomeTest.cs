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
using NUnit.Framework;

namespace Langman.MathExpressionParser
{
    [TestFixture]
    public class SomeTest
    {
        [Test]
        public void Empty()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();

            Func<double> x = parser.Parse("");
            Assert.True(x() == 0);

            x = parser.Parse("   ");
            Assert.True(x() == 0);

            x = parser.Parse(" \r\n\t  ");
            Assert.True(x() == 0);
        }

        [Test]
        public void Constant()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();

            Func<double> x = parser.Parse("0");
            Assert.True(x() == 0);

            x = parser.Parse("-999");
            Assert.True(x() == -999);

            x = parser.Parse(" 999  \t   ");
            Assert.True(x() == 999);

            x = parser.Parse(" -999.123  \t   ");
            Assert.True(x() == -999.123);

            x = parser.Parse(" 999.1  \t   ");
            Assert.True(x() == 999.1);

            x = parser.Parse(" \r\n 1.999  \t   ");
            Assert.True(x() == 1.999);

        }

        [Test]
        public void Equality()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 == 1");
            Assert.True(x() == (2 == 1));

            x = parser.Parse("2== 1+1");
            Assert.True(x() == (2 == 1+1));

            x = parser.Parse("2== (1+1-1)+1-1");
            Assert.True(x() == (2 == (1 + 1 - 1) + 1 - 1));
        }

        [Test]
        public void LessThan()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 < 1");
            Assert.True(x() == (2 < 1));

            x = parser.Parse("2 < 2");
            Assert.True(x() == false);

            x = parser.Parse("2 < 3");
            Assert.True(x() == true);

            x = parser.Parse("-1 <-6");
            Assert.True(x() == false);

            x = parser.Parse("-10.0001 <-10.00");
            Assert.True(x() == true);

            x = parser.Parse("10.0001 <-10.00");
            Assert.True(x() == false);
        }

        [Test]
        public void GreaterThan()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 > 1");
            Assert.True(x() == (2 > 1));

            x = parser.Parse("2 > 2");
            Assert.True(x() == false);

            x = parser.Parse("2 > 3");
            Assert.True(x() == false);

            x = parser.Parse("-1 >-6");
            Assert.True(x() == -1 > -6);

            x = parser.Parse("-10.0001 >-10.00");
            Assert.True(x() == false);

            x = parser.Parse("10.0001 >-10.00");
            Assert.True(x() == 10.0001 > -10.00);
        }

        [Test]
        public void LogicalAnd()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 > 1");
            Assert.True(x() == (2 > 1));

            x = parser.Parse("2 > 2");
            Assert.True(x() == false);

            x = parser.Parse("2 > 3");
            Assert.True(x() == false);

            x = parser.Parse("-1 >-6");
            Assert.True(x() == -1 > -6);

            x = parser.Parse("-10.0001 >-10.00");
            Assert.True(x() == false);

            x = parser.Parse("10.0001 >-10.00");
            Assert.True(x() == 10.0001 > -10.00);
        }

        [Test]
        public void LessThanOrEqualTo()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 <= 1");
            Assert.True(x() == (2 <= 1));

            x = parser.Parse("2 <= 2");
            Assert.True(x() == 2 <= 2);

            x = parser.Parse("2 <= 3");
            Assert.True(x() == 2 <= 3);

            x = parser.Parse("-1 <=-6");
            Assert.True(x() == -1 <= -6);

            x = parser.Parse("-10.0001 <=-10.00");
            Assert.True(x() == -10.0001 <= -10.00);

            x = parser.Parse("10.0001 <=-10.00");
            Assert.True(x() == 10.0001 <= -10.00);

            x = parser.Parse("0 <=-0");
            Assert.True(x() == 0 <= -0);
        }


        [Test]
        public void GreaterThanOrEqualTo()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("2 >= 1");
            Assert.True(x() == (2 >= 1));

            x = parser.Parse("2 >= 2");
            Assert.True(x() == 2 >= 2);

            x = parser.Parse("2 >= 3");
            Assert.True(x() == 2 >= 3);

            x = parser.Parse("-1 >=-6");
            Assert.True(x() == -1 >= -6);

            x = parser.Parse("-10.0001 >=-10.00");
            Assert.True(x() == -10.0001 >= -10.00);

            x = parser.Parse("10.0001 >=-10.00");
            Assert.True(x() == 10.0001 >= -10.00);

            x = parser.Parse("0 >=-0");
            Assert.True(x() == 0 >= -0);
        }

        [Test]
        public void BooleanLogic()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();

            var x = parser.Parse("false || true && false || true && true && true || false");
            Assert.True(x() == (false || true && false || true && true && true || false));

            x = parser.Parse("(false || true) && false || (true && (true)) && true || false");
            Assert.True(x() == ((false || true) && false || (true && (true)) && true || false));

            x = parser.Parse("(1 < 2 + 3 == 4 < 5 || true) && false || (1 < (2 + 3) == 4 < 5 && (true)) &&  1 < 2*4 + 3 == (4 + 5) < 6 == 7 < 9");
            Assert.True(x() == ((1 < 2 + 3 == 4 < 5 || true) && false || (1 < (2 + 3) == 4 < 5 && (true)) &&  1 < 2*4 + 3 == (4 + 5) < 6 == 7 < 9));
        }



        [Test]
        public void Add()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();

            Func<double> x = parser.Parse( "0 +   0 " );
            Assert.True(x() == 0);

            x = parser.Parse("1 + 2 + -3");
            Assert.True(x() == 1+2+ -3);

            x = parser.Parse(" 1.2 + -4.6 + 8.88   ");
            Assert.True(x() == 1.2 + -4.6 + 8.88);

            x = parser.Parse(" 1 + 2 +  3 + 4+5+6+7+ 8+9  \t   ");
            Assert.True(x() == 1+2+3+4+5+6+7+8+9);

        }

        [Test]
        public void Subtract()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();


            Func<double> x = parser.Parse("1 - 2 - -3");
            double expected = 1 - 2 - -3;
            double result = x();
            Assert.True(expected == result);


             x = parser.Parse("0 +   0 ");
            Assert.True(x() == 0);

            x = parser.Parse(" 1.2 - -4.6 - 8.88   ");
            Assert.True(x() == 1.2 - -4.6 - 8.88);

            x = parser.Parse(" 1 + 2 -  3 + 4+5+6+7+ 8+9  \t   ");
            Assert.True(x() == 1 + 2 - 3 + 4 + 5 + 6 + 7 + 8 + 9);

        }

        [Test]
        public void Multiply()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();


            Func<double> x = parser.Parse("1 * 2 * -3");
            double expected = 1 * 2 * -3;
            double result = x();
            Assert.True(expected == result);


            x = parser.Parse("0 *   0 ");
            Assert.True(x() == 0);

            x = parser.Parse(" 1.2 * -4.6 * 8.88   ");
            Assert.True(x() == 1.2 * -4.6 * 8.88);

            x = parser.Parse(" 1 * 2 *  3 * 4*5*6*7* 8*9  \t   ");
            Assert.True(x() == 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9);

        }

        [Test]
        public void Divide()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();


            Func<double> x = parser.Parse("1 / 2 / -3");
            double expected = 1d / 2d /-3d;
            double result = x();
            Assert.True(expected == result);


            x = parser.Parse("0 / 0");
            expected = 0d/0d;
            result = x();
            Assert.True(double.IsNaN(expected) && double.IsNaN(result));

            x = parser.Parse(" 1.2 /-4.6 / 8.88   ");
            Assert.True(x() == 1.2d / -4.6d / 8.88d);

            x = parser.Parse(" 1 / 2 /  3 / 4/5/6/7/ 8/9  \t   ");
            Assert.True(x() == 1d / 2d / 3d / 4d / 5d / 6d / 7d / 8d / 9d);
        }

        [Test]
        public void Mixed()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();


            Func<double> x = parser.Parse("2 + 3 * 4");
            double expected = 2d + 3d * 4d;
            double result = x();
            Assert.True(expected == result);

            x = parser.Parse("   1* 2 + 3 + 4 - -5 /-2 + 6 *4+-3- -2-1");
            expected = 1d* 2d + 3d + 4d - -5d /-2d + 6d *4d+-3d- -2d-1d;
            result = x();
            Assert.True(expected == result);

        }

        [Test]
        public void Groups()
        {
            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();


            Func<double> x = parser.Parse("2 * (3 + 4)");
            double expected = 2d * (3d + 4d);
            double result = x();
            Assert.True(expected == result);

            x = parser.Parse("(((2+3)*4+(((1))))*5*(6+7)*(8*(9+10)))");
            expected = (((2d+3d)*4d+(((1d))))*5d*(6d+7d)*(8d*(9d+10d)));
            result = x();
            Assert.True(expected == result);

        }

        [Test]
        public void ParseExceptions()
        {

            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser();

            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "2 + 3 * "));
            Assert.Throws<ArgumentNullException>(ParsePrintThrow(parser, null));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "~"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "2 + 2.2.2"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, " + 2"));


        }

        [Test]
        public void ParseExceptionsB()
        {
            ExpressionParser<bool> parser = ExpressionParser.Factory.CreateBooleanLogicParser();
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 == 2 == 3"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 == "));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 == true"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "== true"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "false + 1"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "false <= true"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 <= 2 && 3"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 <= 2 || 3"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1 <= (2 || 3)"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, "1"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrowB(parser, ""));
        }

        [Test]
        public void StringFuncs()
        {
            ParserContext context = new ParserContext();
            context.AddStringFunction(new StringFunction("M", M));

            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser(context);



            Func<double> x = parser.Parse("M(a) + M(b)  ");
            double expected = 1d+2d;
            double result = x();
            Assert.True(expected == result);

            x = parser.Parse("(2 * ( M(a) * ((M  ( b)) +M(c))))");
            expected = (2 * ( M("a") * ((M  ( "b")) +M("c"))));
            result = x();
            Assert.True(expected == result);

            
        }

        [Test]
        public void StringFuncs2()
        {
            ParserContext context = new ParserContext(stringComparer: StringComparer.OrdinalIgnoreCase);
            context.AddStringFunction(new StringFunction("M", M));

            ExpressionParser<double> parser = ExpressionParser.Factory.CreateMathParser(context);



            Func<double> x = parser.Parse("m(a) + M(b)  ");
            double expected = 1d + 2d;
            double result = x();
            Assert.True(expected == result);

            x = parser.Parse("(2 * ( M(a) * ((m  ( b)) +m(c))))");
            expected = (2 * (M("a") * ((M("b")) + M("c"))));
            result = x();
            Assert.True(expected == result);


        }

        [Test]
        public void DotAccessor1()
        {
            ParserContext context = new ParserContext(stringComparer: StringComparer.OrdinalIgnoreCase);

            ExpressionParser<Record, double> parser = ExpressionParser.Factory.CreateMathParser(new ParamDescriptor<Record,double>("Rx", (r,s) => r.Method(s)), context);

            var record = new Record();

            Func<Record, double> x = parser.Parse("Rx.A + rx.b  ");
            double expected = 2d + 3d;
            double result = x(record);
            Assert.True(expected == result);



            x = parser.Parse("(2 * ( rx.A * ((rx.B) +rx.A)))");
            expected = (2 * (record.Method("A") * ((record.Method("b")) + record.Method("A"))));
            result = x(record);
            Assert.True(expected == result);


        }

        Dictionary<string, double> _dict = new Dictionary<string, double> { { "a", 1d }, { "b", 2d }, { "c", 3d }, { "d", 4d }, { "e", 5d }, };

        private double M(string s)
        {
            return _dict[s];
        }

        private TestDelegate ParsePrintThrow(ExpressionParser<double> parser, string s)
        {
            Console.Write("\""+s+"\": ");
            return () =>
                {
                    try
                    {
                        parser.Parse(s);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }                    
                };
        }

        private TestDelegate ParsePrintThrowB(ExpressionParser<bool> parser, string s)
        {
            Console.Write("\"" + s + "\": ");
            return () =>
            {
                try
                {
                    var x = parser.Parse(s);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            };
        }
    }

    internal class Record
    {
        public double Method(string param)
        {
            if (param == "A")
                return 2;
            else
                return 3;
        }
    }
}
