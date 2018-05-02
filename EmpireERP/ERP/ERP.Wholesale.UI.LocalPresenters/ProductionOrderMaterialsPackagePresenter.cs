using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderMaterialsPackagePresenter : IProductionOrderMaterialsPackagePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderMaterialsPackageService productionOrderMaterialsPackageService;
        private readonly IProductionOrderService productionOrderService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ProductionOrderMaterialsPackagePresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderMaterialsPackageService productionOrderMaterialsPackageService,
            IProductionOrderService productionOrderService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderMaterialsPackageService = productionOrderMaterialsPackageService;
            this.productionOrderService = productionOrderService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderMaterialsPackageListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderMaterialsPackage_List_Details);

                var model = new ProductionOrderMaterialsPackageListViewModel()
                {
                    Title = "Пакеты материалов",
                    MaterialsPackageGrid = GetProductionOrderMaterialsPackageGridLocal(new GridState() { Sort = "Name=Asc;" }, user),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                             new FilterDateRangePicker("CreationDate","Дата создания"),
                             new FilterTextEditor("Name","Название пакета"),
                             new FilterTextEditor("ProductionOrder_Name","Заказ"),
                             new FilterTextEditor("Description","Описание")
                        }
                    }
                };

                return model;
            }
        }

        public GridData GetProductionOrderMaterialsPackageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderMaterialsPackageGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderMaterialsPackageGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            var list = productionOrderMaterialsPackageService.GetFilteredList(state, user);

            var model = new GridData() { State = state };
            model.AddColumn("Name", "Название", Unit.Percentage(35));
            model.AddColumn("ProductionOrderName", "Заказ", Unit.Percentage(35));
            model.AddColumn("ProductionOrderId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("CreationDate", "Дата создания", Unit.Pixel(54));
            model.AddColumn("LastChangeDate", "Дата обновления", Unit.Pixel(70));
            model.AddColumn("Description", "Описание", Unit.Percentage(30));
            model.AddColumn("MaterialCount", "Кол-во док-ов", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ProductionOrderMaterialsPackage_Create_Edit);

            foreach (var row in list)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = row.Name },
                    productionOrderService.IsPossibilityToViewDetails(row.ProductionOrder, user) ?
                        (GridCell)new GridLinkCell("ProductionOrderName") { Value = row.ProductionOrder.Name } :
                        new GridLabelCell("ProductionOrderName") { Value = row.ProductionOrder.Name },
                    new GridHiddenCell("ProductionOrderId") { Value = row.ProductionOrder.Id.ToString() },
                    new GridLabelCell("CreationDate") { Value = row.CreationDate.ToShortDateString() },
                    new GridLabelCell("LastChangeDate") { Value = row.LastChangeDate == null ? "" : row.LastChangeDate.ToShortDateString() },
                    new GridLabelCell("Description") { Value = row.Description },
                    new GridLabelCell("MaterialCount") { Value = row.DocumentCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Добавление/редактирование

        public ProductionOrderMaterialsPackageEditViewModel Create(Guid? productionOrderId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderMaterialsPackage_Create_Edit);

                ProductionOrder productionOrder = null;
                if (productionOrderId != null)
                {
                    productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId.Value, user);
                    productionOrderService.CheckPossibilityToCreateMaterialsPackage(productionOrder, user);
                }

                var model = new ProductionOrderMaterialsPackageEditViewModel()
                {
                    Title = "Добавление пакета материалов",
                    CreationDate = DateTime.Now.ToShortDateString(),
                    LastChangeDate = "---",
                    ProductionOrderId = productionOrder != null ? productionOrder.Id.ToString() : "",
                    ProductionOrder = productionOrder != null ? productionOrder.Name : "Выберите заказ",
                    BackURL = backURL,
                    AllowToChangeProductionOrder = productionOrderId == null
                };

                return model;
            }
        }

        public object Save(ProductionOrderMaterialsPackageEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ProductionOrderMaterialsPackage pm;
                if (model.Id.Length > 0)
                {
                    // Редактирование
                    productionOrderMaterialsPackageService.CheckProductionOrderMaterialsPackageNameUniqueness(model.Name, ValidationUtils.TryGetGuid(model.Id),
                        ValidationUtils.TryGetGuid(model.ProductionOrderId));

                    pm = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(ValidationUtils.TryGetGuid(model.Id), user);

                    productionOrderMaterialsPackageService.CheckPossibilityToEdit(pm, user);

                    pm.Description = model.Description;
                    pm.Comment = StringUtils.ToHtml(model.Comment);
                    pm.Description = model.Description;
                    pm.Name = model.Name;
                    pm.ProductionOrder = productionOrderService.CheckProductionOrderExistence(ValidationUtils.TryGetGuid(model.ProductionOrderId), user);
                }
                else
                {
                    // Создание нового пакета
                    productionOrderMaterialsPackageService.CheckProductionOrderMaterialsPackageNameUniqueness(model.Name, Guid.Empty,
                        ValidationUtils.TryGetGuid(model.ProductionOrderId));

                    var productionOrder = productionOrderService.CheckProductionOrderExistence(ValidationUtils.TryGetGuid(model.ProductionOrderId), user);
                    productionOrderService.CheckPossibilityToCreateMaterialsPackage(productionOrder, user);

                    pm = new ProductionOrderMaterialsPackage(productionOrder, model.Name)
                    {
                        Description = model.Description,
                        Comment = StringUtils.ToHtml(model.Comment)
                    };
                }

                productionOrderMaterialsPackageService.Save(pm);

                uow.Commit();

                return new { Id = pm.Id.ToString() };
            }
        }

        public ProductionOrderMaterialsPackageEditViewModel Edit(Guid materialsPackageId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ProductionOrderMaterialsPackage materialsPackage = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(materialsPackageId, user);

                productionOrderMaterialsPackageService.CheckPossibilityToEdit(materialsPackage, user);

                var model = new ProductionOrderMaterialsPackageEditViewModel()
                {
                    BackURL = backURL,
                    Title = "Редактирование пакета материалов",
                    ProductionOrderId = materialsPackage.ProductionOrder.Id.ToString(),
                    ProductionOrder = materialsPackage.ProductionOrder.Name,
                    Comment = materialsPackage.Comment,
                    CreationDate = materialsPackage.CreationDate.ToShortDateString(),
                    LastChangeDate = materialsPackage.LastChangeDate.ToShortDateString(),
                    Description = materialsPackage.Description,
                    Name = materialsPackage.Name
                };

                return model;
            }
        }

        #endregion

        #region Детали

        private ProductionOrderMaterialsPackageMainDetailsViewModel GetMainDetails(ProductionOrderMaterialsPackage materialsPackage, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            var model = new ProductionOrderMaterialsPackageMainDetailsViewModel()
            {
                Comment = materialsPackage.Comment,
                CreationDate = materialsPackage.CreationDate.ToShortDateString(),
                Description = materialsPackage.Description,
                LastChangeDate = materialsPackage.LastChangeDate.ToShortDateString(),
                DocumentCount = materialsPackage.DocumentCount.ToString(),
                Name = materialsPackage.Name,
                PakageSize = materialsPackage.ProductionOrderMaterialsPackageSize.ForDisplay(ValueDisplayType.FileSize),
                ProductionOrder = materialsPackage.ProductionOrder.Name,
                ProductionOrderId = materialsPackage.ProductionOrder.Id.ToString(),

                AllowToViewProductionOrder = productionOrderService.IsPossibilityToViewDetails(materialsPackage.ProductionOrder, user)
            };

            return model;
        }

        public ProductionOrderMaterialsPackageDetailsViewModel Details(Guid materialsPackageId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var materialsPackage = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(materialsPackageId, user);
                var model = new ProductionOrderMaterialsPackageDetailsViewModel()
                {
                    Id = materialsPackageId.ToString(),
                    BackURL = backURL,
                    Title = "Детали пакета материалов",
                    PackageName = materialsPackage.Name,
                    MainDetails = GetMainDetails(materialsPackage, currentUser),
                    Grid = GetMaterialsPackageDocumentGridLocal(new GridState() { Parameters = "MaterialsPackageId=" + materialsPackageId.ToString() }, user),

                    AllowToEdit = productionOrderMaterialsPackageService.IsPossibilityToEdit(materialsPackage, user),
                    AllowToDelete = productionOrderMaterialsPackageService.IsPossibilityToDelete(materialsPackage, user)
                };

                return model;
            }
        }

        public GridData GetMaterialsPackageDocumentGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetMaterialsPackageDocumentGridLocal(state, user);
            }
        }

        private GridData GetMaterialsPackageDocumentGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var ps = new ParameterString(state.Parameters);
            var packageId = ValidationUtils.TryGetGuid(ps["MaterialsPackageId"].Value as string);

            var package = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(packageId, user);

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(60), GridCellStyle.Action);
            model.AddColumn("Name", "Название файла", Unit.Percentage(50), GridCellStyle.Link);
            model.AddColumn("Description", "Описание", Unit.Percentage(50));
            model.AddColumn("Date", "Дата", Unit.Pixel(70));
            model.AddColumn("Size", "Размер (Мб)", Unit.Pixel(50));
            model.AddColumn("Author", "Автор", Unit.Pixel(100), GridCellStyle.Link);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = productionOrderMaterialsPackageService.IsPossibilityToEdit(package, user);

            var action = new GridActionCell("Action");

            if (productionOrderMaterialsPackageService.IsPossibilityToEdit(package, user))
            {
                action.AddAction("Ред.", "edit");
                action.AddAction("Удал.", "delete");
            };

            foreach (var material in GridUtils.GetEntityRange(package.Documents.OrderByDescending(x => x.CreationDate), state))
            {
                model.AddRow(new GridRow
                    (
                        action.ActionCount > 0 ? (GridCell)action : new GridLabelCell("Action"),
                        new GridLinkCell("Name") { Value = material.FileName },
                        new GridLabelCell("Description") { Value = material.Description },
                        new GridLabelCell("Date") { Value = material.CreationDate.ToShortDateString() },
                        new GridLabelCell("Size") { Value = material.Size.ForDisplay(ValueDisplayType.FileSize) },
                        new GridLabelCell("Author") { Value = material.CreatedBy.DisplayName },
                        new GridHiddenCell("Id") { Value = material.Id.ToString() }
                    ));
            }

            return model;
        }

        public void DeleteMaterialsPackage(Guid materialsPackageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var materialsPackage = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(materialsPackageId, user);
                productionOrderMaterialsPackageService.CheckPossibilityToDelete(materialsPackage, user);

                materialsPackage.DeletionDate = DateTime.Now;
                productionOrderMaterialsPackageService.Delete(materialsPackage, user);

                uow.Commit();
            }
        }

        #endregion

        #region Создание/редактирование/удаление документа пакета материалов

        public ProductionOrderMaterialsPackageDocumentEditViewModel ProductionOrderMaterialsPackageDocumentCreate(Guid packageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var package = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(packageId, user);
                productionOrderMaterialsPackageService.CheckPossibilityToEdit(package, user);

                var model = new ProductionOrderMaterialsPackageDocumentEditViewModel()
                {
                    DocumentId = Guid.Empty.ToString(),
                    PackageId = packageId.ToString(),
                    Title = "Добавление файла в пакет материалов",
                    AllowSelectFile = true
                };

                return model;
            }
        }

        public object ProductionOrderMaterialsPackageDocumentSave(ProductionOrderMaterialsPackageDocumentEditViewModel model, UserInfo userInfo)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(userInfo.Id);

                ProductionOrderMaterialsPackageDocument doc;
                Guid docId = ValidationUtils.TryGetGuid(model.DocumentId);
                var package = productionOrderMaterialsPackageService.CheckMaterialsPackageExistence(ValidationUtils.TryGetGuid(model.PackageId), user);
                productionOrderMaterialsPackageService.CheckPossibilityToEdit(package, user);

                if (docId == Guid.Empty)
                {
                    var stream = model.FileUpload.GetType().GetProperty("InputStream").GetValue(model.FileUpload, null) as Stream;

                    doc = new ProductionOrderMaterialsPackageDocument(Path.GetFileName(model.FileName), model.Description, user);
                    doc.SetFileSizeInBytes(stream.Length);

                    package.AddDocument(doc);

                    // для генерации Id
                    productionOrderMaterialsPackageService.Save(package);

                    productionOrderMaterialsPackageService.SaveFile(model.FileUpload, doc);
                }
                else
                {
                    doc = productionOrderMaterialsPackageService.CheckProductionOrderMaterialsPackageDocumentExistence(docId);
                    doc.Description = model.Description;

                    productionOrderMaterialsPackageService.Save(package);
                }

                uow.Commit();

                return GetMainDetails(package, userInfo);
            }
        }

        public DownloadProductionOrderMaterialsPackageDocumentViewModel DownloadProductionOrderMaterialsPackageDocument(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var doc = productionOrderMaterialsPackageService.CheckProductionOrderMaterialsPackageDocumentExistence(id);

                var model = new DownloadProductionOrderMaterialsPackageDocumentViewModel()
                {
                    FileName = doc.FileName,
                    FilePath = productionOrderMaterialsPackageService.ProductionOrderMaterialsPackageStoragePath + doc.Path
                };

                return model;
            }
        }

        public ProductionOrderMaterialsPackageDocumentEditViewModel ProductionOrderMaterialsPackageDocumentEdit(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var doc = productionOrderMaterialsPackageService.CheckProductionOrderMaterialsPackageDocumentExistence(id);
                productionOrderMaterialsPackageService.CheckPossibilityToEdit(doc.MaterialsPackage, user);

                var model = new ProductionOrderMaterialsPackageDocumentEditViewModel()
                {
                    Title = "Редактирование документа пакета материалов",
                    AllowSelectFile = false,
                    Description = doc.Description,
                    FileName = doc.FileName,
                    DocumentId = doc.Id.ToString(),
                    PackageId = doc.MaterialsPackage.Id.ToString()
                };

                return model;
            }
        }

        public object ProductionOrderMaterialsPackageDocumentDelete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var package = productionOrderMaterialsPackageService.ProductionOrderMaterialsPackageDocumentDeleteFile(id, user);

                uow.Commit();

                return GetMainDetails(package, currentUser);
            }
        }

        #endregion

        #endregion
    }
}