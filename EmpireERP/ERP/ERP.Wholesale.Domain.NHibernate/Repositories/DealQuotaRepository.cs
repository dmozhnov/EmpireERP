using System;
using System.Linq;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealQuotaRepository : BaseRepository, IDealQuotaRepository
    {
        public DealQuotaRepository()
        {
        }

        public DealQuota GetById(int id)
        {
            return CurrentSession.Get<DealQuota>(id);
        }

        public void Save(DealQuota entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(DealQuota entity)
        {
            entity.DeletionDate = DateTime.Now;

            CurrentSession.SaveOrUpdate(entity);
        }

        public IList<DealQuota> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealQuota>(state, ignoreDeletedRows);
        }
        public IList<DealQuota> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealQuota>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Определяем собственный метод фильтрации организаций контрагента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected override void CreateFilter<T>(Infrastructure.Repositories.Criteria.ICriteria<T> criteria)
        {
            Infrastructure.Repositories.Criteria.ICriteria<DealQuota> query = (Infrastructure.Repositories.Criteria.ICriteria<DealQuota>)criteria;

            ParameterString df = parameterString;

            if (df["DealId"] != null && df["DealId"].Value != null)
            {
                string str = df["DealId"].Value is IList<string> ? (df["DealId"].Value as IList<string>)[0] : df["DealId"].Value as string;
                if (str.Length > 0)
                {
                    var dealId = ValidationUtils.TryGetInt(str);
                    var subQuery = SubQuery<Deal>().Where(x => x.Id == dealId);
                    subQuery.Restriction<DealQuota>(x => x.Quotas).Select(x => x.Id);

                    string modeStr = df["Mode"].Value is IList<string> ? (df["Mode"].Value as IList<string>)[0] : df["Mode"].Value as string;

                    switch(modeStr)
                    {
                        case "Sale":
                            query.PropertyIn(x => x.Id, subQuery); break;

                        case "Deal":
                            query.PropertyNotIn(x => x.Id, subQuery); break;

                        default:
                            throw new Exception("Неверное значение входного параметра.");
                    }                    
                }
            }

            df.Delete("DealId");
            df.Delete("Mode");

            base.CreateFilter<T>(criteria);            
        }

        /// <summary>
        /// Подгрузка квот по списку реализаций
        /// </summary>
        /// <param name="sales">Список реализаций</param>
        /// <returns>Словарь квот</returns>
        public IDictionary<Guid, DealQuota> GetList(IEnumerable<Guid> saleIds)
        {
            return CurrentSession.Query<SaleWaybill>()
                .Where(x => saleIds.Contains(x.Id))
                .Select(x => new { Quota = x.Quota, SaleId = x.Id })
                .ToList()
                .ToDictionary(x=>x.SaleId, x=>x.Quota);
                
        }
    }
}
