﻿/*
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
using System.Globalization;

namespace Langman.MathExpressionParser
{
    public class ParserContext
    {
        private readonly IEqualityComparer<string> _stringComparer;
        private NumberFormatInfo _numberFormat;
        private readonly Dictionary<string, StringFunction> _stringFunctions;

        public ParserContext(CultureInfo culture = null, IEqualityComparer<string> stringComparer = null )
        {
            _stringComparer = stringComparer;
            Culture = culture ?? CultureInfo.CurrentCulture;
            _stringFunctions = new Dictionary<string, StringFunction>(stringComparer ?? StringComparer.CurrentCulture);
            NumberFormat = NumberFormatInfo.GetInstance(culture);
            
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

        /// <summary>
        /// String functions will be passed whatever is inside the parentheses exactly as a string (trimmed)
        /// </summary>
        public void AddStringFunction(StringFunction func)
        {
            _stringFunctions.Add(func.FunctionName,func);
        }

        public void ClearStringFunctions(StringFunction func)
        {
            _stringFunctions.Clear();
        }



        internal Dictionary<string, StringFunction> StringFunctions { get { return _stringFunctions; } }

        public IEqualityComparer<string> CurrentStringComparer
        {
            get { return _stringComparer; }
        }
    }
}