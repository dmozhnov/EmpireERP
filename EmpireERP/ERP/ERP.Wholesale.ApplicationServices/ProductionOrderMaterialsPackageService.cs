using System;
using System.Collections.Generic;
using System.IO;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;
using ERP.Wholesale.Domain.Entities.Security;
using System.Linq;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProductionOrderMaterialsPackageService : IProductionOrderMaterialsPackageService
    {
        #region Поля

        private readonly IProductionOrderMaterialsPackageRepository productionOrderMaterialsPackageRepository;

        #endregion

        #region Свойства

        /// <summary>
        /// Каталог хранения файлов документов материалов 
        /// </summary>
        public string ProductionOrderMaterialsPackageStoragePath
        {
            get { return AppSettings.ProductionOrderMaterialsPackageStoragePath; }
        }

        #endregion

        #region Конструкторы

        public ProductionOrderMaterialsPackageService(IProductionOrderMaterialsPackageRepository productionOrderMaterialsPackageRepository)
        {
            this.productionOrderMaterialsPackageRepository = productionOrderMaterialsPackageRepository;
        }

        #endregion

        #region Методы

        private ProductionOrderMaterialsPackage GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ProductionOrderMaterialsPackage_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var package = productionOrderMaterialsPackageRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return package;
                }
                else
                {
                    bool contains = (user.Teams.SelectMany(x => x.ProductionOrders).Contains(package.ProductionOrder));

                    if ((type == PermissionDistributionType.Personal && package.ProductionOrder.Curator == user && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return package;
                    }
                }

                return null;
            }
        }

        public ProductionOrderMaterialsPackage CheckMaterialsPackageExistence(Guid id, User user)
        {
            var p = GetById(id, user);
            ValidationUtils.NotNull(p, "Пакет материалов заказа не найден. Возможно, он был удален.");

            return p;
        }

        public void Save(ProductionOrderMaterialsPackage entity)
        {
            productionOrderMaterialsPackageRepository.Save(entity);
        }

        public void Delete(ProductionOrderMaterialsPackage entity, User user)
        {
            CheckPossibilityToDelete(entity, user);

            foreach (var doc in entity.Documents)
            {
                doc.DeletionDate = DateTime.Now;

                //Удаляем файл из ФС
                var fileInfo = new FileInfo(ProductionOrderMaterialsPackageStoragePath + doc.Path);
                if (fileInfo.Exists)
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch
                    {
                        throw new Exception("Невозможно удалить пакет материалов, так как некоторые из его файлов используются в настоящий момент.");
                    }
                }
            }
            var dirInfo = new DirectoryInfo(ProductionOrderMaterialsPackageStoragePath + entity.Id.ToString());
            if (dirInfo.Exists)
            {
                dirInfo.Delete();
            }
            productionOrderMaterialsPackageRepository.Delete(entity);
        }

        public IEnumerable<ProductionOrderMaterialsPackage> GetAll()
        {
            return productionOrderMaterialsPackageRepository.GetAll();
        }

        public IEnumerable<ProductionOrderMaterialsPackage> GetAll(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderMaterialsPackage>();

                case PermissionDistributionType.Personal:
                    var teamPackages = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.MaterialsPackages));

                    return productionOrderMaterialsPackageRepository.GetAll().Where(x => x.ProductionOrder.Curator.Id == user.Id).Concat(teamPackages).Distinct();

                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.MaterialsPackages)).Distinct();

                case PermissionDistributionType.All:
                    return productionOrderMaterialsPackageRepository.GetAll();

                default:
                    return null;
            }
        }

        public IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, User user, ParameterString parameterString = null)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(Permission.ProductionOrderMaterialsPackage_List_Details);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<ProductionOrderMaterialsPackage>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    var list = GetAll(user, Permission.ProductionOrderMaterialsPackage_List_Details).Select(x => x.Id).ToList();

                    if (type == PermissionDistributionType.Personal)
                    {
                        parameterString.Add("ProductionOrder.Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    }

                    // если список пуст - то добавляем несуществующее значение
                    if (!list.Any()) { list.Add(Guid.Empty); }

                    parameterString.Add("Id", ParameterStringItem.OperationType.OneOf);
                    parameterString["Id"].Value = list;
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return productionOrderMaterialsPackageRepository.GetFilteredList(state, parameterString);

        }

        public IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, User user)
        {
            return GetFilteredList(state, user, null);
        }

        public IList<ProductionOrderMaterialsPackage> GetFilteredList(object state, ParameterString parameterString)
        {
            return productionOrderMaterialsPackageRepository.GetFilteredList(state, parameterString);
        }

        public ProductionOrderMaterialsPackageDocument CheckProductionOrderMaterialsPackageDocumentExistence(Guid id)
        {
            return productionOrderMaterialsPackageRepository.Query<ProductionOrderMaterialsPackageDocument>().Where(x => x.Id == id).FirstOrDefault<ProductionOrderMaterialsPackageDocument>();
        }

        /// <summary>
        /// Сохранение файла
        /// </summary>
        /// <param name="file">Содержимое файла. Передается как object, т.к. приходит объект HttpPostedFileBase из сборки System.Web.
        /// Использование стандартных Stream-ов приведет с "перекачке" содержимого файла из одного Stream-а в другой.</param>
        /// <param name="document"></param>
        public void SaveFile(object file, ProductionOrderMaterialsPackageDocument document)
        {
            var fileName = ProductionOrderMaterialsPackageStoragePath + document.Path;

            var dir = fileName.Substring(0, fileName.LastIndexOf('\\'));
            var dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            file.GetType().GetMethod("SaveAs").Invoke(file, new object[] { fileName });
        }

        public ProductionOrderMaterialsPackage ProductionOrderMaterialsPackageDocumentDeleteFile(Guid id, User user)
        {
            var doc = CheckProductionOrderMaterialsPackageDocumentExistence(id);

            var package = doc.MaterialsPackage;

            CheckPossibilityToEdit(package, user);

            package.DeleteDocument(doc);
            doc.DeletionDate = DateTime.Now;

            //Удаляем файл из ФС
            var fileInfo = new FileInfo(ProductionOrderMaterialsPackageStoragePath + doc.Path);
            if (fileInfo.Exists)
            {
                try
                {
                    fileInfo.Delete();
                }
                catch
                {
                    throw new Exception("Невозможно удалить файл, так как он используется в настоящий момент.");
                }
            }

            Save(package);

            return package;
        }

        public void CheckProductionOrderMaterialsPackageNameUniqueness(string name, Guid id, Guid productionOrderId)
        {
            ValidationUtils.Assert(productionOrderMaterialsPackageRepository.Query<ProductionOrderMaterialsPackage>()
                .Where(x => x.Name == name && x.ProductionOrder.Id == productionOrderId && x.Id != id).Count() == 0,
                String.Format("Пакет материалов с названием «{0}» уже существует.", name));
        }

        #region Права на совершение операций

        private bool IsPermissionToPerformOperation(ProductionOrderMaterialsPackage package, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = package.ProductionOrder.Curator == user && user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.MaterialsPackages)).Contains(package);
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.ProductionOrders.SelectMany(y => y.MaterialsPackages)).Contains(package);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ProductionOrderMaterialsPackage package, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(package, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #region Удаление

        public bool IsPossibilityToDelete(ProductionOrderMaterialsPackage package, User user)
        {
            try
            {
                CheckPossibilityToDelete(package, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(ProductionOrderMaterialsPackage package, User user)
        {
            // права
            CheckPermissionToPerformOperation(package, user, Permission.ProductionOrderMaterialsPackage_Delete);

            // сущность
            //package.CheckPossibilityToDelete();
        }

        #endregion

        #region Создание и редактирование

        public bool IsPossibilityToEdit(ProductionOrderMaterialsPackage package, User user)
        {
            try
            {
                CheckPossibilityToEdit(package, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(ProductionOrderMaterialsPackage package, User user)
        {
            // права
            CheckPermissionToPerformOperation(package, user, Permission.ProductionOrderMaterialsPackage_Create_Edit);

            // сущность
            //package.CheckPossibilityToDelete();
        }

        #endregion

        #endregion

        #endregion
    }
}