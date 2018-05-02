using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class LikeOrExpression<T> : BaseExpression, IExpression, ISubExpression
    {
        #region Поля

        /// <summary>
        /// Значение выражения "как"
        /// </summary>
        private Expression<Func<T, string>> likeExpression = null;

        /// <summary>
        /// Имя поля
        /// </summary>
        private string fieldName = null;

        /// <summary>
        /// Значение шаблона строки
        /// </summary>
        private IList<string> valueTemplates;

        #endregion

        #region Конструкторы

        public LikeOrExpression(Expression<Func<T, string>> expr, IList<string> values)
        {
            likeExpression = expr;
            valueTemplates = values;
        }

        public LikeOrExpression(string fieldName, IList<string> values)
        {
            if (String.IsNullOrEmpty(fieldName))
                throw new Exception("Необходимо указать поле для операции Like.");

            this.fieldName = fieldName;
            valueTemplates = values;
        }

        #endregion

        #region Методы

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
            }

            return result;
        }

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            iCriteria.Add((this as IExpression).Compile(ref iCriteria, String.Empty));
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            string result;
            if (likeExpression != null)
            {
                //Поле задано лямбда выражением
                result = ParseExpression(likeExpression.Body);
                if (result.Length == 0)
                {
                    throw new Exception("Необходимо указать поле для операции LikeOr.");
                }
                if (valueTemplates == null || valueTemplates.Count == 0)
                {
                    throw new Exception("Необходимо указать строки для поиска.");
                }
            }
            else
            {
                //Поле задано именем
                result = (String.IsNullOrEmpty(alias) ? "" : alias + '.') + fieldName;
            }

            //Генерим критерий
            global::NHibernate.Criterion.ICriterion expr = null;
            foreach (var valueTemplate in valueTemplates)
            {
                if (expr == null)
                {
                    expr = global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Exact);
                }
                else
                {
                    var expression = global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Exact);
                    expr = global::NHibernate.Criterion.Expression.Or(expr, expression);
                }
            }

            return expr;
        }

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            string result;
            if (likeExpression != null)
            {
                //Поле задано лямбда выражением
                result = ParseExpression(likeExpression.Body);
                if (result.Length == 0)
                {
                    throw new Exception("Необходимо указать поле для операции LikeOr.");
                }
                if (valueTemplates == null || valueTemplates.Count == 0)
                {
                    throw new Exception("Необходимо указать строки для поиска.");
                }
            }
            else
            {
                //Поле задано именем
                result = fieldName;
            }

            //Генерим критерий
            global::NHibernate.Criterion.ICriterion expr = null;
            foreach (var valueTemplate in valueTemplates)
            {
                if (expr == null)
                {
                    expr = global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Exact);
                }
                else
                {
                    var expression = global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Exact);
                    expr = global::NHibernate.Criterion.Expression.Or(expr, expression);
                }
            }

            iCriteria.Add(expr);
        }
        #endregion

    }
}
