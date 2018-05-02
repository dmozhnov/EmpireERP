using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.Country;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ICountryPresenter
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        GridData GetCountryGrid(GridState state, UserInfo currentUser);
        CountryEditViewModel Create(UserInfo currentUser);
        CountryEditViewModel Edit(short id, UserInfo currentUser);
        object Save(CountryEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        object GetList();

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
