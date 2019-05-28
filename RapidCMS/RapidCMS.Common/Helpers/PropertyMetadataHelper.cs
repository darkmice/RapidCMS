﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RapidCMS.Common.Models.Metadata;

#nullable enable

namespace RapidCMS.Common.Helpers
{
    internal static class PropertyMetadataHelper
    {
        /// <summary>
        /// Converts a given LambdaExpression containing MemberExpressions to getters and setters, and the name of each nested object plus the name of the property.
        /// 
        /// (Person x) => x.Company.Owner.Name becomes:
        /// getter: (object x) => (object)(((Person) x).get_Company().get_Owner().get_Name())
        /// setter: (object x, object y) => ((Person) x).get_Company().get_Owner().set_Name((string)y)
        /// objectType: Person
        /// propertyType: string
        /// name: CompanyOwnerName
        /// </summary>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, null when not.</returns>
        public static IPropertyMetadata? GetPropertyMetadata(LambdaExpression lambdaExpression)
        {
            try
            {
                Type? parameterTType = null;
                Type? parameterTPropertyType = null;

                var parameterT = Expression.Parameter(typeof(object), "x");
                var parameterTProperty = Expression.Parameter(typeof(object), "y");

                var x = lambdaExpression.Body;

                if (!(x is MemberExpression) && !(x is ParameterExpression))
                {
                    parameterTType = lambdaExpression.Parameters.First().Type;
                    var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;

                    return new PropertyMetadata
                    {
                        ObjectType = parameterTType,
                        PropertyName = lambdaExpression.ToString(),
                        PropertyType = lambdaExpression.Body.Type,
                        Getter = ConvertToGetterViaLambda(parameterT, lambdaExpression, parameterTAsType)
                    };
                }
                else
                {
                    var isAtTail = true;
                    var getNestedObjectMethods = new List<MethodInfo>();
                    var names = new List<string>();

                    MethodInfo? getValueMethod = null;
                    MethodInfo? setValueMethod = null;

                    do
                    {
                        if (x is MemberExpression memberExpression)
                        {
                            var propertyInfo = memberExpression.Member as PropertyInfo;

                            if (propertyInfo == null)
                            {
                                throw new Exception("Failed to get PropertyInfo from Member.");
                            }

                            if (isAtTail)
                            {
                                setValueMethod = propertyInfo.GetSetMethod();
                                getValueMethod = propertyInfo.GetGetMethod();
                                names.Add(propertyInfo.Name);

                                parameterTPropertyType = propertyInfo.PropertyType;

                                isAtTail = false;

                                x = memberExpression.Expression;
                            }
                            else
                            {
                                getNestedObjectMethods.Insert(0, propertyInfo.GetGetMethod());
                                names.Insert(0, propertyInfo.Name);

                                x = memberExpression.Expression;
                            }
                        }
                        else if (x is ParameterExpression parameterExpression)
                        {
                            parameterTType = x.Type;

                            // done, arrived at root
                            break;
                        }
                        else
                        {
                            throw new Exception("Failed to interpret given LambdaExpression");
                        }
                    }
                    while (true);

                    if (getValueMethod == null || setValueMethod == null)
                    {
                        throw new Exception("Failed to process given LambdaExpression");
                    }

                    var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;
                    var valueToType = Expression.Convert(parameterTProperty, parameterTPropertyType) as Expression;
                    var valueToObject = Expression.Convert(Expression.Parameter(parameterTPropertyType, "z"), typeof(object));

                    var instanceExpression = (getNestedObjectMethods.Count == 0)
                        ? parameterTAsType
                        : getNestedObjectMethods.Aggregate(
                            parameterTAsType,
                            (parameter, method) => Expression.Call(parameter, method));

                    var setter = ConvertToSetter(parameterT, parameterTProperty, setValueMethod, valueToType, instanceExpression);
                    var getter = ConvertToGetterViaMethod(parameterT, getValueMethod, instanceExpression);
                    var name = string.Join("", names);

                    return new FullPropertyMetadata
                    {
                        ObjectType = parameterTType,
                        Getter = getter,
                        Setter = setter,
                        PropertyName = name,
                        PropertyType = parameterTPropertyType
                    };
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a given LambdaExpression containing expression to get value from an object.
        /// 
        /// (Person x) => $"{x.FirstName} - {x.LastName}"  becomes:
        /// getter: (object x) => (object)
        /// objectType: Person
        /// propertyType: string
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when given LambdaExpression cannot be converted to a getter.</exception>
        /// <param name="lambdaExpression">The LambdaExpression to be converted</param>
        /// <returns>GetterAndSetter object when successful, null when not.</returns>
        public static IExpressionMetadata? GetExpressionMetadata(LambdaExpression lambdaExpression)
        {
            try
            {
                var parameterT = Expression.Parameter(typeof(object), "x");
                var parameterTType = lambdaExpression.Parameters.First().Type;
                var parameterTAsType = Expression.Convert(parameterT, parameterTType) as Expression;

                var name = ((lambdaExpression.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? lambdaExpression.ToString();

                return new ExpressionMetadata
                {
                    PropertyName = name,
                    PropertyType = lambdaExpression.Body.Type,
                    StringGetter = ConvertToStringGetterViaLambda(parameterT, lambdaExpression, parameterTAsType)
                };
            }
            catch
            {
                return null;
            }
        }

        private static Func<object, object> ConvertToGetterViaMethod(ParameterExpression parameterT, MethodInfo getValueMethod, Expression instanceExpression)
        {
            var getExpression = Expression.Lambda<Func<object, object>>(
                Expression.Convert(Expression.Call(instanceExpression, getValueMethod), typeof(object)),
                parameterT
            );

            return getExpression.Compile();
        }

        private static Func<object, object> ConvertToGetterViaLambda(ParameterExpression parameterT, LambdaExpression lambdaExpression, Expression parameterExpression)
        {
            var getExpression = Expression.Lambda<Func<object, object>>(
                Expression.Convert(Expression.Invoke(lambdaExpression, parameterExpression), typeof(object)),
                parameterT
            );

            return getExpression.Compile();
        }

        private static Func<object, string> ConvertToStringGetterViaLambda(ParameterExpression parameterT, LambdaExpression lambdaExpression, Expression parameterExpression)
        {
            var method = typeof(object).GetMethod(nameof(object.ToString));

            var getExpression = Expression.Lambda<Func<object, string>>(
                Expression.Call(Expression.Invoke(lambdaExpression, parameterExpression), method),
                parameterT
            );

            return getExpression.Compile();
        }

        private static Action<object, object> ConvertToSetter(ParameterExpression parameterT, ParameterExpression parameterTProperty, MethodInfo setValueMethod, Expression valueToType, Expression instanceExpression)
        {
            var setExpression = Expression.Lambda<Action<object, object>>(
                Expression.Call(instanceExpression, setValueMethod, valueToType),
                parameterT,
                parameterTProperty
            );

            return setExpression.Compile();
        }
    }
}
