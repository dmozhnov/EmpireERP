using ERP.Infrastructure.Security;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.ClientContract;

namespace ERP.Wholesale.UI.AbstractPresenters.Mediators
{
    public interface IClientContractPresenterMediator
    {
        ClientContract SaveContract(ClientContractEditViewModel model, UserInfo currentUser);
    }
}
