using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TLIS_DAL.Helper.Filters;

namespace TLIS_DAL.Helper
{
    public static partial class ExpressionUtils
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
        {

            var parameter = Expression.Parameter(typeof(T), "x");

            var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);

            var body1 = MakeComparison(left, comparison, value);

            return Expression.Lambda<Func<T, bool>>(body1, parameter);

        }
  
        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterExperssion> filter)
        {
 
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body2 = null;
            Expression bodytemp1 = null;
            Expression bodytemp2 = null;
            Expression binExp = null;
            Expression body1 = null;
            foreach (var item in filter)
            {
                var left = item.propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
                
                foreach (var item2 in item.value)
                {
                    var bodytemp = MakeComparison(left, item.comparison, item2);
                    if (bodytemp1 == null)
                    {
                        bodytemp1 = bodytemp;
                    }
                    else
                    {
                        bodytemp2 = bodytemp;
                        binExp = Expression.Or(bodytemp1, bodytemp2);
                        bodytemp1 = binExp;
                    }
                }
                //var body = MakeComparison(left, item.comparison, item.value);
                if (body1==null)
                {
                    body1 = bodytemp1;
                    binExp = body1;
                }
                else
                {
                    body2 = bodytemp1;
                    binExp = Expression.And(body1, body2);
                    body1 = binExp;
                }
            }
            
       
          var x= Expression.Lambda<Func<T, bool>>(binExp, parameter);
            return Expression.Lambda<Func<T, bool>>(binExp, parameter);
          
        }

        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterExperssionOneValue> filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body1 = null;
            Expression body2 = null;
            Expression bodytemp1 = null;
            Expression binExp = null;
            foreach (var item in filter)
            {
                var left = item.propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
                bodytemp1 = MakeComparison(left, item.comparison, item.value);
                if (body1 == null)
                {
                    body1 = bodytemp1;
                    binExp = body1;
                }
                else
                {
                    body2 = bodytemp1;
                    binExp = Expression.And(body1, body2);
                    body1 = binExp;
                }
            }  
            var x = Expression.Lambda<Func<T, bool>>(binExp, parameter);
            return Expression.Lambda<Func<T, bool>>(binExp, parameter);
           
        }
        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterObject> filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression body1 = null;
            Expression body2 = null;
            Expression bodytemp1 = null;
            Expression binExp = null;
            Expression body = parameter;
            foreach (var item in filter)
            {
                body = parameter;
                foreach (var subMember in item.key.Split('.'))
                {
                    body = Expression.PropertyOrField(body, subMember);
                }
                //  var left = item.propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
              // var val =  Convert.ChangeType(item.value, body.Type);
                if(string.IsNullOrEmpty(item.op))
                {
                    bodytemp1 = Expression.Equal(body, Expression.Constant(item.value2, body.Type));
                }
                else
                {
                    bodytemp1 = MakeComparison(body, item.op, item.value2.ToString());
                }
                if (body1 == null)
                {
                    body1 = bodytemp1;
                    binExp = body1;
                }
                else
                {
                    body2 = bodytemp1;
                    binExp = Expression.And(body1, body2);
                    body1 = binExp;
                }
            }
            var x = Expression.Lambda<Func<T, bool>>(binExp, parameter);
            return Expression.Lambda<Func<T, bool>>(binExp, parameter);

        }

        public static Expression<Func<T, bool>> BuildPredicate<T>(List<FilterObjectList> filter)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression body2 = null;
                Expression body1 = null;
                Expression bodytemp1 = null;
                Expression bodytemp2 = null;
                Expression binExp = null;
                Expression body = parameter;

                foreach (var item in filter)
                {
                    bodytemp1 = null;
                    bodytemp2 = null;
                    binExp = null;
                    body = parameter;
                    foreach (var subMember in item.key.Split('.'))
                    {
                        body = Expression.PropertyOrField(body, subMember);

                    }

                    foreach (var item2 in item.value)
                    {
                        object val;
                        try
                        {
                            var s = Convert.ChangeType(item2, body.Type);
                            val = Convert.ChangeType(item2, body.Type);
                            

                        }
                        catch (Exception)
                        {
                            val = Int32.Parse(item2.ToString());
                        }

                        Expression bodytemp;
                        try
                        {
                           if(val=="List")
                            {
                                bodytemp = Expression.NotEqual(body, Expression.Constant(val, body.Type));

                            }

                          else if(item.key=="key")

                            {
                                ConstantExpression constant = Expression.Constant(val,body.Type);
                                Expression left = Expression.Call(body, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                Expression right = Expression.Call(constant, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                                bodytemp = Expression.Call(left, "StartsWith", null, right);

                            }


                            else
                            {
                                bodytemp = Expression.Equal(body, Expression.Constant(val, body.Type));

                            }



                        }
                        catch (Exception e)
                        {
                            val = float.Parse(item2.ToString());
                            bodytemp = Expression.Equal(body, Expression.Constant(val, body.Type));
                        }
                         //bodytemp = Expression.Equal(body, Expression.Constant(val, body.Type));
                        if (bodytemp1 == null)
                        {
                            bodytemp1 = bodytemp;
                        }
                        else
                        {
                            bodytemp2 = bodytemp;
                            binExp = Expression.Or(bodytemp1, bodytemp2);
                            bodytemp1 = binExp;
                        }
                    }

                    if (body1 == null)
                    {
                        body1 = bodytemp1;
                        binExp = body1;
                    }
                    else
                    {
                        body2 = bodytemp1;
                        binExp = Expression.And(body1, body2);
                        body1 = binExp;
                    }
                }

                var x = Expression.Lambda<Func<T, bool>>(binExp, parameter);
                return Expression.Lambda<Func<T, bool>>(binExp, parameter);
            }
            catch(Exception e)
            {
                return null;
            }
          



        }
        public static Expression<Func<T, bool>> BuildPredicate<T>(string member, object value)
        {  
            var p = Expression.Parameter(typeof(T),"x");
            Expression body = p;
            foreach (var subMember in member.Split('.'))
            {
                body = Expression.PropertyOrField(body, subMember);
            }
            try
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(
              body, Expression.Constant(value, body.Type)), p);
            }
            catch(Exception)
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(
              body, Expression.Constant(value, body.Type)), p);
            }
          
        }

        public static Expression MakeComparison(Expression left, string comparison, string value)
        {
            switch (comparison)
            {
                case "==":
                    return MakeBinary(ExpressionType.Equal, left, value);
                case "!=":
                    return MakeBinary(ExpressionType.NotEqual, left, value);
                case ">":
                    return MakeBinary(ExpressionType.GreaterThan, left, value);
                case ">=":
                    return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
                case "<":
                    return MakeBinary(ExpressionType.LessThan, left, value);
                case "<=":
                    return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
                case "Contains":
                case "StartsWith":
                case "EndsWith":
                    return Expression.Call(MakeString(left), comparison, Type.EmptyTypes, Expression.Constant(value, typeof(string)));
                default:
                    throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
            }
        }

        private static Expression MakeString(Expression source)
        {
            return source.Type == typeof(string) ? source : Expression.Call(source, "ToString", Type.EmptyTypes);
        }

        private static Expression MakeBinary(ExpressionType type, Expression left, string value)
        {
            object typedValue = value;
            if (left.Type != typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    typedValue = null;
                    if (Nullable.GetUnderlyingType(left.Type) == null)
                        left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
                else
                {
                    var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                    typedValue = valueType.IsEnum ? Enum.Parse(valueType, value) :
                        valueType == typeof(Guid) ? Guid.Parse(value) :
                        Convert.ChangeType(value, valueType);
                }
            }
            var right = Expression.Constant(typedValue, left.Type);
            return Expression.MakeBinary(type, left, right);
        }

        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

    }
}
