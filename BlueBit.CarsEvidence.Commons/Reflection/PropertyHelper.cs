using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace BlueBit.CarsEvidence.Commons.Reflection
{
    public static class PropertyHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var name = (propertyExpression.Body as MemberExpression).Member.Name;
            Contract.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }

        public static Tuple<object, string> GetPropertyPath<T,TVal>(T obj,  Expression<Func<T, TVal>> propertyExpression)
        {
            var memberExpression = (propertyExpression.Body as MemberExpression);
            Contract.Assert(memberExpression != null);

            var names = new List<string>();
            while (memberExpression != null)
            {
                var name = memberExpression.Member.Name;
                Contract.Assert(!string.IsNullOrWhiteSpace(name));
                names.Add(name);

                memberExpression = (memberExpression.Expression as MemberExpression);
            }

            names.Reverse();
            return Tuple.Create((object)obj, string.Join(".", names));
        }
    }

    public static class PropertyHelper<TObj>
    {
        public static string GetPropertyName<T>(Expression<Func<TObj, T>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
        public static string GetPropertyName<P, T>(Expression<Func<P, T>> expression)
        {
            var name = string.Empty;
            if (expression.Body is UnaryExpression)
            {
                UnaryExpression unex = (UnaryExpression)expression.Body;
                if (unex.NodeType == ExpressionType.Convert)
                {
                    Expression ex = unex.Operand;
                    MemberExpression mex = (MemberExpression)ex;
                    name = mex.Member.Name;
                }
            }
            else
            {
                MemberExpression memberExpression = (MemberExpression)expression.Body;
                name = memberExpression.Member.Name;
            }
            Contract.Assert(!string.IsNullOrWhiteSpace(name));
            return name;
        }
    }
}
