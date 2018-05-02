using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleSaleService
    {
        /// <summary>
        /// Пересчет показателей реализации при проводке накладной реализации
        /// </summary>
        void ExpenditureWaybillAccepted(ExpenditureWaybill waybill);
        
        /// <summary>
        /// Пересчет показателей реализации при отмене проводки накладной реализации
        /// </summary>
        void ExpenditureWaybillAcceptanceCancelled(ExpenditureWaybill waybill);
                
        /// <summary>
        /// Пересчет показателей реализации при отгрузке накладной реализации
        /// </summary>
        void ExpenditureWaybillShipped(ExpenditureWaybill waybill);

        /// <summary>
        /// Пересчет показателей реализации при отмене отгрузки накладной реализации
        /// </summary>
        void ExpenditureWaybillShippingCancelled(ExpenditureWaybill waybill);

        /// <summary>
        /// Получить коллекцию сущностей ArticleSaleAvailability, содержащую сведения о реализации и возвратах товаров
        /// </summary>
        /// <param name="article">Товар</param>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns></returns>
        IEnumerable<ArticleSaleAvailability> GetArticleSaleAvailability(Article article, Deal deal, Team team);
    }
}
