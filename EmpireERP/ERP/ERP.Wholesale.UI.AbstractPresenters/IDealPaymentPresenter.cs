using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IDealPaymentPresenter
    {
        DealPaymentListViewModel List(UserInfo currentUser);
        GridData GetDealPaymentGrid(GridState state, UserInfo currentUser);

        void DeleteDealPaymentFromClient(Guid dealPaymentFromClientId, UserInfo currentUser);
        void DeleteDealPaymentToClient(Guid dealPaymentToClientId, UserInfo currentUser);        

        DealPaymentFromClientDetailsViewModel DealPaymentFromClientDetails(Guid paymentId, UserInfo currentUser);
        DealPaymentToClientDetailsViewModel DealPaymentToClientDetails(Guid paymentId, UserInfo currentUser);

        DealPaymentFromClientEditViewModel CreateDealPaymentFromClient(UserInfo currentUser);
        void SaveDealPaymentFromClient(DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        ClientOrganizationPaymentFromClientEditViewModel CreateClientOrganizationPaymentFromClient(UserInfo currentUser);
        void SaveClientOrganizationPaymentFromClient(DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel model, UserInfo currentUser);

        DealPaymentToClientEditViewModel CreateDealPaymentToClient(UserInfo currentUser);
        void SaveDealPaymentToClient(DealPaymentToClientEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по организации клиента
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel SelectDestinationDocumentsForClientOrganizationPaymentFromClientDistribution(
            ClientOrganizationPaymentFromClientEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Получение грида реализаций для организации клиента для разнесения оплаты
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationSaleGridForClientOrganizationPaymentFromClientDistribution(int clientOrganizationId, short teamId, UserInfo currentUser);
        
        /// <summary>
        /// Получение грида платежных документов для организации клиента для разнесения оплаты
        /// </summary>
        /// <param name="clientOrganizationId">Код организации клиента</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationPaymentDocumentGridForClientOrganizationPaymentFromClientDistribution(int clientOrganizationId, short teamId, UserInfo currentUser);

        /// <summary>
        /// Получение модели для грида реализаций при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationSaleGridForDealPaymentFromClientDistribution(int dealId, short teamId, UserInfo currentUser);

        /// <summary>
        /// Получение модели для грида платежных документов при ручном разнесении оплаты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        GridData ShowDestinationPaymentDocumentGridForDealPaymentFromClientDistribution(int dealId, short teamId, UserInfo currentUser);

        /// <summary>
        /// Модальная форма выбора документов для ручного разнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="model">Модель, заполненная на предыдущем этапе (в форме параметров оплаты при создании)</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel SelectDestinationDocumentsForDealPaymentFromClientDistribution(
            DealPaymentFromClientEditViewModel model, UserInfo currentUser);

        /// <summary>
        /// Модальная форма выбора документов для ручного переразнесения оплаты от клиента по сделке
        /// </summary>
        /// <param name="dealPaymentFromClientId">Код оплаты от клиента по сделке</param>
        /// <param name="destinationDocumentSelectorControllerName">Название контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="destinationDocumentSelectorActionName">Название метода контроллера, который будет вызываться при submit формы разнесения</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        DestinationDocumentSelectForDealPaymentFromClientDistributionViewModel SelectDestinationDocumentsForDealPaymentFromClientRedistribution(
            Guid dealPaymentFromClientId, string destinationDocumentSelectorControllerName, string destinationDocumentSelectorActionName, UserInfo currentUser);

        /// <summary>
        /// Смена пользователя, вернувшего оплату клиенту
        /// </summary>
        void ChangeReturnedByInPaymentToClient(Guid dealPaymentId, int newReturnedById, UserInfo currentUser);

        /// <summary>
        /// Смена пользователя, принявшего оплату клиенту
        /// </summary>
        void ChangeTakenByInPaymentFromClient(Guid dealPaymentId, int newTakenById, UserInfo currentUser);
    }
}
