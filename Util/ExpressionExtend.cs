using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Util
{
    public class ExpressionExtend : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(_parameter);
        }

        public ExpressionExtend(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        public static Expression<Func<T, bool>> Ou<T>(Expression<Func<T, bool>> expressao1, Expression<Func<T, bool>> expressao2)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.Or(expressao1.Body, expressao2.Body);

            exprBody = (BinaryExpression)new ExpressionExtend(paramExpr).Visit(exprBody);

            return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
        }

        public static Expression<Func<T, bool>> E<T>(Expression<Func<T, bool>> expressao1, Expression<Func<T, bool>> expressao2)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.And(expressao1.Body, expressao2.Body);

            exprBody = (BinaryExpression)new ExpressionExtend(paramExpr).Visit(exprBody);

            return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
        }
    }
}
