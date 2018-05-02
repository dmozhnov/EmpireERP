using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.Manufacturer;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IManufacturerPresenter
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);

        GridData GetManufacturerGrid(GridState state, UserInfo currentUser);
        object Save(ManufacturerEditViewModel model, UserInfo currentUser);
        ManufacturerEditViewModel Create(int? producerId, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        ManufacturerSelectorViewModel SelectManufacturer(int producerId, UserInfo currentUser, string mode = "exclude");
        ManufacturerSelectorViewModel SelectManufacturer(UserInfo currentUser);

        GridData GetManufacturerGrid(GridState state);
        ManufacturerEditViewModel Edit(short id, UserInfo currentUser);
    }
}
