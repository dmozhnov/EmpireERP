using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    internal interface IExpression
    {
        /// <summary>
        /// Генерация критериев
        /// </summary>
        /// <param name="iCriteria"></param>
        /// <returns></returns>
        void Compile(ref ICriteria iCriteria);

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion Compile(ref ICriteria iCriteria, string alias);
    }
}
