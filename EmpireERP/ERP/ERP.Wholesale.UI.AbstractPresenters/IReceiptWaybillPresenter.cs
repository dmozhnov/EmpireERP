using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.DivergenceAct;
using ERP.Wholesale.UI.ViewModels.ReceiptWaybill;
using ERP.Wholesale.UI.ViewModels.Common;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReceiptWaybillPresenter : IBaseWaybillPresenter<ReceiptWaybill>
    {
        ReceiptWaybillListViewModel List(UserInfo currentUser);
        GridData GetDeliveryPendingGrid(GridState state, UserInfo currentUser);
        GridData GetDivergenceWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetApprovedWaybillGrid(GridState state, UserInfo currentUser);

        GridData GetAddedRowInReceiptWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetDifRowInReceiptWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetMatchRowInReceiptWaybillGrid(GridState state, UserInfo currentUser);

        GridData GetApproveWaybillRowGrid(GridState state, UserInfo currentUser);

        GridData GetReceiptArticlesGrid(GridState state, UserInfo currentUser);

        GridData GetReceiptWaybillRowGrid(GridState state, UserInfo currentUser);

        GridData GetDocumentsGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Получить грид с группами товаров  для непринятой накладной 
        /// </summary>
        GridData GetReceiptWaybillArticleGroupGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров  для добавленных товаров
        /// </summary>
        GridData GetAddedArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров  для расхождений
        /// </summary>
        GridData GetDifArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров  для соответствий
        /// </summary>
        GridData GetMatchArticleGroupInReceiptWaybillGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров  для согласованной накладной
        /// </summary>
        GridData GetApproveWaybillArticleGroupGrid(GridState state, UserInfo currentUser);

        ReceiptWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser);
        GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser);

        ReceiptWaybillDetailsViewModel Details(string id, string backURL, UserInfo currentUser);
        ReceiptWaybillEditViewModel Create(int? providerId, string backUrl, UserInfo currentUser);
        ReceiptWaybillEditViewModel CreateFromProductionOrderBatch(Guid productionOrderBatchId, string backUrl, UserInfo currentUser);
        ReceiptWaybillEditViewModel Edit(Guid id, string backUrl, UserInfo currentUser);
        object Delete(Guid id, bool returnProductionOrderBatchDetails, out string message, UserInfo currentUser);
        string Save(ReceiptWaybillEditViewModel model, out string message, UserInfo currentUser);

        ReceiptWaybillRowEditViewModel AddRow(Guid waybilld, UserInfo currentUser);
        ReceiptWaybillRowEditViewModel EditRow(Guid waybillId, Guid rowId, UserInfo currentUser);
        object SaveRow(ReceiptWaybillRowEditViewModel model, UserInfo currentUser);
        object DeleteRow(Guid waybillId, Guid rowId, UserInfo currentUser);

        DateTimeSelectViewModel GetRetroactivelyReceiptViewModel(UserInfo currentUser);
        void ReceiptRetroactively(Guid waybillId, DateTime receiptDate, decimal? sum, out string message, UserInfo currentUser);
        ReceiptViewModel GetReceiptViewModel(Guid id, string backUrl, UserInfo currentUser);
        void PerformReceiption(Guid waybillId, decimal? sum, out string message, UserInfo currentUser);
        object CancelReceipt(Guid id, UserInfo currentUser);

        DateTimeSelectViewModel GetRetroactivelyAcceptanceViewModel(UserInfo currentUser);
        object Accept(Guid waybillId, UserInfo currentUser);
        object AcceptRetroactively(Guid waybillId, DateTime acceptanceDate, UserInfo currentUser);
        object CancelAcceptance(Guid waybillId, UserInfo currentUser);

        ReceiptRowAddViewModel AddWaybillRowFromReceipt(Guid id, UserInfo currentUser);
        void PerformWaybillRowAdditionFromReceipt(ReceiptRowAddViewModel model, UserInfo currentUser);

        void EditWaybillRowFromReceipt(Guid waybillGuid, Guid rowGuid, decimal receiptedCount, decimal providerCount, decimal providerSum, UserInfo currentUser);
        void DeleteWaybillRowFromReceipt(Guid waybillId, Guid rowId, UserInfo currentUser);

        DateTimeSelectViewModel GetRetroactivelyApprovementViewModel(UserInfo currentUser);
        void ApproveRetroactively(Guid waybillId, decimal sum, DateTime approvementDate, UserInfo currentUser);
        ApprovementViewModel GetApprovementViewModel(Guid waybillId, string backUrl, UserInfo currentUser);
        GridData GetApproveArticlesGrid(GridState state, UserInfo currentUser);
        object CancelApprovement(Guid waybillId, UserInfo currentUser);
        void EditWaybillRowFromApprovement(Guid waybillId, Guid rowId, decimal approvedCount, decimal purchaseCost, UserInfo currentUser);
        void EditWaybillRowValueAddedTaxFromApprovement(Guid waybillId, Guid rowId, short valueAddedTaxId, UserInfo currentUser);
        void PerformApprovement(Guid waybillGuid, decimal sum, UserInfo currentUser);

       
        object GetArticleInfo(int articleId);
        /// <summary>
        /// Получить последнюю по проводке закупочную цену на товар.
        /// </summary>
        string GetLastPurchaseCost(int articleId, Guid waybillId, UserInfo currentUser);
        
        /// <summary>
        /// Получить номер ГТД для позиции
        /// </summary>
        string GetCustomsDeclarationNumberForRow(int articleId, Guid waybillId, UserInfo currentUser);

        object GetManufacturerList();
        object GetCountryList();

        ReceiptWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(Guid waybillId, UserInfo currentUser);
        ReceiptWaybillPrintingFormViewModel GetPrintingForm(ReceiptWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser);

        DivergenceActPrintingFormViewModel GetDivergenceActPrintingForm(Guid waybillId, UserInfo currentUser);

        object GetContractList(int providerId, int receiptStorageId, int accountOrganizationId);
        object GetReceiptStorageList(int providerId, short contractId, int accountOrganizationId, UserInfo currentUser);
        object GetAccountOrganizationList(int providerId, short contractId, int receiptStorageId, UserInfo currentUser);

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser);
        TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);
        TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Экспорт ТОРГ-12 в Excel
        /// </summary>
        byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

    }
}
