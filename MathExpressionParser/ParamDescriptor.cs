using System;
using System.Linq.Expressions;

namespace Langman.MathExpressionParser
{

    public class ParamDescriptor<TOut, TIn> : ParamDescriptor<TOut>
    {
        private readonly Expression<Func<TIn, string, TOut>> _resolver;

        public ParamDescriptor(string token, Expression<Func<TIn, string, TOut>> resolver, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : base(token, comparison)
        {
            if(resolver==null)
                throw new ArgumentException("resolver");
            _resolver = resolver;
        }

        public override Expression Resolve(string token)
        {
            return Expression.Invoke(_resolver,  Expression.Constant(token));
        }
    }

    public abstract class ParamDescriptor<TOut> : ParamDescriptor
    {
        public ParamDescriptor(string token, StringComparison comparison = StringComparison.OrdinalIgnoreCase) : base(token, comparison)
        {
        }

        public override Type Type
        {
            get { return typeof (TOut); }
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

        public abstract Expression Resolve(string token);
        public abstract Type Type { get; }
    }
}