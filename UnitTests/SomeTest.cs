using System;
using System.Linq.Expressions;
using MathExpressionParser;
using NUnit.Framework;


namespace UnitTests
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
    }
}
