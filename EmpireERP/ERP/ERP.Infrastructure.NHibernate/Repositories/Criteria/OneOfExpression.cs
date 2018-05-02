using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;


namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    class OneOfExpression<T> : BaseExpression, IExpression, ISubExpression
    {
        #region
        
        /// <summary>
        /// Свойство
        /// </summary>
        private Expression<Func<T, object>> propertyExpression = null;

        /// <summary>
        /// Имя поля свойства
        /// </summary>
        private string fieldName;

        /// <summary>
        /// Значения
        /// </summary>
        private IEnumerable values;

        private bool inverseCondition;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="prop">Свойство</param>
        /// <param name="vals">Значения</param>
        public OneOfExpression(Expression<Func<T, object>> prop, IEnumerable vals, bool inverse = false)
        {
            propertyExpression = prop;
            values = vals;
            inverseCondition = inverse;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fieldName">Имя поля свойства</param>
        /// <param name="vals">Значения</param>
        public OneOfExpression(string fieldName, IEnumerable vals, bool inverse = false)
        {
            this.fieldName = fieldName;
            values = vals;
            inverseCondition = inverse;
        }

        #region ExpressionBase

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            string result;

            if (propertyExpression != null)
            {
                result = ParseExpression(propertyExpression.Body);
            }
            else
            {
                result = fieldName;
            }

            Type type = GetTypeField(result, typeof(T));     //Получаем тип данных поля

            List<object> objs = new List<object>();
            foreach (var value in values)
            {
                objs.Add(ConvertToType(value, type));
            }

            if (!inverseCondition)
            {
                iCriteria.Add(global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>()));
            }
            else
            {
                iCriteria.Add
                (
                    global::NHibernate.Criterion.Expression.Not
                    (
                        global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>())
                    )
                );
            }
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            string result;

            if (propertyExpression != null)
            {
                result = ParseExpression(propertyExpression.Body);
            }
            else
            {
                result = (String.IsNullOrEmpty(alias) ? "" : alias + '.') + fieldName;
            }

            Type type = GetTypeField(result, typeof(T));     //Получаем тип данных поля

            List<object> objs = new List<object>();
            foreach (var value in values)
            {
                objs.Add(ConvertToType(value, type));
            }

            if (!inverseCondition)
            {
                return global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>());
            }

            return global::NHibernate.Criterion.Expression.Not(global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>()));
        }
        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            string result;

            if (propertyExpression != null)
            {
                result = ParseExpression(propertyExpression.Body);
            }
            else
            {
                result = fieldName;
            }

            Type type = GetTypeField(result, typeof(T));     //Получаем тип данных поля

            List<object> objs = new List<object>();
            foreach (var value in values)
            {
                objs.Add(ConvertToType(value, type));
            }

            if (!inverseCondition)
            {
                iCriteria.Add(global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>()));
            }
            else
            {
                iCriteria.Add
                (
                    global::NHibernate.Criterion.Expression.Not
                    (
                        global::NHibernate.Criterion.Expression.In(result, objs.ToArray<object>())
                    )
                );
            }
        }

        /// <summary>
        /// Разбор лямбда выражения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        /// <returns></returns>
        private string ParseExpression(Expression expr)
        {
            string operation = "";
            OperationType opType = OperationType.Binary;

            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    opType = OperationType.Parameter;
                    var exp = expr as MemberExpression;
                    operation = exp.Member.Name;
                    break;
                case ExpressionType.Parameter:
                    opType = OperationType.Parameter;
                    var exp3 = expr as ParameterExpression;
                    operation = exp3.Name;
                    break;
                case ExpressionType.Convert:
                    opType = OperationType.Convert;
                    operation = ParseExpression((expr as UnaryExpression).Operand);
                    break;
                default:
                    throw new Exception(String.Format("Операция {0} не поддерживается.", expr.NodeType.ToString()));
            }

            string result = "";
            switch (opType)
            {
                case OperationType.Parameter:
                    if (expr is MemberExpression)
                    {
                        var member = ParseExpression((expr as MemberExpression).Expression);
                        if (member.Length > 0)
                        {
                            result = ParseExpression((expr as MemberExpression).Expression) + '.' + operation;
                        }
                        else
                        {
                            result = operation;
                        }
                    }
                    else
                    {
                        result = "";
                    }
                    break;
                case OperationType.Convert:
                    result = operation;
                    break;
            }

            return result;
        }
        #endregion

    }
}
