using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using System.Collections.Generic;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IReturnFromClientService
    {
        /// <summary>
        /// Пересчет показателей возвратов от клиентов при проводке накладной возврата от клиента
        /// </summary>
        void ReturnFromClientWaybillAccepted(ReturnFromClientWaybill waybill);
        
        /// <summary>
        /// Пересчет показателей возвратов от клиентов при отмене проводки накладной возврата от клиента
        /// </summary>
        void ReturnFromClientWaybillAcceptanceCancelled(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Пересчет показателей возвратов от клиентов при приемке накладной возврата от клиента
        /// </summary>
        void ReturnFromClientWaybillFinalized(ReturnFromClientWaybill waybill);
        
        /// <summary>
        /// Пересчет показателей возвратов от клиентов при отмене приемки накладной возврата от клиента
        /// </summary>
        void ReturnFromClientWaybillFinalizationCancelled(ReturnFromClientWaybill waybill);

        /// <summary>
        /// Получить фильтрованный список товаров, доступных к возврату
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns>Список товаров</returns>
        IEnumerable<Article> GetAvailableToReturnArticleFilteredList(object state, Deal deal, Team team);

        /// <summary>
        /// Расчет суммы накладной реализации с учетом всех сделанных по ней возвратов
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        decimal CalculateSaleWaybillCostWithReturns(SaleWaybill sale, ReturnFromClientWaybill returnFromClientWaybillToExclude = null);
    }
}
