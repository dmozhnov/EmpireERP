using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill;
using ERP.Wholesale.UI.ViewModels.WriteoffWaybill;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IWriteoffWaybillPresenter : IBaseWaybillPresenter<WriteoffWaybill>
    {
        WriteoffWaybillListViewModel List(UserInfo currentUser);

        GridData GetWriteoffPendingGrid(GridState state, UserInfo currentUser);
        GridData GetWrittenOffGrid(GridState state, UserInfo currentUser);

        WriteoffWaybillEditViewModel Create(string backURL, UserInfo currentUser);
        WriteoffWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser);
        string Save(WriteoffWaybillEditViewModel model, UserInfo currentUser);

        object GetAccountOrganizationsForStorage(short storageId);

        void Delete(Guid id, UserInfo currentUser);
        WriteoffWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser);

        GridData GetWriteoffWaybillRowGrid(GridState state, UserInfo currentUser);
        
        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        GridData GetWriteoffWaybillArticleGroupGrid(GridState state, UserInfo currentUser);

        WriteoffWaybillRowEditViewModel AddRow(Guid writeoffWaybillId, UserInfo currentUser);
        OutgoingWaybillAddRowsByListViewModel AddRowsByList(Guid waybillId, string backURL, UserInfo currentUser);
        void AddRowSimply(Guid waybillId, int articleId, decimal count, UserInfo currentUser);
        GridData GetArticlesForWaybillRowsAdditionByListGrid(GridState state, UserInfo currentUser);
        WriteoffWaybillRowEditViewModel EditRow(Guid writeoffWaybillId, Guid writeoffWaybillRowId, UserInfo currentUser);
        object GetRowInfo(Guid waybillId, Guid batchId, UserInfo currentUser);
        object SaveRow(WriteoffWaybillRowEditViewModel model, UserInfo currentUser);

        object DeleteRow(Guid writeoffWaybillId, Guid writeoffWaybillRowId, UserInfo currentUser);

        object PrepareToAccept(Guid id, UserInfo currentUser);
        object CancelReadinessToAccept(Guid id, UserInfo currentUser);

        object Accept(Guid id, UserInfo currentUser);
        object CancelAcceptance(Guid id, UserInfo currentUser);

        GridData GetDocumentGrid(GridState state = null);
        object Writeoff(Guid writeoffWaybillId, UserInfo currentUser);
        object CancelWriteoff(Guid writeoffWaybillId, UserInfo currentUser);

        WriteoffWaybillPrintingFormSettingsViewModel GetWriteoffWaybillPrintingFormSettings(Guid waybillId, UserInfo currentUser);
        WriteoffWaybillPrintingFormViewModel GetWriteoffWaybillPrintingForm(WriteoffWaybillPrintingFormSettingsViewModel settings, UserInfo currentUser);
    }
}