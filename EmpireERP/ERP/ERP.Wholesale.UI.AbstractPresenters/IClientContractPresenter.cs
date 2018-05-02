using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Client;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.ClientContract;
using ERP.Wholesale.UI.ViewModels.Deal;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IClientContractPresenter
    {
        /// <summary>
        /// Выбор договора с организацией клиента.
        /// </summary>
        /// <param name="dealId">Идентификатор сделки, для которой выбирается договор.</param>
        /// <param name="currentUser">Информация о текущем пользователе.</param>
        /// <returns>ViewModel для модальной формы выбора договора с организацией клиента.</returns>
        ClientContractSelectViewModel Select(int dealId, UserInfo currentUser);

        /// <summary>
        /// Грид списка договоров с клиентом для выбора.
        /// </summary>
        /// <param name="state">Состояние грида.</param>        
        GridData GetSelectGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Сохранение договора с клиентом.
        /// </summary>
        /// <param name="model">Модель представления.</param>
        /// <param name="currentUser">Пользователь, совершающий операцию.</param>
        object SaveContract(ClientContractEditViewModel model, UserInfo currentUser);

        ClientContractEditViewModel EditContract(short contractId, UserInfo currentUser);

        bool IsUsedBySingleDeal(short clientContractId, int dealId, UserInfo currentUser);
    }
}
