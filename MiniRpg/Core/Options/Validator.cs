using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MiniRpg.Core.Options
{
    public class Validator
    {
        public List<string> Errors { get; } = new List<string>();

        public Validator ShouldBeValid<T>(Expression<Func<T>> propGetter, Func<T, bool> validator, string message)
        {
            var propName = ((propGetter.Body as MemberExpression).Member as PropertyInfo).Name;
            var propValue = propGetter.Compile()();

            if (!validator(propValue))
            {
                Errors.Add($"{propName} value '{propValue}' {message}");
            }

            return this;
        }

        public Validator ShouldBeGreaterThanZero(Expression<Func<int>> propGetter) =>
            ShouldBeValid(propGetter, x => x > 0, "should be greater than zero.");
        
        public Validator ShouldBeGreaterOrEqualThanZero(Expression<Func<int>> propGetter) =>
            ShouldBeValid(propGetter, x => x >= 0, "should be greater than zero or equal.");
    }
}