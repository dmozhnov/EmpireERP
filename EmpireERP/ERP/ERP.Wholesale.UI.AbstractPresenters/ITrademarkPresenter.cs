using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ITrademarkPresenter
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        GridData GetTrademarkSelectGrid(GridState state);
        GridData GetTrademarkGrid(GridState state, UserInfo currentUser);
        BaseDictionarySelectViewModel SelectTrademark(UserInfo currentUser);
        BaseDictionaryEditViewModel Create(UserInfo currentUser);
        BaseDictionaryEditViewModel Edit(short id, UserInfo currentUser);
        object Save(BaseDictionaryEditViewModel model, UserInfo currentUser);
        object GetTrademarks();
        void Delete(short id, UserInfo currentUser);

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
