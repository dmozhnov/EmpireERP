using System;
using System.Linq.Expressions;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class OrderByExpression<T> : BaseExpression, IExpression, ISubExpression
    {
        /// <summary>
        /// Значение выражения сортировки
        /// </summary>
        private Expression<Func<T, object>> sortByExpression = null;

        /// <summary>
        /// Имя поля 
        /// </summary>
        private string fieldName = null;

        /// <summary>
        /// Признак сортировки по возрастанию
        /// </summary>
        private bool sortByAsc;

        /// <summary>
        /// Выражение сортировки
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        public OrderByExpression(Expression<Func<T, object>> expr, bool sortBy_Asc)
        {
            sortByExpression = expr;
            sortByAsc = sortBy_Asc;
        }

        /// <summary>
        /// Выражение сортировки
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        public OrderByExpression(string field_Name, bool sortBy_Asc)
        {
            if (String.IsNullOrEmpty(field_Name))
                throw new Exception("Необходимо указать поле для сортировки.");

            fieldName = field_Name;
            sortByAsc = sortBy_Asc;
        }

        #region ExpressionBase

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria"></param>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            string result;
            if (sortByExpression != null)
            {
                //Поле сортировки задано лямбдой
                result = ParseExpression(sortByExpression.Body);
                if (result.Length == 0)
                    throw new Exception("Необходимо указать поле для сортировки.");
            }
            else
            {
                //Поле задано именем
                result = fieldName;
            }

            iCriteria.AddOrder(new global::NHibernate.Criterion.Order(result, sortByAsc));  //Генерим критерий
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            // Заглушка
            return null;
        }

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria"></param>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            string result;

            if (sortByExpression != null)
            {
                //Поле сортировки задано лямбдой
                result = ParseExpression(sortByExpression.Body);
                if (result.Length == 0)
                    throw new Exception("Необходимо указать поле для сортировки.");
            }
            else
            {
                //Поле задано именем
                result = fieldName;
            }

            iCriteria.AddOrder(new global::NHibernate.Criterion.Order(result, sortByAsc));  //Генерим критерий
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
