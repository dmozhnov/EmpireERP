using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IProductionOrderMaterialsPackagePresenter
    {
        ProductionOrderMaterialsPackageListViewModel List(UserInfo currentUser);
        GridData GetProductionOrderMaterialsPackageGrid(GridState state, UserInfo currentUser);
        ProductionOrderMaterialsPackageEditViewModel Create(Guid? productionOrderId, string backURL, UserInfo currentUser);
        object Save(ProductionOrderMaterialsPackageEditViewModel model, UserInfo currentUser);
        ProductionOrderMaterialsPackageEditViewModel Edit(Guid materialsPackageId, string backURL, UserInfo currentUser);
        ProductionOrderMaterialsPackageDetailsViewModel Details(Guid materialsPackageId, string backURL, UserInfo currentUser);
        GridData GetMaterialsPackageDocumentGrid(GridState state, UserInfo currentUser);
        void DeleteMaterialsPackage(Guid materialsPackageId, UserInfo currentUser);
        ProductionOrderMaterialsPackageDocumentEditViewModel ProductionOrderMaterialsPackageDocumentCreate(Guid packageId, UserInfo currentUser);
        object ProductionOrderMaterialsPackageDocumentSave(ProductionOrderMaterialsPackageDocumentEditViewModel model, UserInfo userInfo);
        DownloadProductionOrderMaterialsPackageDocumentViewModel DownloadProductionOrderMaterialsPackageDocument(Guid id, UserInfo currentUser);
        ProductionOrderMaterialsPackageDocumentEditViewModel ProductionOrderMaterialsPackageDocumentEdit(Guid id, UserInfo currentUser);
        object ProductionOrderMaterialsPackageDocumentDelete(Guid id, UserInfo currentUser);
    }
}
