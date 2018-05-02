using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DealDebitInitialBalanceCorrectionRepository : BaseNHRepository, IDealDebitInitialBalanceCorrectionRepository
    {
        public DealDebitInitialBalanceCorrectionRepository()
        {
        }

        public DealDebitInitialBalanceCorrection GetById(Guid id)
        {
            return Query<DealDebitInitialBalanceCorrection>().Where(x => x.Id == id).FirstOrDefault<DealDebitInitialBalanceCorrection>();
        }

        /// <summary>
        /// Получение списка сущностей по Id с учетом подкритерия для видимости
        /// </summary>
        /// <param name="idList">Список идентификаторов сущности</param>
        /// <returns>Словарь сущностей</returns>
        public IDictionary<Guid, DealDebitInitialBalanceCorrection> GetById(IEnumerable<Guid> idList, ISubCriteria<Deal> dealSubQuery)
        {
            return Query<DealDebitInitialBalanceCorrection>()
                .OneOf(x => x.Id, idList.Distinct())
                .PropertyIn(x => x.Deal, dealSubQuery)
                .ToList<DealDebitInitialBalanceCorrection>()
                .ToDictionary(x => x.Id);
        }

        public void Save(DealDebitInitialBalanceCorrection value)
        {
            CurrentSession.SaveOrUpdate(value);
            CurrentSession.Flush();
        }

        public void Delete(DealDebitInitialBalanceCorrection value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public IList<DealDebitInitialBalanceCorrection> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealDebitInitialBalanceCorrection>(state, ignoreDeletedRows);
        }
        public IList<DealDebitInitialBalanceCorrection> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<DealDebitInitialBalanceCorrection>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получить список не полностью оплаченных дебетовых корректировок сальдо по списку сделок
        /// (отсортированные по дате, затем по дате создания по возрастанию)
        /// </summary>
        /// <param name="dealIdList">Список кодов сделок</param>
        /// <param name="teamId">Код команды</param>
        /// <returns>Список дебетовых корректировок сальдо</returns>
        public IEnumerable<DealDebitInitialBalanceCorrection> GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(IEnumerable<int> dealIdList, short teamId)
        {
            return Query<DealDebitInitialBalanceCorrection>()
                .Where(x => x.IsFullyDistributed == false && x.Team.Id == teamId)
                .OneOf(x => x.Deal.Id, dealIdList)
                .OrderByAsc(x => x.Date)
                .OrderByAsc(x => x.CreationDate)
                .ToList<DealDebitInitialBalanceCorrection>()
                .Distinct();
        }
    }
}
