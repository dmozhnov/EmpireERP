using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.ViewModels.Producer;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProducerPresenter
    {
        ProducerListViewModel List(UserInfo currentUser);
        GridData GetProducersGrid(GridState state, UserInfo currentUser);

        ProducerEditViewModel Create(string backURL, UserInfo curator);
        ProducerEditViewModel Edit(int producerId, string backURL, UserInfo currentUser);
        int Save(ProducerEditViewModel model, UserInfo currentUser);
        void Delete(int producerId, UserInfo currentUser);

        ProducerDetailsViewModel Details(int id, string backURL, UserInfo currentUser);
        GridData GetProductionOrdersGrid(GridState state, UserInfo currentUser);
        GridData GetPaymentsGrid(GridState state, UserInfo currentUser);
        GridData GetBankAccountsGrid(GridState state, UserInfo currentUser);
        GridData GetForeignBankAccountsGrid(GridState state, UserInfo currentUser);
        GridData GetManufacturerGrid(GridState state, UserInfo currentUser);
        GridData GetTaskGrid(GridState state, UserInfo currentUser);

        RussianBankAccountEditViewModel AddRussianBankAccount(int producerId, UserInfo currentUser);
        ForeignBankAccountEditViewModel AddForeignBankAccount(int producerId, UserInfo currentUser);

        RussianBankAccountEditViewModel EditRussianBankAccount(int producerId, int bankAccountId, UserInfo currentUser);
        ForeignBankAccountEditViewModel EditForeignBankAccount(int producerId, int bankAccountId, UserInfo currentUser);

        void SaveRussianBankAccount(RussianBankAccountEditViewModel model, UserInfo currentUser);
        void SaveForeignBankAccount(ForeignBankAccountEditViewModel model, UserInfo currentUser);

        void RemoveRussianBankAccount(int producerId, int bankAccountId, UserInfo currentUser);
        void RemoveForeignBankAccount(int producerId, int bankAccountId, UserInfo currentUser);

        void AddManufacturer(int producerId, short manufacturerId, UserInfo currentUser);
        void RemoveManufacturer(int producerId, short manufacturerId, UserInfo currentUser);

        ProducerSelectViewModel SelectProducer();
        GridData GetProducerSelectGrid(GridState state = null);

        object GetMainChangeableIndicators(int producerId, UserInfo currentUser);
        object DeletePayment(Guid productionOrderId, Guid paymentId, UserInfo currentUser, DateTime currentDateTime);
    }
}
