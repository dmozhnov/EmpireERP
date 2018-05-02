using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProviderTypePresenter : IBaseDictionaryPresenter<ProviderType>
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        GridData GetProviderTypeSelectGrid(GridState state);
        GridData GetProviderTypeGrid(GridState state, UserInfo currentUser);
        BaseDictionarySelectViewModel SelectProviderType(UserInfo currentUser);
        BaseDictionaryEditViewModel Create(UserInfo currentUser);
        BaseDictionaryEditViewModel Edit(short id, UserInfo currentUser);
        object Save(BaseDictionaryEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
