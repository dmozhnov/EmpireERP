using ERP.Wholesale.UI.ViewModels.MeasureUnit;
using ERP.UI.ViewModels.Grid;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IMeasureUnitPresenter
    {
        MeasureUnitListViewModel List(UserInfo currentUser);
        GridData GetMeasureUnitSelectGrid(GridState state);
        GridData GetMeasureUnitGrid(GridState state, UserInfo currentUser);
        MeasureUnitSelectViewModel SelectMeasureUnit(UserInfo currentUser);
        MeasureUnitEditViewModel Create(UserInfo currentUser);
        MeasureUnitEditViewModel Edit(short id, UserInfo currentUser);
        object Save(MeasureUnitEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);
    }
}
