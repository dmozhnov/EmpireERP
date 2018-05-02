using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.AbstractServices.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Indicators.PurchaseIndicators;
using ERP.Wholesale.Domain.Repositories.Indicators.PurchaseIndicators;

namespace ERP.Wholesale.Domain.Services
{
    public class ArticlePurchaseService : IArticlePurchaseService
    {
        #region Поля

        private readonly IAcceptedPurchaseIndicatorService acceptedPurchaseIndicatorService;
        private readonly IApprovedPurchaseIndicatorService approvedPurchaseIndicatorService;
        private readonly IAcceptedPurchaseIndicatorRepository acceptedPurchaseIndicatorRepository;
        private readonly IApprovedPurchaseIndicatorRepository approvedPurchaseIndicatorRepository;
        
        #endregion

        #region Конструкторы

        public ArticlePurchaseService(IAcceptedPurchaseIndicatorService acceptedPurchaseIndicatorService, IApprovedPurchaseIndicatorService approvedPurchaseIndicatorService,
            IAcceptedPurchaseIndicatorRepository acceptedPurchaseIndicatorRepository, IApprovedPurchaseIndicatorRepository approvedPurchaseIndicatorRepository)
        {
            this.acceptedPurchaseIndicatorService = acceptedPurchaseIndicatorService;
            this.approvedPurchaseIndicatorService = approvedPurchaseIndicatorService;
            this.acceptedPurchaseIndicatorRepository = acceptedPurchaseIndicatorRepository;
            this.approvedPurchaseIndicatorRepository = approvedPurchaseIndicatorRepository;
        }

        #endregion

        #region Методы

        #region Пересчет показателей закупок

        #region Проводка накладной закупки

        /// <summary>
        /// Пересчет показателей закупок при проводке накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода долна быть еще в состоянии "Проведена".</param>
        public void ReceiptWaybillAccepted(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, waybill.AcceptanceDate.Value, 1);
        }

        /// <summary>
        /// Пересчет показателей закупок при отмене проводки накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода долна быть еще в состоянии "Проведена".</param>
        public void ReceiptWaybillAcceptanceCancelled(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill,waybill.AcceptanceDate.Value, -1);
        }

        /// <summary>
        /// Обновление показателя "проведенных закупок".
        /// </summary>
        /// <param name="waybill">Накладная прихода.</param>
        /// <param name="date">Дата начала действия индикатора.</param>
        /// <param name="sign">Коэффициент, определяющий направление изменения индикатора. Принимает значение 1, или -1. При значении 1 индикатор будет увеличен, -1 — уменьшен.</param>
        /// <param name="rowsFilter">Фильтр позиций накладной. Должен возвращать true, если по этой позиции создавать индикатор и false в противном случае.</param>
        private void UpdateAcceptedIndicators(ReceiptWaybill waybill, DateTime date, short sign, Func<ReceiptWaybillRow, bool> rowsFilter = null)
        {
            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var acceptedIndicators = new List<AcceptedPurchaseIndicator>();            

            var rows = rowsFilter != null ? waybill.Rows.Where(x => rowsFilter(x)) : waybill.Rows;

            foreach (var item in rows)
            {
                var acceptedIndicator = CreateAcceptedPurchaseIndicator(date, item, sign);

                acceptedIndicators.Add(acceptedIndicator);
            }

            acceptedPurchaseIndicatorService.Update(date, waybill.Curator.Id, waybill.ReceiptStorage.Id, waybill.Contract.Id,
                waybill.Contractor.Id, waybill.AccountOrganization.Id, waybill.ContractorOrganization.Id, acceptedIndicators);
        }

        /// <summary>
        /// Создание показателя проведенных накладных реализации по параметрам.
        /// </summary>
        /// <param name="startDate">Дата начала действия показателя.</param>
        /// <param name="row">Позиция, по которой создавать индикатор.</param>
        /// <param name="sign">Коэффициент, определяющий направление изменения индикатора. Принимает значение 1, или -1. При значении 1 индикатор будет увеличен, -1 — уменьшен.</param>        
        private AcceptedPurchaseIndicator CreateAcceptedPurchaseIndicator(DateTime startDate, ReceiptWaybillRow row, short sign)
        {
            var waybill = row.ReceiptWaybill;

            return new AcceptedPurchaseIndicator(startDate, row.Article.Id, waybill.Curator.Id, waybill.Contractor.Id, waybill.ReceiptStorage.Id,
                waybill.AccountOrganization.Id,
                waybill.ContractorOrganization.Id,
                waybill.Contract.Id,
                sign * Math.Round(row.PurchaseCost * row.CurrentCount, 6),
                sign * row.CurrentCount);
        }


        #endregion

        #region Прием накладной закупки

        /// <summary>
        /// Пересчет показателей закупок при приемке накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Принята".</param>
        public void ReceiptWaybillReceipted(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, waybill.ReceiptDate.Value, -1, x => x.AreDivergencesAfterReceipt);
            UpdateApprovedIndicators(waybill, waybill.ReceiptDate.Value, 1, x => !x.AreDivergencesAfterReceipt);
        }

        /// <summary>
        /// Пересчет показателей закупок при отмене приемки накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Принята".</param>
        public void ReceiptWaybillReceiptCancelled(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, waybill.ReceiptDate.Value, 1, x => x.AreDivergencesAfterReceipt);
            UpdateApprovedIndicators(waybill, waybill.ReceiptDate.Value, -1, x => !x.AreDivergencesAfterReceipt);
        }

        /// <summary>
        /// Пересчет показателей закупок при согласовании накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Согласована".</param>
        public void ReceiptWaybillApproved(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, waybill.ApprovementDate.Value, 1, x => x.AreDivergencesAfterReceipt);
            UpdateApprovedIndicators(waybill, waybill.ApprovementDate.Value, 1, x => x.AreDivergencesAfterReceipt);
        }

        /// <summary>
        /// Пересчет показателей закупок при отмене согласования накладной прихода.
        /// </summary>
        /// <param name="waybill">Накладная прихода. На момент вызова метода должна быть в состоянии "Согласована".</param>
        public void ReceiptWaybillApprovementCancelled(ReceiptWaybill waybill)
        {
            UpdateAcceptedIndicators(waybill, waybill.ApprovementDate.Value, -1, x => x.AreDivergencesAfterReceipt);
            UpdateApprovedIndicators(waybill, waybill.ApprovementDate.Value, -1, x => x.AreDivergencesAfterReceipt);

            if(!waybill.Rows.Any(x => x.AreDivergencesAfterReceipt))
            {
                UpdateApprovedIndicators(waybill, waybill.ReceiptDate.Value, -1, x => !x.AreDivergencesAfterReceipt);
            }
        }

        /// <summary>
        /// Создание показателя отгруженных накладных реализации по параметрам
        /// </summary>        
        /// <param name="startDate">Дата начала действия показателя.</param>
        /// <param name="row">Позиция, по которой создавать индикатор.</param>
        /// <param name="sign">Коэффициент, определяющий направление изменения индикатора. Принимает значение 1, или -1. При значении 1 индикатор будет увеличен, -1 — уменьшен.</param>     
        private ApprovedPurchaseIndicator CreateApprovedPurchaseIndicator(DateTime startDate, ReceiptWaybillRow row, short sign)
        {
            var waybill = row.ReceiptWaybill;

            return new ApprovedPurchaseIndicator(startDate, row.Article.Id, waybill.Curator.Id, waybill.Contractor.Id, waybill.ReceiptStorage.Id,
                waybill.AccountOrganization.Id, 
                waybill.ContractorOrganization.Id,
                waybill.Contract.Id,
                sign * Math.Round(row.PurchaseCost * row.CurrentCount, 6),
                sign * row.CurrentCount);
        }

        /// <summary>
        /// Обновление показателя "отгруженных закупок".
        /// </summary>
        /// <param name="waybill">Накладная прихода.</param>
        /// <param name="date">Дата начала действия индикатора.</param>
        /// <param name="sign">Коэффициент, определяющий направление изменения индикатора. Принимает значение 1, или -1. При значении 1 индикатор будет увеличен, -1 — уменьшен.</param>
        /// <param name="rowsFilter">Фильтр позиций накладной. Должен возвращать true, если по этой позиции создавать индикатор и false в противном случае.</param>
        private void UpdateApprovedIndicators(ReceiptWaybill waybill, DateTime date, short sign, Func<ReceiptWaybillRow, bool> rowsFilter = null )
        {
            var approvedIndicators = new List<ApprovedPurchaseIndicator>();            

            ValidationUtils.Assert(sign == 1 || sign == -1, "Значение множителя может быть только 1 или -1.");

            var rows = rowsFilter != null ? waybill.Rows.Where(x => rowsFilter(x)) : waybill.Rows;

            foreach(var item in rows)
            {             
                approvedIndicators.Add(CreateApprovedPurchaseIndicator(date, item, sign));             
            }

            approvedPurchaseIndicatorService.Update(date, waybill.Curator.Id, waybill.ReceiptStorage.Id, waybill.Contract.Id,
                waybill.Contractor.Id, waybill.AccountOrganization.Id, waybill.ContractorOrganization.Id, approvedIndicators);
        }

        #endregion

        #endregion       

        #region Получение показателей

        #region Для договора

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к одному из договоров из списка <paramref name="contracts"/>.
        /// </summary>
        /// <param name="contracts">Список договоров с контрагентами (поставщиками или производителями).</param>        
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок для этого договора.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public DynamicDictionary<short, decimal> GetTotalPurchaseCostSum(IEnumerable<Contract> contracts, User user)
        {
            return GetPurchaseCostSums((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContract(contracts.Select(x => x.Id)), user);
        }

        #endregion

        #region Для контрагента

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем ожидающим поставки закупкам контрагента <paramref name="contractor"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются только закупки, ожидающие поставки (еще не принятые).</remarks>
        public decimal GetPendingPurchaseCostSum(Contractor contractor, User user)
        {
            var acceptedSum = GetPurchaseCostSum((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractor(contractor.Id, storageIds, userId), user);
            var approvedSum = GetPurchaseCostSum((storageIds, userId) => approvedPurchaseIndicatorRepository.GetPurchaseCostSumByContractor(contractor.Id, storageIds, userId), user);

            return acceptedSum - approvedSum;
        }

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем закупкам контрагента <paramref name="contractor"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public decimal GetTotalPurchaseCostSum(Contractor contractor, User user)
        {
            return GetPurchaseCostSum((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractor(contractor.Id, storageIds, userId), user);
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к одному из контрагентов из списка <paramref name="contractors"/>.
        /// </summary>
        /// <param name="contractors">Список контрагентов.</param>        
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор контрагента, значение - сумма закупок для этого контрагента.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public DynamicDictionary<int, decimal> GetTotalPurchaseCostSum(IEnumerable<Contractor> contractors, User user)
        {
            return GetPurchaseCostSums((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractor(contractors.Select(x => x.Id), storageIds, userId), user);
        }

        #endregion

        #region Для организации контрагента

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем ожидающим поставки закупкам организации контрагента <paramref name="contractorOrganization"/>.
        /// </summary>
        /// <param name="contractorOrganization">Организация контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются только закупки, ожидающие поставки (еще не принятые).</remarks>
        public decimal GetPendingPurchaseCostSum(ContractorOrganization contractorOrganization, User user)
        {
            var acceptedSum = GetPurchaseCostSum((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractorOrganization(contractorOrganization.Id, storageIds, userId), user);
            var approvedSum = GetPurchaseCostSum((storageIds, userId) => approvedPurchaseIndicatorRepository.GetPurchaseCostSumByContractorOrganization(contractorOrganization.Id, storageIds, userId), user);

            return acceptedSum - approvedSum;
        }

        /// <summary>
        /// Рассчитать сумму закупочных цен по всем закупкам организации контрагента <paramref name="contractorOrganization"/>.
        /// </summary>
        /// <param name="contractorOrganization">Организация контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public decimal GetTotalPurchaseCostSum(ContractorOrganization contractorOrganization, User user)
        {
            return GetPurchaseCostSum((storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractorOrganization(contractorOrganization.Id, storageIds, userId), user);
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя. 
        /// Будут учтены суммы тех закупок, которые относятся к контрагенту <paramref name="contractor"/> и в то же время к одной из организаций из списка <paramref name="contractorOrganizations"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="contractorOrganizations">Список организаций контрагента.</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор организации контрагента, значение - сумма закупок этой организации для контрагента <paramref name="contractor"/>.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public DynamicDictionary<int, decimal> GetTotalPurchaseCostSum(Contractor contractor, IEnumerable<ContractorOrganization> contractorOrganizations, User user)
        {
            return GetPurchaseCostSums(
                (storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractorAndContractorOrganization(contractor.Id, contractorOrganizations.Select(x => x.Id), storageIds, userId), 
                user);
        }

        /// <summary>
        /// Рассчитать суммы закупочных цен с учетом прав пользователя.
        /// Будут учтены суммы тех закупок, которые относятся к контрагенту <paramref name="contractor"/> и в то же время к одному из договоров из списка <paramref name="contracts"/>.
        /// </summary>
        /// <param name="contractor">Контрагент.</param>
        /// <param name="contracts">Список договоров контрагента (поставщика или производителя).</param>
        /// <param name="user">Пользователь, совершающий операцию.</param>
        /// <returns>Словарь. Ключ - идентификатор договора с контрагентом, значение - сумма закупок по этому договору для контрагента <paramref name="contractor"/>.</returns>
        /// <remarks>Учитываются как принятые, так и ожидающие поставки закупки.</remarks>
        public DynamicDictionary<short, decimal> GetTotalPurchaseCostSum(Contractor contractor, IEnumerable<Contract> contracts, User user)
        {
            return GetPurchaseCostSums(
                (storageIds, userId) => acceptedPurchaseIndicatorRepository.GetPurchaseCostSumByContractorAndContract(contractor.Id, contracts.Select(x => x.Id), storageIds, userId),
                user);
        }

        #endregion

        /// <summary>
        /// Определяет сущности, для которых пользователь может видеть закупки.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        /// <param name="permission">Право, которое будет использоваться для определения видимости.</param>
        /// <returns>Сущности, чьи приходы может видеть пользователь. 
        /// Если первый параметр (storageIds) не равен null - пользователь может видеть только закупки на места хранения с идентификаторами из списка storageIds, иначе - все закупки.
        /// Если второй параметр (userId) не равен null - пользователь может видеть только закупки куратора с идентификатором userId. 
        /// Если возвращается null - пользователь не может видеть никаких закупок.
        /// </returns>
        private Tuple<IEnumerable<short>, int?> GetAvailableDistributionEntities (User user, Permission permission)
        {
            switch(user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.All:
                    return new Tuple<IEnumerable<short>, int?>(null, null);
                
                case PermissionDistributionType.Personal:
                    return new Tuple<IEnumerable<short>, int?>(user.Storages.Select(x => x.Id), user.Id);

                case PermissionDistributionType.Teams:
                    return new Tuple<IEnumerable<short>, int?>(user.Storages.Select(x => x.Id), null);

                case PermissionDistributionType.None:
                    return null;

                default:
                    throw new Exception("Неизвестный тип распространения права.");
            }           

        }

        private decimal GetPurchaseCostSum(Func<IEnumerable<short>, int?, decimal> action, User user)
        {
            var distributionEntities = GetAvailableDistributionEntities(user, Permission.PurchaseCost_View_ForReceipt);
            
            if (distributionEntities == null) return 0;

            return action(distributionEntities.Item1, distributionEntities.Item2);
        }

        private DynamicDictionary<TId, decimal> GetPurchaseCostSums<TId>(Func<IEnumerable<short>, int?, DynamicDictionary<TId, decimal>> action, User user)
        {
            var result = new DynamicDictionary<TId, decimal>();

            var distributionEntities = GetAvailableDistributionEntities(user, Permission.PurchaseCost_View_ForReceipt);

            if (distributionEntities == null) return result;

            return action(distributionEntities.Item1, distributionEntities.Item2);
        }

        #endregion

        #endregion
    }
}
