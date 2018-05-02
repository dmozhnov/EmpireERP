using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Contractor;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IContractorPresenter
    {
        /// <summary>
        /// Получение модели для окна выбора контрагента
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ContractorSelectViewModel SelectContractor(UserInfo currentUser);

        /// <summary>
        /// Получение модели грида выбора контрагента
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        GridData GetContractorGrid(GridState state, UserInfo currentUser);
    }
}
