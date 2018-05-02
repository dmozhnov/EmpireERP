using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.ValueAddedTax;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IValueAddedTaxPresenter : IBaseDictionaryPresenter<ValueAddedTax>
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        ValueAddedTaxEditViewModel Create(UserInfo currentUser);
        ValueAddedTaxEditViewModel Edit(short id, UserInfo currentUser);
        object Save(ValueAddedTaxEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);
        GridData GetValueAddedTaxGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
