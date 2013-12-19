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

namespace Langman.MathExpressionParser
{
    public static class ExpressionParser
    {
        public static class Factory
       {
           public static ExpressionParser<double> CreateMathParser(ParserContext context = null)
           {
               return new ExpressionParser<double>(new[]{typeof(double)}, context);
           }

           public static ExpressionParser<T1,double> CreateMathParser<T1>(ParamDescriptor<T1> param1, ParserContext context = null)
           {
               return new ExpressionParser<T1,double>(new[] { typeof(double) }, param1, context);
           }

           public static ExpressionParser<T1, T2, double> CreateMathParser<T1, T2>(ParamDescriptor<T1> param1, ParamDescriptor<T2> param2, ParserContext context = null)
           {
               return new ExpressionParser<T1, T2, double>(new[] { typeof(double) }, param1, param2, context);
           } 

           public static ExpressionParser<bool> CreateBooleanLogicParser(ParserContext context = null)
           {
               return new ExpressionParser<bool>(new[] { typeof(bool), typeof(double) }, context);
           }

           public static ExpressionParser<T1, bool> CreateBooleanLogicParser<T1>(ParamDescriptor<T1> param1, ParserContext context = null)
           {
               return new ExpressionParser<T1, bool>(new[] { typeof(bool) }, param1, context);
           }

           public static ExpressionParser<T1, T2, bool> CreateBooleanLogicParser<T1, T2>(ParamDescriptor<T1> param1, ParamDescriptor<T2> param2, ParserContext context = null)
           {
               return new ExpressionParser<T1, T2, bool>(new[] { typeof(bool) }, param1, param2, context);
           } 


           public static ExpressionParser<object> CreateParser(ParserContext context = null)
           {
               return new ExpressionParser<object>(new[] { typeof(bool), typeof(double) }, context);
           } 
       }
    }
}