using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill;
using ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReturnFromClientWaybillPresenter : IBaseWaybillPresenter<ReturnFromClientWaybill>
    {
        ReturnFromClientWaybillListViewModel List(UserInfo currentUser);
        GridData GetNewAndAcceptedReturnFromClientWaybillGrid(GridState state, UserInfo currentUser);
        GridData GetReceiptedReturnFromClientWaybillGrid(GridState state, UserInfo currentUser);

        ReturnFromClientWaybillSelectViewModel SelectWaybill(int articleId, UserInfo currentUser);
        GridData GetWaybillSelectGrid(GridState state, UserInfo currentUser);

        ReturnFromClientWaybillEditViewModel Create(int? clientid, int? dealId, string backURL, UserInfo currentUser);
        ReturnFromClientWaybillEditViewModel Edit(Guid id, string backURL, UserInfo currentUser);
        Guid Save(ReturnFromClientWaybillEditViewModel model, UserInfo currentUser);
        void Delete(Guid id, UserInfo currentUser);
        
        object GetStorageListForAccountOrganization(int accountOrganizationId, UserInfo currentUser);
        object GetReturnFromClientReasonList(UserInfo currentUser);
        
        ReturnFromClientWaybillDetailsViewModel Details(Guid id, string backURL, UserInfo currentUser);
        GridData GetDocumentGrid(GridState state);
        GridData GetReturnFromClientWaybillRowGrid(GridState state, UserInfo currentUser);
        /// <summary>
        /// Получить грид с группами товаров
        /// </summary>
        GridData GetReturnFromClientWaybillArticleGroupGrid(GridState state, UserInfo currentUser);
                
        ReturnFromClientWaybillRowEditViewModel AddRow(Guid id, UserInfo currentUser);
        ReturnFromClientWaybillRowEditViewModel EditRow(Guid returnFromClientWaybillId, Guid returnFromClientWaybillRowId, UserInfo currentUser);
        object SaveRow(ReturnFromClientWaybillRowEditViewModel model, UserInfo currentUser);
        object DeleteRow(Guid returnFromClientWaybillId, Guid returnFromClientWaybillRowId, UserInfo currentUser);

        object PrepareToAccept(Guid id, UserInfo currentUser);
        object CancelReadinessToAccept(Guid id, UserInfo currentUser);
        object Accept(Guid id, UserInfo currentUser);
        object CancelAcceptance(Guid id, UserInfo currentUser);
        object Receipt(Guid id, UserInfo currentUser);
        object CancelReceipt(Guid id, UserInfo currentUser);

        /// <summary>
        /// Получение списка команд для возврата товара по сделке
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        object GetTeamListForReturnFromClientWaybill(int dealId, UserInfo currentUser);

        /// <summary>
        /// Получение модели параметров ТОРГ12
        /// </summary>
        TORG12PrintingFormSettingsViewModel GetTORG12PrintingFormSettings(Guid waybillId, UserInfo currentUser);

        /// <summary>
        /// Экспорт ТОРГ-12 в Excel
        /// </summary>
        byte[] ExportTORG12PrintingFormToExcel(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение модели печатной формы ТОРГ 12
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        TORG12PrintingFormViewModel GetTORG12PrintingForm(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);

        /// <summary>
        /// Получение информации о строках печатной формы
        /// </summary>
        TORG12PrintingFormRowsViewModel GetTORG12PrintingFormRows(TORG12PrintingFormSettingsViewModel settings, UserInfo currentUser);
    }
}
