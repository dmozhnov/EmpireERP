using NHibernate.Criterion;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    interface ISubExpression
    {
        /// <summary>
        /// Генерация критериев
        /// </summary>
        /// <param name="iCriteria"></param>
        /// <returns></returns>
        void Compile(ref DetachedCriteria iCriteria);
    }
}
