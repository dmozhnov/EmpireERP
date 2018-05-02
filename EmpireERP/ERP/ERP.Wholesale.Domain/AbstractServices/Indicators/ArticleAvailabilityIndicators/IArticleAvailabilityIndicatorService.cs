using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticleAvailabilityIndicatorService<T> where T : ArticleAvailabilityIndicator
    {
        /// <summary>
        /// Обновление значения показателя
        /// </summary>
        void Update(short storageId, int accountOrganizationId, ISubQuery batchSubquery, IEnumerable<T> indicators);

        /// <summary>
        /// Установка закупочных цен по заданной приходной накладной из 0 в заданные значения (из позиций приходной накладной)
        /// </summary>
        /// <param name="receiptWaybill"></param>
        void SetPurchaseCosts(ReceiptWaybill receiptWaybill);

        /// <summary>
        /// Сброс закупочных цен по заданной приходной накладной в 0
        /// </summary>
        /// <param name="receiptWaybill"></param>
        void ResetPurchaseCosts(ReceiptWaybill receiptWaybill);

        /// <summary>
        /// Получение списка показателей по параметрам
        /// </summary>
        /// <param name="storageIds">Подзапрос для списка МХ</param>
        /// <param name="articleIds">Подзапрос для списка товаров</param>
        /// <param name="date"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(ISubQuery storageIds, ISubQuery articleIds, DateTime date);

        /// <summary>
        /// Получение списка показателей по параметрам по всем товарам
        /// </summary>
        /// <param name="storageIds"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(ISubQuery storageIds, DateTime date);

        /// <summary>
        /// Проверка наличия товаров по параметрам на указанную дату
        /// </summary>        
        bool IsArticleAvailability(short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение списка показателей по МХ и организации на указанную дату
        /// </summary>
        IEnumerable<T> GetList(short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение показателя по партии, МХ и организации на указанную дату
        /// </summary>
        T GetList(Guid articleBatchId, short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение списка показателей по товару, МХ и организации на указанную дату
        /// </summary>        
        IEnumerable<T> GetList(int articleId, short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение списка показателей по списку товаров, МХ и организации на указанную дату
        /// </summary>
        IEnumerable<T> GetList(IEnumerable<int> articleIdList, short storageId, int accountOrganizationId, DateTime date);

        /// <summary>
        /// Получение списка показателей по списку партий, МХ и организации на дату
        /// </summary>
        IEnumerable<T> GetList(ISubQuery articleBatchIds, short storageId, int accountOrganizationId, DateTime date);
    }
}
