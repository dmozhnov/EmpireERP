using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.Common;
using ERP.Wholesale.UI.ViewModels.ExpenditureWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IExpenditureWaybillPresenter : IBaseWaybillPresenter<ExpenditureWaybill>
    {
        ExpenditureWaybillListViewModel List(UserInfo currentUser);
        GridData GetNewAndAcceptedExpenditureWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetShippedExpenditureWaybillGrid(GridState state, UserInfo currentUser);

        void CheckPosibilityToCreate(int dealId, UserInfo currentUser);
        ExpenditureWaybillEditViewModel Create(string backURL, int? dealId, int? clientId, int? clientOrganizationId, UserInfo currentUser);
        ExpenditureWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser);
        Guid Save(ExpenditureWaybillEditViewModel model, UserInfo currentUser);
        void Delete(Guid id, UserInfo currentUser);

        ExpenditureWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser);
        GridData GetExpenditureWaybillRowGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        GridData GetExpenditureWaybillArticleGroupGrid(GridState state, UserInfo currentUser);

        GridData GetDocumentGrid(GridState state);

        ExpenditureWaybillRowEditViewModel CreateRow(Guid id, UserInfo currentUser);
        ExpenditureWaybillRowEditViewModel EditRow(Guid expenditureWaybillId, Guid expenditureWaybillRowId, UserInfo currentUser);
        object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser);
        object SaveRow(ExpenditureWaybillRowEditViewModel model, UserInfo currentUser);
        object DeleteRow(Guid expenditureWaybillId, Guid expenditureWaybillRowId, UserInfo currentUser);

        object PrepareToAccept(Guid id, UserInfo currentUser);
        object CancelReadinessToAccept(Guid id, UserInfo currentUser);

        object Accept(Guid id, UserInfo currentUser);
        object CancelAcceptance(Guid id, UserInfo currentUser);

        DateTimeSelectViewModel GetRetroactivelyAcceptanceViewModel(UserInfo currentUser);
        object AcceptRetroactively(Guid waybillId, DateTime acceptanceDate, UserInfo currentUser);

        object Ship(Guid id, UserInfo currentUser);
        object CancelShipping(Guid id, UserInfo currentUser);

        DateTimeSelectViewModel GetRetroactivelyShippingViewModel(UserInfo currentUser);
        object ShipRetroactively(Guid waybillId, DateTime shippingDate, UserInfo currentUser);

        OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser);
        GridData GetArticlesForWaybillRowsAdditionByListGrid(GridState state, UserInfo currentUser);
        void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser);

        CashMemoPrintingFormViewModel GetCashMemoPrintingForm(Guid id, UserInfo currentUser);
        ExpenditureWaybillPrintingFormViewModel GetExpenditureWaybillPrintingForm(ExpenditureWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser);
        ExpenditureWaybillPrintingFormSettingsViewModel GetExpenditureWaybillPrintingFormSettings(Guid waybillId, UserInfo currentUser);
        
        /// <summary>
        /// Получение модели параметров счет-фактуры
        /// </summary>        
        InvoicePrintingFormSettingsViewModel GetInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser);
        InvoicePrintingFormViewModel GetInvoicePrintingForm(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);
        InvoicePrintingFormRowsViewModel GetInvoicePrintingFormRows(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Экспорт счет-фактуры в Excel
        /// </summary>
        byte[] ExportInvoicePrintingFormToExcel(InvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);

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

        PaymentInvoicePrintingFormSettingsViewModel GetPaymentInvoicePrintingFormSettings(Guid waybillId, UserInfo currentUser);
        PaymentInvoicePrintingFormViewModel GetPaymentInvoicePrintingForm(PaymentInvoicePrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Назначить накладной реализации товаров квоту.
        /// </summary>
        /// <param name="expenditureWaybillId">Идентификатор накладной реализации товаров.</param>
        /// <param name="dealQuotaId">Идентификатор квоты по сделке.</param>
        /// <param name="currentUser">Пользователь, выполняющий операцию.</param>        
        object SetDealQuota(Guid expenditureWaybillId, int dealQuotaId, UserInfo currentUser);

        #region T1 (TTH)

        /// <summary>
        /// Форма настройки печати Т1 (ТТН)
        /// </summary>
        /// <param name="waybillId">Код накладной</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель настроек</returns>
        T1PrintingFormSettingsViewModel GetT1PrintingFormSettings(Guid waybillId, UserInfo currentUser);

        /// <summary>
        /// Формирование общей части формы Т1
        /// </summary>
        /// <param name="settings">Настройки печати</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель</returns>
        T1ProductSectionPrintingFormViewModel ShowT1ProductSectionPrintingForm(T1PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение позиций для формы Т1
        /// </summary>
        /// <param name="settings">Настройки печати</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель позиций</returns>
        T1ProductSectionPrintingFormRowsViewModel ShowT1ProductSectionPrintingFormRows(T1PrintingFormSettingsViewModel settings, UserInfo currentUser);
        
        #endregion
    }
}
