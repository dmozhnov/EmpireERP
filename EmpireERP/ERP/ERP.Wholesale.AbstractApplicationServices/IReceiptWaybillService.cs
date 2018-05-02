using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IReceiptWaybillService: IBaseWaybillService<ReceiptWaybill>
    {
        ReceiptWaybillRow GetRowById(Guid id);
        IEnumerable<ReceiptWaybillRow> GetRows(ReceiptWaybill waybill);
        Dictionary<Guid, ReceiptWaybillRow> GetRows(IEnumerable<Guid> idList);
        
        IEnumerable<ReceiptWaybill> GetFilteredList(object state, User user, ParameterString param = null);

        Guid Save(ReceiptWaybill receiptWaybill, User user);
        void Delete(ReceiptWaybill receiptWaybill, DateTime currentDateTime, User user);

        void Accept(ReceiptWaybill waybill, DateTime currentDateTime, User acceptedBy);
        void AcceptRetroactively(ReceiptWaybill waybill, DateTime acceptanceDate, DateTime currentDateTime, User acceptedBy);
        void CancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user, DateTime currentDateTime);

        /// <summary>
        /// Создание накладной из партии заказа. Накладная создается со статусом "Новая". В нее добавляются позиции по позициям партии заказа
        /// </summary>
        ReceiptWaybill CreateReceiptWaybillFromProductionOrderBatch(ProductionOrderBatch productionOrderBatch, string number, DateTime date,
            ValueAddedTax pendingValueAddedTax, string customsDeclarationNumber, User curator, User createdBy, DateTime creationDate);

        /// <summary>
        /// Установить закупочные цены во всех индикаторах по позициям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная (цены должны быть уже вычислены и записаны в нее)</param>
        void SetPurchaseCosts(ReceiptWaybill receiptWaybill);

        /// <summary>
        /// Сбросить в 0 закупочные цены во всех индикаторах по позициям данной приходной накладной
        /// </summary>
        /// <param name="receiptWaybill">Приходная накладная</param>
        void ResetPurchaseCosts(ReceiptWaybill receiptWaybill);

        void PrepareReceiptWaybillForReceipt(ReceiptWaybill waybill);
        void Receipt(ReceiptWaybill waybill, decimal? sum, DateTime currentDateTime, User user);
        void ReceiptRetroactively(ReceiptWaybill waybill, decimal? sum, DateTime receiptDate, DateTime currentDateTime, User user);
        void CancelReceipt(ReceiptWaybill waybill, User user, DateTime currentDateTime);

        void PrepareReceiptWaybillForApprovement(ReceiptWaybill waybill, User user);
        void Approve(ReceiptWaybill waybill, decimal sum, DateTime currentDateTime, User user);
        void ApproveRetroactively(ReceiptWaybill waybill, decimal sum, DateTime approvementDate, DateTime currentDateTime, User user);
        void CancelApprovement(ReceiptWaybill waybill, User user, DateTime currentDateTime);

        void AddRow(ReceiptWaybill waybill, ReceiptWaybillRow waybillRow);
        void DeleteRow(ReceiptWaybill waybill, ReceiptWaybillRow waybillRow);

        decimal CalculateShippingPercent(ReceiptWaybill waybill);

        IEnumerable<Contract> GetContractList(int providerId, int receiptStorageId, int accountOrganizationId);
        IEnumerable<Storage> GetReceiptStorageList(int providerId, short contractId, int accountOrganizationId, User user);
        IEnumerable<AccountOrganization> GetAccountOrganizationList(int providerId, short contractId, int receiptStorageId);

        bool IsPossibilityToEdit(ReceiptWaybill waybill, User user, bool checkLogic = true);
        bool IsPossibilityToEditProviderDocuments(ReceiptWaybill waybill, User user);
        bool IsPossibilityToViewPurchaseCosts(ReceiptWaybill waybill, User user);
        bool IsPossibilityToDeleteRow(ReceiptWaybillRow row, User user);
        bool IsPossibilityToDelete(ReceiptWaybill waybill, User user);
        bool IsPossibilityToDeleteFromProductionOrderBatch(ReceiptWaybill waybill, User user);
        bool IsPossibilityToAccept(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToChangeDate(ReceiptWaybill waybill, User user);
        bool IsPossibilityToAcceptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToReceiptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToApproveRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);

        /// <summary>
        /// Есть ли возможность отменить проведение прихода
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="checkIfCreatedFromProductionOrderBatch">true - операция запрещается, если приход создан по партии заказа</param>
        /// <param name="user">Пользователь</param>
        bool IsPossibilityToCancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user);
        bool IsPossibilityToReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true);
        bool IsPossibilityToEditRowFromReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true);
        bool IsPossibilityToCancelReceipt(ReceiptWaybill waybill, User user);
        bool IsPossibilityToApprove(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        bool IsPossibilityToCancelApprovement(ReceiptWaybill waybill, User user);
        bool IsPossibilityToViewDetails(ReceiptWaybill waybill, User user);
        bool IsPossibilityToPrintForms(ReceiptWaybill waybill, User user);
        bool IsPossibilityToPrintDivergenceAct(ReceiptWaybill waybill, User user);
        bool IsPossibilityToPrintFormInRecipientAccountingPrices(ReceiptWaybill waybill, User user);
        bool IsPossibilityToPrintFormInPurchaseCosts(ReceiptWaybill waybill, User user);

        void CheckPossibilityToEdit(ReceiptWaybill waybill, User user, bool checkLogic = true);
        void CheckPossibilityToEditProviderDocuments(ReceiptWaybill waybill, User user);
        void CheckPossibilityToViewPurchaseCosts(ReceiptWaybill waybill, User user);
        void CheckPossibilityToDeleteRow(ReceiptWaybillRow row, User user);
        void CheckPossibilityToDelete(ReceiptWaybill waybill, User user);
        void CheckPossibilityToDeleteFromProductionOrderBatch(ReceiptWaybill waybill, User user);
        void CheckPossibilityToAccept(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToChangeDate(ReceiptWaybill waybill, User user);
        void CheckPossibilityToAcceptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToReceiptRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToApproveRetroactively(ReceiptWaybill waybill, User user, bool onlyPermission = false);

        /// <summary>
        /// Есть ли возможность отменить проведение прихода
        /// </summary>
        /// <param name="waybill">Приходная накладная</param>
        /// <param name="checkIfCreatedFromProductionOrderBatch">true - операция запрещается, если приход создан по партии заказа</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkOutgoingWaybills">Проверять ли на существование исходящих позиций</param>
        void CheckPossibilityToCancelAcceptance(ReceiptWaybill waybill, bool checkIfCreatedFromProductionOrderBatch, User user, bool checkOutgoingWaybills = false);
        void CheckPossibilityToReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true);
        void CheckPossibilityToEditRowFromReceipt(ReceiptWaybill waybill, User user, bool checkLogic = true);
        void CheckPossibilityToCancelReceipt(ReceiptWaybill waybill, User user, bool checkOutgoingWaybills = false);
        void CheckPossibilityToApprove(ReceiptWaybill waybill, User user, bool onlyPermission = false);
        void CheckPossibilityToCancelApprovement(ReceiptWaybill waybill, User user);
        void CheckPossibilityToViewDetails(ReceiptWaybill waybill, User user);
        void CheckPossibilityToPrintForms(ReceiptWaybill waybill, User user);        
        void CheckPossibilityToPrintDivergenceAct(ReceiptWaybill waybill, User user);
        void CheckPossibilityToPrintFormInRecipientAccountingPrices(ReceiptWaybill waybill, User user);
        void CheckPossibilityToPrintFormInPurchaseCosts(ReceiptWaybill waybill, User user);
        
        /// <summary>
        /// Все позиции приходов на один из указанных складов, с указанным товаром и в указанные сроки
        /// </summary>
        /// <param name="finallyMovedWaybillsOnly">Учитывать только закрытые накладные. 
        /// ПРИМЕЧАНИЕ: при true учитываются накладные, у которых startDate <= ReceiptDate <= endDate && CreationDate <= endDate
        /// </param>
        /// <returns></returns>
        IEnumerable<ReceiptWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);

        /// <summary>
        /// Все позиции приходов c расхождениями на один из указанных складов, с указанным товаром и в указанные сроки
        /// </summary>
        /// <param name="finallyMovedWaybillsOnly">Учитывать только закрытые накладные. 
        /// Так как метод используется только для построения Report0004, то для выборки действуют специфические правила: берутся те позиции, у которых в указанный период произошло согласование
        /// </param>
        /// <returns></returns>
        IEnumerable<ReceiptWaybillRow> GetDivergenceRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly);
    
        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// Если не найдено, то вернет 0.
        /// </summary>
        decimal GetLastPurchaseCost(Article article);

        /// <summary>
        /// Получить последнюю ГТД на товар.
        /// </summary>
        string GetLastCustomsDeclarationNumber(Article article);

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="providerIdList">Список кодов поставщиков</param>
        /// <param name="providerPermission">Право, которым определяются доступные поставщики</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        IEnumerable<ReceiptWaybill> GetList(ReceiptWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
             Permission curatorPermission, IEnumerable<int> providerIdList, Permission providerPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType
            , DateTime? priorToDate, User user);
    
        /// <summary>
        /// Получить строки накладных, удовлетворяющих условиям
        /// </summary>
        /// <param name="rowType">Тип отбора</param>
        /// <param name="startDate">Дата с которой отбираем</param>
        /// <param name="endDate">Дата до которой отбираем</param>
        IEnumerable<ReceiptWaybillRow> GetRowsForReport0009(Report0009RowType rowType, DateTime startDate, DateTime endDate, IEnumerable<short> storageIds,
            IEnumerable<short> articleGroupIds, IEnumerable<int> providerIds, IEnumerable<int> userIds, int pageNumber, int batchSize);

    }
}
