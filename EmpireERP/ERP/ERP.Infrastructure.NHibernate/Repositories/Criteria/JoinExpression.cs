using System;
using System.Linq.Expressions;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    /// <summary>
    /// Подзапрос
    /// </summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <typeparam name="TSub">Тип критерия подзапроса</typeparam>
    public class JoinExpression<T> : BaseExpression, IExpression, ISubExpression
        where T : class
    {
        #region Данные

        /// <summary>
        /// Поле
        /// </summary>
        private Expression<Func<T, object>> joinExpression;

        /// <summary>
        /// Поле строкой
        /// </summary>
        private string joinProperty;

        /// <summary>
        /// Подзапрос
        /// </summary>
        private ISubQuery subCriteria;

        /// <summary>
        /// Возможные типы соединения
        /// </summary>
        public enum JoinExpressionType { In, NotIn }

        /// <summary>
        /// Тип соединения
        /// </summary>
        public JoinExpressionType joinType;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="joinExpr">Свойство</param>
        /// <param name="subCrit">Подзапрос</param>
        /// <param name="joinT">Тип соединения</param>
        public JoinExpression(Expression<Func<T, object>> joinExpr, ISubQuery subCrit, JoinExpressionType joinT)
        {
            joinExpression = joinExpr;
            subCriteria = subCrit;
            joinType = joinT;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="joinExpr">Свойство</param>
        /// <param name="subCrit">Подзапрос</param>
        /// <param name="joinT">Тип соединения</param>
        public JoinExpression(string field, ISubQuery subCrit, JoinExpressionType joinT)
        {
            joinProperty = field;
            subCriteria = subCrit;
            joinType = joinT;
        }

        #region Генерирование критериев

        /// <summary>
        /// Приведение подзапроса от интерфейса к объекту класса
        /// </summary>
        /// <param name="subQuery"></param>
        /// <returns></returns>
        private global::NHibernate.Criterion.DetachedCriteria GetSubCriteria(ISubQuery subQuery)
        {
            var baseType = typeof(SubCriteria<>);   //Базовый тип подзапроса
            var genericType = baseType.MakeGenericType(new Type[1] { subCriteria.GetParameterType() }); // Дженерик тип подзапроса

            var obj = Convert.ChangeType(subQuery, genericType); //Приводим интерфейс к дженерик типу подзапроса

            var methodCompile = genericType.GetMethod("Compile", Type.EmptyTypes);  //Получаем метод компилирования

            return methodCompile.Invoke(obj, null) as global::NHibernate.Criterion.DetachedCriteria;   //Получаем критерий подзапроса NHibernate
        }

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void IExpression.Compile(ref global::NHibernate.ICriteria iCriteria)
        {
            string propName = "";
            global::NHibernate.Criterion.DetachedCriteria dCrit = null;


            propName = joinExpression!= null? ParseExpression(joinExpression.Body): joinProperty;
            if (propName.Length == 0)
            {
                throw new Exception("Необходимо указать поле.");
            }
            if (subCriteria == null)
            {
                throw new Exception("Необходимо указать подзапрос.");
            }

            dCrit = GetSubCriteria(subCriteria);    //Получаем критерий подзапроса NHibernate

            switch (joinType)
            {
                case JoinExpressionType.In:
                    iCriteria.Add(global::NHibernate.Criterion.Subqueries.PropertyIn(propName, dCrit));
                    break;
                case JoinExpressionType.NotIn:
                    iCriteria.Add(global::NHibernate.Criterion.Subqueries.PropertyNotIn(propName, dCrit));
                    break;
            }
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref global::NHibernate.ICriteria iCriteria, string alias)
        {
            string propName = "";
            global::NHibernate.Criterion.DetachedCriteria dCrit = null;

            propName = joinExpression != null ? ParseExpression(joinExpression.Body) : (String.IsNullOrEmpty(alias) ? "" : alias + '.') + joinProperty;
            if (propName.Length == 0)
            {
                throw new Exception("Необходимо указать поле.");
            }
            if (subCriteria == null)
            {
                throw new Exception("Необходимо указать подзапрос.");
            }

            dCrit = GetSubCriteria(subCriteria);    //Получаем критерий подзапроса NHibernate

            switch (joinType)
            {
                case JoinExpressionType.In:
                    return global::NHibernate.Criterion.Subqueries.PropertyIn(propName, dCrit);
                    
                case JoinExpressionType.NotIn:
                    return global::NHibernate.Criterion.Subqueries.PropertyNotIn(propName, dCrit);
                    
            }

            return null;
        }

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            string propName = "";
            global::NHibernate.Criterion.DetachedCriteria dCrit = null;

            propName = joinExpression != null ? ParseExpression(joinExpression.Body) : joinProperty;
            if (propName.Length == 0)
            {
                throw new Exception("Необходимо указать поле.");
            }
            if (subCriteria == null)
            {
                throw new Exception("Необходимо указать подзапрос.");
            }

            dCrit = GetSubCriteria(subCriteria);    //Получаем критерий подзапроса NHibernate

            switch (joinType)
            {
                case JoinExpressionType.In:
                    iCriteria.Add(global::NHibernate.Criterion.Subqueries.PropertyIn(propName, dCrit));
                    break;
                case JoinExpressionType.NotIn:
                    iCriteria.Add(global::NHibernate.Criterion.Subqueries.PropertyNotIn(propName, dCrit));
                    break;
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
            string result = "";
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
                    result = ParseExpression((expr as UnaryExpression).Operand);
                    break;
                default:
                    throw new Exception(String.Format("Операция {0} не поддерживается.", expr.NodeType.ToString()));
            }


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
