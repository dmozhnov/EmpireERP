using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels;
using ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IChangeOwnerWaybillPresenter : IBaseWaybillPresenter<ChangeOwnerWaybill>
    {
        #region Список

        ChangeOwnerWaybillListViewModel List(UserInfo currentUser);
        GridData GetChangeOwnerWaybillNewGrid(GridState state, UserInfo currentUser);
        GridData GetChangeOwnerWaybillAcceptedGrid(GridState state, UserInfo currentUser);

        #endregion

        #region Создание/Редактирование 

        ChangeOwnerWaybillEditViewModel Create(string backURL, UserInfo currentUser);
        Guid Save(ChangeOwnerWaybillEditViewModel model, UserInfo currentUser);
        ChangeOwnerWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser);
        object GetOrganizationList(short storageId);
        
        #endregion

        #region Детали

        ChangeOwnerWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser);
        
        GridData GetChangeOwnerWaybillRowGrid(GridState state, UserInfo currentUser);
        
        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        GridData GetChangeOwnerWaybillArticleGroupGrid(GridState state, UserInfo currentUser);

        GridData GetChangeOwnerWaybillDocsGrid(GridState state, UserInfo currentUser);

        void DeleteChangeOwnerWaybill(Guid id, UserInfo currentUser);

        #endregion

        #region Добавление/Редактирование строк накладной

        ChangeOwnerWaybillRowEditViewModel AddRow(Guid id, UserInfo currentUser);
        OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser);
        void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser);
        GridData GetArticlesForWaybillRowsAdditionByListGrid(GridState state, UserInfo currentUser);
        object SaveRow(ChangeOwnerWaybillRowEditViewModel model, UserInfo currentUser);
        ChangeOwnerWaybillRowEditViewModel EditRow(Guid waybillId, Guid waybillRowId, UserInfo currentUser);
        object DeleteRow(Guid waybillId, Guid waybillRowId, UserInfo currentUser);
        object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser);

        #endregion

        #region Подготовка/отмена готовности к проводке накладной

        object PrepareToAccept(Guid id, UserInfo currentUser);

        object CancelReadinessToAccept(Guid id, UserInfo currentUser);

        #endregion

        #region Смена получателя

        ChangeOwnerWaybillChangeRecipientViewModel ChangeRecipient(Guid id, UserInfo currentUser);
        object SaveNewRecipient(ChangeOwnerWaybillChangeRecipientViewModel model, UserInfo currentUser);

        #endregion

        #region Проводка/отмена проводки накладной

        object Accept(Guid Id, UserInfo currentUser);
        object CancelAcceptance(Guid Id, UserInfo currentUser);

        #endregion

        #region Печатные формы

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
        /// Получение модели параметров формы ТОРГ-12
        /// </summary>
        TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser);
        TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);
        TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Экспорт ТОРГ-12 в Excel
        /// </summary>
        byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        CashMemoPrintingFormViewModel GetCashMemoPrintingForm(Guid id, UserInfo currentUser);

        ChangeOwnerWaybillPrintingFormSettingsViewModel GetPrintingFormSettings(Guid waybillId, UserInfo currentUser);
        ChangeOwnerWaybillPrintingFormViewModel GetPrintingForm(ChangeOwnerWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser);

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

        #endregion

        #region Выбор накладной
        
        ChangeOwnerWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser);
        GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser);

        #endregion
    }
}
