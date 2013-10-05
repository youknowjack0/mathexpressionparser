using System;
using System.Globalization;

namespace MathExpressionParser
{
    public class ParserContext
    {
        private NumberFormatInfo _numberFormat;

        public ParserContext(CultureInfo culture)
        {
            Culture = culture;
            NumberFormat = NumberFormatInfo.GetInstance(culture);
        }

        public ParserContext()
        {
            Culture = CultureInfo.CurrentCulture;
            NumberFormat = NumberFormatInfo.GetInstance(Culture);
        }

        public CultureInfo Culture { get; set; }
        
        public NumberFormatInfo NumberFormat
        {
            get { return _numberFormat; }
            set { 
                _numberFormat = value;
                DecimalSeparator = Convert.ToChar(_numberFormat.NumberDecimalSeparator);
            }
        }

        internal char DecimalSeparator { get; set; }
    }
}