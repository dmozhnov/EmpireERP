using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProductionOrderBatchLifeCycleTemplatePresenter : IProductionOrderBatchLifeCycleTemplatePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IProductionOrderBatchLifeCycleTemplateService productionOrderBatchLifeCycleTemplateService;
        private readonly IProductionOrderService productionOrderService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ProductionOrderBatchLifeCycleTemplatePresenter(IUnitOfWorkFactory unitOfWorkFactory, IProductionOrderBatchLifeCycleTemplateService productionOrderBatchLifeCycleTemplateService,
            IProductionOrderService productionOrderService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.productionOrderBatchLifeCycleTemplateService = productionOrderBatchLifeCycleTemplateService;
            this.productionOrderService = productionOrderService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public ProductionOrderBatchLifeCycleTemplateListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details);

                return new ProductionOrderBatchLifeCycleTemplateListViewModel
                {
                    ProductionOrderBatchLifeCycleTemplateGrid = GetProductionOrderBatchLifeCycleTemplateGridLocal(new GridState() { Sort = "Name=Asc" }, user),
                };
            }
        }

        public GridData GetProductionOrderBatchLifeCycleTemplateGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderBatchLifeCycleTemplateGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderBatchLifeCycleTemplateGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData() { State = state };

            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("StageCount", "Количество этапов", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

            var productionOrderBatchLifeCycleTemplateList = productionOrderBatchLifeCycleTemplateService.GetFilteredList(state);
            foreach (var productionOrderBatchLifeCycleTemplate in productionOrderBatchLifeCycleTemplateList)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = productionOrderBatchLifeCycleTemplate.Name },
                    new GridLabelCell("StageCount") { Value = productionOrderBatchLifeCycleTemplate.StageCount.ToString() },
                    new GridHiddenCell("Id") { Value = productionOrderBatchLifeCycleTemplate.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        }

        #endregion

        #region Создание / редактирование / удаление

        public ProductionOrderBatchLifeCycleTemplateEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var model = new ProductionOrderBatchLifeCycleTemplateEditViewModel();
                model.Title = "Добавление шаблона заказа";
                model.Id = "0";

                return model;
            }
        }

        public ProductionOrderBatchLifeCycleTemplateEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var template = productionOrderBatchLifeCycleTemplateService.CheckProductionOrderBatchLifeCycleTemplateExistence(id, user);

                var model = new ProductionOrderBatchLifeCycleTemplateEditViewModel();
                model.Title = "Редактирование шаблона заказа";
                model.Id = id.ToString();
                model.Name = template.Name;

                return model;
            }
        }

        public object Save(ProductionOrderBatchLifeCycleTemplateEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                short templateId = ValidationUtils.TryGetShort(model.Id);

                if (!productionOrderBatchLifeCycleTemplateService.IsNameUnique(model.Name, templateId))
                {
                    throw new Exception("Шаблон жизненного цикла заказа с таким названием уже существует. Введите другое.");
                }

                ProductionOrderBatchLifeCycleTemplate template;

                if (templateId == 0)
                {
                    var calculationStage = new ProductionOrderBatchLifeCycleTemplateStage(productionOrderService.GetDefaultProductionOrderBatchStageById(1));
                    var successfullClosingStage = new ProductionOrderBatchLifeCycleTemplateStage(productionOrderService.GetDefaultProductionOrderBatchStageById(2));
                    var unsuccessfulClosingStage = new ProductionOrderBatchLifeCycleTemplateStage(productionOrderService.GetDefaultProductionOrderBatchStageById(3));

                    template = new ProductionOrderBatchLifeCycleTemplate(model.Name, calculationStage, successfullClosingStage, unsuccessfulClosingStage);
                }
                else
                {
                    template = productionOrderBatchLifeCycleTemplateService.GetById(templateId);
                    template.Name = model.Name;
                }

                productionOrderBatchLifeCycleTemplateService.Save(template);

                uow.Commit();

                return GetMainChangeableIndicators(template);
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Delete);

                var template = productionOrderBatchLifeCycleTemplateService.CheckProductionOrderBatchLifeCycleTemplateExistence(id, user);

                productionOrderBatchLifeCycleTemplateService.Delete(template);

                uow.Commit();
            }
        }

        #endregion

        #region Детали

        #region Детали общие

        public ProductionOrderBatchLifeCycleTemplateDetailsViewModel Details(short id, string backUrl, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(id, user);

                var model = new ProductionOrderBatchLifeCycleTemplateDetailsViewModel();

                model.Title = "Детали шаблона жизненного цикла заказа";
                model.ProductionOrderBatchLifeCycleTemplateId = id.ToString();
                model.BackUrl = backUrl;
                model.Name = productionOrderBatchLifeCycleTemplate.Name;

                model.ProductionOrderBatchLifeCycleTemplateStageList = GetProductionOrderBatchLifeCycleTemplateStageGridLocal(new GridState { Parameters = "ProductionOrderBatchLifeCycleTemplateId=" + id.ToString() }, user);

                model.AllowToEdit = productionOrderBatchLifeCycleTemplateService.IsPossibilityToEdit(productionOrderBatchLifeCycleTemplate, user);
                model.AllowToDelete = productionOrderBatchLifeCycleTemplateService.IsPossibilityToDelete(productionOrderBatchLifeCycleTemplate, user);

                return model;
            }
        }

        /// <summary>
        /// Получение главных деталей
        /// </summary>
        private object GetMainDetails(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate)
        {
            return new
            {
                Id = productionOrderBatchLifeCycleTemplate.Id,
                Name = productionOrderBatchLifeCycleTemplate.Name
            };
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate)
        {
            var j = new
            {
                MainDetails = GetMainDetails(productionOrderBatchLifeCycleTemplate),
                Permissions = new object()
            };

            return j;
        }

        #endregion

        #region Гриды

        public GridData GetProductionOrderBatchLifeCycleTemplateStageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderBatchLifeCycleTemplateStageGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderBatchLifeCycleTemplateStageGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData() { State = state };

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var productionOrderBatchLifeCycleTemplateId = ValidationUtils.TryGetShort(deriveParams["ProductionOrderBatchLifeCycleTemplateId"].Value as string);
            var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

            model.AddColumn("Position", "Поз.", Unit.Pixel(36));
            model.AddColumn("Action", "Действие", Unit.Pixel(108), align: GridColumnAlign.Left);
            model.AddColumn("Name", "Название", Unit.Percentage(65));
            model.AddColumn("TypeName", "Тип", Unit.Percentage(35));
            model.AddColumn("PlannedDuration", "Длительность, дн.", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("InWorkDays", "В рабочих днях", Unit.Pixel(65), align: GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("OrdinalNumber", "", Unit.Pixel(0), GridCellStyle.Hidden);

            foreach (var stage in productionOrderBatchLifeCycleTemplate.Stages.OrderBy(x => x.OrdinalNumber))
            {
                var position = new GridActionCell("Position");
                if (productionOrderBatchLifeCycleTemplateService.IsPossibilityToMoveStageDown(stage, user))
                {
                    position.AddAction("▼", "linkMoveStageDown");
                }
                if (productionOrderBatchLifeCycleTemplateService.IsPossibilityToMoveStageUp(stage, user))
                {
                    position.AddAction("▲", "linkMoveStageUp");
                }

                var action = new GridActionCell("Action");
                if (productionOrderBatchLifeCycleTemplateService.IsPossibilityToCreateStageAfter(stage, user))
                {
                    action.AddAction("Доб.", "linkAddStage");
                }
                if (productionOrderBatchLifeCycleTemplateService.IsPossibilityToEditStage(stage, user))
                {
                    action.AddAction("Ред.", "linkEditStage");
                }
                if (productionOrderBatchLifeCycleTemplateService.IsPossibilityToDeleteStage(stage, user))
                {
                    action.AddAction("Удал.", "linkDeleteStage");
                }

                model.AddRow(new GridRow(
                    position.ActionCount > 0 ? (GridCell)position : new GridLabelCell("Position"),
                    action.ActionCount > 0 ? (GridCell)action : new GridLabelCell("Action"),
                    new GridLabelCell("Name") { Value = stage.Name },
                    new GridLabelCell("TypeName") { Value = stage.Type.GetDisplayName() },
                    new GridLabelCell("PlannedDuration") { Value = stage.PlannedDuration.HasValue ? stage.PlannedDuration.Value.ForDisplay() : "---" },
                    new GridLabelCell("InWorkDays") { Value = stage.InWorkDays ? "Да" : "Нет" },
                    new GridLabelCell("Id") { Value = stage.Id.ToString() },
                    new GridLabelCell("OrdinalNumber") { Value = stage.OrdinalNumber.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #endregion

        #region Работа с этапами

        public ProductionOrderBatchLifeCycleTemplateStageEditViewModel AddStage(short productionOrderBatchLifeCycleTemplateId, int id, short position, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                var productionOrderBatchLifeCycleTemplateStage =
                    CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                var model = new ProductionOrderBatchLifeCycleTemplateStageEditViewModel();

                if (position < 1 || position > productionOrderBatchLifeCycleTemplate.StageCount - 2)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                model.Title = String.Format("Добавление этапа после этапа «{0}»", productionOrderBatchLifeCycleTemplateStage.Name);
                model.ProductionOrderBatchLifeCycleTemplateStageId = 0.ToString();
                model.ProductionOrderBatchLifeCycleTemplateId = productionOrderBatchLifeCycleTemplate.Id.ToString();
                model.Position = position.ToString();
                model.TypeList = GetProductionOrderBatchStageTypeList();
                model.InWorkDays = false.ForDisplay();

                return model;
            }
        }

        public ProductionOrderBatchLifeCycleTemplateStageEditViewModel EditStage(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                var productionOrderBatchLifeCycleTemplateStage =
                    CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                var model = new ProductionOrderBatchLifeCycleTemplateStageEditViewModel();

                model.Title = "Редактирование этапа";
                model.ProductionOrderBatchLifeCycleTemplateStageId = id.ToString();
                model.ProductionOrderBatchLifeCycleTemplateId = productionOrderBatchLifeCycleTemplate.Id.ToString();
                model.Name = productionOrderBatchLifeCycleTemplateStage.Name;
                model.PlannedDuration = productionOrderBatchLifeCycleTemplateStage.PlannedDuration.ToString();
                model.Type = (byte)productionOrderBatchLifeCycleTemplateStage.Type;
                model.TypeList = GetProductionOrderBatchStageTypeList();
                model.InWorkDays = productionOrderBatchLifeCycleTemplateStage.InWorkDays.ForDisplay();

                return model;
            }
        }

        /// <summary>
        /// Заполнить список этапов всеми типами этапов, кроме типа "Закрыто".
        /// </summary>
        private IEnumerable<SelectListItem> GetProductionOrderBatchStageTypeList()
        {
            return productionOrderService.GetProductionOrderBatchStageTypeList()
                .GetComboBoxItemList<ProductionOrderBatchStageType>(x => x.GetDisplayName(), x => x.ValueToString(), sort: false);
        }

        public int SaveStage(ProductionOrderBatchLifeCycleTemplateStageEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplateId = ValidationUtils.TryGetShort(model.ProductionOrderBatchLifeCycleTemplateId);
                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                ProductionOrderBatchStageType type = (ProductionOrderBatchStageType)model.Type;
                if (!Enum.IsDefined(typeof(ProductionOrderBatchStageType), type))
                {
                    throw new Exception("Выберите тип этапа из списка.");
                }

                int id = ValidationUtils.TryGetInt(model.ProductionOrderBatchLifeCycleTemplateStageId);

                ProductionOrderBatchLifeCycleTemplateStage stage;

                if (id == 0)
                // Вставка нового этапа
                {
                    stage = new ProductionOrderBatchLifeCycleTemplateStage(model.Name, type, ValidationUtils.TryGetShort(model.PlannedDuration),
                        ValidationUtils.TryGetBool(model.InWorkDays));

                    productionOrderBatchLifeCycleTemplate.AddStage(stage, ValidationUtils.TryGetShort(model.Position));
                }
                else
                // Редактирование существующего этапа
                {
                    stage = CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                    ValidationUtils.Assert(!stage.IsDefault, "Невозможно отредактировать системный этап.");

                    stage.Name = model.Name;
                    stage.Type = type;
                    stage.PlannedDuration = ValidationUtils.TryGetShort(model.PlannedDuration);
                    stage.InWorkDays = ValidationUtils.TryGetBool(model.InWorkDays);

                    productionOrderBatchLifeCycleTemplate.CheckStageOrder();
                }

                productionOrderBatchLifeCycleTemplateService.Save(productionOrderBatchLifeCycleTemplate);

                uow.Commit();

                return stage.Id;
            }
        }

        public void DeleteStage(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                var stage = CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                if (stage.IsDefault)
                {
                    throw new Exception("Невозможно удалить системный этап.");
                }

                productionOrderBatchLifeCycleTemplate.DeleteStage(stage);

                productionOrderBatchLifeCycleTemplateService.Save(productionOrderBatchLifeCycleTemplate);

                uow.Commit();
            }
        }

        public void MoveStageUp(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                var stage = CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                if (stage.IsDefault)
                {
                    throw new Exception("Невозможно переместить системный этап.");
                }

                productionOrderBatchLifeCycleTemplate.MoveStageUp(stage);

                productionOrderBatchLifeCycleTemplateService.Save(productionOrderBatchLifeCycleTemplate);

                uow.Commit();
            }
        }

        public void MoveStageDown(short productionOrderBatchLifeCycleTemplateId, int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_Create_Edit);

                var productionOrderBatchLifeCycleTemplate = CheckProductionOrderBatchLifeCycleTemplateExistence(productionOrderBatchLifeCycleTemplateId, user);

                var stage = CheckProductionOrderBatchLifeCycleTemplateStageExistence(productionOrderBatchLifeCycleTemplate, id);

                if (stage.IsDefault)
                {
                    throw new Exception("Невозможно переместить системный этап.");
                }

                productionOrderBatchLifeCycleTemplate.MoveStageDown(stage);

                productionOrderBatchLifeCycleTemplateService.Save(productionOrderBatchLifeCycleTemplate);

                uow.Commit();
            }
        }

        #endregion

        #region Модальная форма выбора шаблона

        /// <summary>
        /// Заполнение модели модальной формы выбора шаблона
        /// </summary>
        /// <returns></returns>
        public ProductionOrderBatchLifeCycleTemplateSelectViewModel SelectProductionOrderBatchLifeCycleTemplate(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ProductionOrderBatchLifeCycleTemplateSelectViewModel();
                model.Title = "Выбор шаблона заказа";
                model.GridData = GetProductionOrderBatchLifeCycleTemplateSelectGridLocal(new GridState() { Sort = "Name=Asc" }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование модели грида шаблонов в модальной форме выбора
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetProductionOrderBatchLifeCycleTemplateSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProductionOrderBatchLifeCycleTemplateSelectGridLocal(state, user);
            }
        }

        private GridData GetProductionOrderBatchLifeCycleTemplateSelectGridLocal(GridState state, User user)
        {
            user.CheckPermission(Permission.ProductionOrderBatchLifeCycleTemplate_List_Details);

            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData { State = state };
            model.Title = "Шаблоны заказов";

            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("StageCount", "Количество этапов", Unit.Pixel(90));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var productionOrderBatchLifeCycleTemplateList = productionOrderBatchLifeCycleTemplateService.GetFilteredList(state);
            foreach (var productionOrderBatchLifeCycleTemplate in productionOrderBatchLifeCycleTemplateList)
            {
                model.AddRow(new GridRow(
                    new GridActionCell("Action", new GridActionCell.Action("Выбрать", "linkProductionOrderBatchLifeCycleTemplateSelect")),
                    new GridLinkCell("Name") { Value = productionOrderBatchLifeCycleTemplate.Name },
                    new GridLabelCell("StageCount") { Value = productionOrderBatchLifeCycleTemplate.StageCount.ToString() },
                    new GridHiddenCell("Id") { Value = productionOrderBatchLifeCycleTemplate.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение шаблона жизненного цикла заказа по id с проверкой его существования
        /// </summary>
        /// <param name="id">Код</param>
        private ProductionOrderBatchLifeCycleTemplate CheckProductionOrderBatchLifeCycleTemplateExistence(short id, User user)
        {
            return productionOrderBatchLifeCycleTemplateService.CheckProductionOrderBatchLifeCycleTemplateExistence(id, user);
        }

        /// <summary>
        /// Получение этапа шаблона жизненного цикла заказа по id с проверкой его существования
        /// </summary>
        /// <param name="id">Код</param>
        private ProductionOrderBatchLifeCycleTemplateStage CheckProductionOrderBatchLifeCycleTemplateStageExistence(
            ProductionOrderBatchLifeCycleTemplate productionOrderBatchLifeCycleTemplate, int id)
        {
            ProductionOrderBatchLifeCycleTemplateStage stage = productionOrderBatchLifeCycleTemplate.Stages.Where(x => x.Id == id).SingleOrDefault();
            ValidationUtils.NotNull(stage, "Этап шаблона жизненного цикла заказа не найден. Возможно, он был удален.");

            return stage;
        }

        #endregion

        #endregion
    }
}