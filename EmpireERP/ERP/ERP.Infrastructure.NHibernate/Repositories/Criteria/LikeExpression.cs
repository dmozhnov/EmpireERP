using System;
using System.Linq.Expressions;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class LikeExpression<T>: BaseExpression,IExpression, ISubExpression
    {
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
        public string valueTemplate;

        /// <summary>
        /// Выражение "как"
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        /// <param name="value">Шаблон</param>
        public LikeExpression(Expression<Func<T, string>> expr, string value)
        {
            likeExpression = expr;
            valueTemplate = value;
        }

        /// <summary>
        /// Выражение "как"
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Шаблон</param>
        public LikeExpression(string fieldName, string value)
        {
            if (String.IsNullOrEmpty(fieldName))
                throw new Exception("Необходимо указать поле для операции Like.");

            this.fieldName = fieldName;
            valueTemplate = value;
        }

        #region ExpressionBase

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            string result;
            if (likeExpression != null) 
            {
                //Поле задано лямбда выражением
                result = ParseExpression(likeExpression.Body);
                if (result.Length == 0)
                {
                    throw new Exception("Необходимо указать поле для операции Like.");
                }
                if (String.IsNullOrEmpty(valueTemplate) || valueTemplate.Length == 0)
                {
                    throw new Exception("Необходимо указать строку для поиска.");
                }
            }
            else
            {
                //Поле задано именем
                result = fieldName;
            }

            iCriteria.Add(global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Anywhere)); //Генерим критерий
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
                    throw new Exception("Необходимо указать поле для операции Like.");
                }
                if (String.IsNullOrEmpty(valueTemplate) || valueTemplate.Length == 0)
                {
                    throw new Exception("Необходимо указать строку для поиска.");
                }
            }
            else
            {
                //Поле задано именем
                result = (String.IsNullOrEmpty(alias) ? "" : alias + '.') + fieldName;
            }


            return global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Anywhere);
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
                    throw new Exception("Необходимо указать поле для операции Like.");
                }
                if (String.IsNullOrEmpty(valueTemplate) || valueTemplate.Length == 0)
                {
                    throw new Exception("Необходимо указать строку для поиска.");
                }
            }
            else
            {
                //Поле задано именем
                result = fieldName;
            }

            iCriteria.Add(global::NHibernate.Criterion.Expression.Like(result, valueTemplate, global::NHibernate.Criterion.MatchMode.Anywhere)); //Генерим критерий
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

        #endregion
    }
}
