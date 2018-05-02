using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderMaterialsPackageService
    {
       // ProductionOrderMaterialsPackage GetById(Guid id);
        ProductionOrderMaterialsPackage CheckMaterialsPackageExistence(Guid id, User user);
        void Save(ProductionOrderMaterialsPackage entity);
        void Delete(ProductionOrderMaterialsPackage entity, User user);
        IEnumerable<ProductionOrderMaterialsPackage> GetAll();
        IEnumerable<ProductionOrderMaterialsPackage> GetAll(User user, Permission permission);
        IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, User user);
        IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, User user, ParameterString parameterString);
        ProductionOrderMaterialsPackageDocument CheckProductionOrderMaterialsPackageDocumentExistence(Guid id);
        void SaveFile(object file, ProductionOrderMaterialsPackageDocument document);
        ProductionOrderMaterialsPackage ProductionOrderMaterialsPackageDocumentDeleteFile(Guid id, User user);
        void CheckProductionOrderMaterialsPackageNameUniqueness(string name, Guid id, Guid productionOrderId);

        /// <summary>
        /// Каталог хранения файлов документов материалов
        /// </summary>
        string ProductionOrderMaterialsPackageStoragePath { get; }

        bool IsPossibilityToDelete(ProductionOrderMaterialsPackage package, User user);
        void CheckPossibilityToDelete(ProductionOrderMaterialsPackage package, User user);

        bool IsPossibilityToEdit(ProductionOrderMaterialsPackage package, User user);
        void CheckPossibilityToEdit(ProductionOrderMaterialsPackage package, User user);
    }
}
