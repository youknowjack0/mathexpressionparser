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
using MathExpressionParser;
using NUnit.Framework;

namespace Langman.MathExpressionParser
{
    [TestFixture]
    public class SomeTest
    {
        [Test]
        public void Empty()
        {
            ExpressionParser parser = new ExpressionParser();

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
            ExpressionParser parser = new ExpressionParser();

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
        public void Add()
        {
            ExpressionParser parser = new ExpressionParser();

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
            ExpressionParser parser = new ExpressionParser();


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
            ExpressionParser parser = new ExpressionParser();


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
            ExpressionParser parser = new ExpressionParser();


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
            ExpressionParser parser = new ExpressionParser();


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
            ExpressionParser parser = new ExpressionParser();


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
            
            ExpressionParser parser = new ExpressionParser();            

            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "2 + 3 * "));
            Assert.Throws<ArgumentNullException>(ParsePrintThrow(parser, null));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "~"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, "2 + 2.2.2"));
            Assert.Throws<ExpressionParseException>(ParsePrintThrow(parser, " + 2"));    

        }

        [Test]
        public void StringFuncs()
        {
            ParserContext context = new ParserContext();
            context.StringFunctions.Add("M", M);

            ExpressionParser parser = new ExpressionParser(context);



            Func<double> x = parser.Parse("M(a) + M(b)  ");
            double expected = 1d+2d;
            double result = x();
            Assert.True(expected == result);

            x = parser.Parse("(2 * ( M(a) * ((M  ( b)) +M(c))))");
            expected = (2 * ( M("a") * ((M  ( "b")) +M("c"))));
            result = x();
            Assert.True(expected == result);

        }

        Dictionary<string, double> _dict = new Dictionary<string, double> { { "a", 1d }, { "b", 2d }, { "c", 3d }, { "d", 4d }, { "e", 5d }, };

        private double M(string s)
        {
            return _dict[s];
        }

        private TestDelegate ParsePrintThrow(ExpressionParser parser, string s)
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
    }
}
