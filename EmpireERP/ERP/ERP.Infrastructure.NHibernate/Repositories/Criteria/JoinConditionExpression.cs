using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class JoinConditionExpression : IExpression, ISubExpression
    {
        #region Данные

        /// <summary>
        /// Левое выражение
        /// </summary>
        private IExpression leftExpression;

        /// <summary>
        /// Правое выражение
        /// </summary>
        private IExpression rightExpression;

        /// <summary>
        /// Возможные типы соединения
        /// </summary>
        public enum CondJoinExpression { Or, And }

        /// <summary>
        /// Тип соединения
        /// </summary>
        private CondJoinExpression condExpression;

        #endregion

        /// <summary>
        /// Конструктоор
        /// </summary>
        /// <param name="left">Левое выражение</param>
        /// <param name="right">Правое выражение</param>
        /// <param name="cond">Условие соединения</param>
        internal JoinConditionExpression(IExpression left, IExpression right, CondJoinExpression cond)
        {
            leftExpression = left;
            rightExpression = right;
            condExpression = cond;
        }

        #region IExpression

        /// <summary>
        /// Генерация критериев
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            global::NHibernate.Criterion.ICriterion left, right;

            left = leftExpression.Compile(ref iCriteria, "");
            right = rightExpression.Compile(ref iCriteria, "");

            switch (condExpression)
            {
                case CondJoinExpression.Or:
                    iCriteria.Add(global::NHibernate.Criterion.Expression.Or(left, right));
                    break;
                case CondJoinExpression.And:
                    iCriteria.Add(global::NHibernate.Criterion.Expression.And(left, right));
                    break;
            }
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            global::NHibernate.Criterion.ICriterion left, right, result = null;

            left = leftExpression.Compile(ref iCriteria, "");
            right = rightExpression.Compile(ref iCriteria, "");

            switch (condExpression)
            {
                case CondJoinExpression.Or:
                    result = global::NHibernate.Criterion.Expression.Or(left, right);
                    break;
                case CondJoinExpression.And:
                    result = global::NHibernate.Criterion.Expression.And(left, right);
                    break;
            }

            return result;
        }

        #endregion

        #region ISubExpression

        /// <summary>
        /// Генерирование критерия
        /// </summary>
        /// <param name="iCriteria">Критерий NHibernate</param>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            global::NHibernate.Criterion.ICriterion left, right;
            ICriteria iCrit = null;

            left = leftExpression.Compile(ref iCrit, "");
            right = rightExpression.Compile(ref iCrit, "");

            switch (condExpression)
            {
                case CondJoinExpression.Or:
                    iCriteria.Add(global::NHibernate.Criterion.Expression.Or(left, right));
                    break;
                case CondJoinExpression.And:
                    iCriteria.Add(global::NHibernate.Criterion.Expression.And(left, right));
                    break;
            }
        }

        #endregion
    }
}
