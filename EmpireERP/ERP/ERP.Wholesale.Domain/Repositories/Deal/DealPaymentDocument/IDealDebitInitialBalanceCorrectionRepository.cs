using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IDealDebitInitialBalanceCorrectionRepository : IRepository<DealDebitInitialBalanceCorrection, Guid>
    {
        /// <summary>
        /// Получение списка сущностей по Id с учетом подкритерия для видимости
        /// </summary>
        /// <param name="idList">Список идентификаторов сущности</param>
        /// <returns>Словарь сущностей</returns>
        IDictionary<Guid, DealDebitInitialBalanceCorrection> GetById(IEnumerable<Guid> idList, ISubCriteria<Deal> dealSubQuery);

        /// <summary>
        /// Получить список не полностью оплаченных дебетовых корректировок сальдо по списку сделок
        /// (отсортированные по дате, затем по дате создания по возрастанию)
        /// </summary>
        /// <param name="dealIdList">Список кодов сделок</param>
        /// <param name="teamId">Код команды</param>
        /// <returns>Список дебетовых корректировок сальдо</returns>
        IEnumerable<DealDebitInitialBalanceCorrection> GetDealDebitInitialBalanceCorrectionListForDealPaymentDocumentDistribution(IEnumerable<int> dealIdList, short teamId);
    }
}
