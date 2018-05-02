using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.EmployeePost;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IEmployeePostPresenter
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        GridData GetEmployeePostGrid(GridState state, UserInfo currentUser);
        EmployeePostEditViewModel Create(UserInfo currentUser);
        EmployeePostEditViewModel Edit(short id, UserInfo currentUser);
        object Save(EmployeePostEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        object GetEmployeePosts();

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
