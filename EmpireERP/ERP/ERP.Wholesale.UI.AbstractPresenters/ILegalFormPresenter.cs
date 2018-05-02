using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.LegalForm;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ILegalFormPresenter : IBaseDictionaryPresenter<LegalForm>
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        LegalFormEditViewModel Create(UserInfo currentUser);
        LegalFormEditViewModel Edit(short id, UserInfo currentUser);
        object Save(LegalFormEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);
        GridData GetLegalFormGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
