using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators;
using NHibernate.Linq;
using System.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public abstract class BaseReturnFromClientIndicatorRepository<T> : BaseIndicatorRepository<T> where T : BaseReturnFromClientIndicator
    {
        public BaseReturnFromClientIndicatorRepository() : base()
        {
        }

        /// <summary>
        /// Получение списка показателей по параметрам >= указанной даты
        /// </summary> 
        public IEnumerable<T> GetFrom(DateTime date, int dealId, int returnFromClientWaybillCuratorId, ISubQuery batchSubQuery)
        {
            return Query<T>()
                .PropertyIn(x => x.BatchId, batchSubQuery)
                .Where(x => x.DealId == dealId && x.ReturnFromClientWaybillCuratorId == returnFromClientWaybillCuratorId &&
                    (x.EndDate > date || x.EndDate == null))
                .ToList<T>();
        }

        /// <summary>
        /// Получение по сделке кол-ва возвращенного товара по каждому из товаров
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <returns>Словарь: Код товара / кол-во возвращенного товара </returns>
        public DynamicDictionary<int, decimal> GetReturnedCountByArticle(int dealId, short teamId, DateTime date)
        {
            return Query<T>()
                .Where(x => x.DealId == dealId && x.TeamId == teamId &&
                    x.ReturnedCount != 0 &&
                    x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .GroupBy(x => x.ArticleId)
                .Sum(true, x => x.ReturnedCount)
                .ToList(x => new { ArticleId = (int)x[0], ReturnedCount = (decimal)x[1] })
                .ToDynamicDictionary(x => x.ArticleId, x => x.ReturnedCount);
        }

        /// <summary>
        /// Получение списка показателей на определенную дату
        /// </summary>
        /// <param name="date">Дата, на которую происходит выборка</param>
        /// <param name="saleIdList">Коллекция кодов реализаций</param>
        public List<T> GetList(DateTime date, IEnumerable<Guid> saleIdList)
        {
            return CurrentSession.Query<T>()
                .Where(x => x.ReturnedCount != 0 && x.StartDate <= date && (x.EndDate > date || x.EndDate == null))
                .Where(x => saleIdList.Contains(x.SaleWaybillId))
                .ToList();
        }
    }
}
