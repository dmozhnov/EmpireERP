using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IArticlePurchaseService
    {
        /// <summary>
        /// Пересчет показателей закупок при проводке накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть еще в состоянии "Проведена".</param>
        void ReceiptWaybillAccepted(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет показателей закупок при отмене проводки накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода долна быть еще в состоянии "Проведена".</param>
        void ReceiptWaybillAcceptanceCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет показателей закупок при приемке накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Принята".</param>
        void ReceiptWaybillReceipted(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет показателей закупок при отмене приемки накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Принята".</param>
        void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет показателей закупок при согласовании накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Согласована".</param>
        void ReceiptWaybillApproved(ReceiptWaybill waybill);

        /// <summary>
        /// Пересчет показателей закупок при отмене согласования накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Согласована".</param>
        void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill);

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя. 
        /// Будут учтены суммы тех закупок, которые относятся к контрагенту <paramref name="contractor"/> и в то же время к одной из организаций из списка <paramref name="contractorOrganizations"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="contractorOrganizations">Список организаций контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор организации контрагента, значение - сумма закупок этой организации для контрагента <paramref name="contractor"/>.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        DynamicDictionary<int, decimal> GetTotalPurchaseCostSum(Contractor contractor, IEnumerable<ContractorOrganization> contractorOrganizations, User user);

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к контрагенту <paramref name="contractor"/> и в то же время к одному из договоров из списка <paramref name="contracts"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="contracts">Список договоров контрагента (поставщика или производителя).</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок по этому договору для контрагента <paramref name="contractor"/>.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        DynamicDictionary<short, decimal> GetTotalPurchaseCostSum(Contractor contractor, IEnumerable<Contract> contracts, User user);      

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем закупкам контрагента <paramref name="contractor"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        decimal GetTotalPurchaseCostSum(Contractor contractor, User user);

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к одному из контрагентов из списка <paramref name="contractors"/>.
        /// </summary>
        /// <param name="contractors">Список контрагентов.</param>        
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор контрагента, значение - сумма закупок для этого контрагента.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        DynamicDictionary<int, decimal> GetTotalPurchaseCostSum(IEnumerable<Contractor> contractors, User user);

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к одному из договоров из списка <paramref name="contracts"/>.
        /// </summary>
        /// <param name="contracts">Список договоров с контрагентами (поставщиками или производителями).</param>        
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        DynamicDictionary<short, decimal> GetTotalPurchaseCostSum(IEnumerable<Contract> contracts, User user);

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем закупкам организации контрагента <paramref name="contractorOrganization"/>.
        /// </summary>
        /// <param name="contractorOrganization">Организация контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        decimal GetTotalPurchaseCostSum(ContractorOrganization contractorOrganization, User user);

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем ожидающим поставки закупкам контрагента <paramref name="contractor"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются только закупки, ожидающие поставки (еще не принятые).</remarks>
        decimal GetPendingPurchaseCostSum(Contractor contractor, User user);

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем ожидающим поставки закупкам организации контрагента <paramref name="contractorOrganization"/>.
        /// </summary>
        /// <param name="contractorOrganization">Организация контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются только закупки, ожидающие поставки (еще не принятые).</remarks>
        decimal GetPendingPurchaseCostSum(ContractorOrganization contractorOrganization, User user);        
    }
}
