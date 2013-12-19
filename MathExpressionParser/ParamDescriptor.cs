using System;
using System.Linq.Expressions;

namespace Langman.MathExpressionParser
{

    public class ParamDescriptor<TIn, TOut> : ParamDescriptor<TIn>
    {
        private readonly Func<string, Expression<Func<TIn, TOut>>> _resolver;

        public ParamDescriptor(string token, Func<string, Expression<Func<TIn, TOut>>> resolver, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : base(token, comparison)
        {
            if(resolver==null)
                throw new ArgumentException("resolver");
            _resolver = resolver;
        }

        public override Expression Resolve(string token, ParameterExpression param)
        {
            Expression<Func<TIn, TOut>> func = _resolver(token);
            return Expression.Invoke(func, param);
        }
    }

    public abstract class ParamDescriptor<TIn> : ParamDescriptor
    {
        public ParamDescriptor(string token, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : base(token, comparison)
        {
        }


        public override Type Type
        {
            get { return typeof (TIn); }
        }
    }

    public abstract class ParamDescriptor
    {
        private readonly StringComparison _comparison;
        private readonly string _token;

        protected ParamDescriptor(string token, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (token == null)
                throw new ArgumentException("token");

            _comparison = comparison;
            _token = token;
        }

        public string Token { get { return _token; } }
        public StringComparison Comparison { get { return _comparison; } }

        public  abstract Type Type { get; }

        public abstract Expression Resolve(string token, ParameterExpression param);

    }
}