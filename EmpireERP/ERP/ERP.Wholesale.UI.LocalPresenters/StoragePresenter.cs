using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Storage;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class StoragePresenter : IStoragePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IUserService userService;

        #endregion

        #region Конструктор

        public StoragePresenter(IUnitOfWorkFactory unitOfWorkFactory, IStorageService storageService, IAccountOrganizationService accountOrganizationService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public StorageListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Storage_List_Details);

                StorageListViewModel model = new StorageListViewModel()
                {
                    GridData = GetStorageGridLocal(new GridState() { Sort = "Type=Asc;Name=Asc" }, user)
                };

                return model;
            }
        }

        public GridData GetStorageGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetStorageGridLocal(state, user);
            }
        }

        private GridData GetStorageGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            bool allowToDelete = user.HasPermission(Permission.Storage_Delete);

            GridData model = new GridData();

            model.ButtonPermissions["AllowToCreate"] = storageService.IsPossibilityToCreate(user);

            var storages = storageService.GetFilteredList(state, user);

            foreach (var item in storages)
            {
                var isPossibilityToDelete = storageService.IsPossibilityToDelete(item, user, false);

                var actions = new GridActionCell("Action");

                GridCell actionCell = actions;
                if (allowToDelete)
                {
                    if (isPossibilityToDelete)
                    {
                        actions.AddAction("Удалить", "delete_link");
                        actionCell = actions;
                    }
                    else
                    {
                        actionCell = new GridLabelCell("Action") { Value = "" };
                    }
                }

                model.AddRow(new GridRow(
                    actionCell,
                    new GridLinkCell("Name") { Value = item.Name, Key = "Name" },
                    new GridLabelCell("TypeName") { Value = item.Type.GetDisplayName() },
                    new GridLabelCell("AccountOrganizationCount") { Value = item.AccountOrganizationCount.ForDisplay() },
                    new GridLabelCell("SectionCount") { Value = item.SectionCount.ForDisplay() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" }
                ));
            }

            if (allowToDelete) { model.AddColumn("Action", "Действие", Unit.Pixel(60)); }
            model.AddColumn("Name", "Название", Unit.Percentage(70));
            model.AddColumn("TypeName", "Тип", Unit.Percentage(30));
            model.AddColumn("AccountOrganizationCount", "Кол-во организаций", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("SectionCount", "Кол-во секций", Unit.Pixel(85), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        #endregion

        #region Создание, редактирование

        public StorageEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                storageService.CheckPossibilityToCreate(user);

                var model = new StorageEditViewModel();
                model.StorageTypeList = GetStorageTypes();
                model.Title = "Добавление места хранения";

                return model;
            }
        }

        public StorageEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(id, user);

                storageService.CheckPossibilityToEdit(storage, user);

                var model = new StorageEditViewModel();
                model.Id = storage.Id;
                model.Name = storage.Name;
                model.Comment = storage.Comment;

                model.StorageTypeList = GetStorageTypes();
                model.StorageTypeId = (byte)storage.Type;
                model.Title = "Редактирование места хранения";

                return model;
            }
        }

        public object Save(StorageEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                Storage storage = null;

                if (model.Id == 0)
                {
                    storageService.CheckPossibilityToCreate(user);
                    storage = new Storage(model.Name, (StorageType)model.StorageTypeId);
                }
                else
                {
                    storage = storageService.CheckStorageExistence(model.Id, user);

                    storageService.CheckPossibilityToEdit(storage, user);

                    storage.Name = model.Name;
                    storage.Type = (StorageType)model.StorageTypeId;
                }
                storage.Comment = StringUtils.ToHtml(model.Comment);

                storageService.Save(storage, user);

                uow.Commit();

                return GetMainChangeableIndicators(storage);
            }
        }

        #endregion

        #region Удаление

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(id, user);

                storageService.Delete(storage, user);

                uow.Commit();
            }
        }

        #endregion

        #region Детали

        public StorageDetailsViewModel Details(short id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(id, user);

                storageService.CheckPossibilityToViewDetails(storage, user);

                StorageDetailsViewModel model = new StorageDetailsViewModel();
                model.MainDetails = GetMainDetails(storage);

                model.AccountOrganizationsGrid = GetAccountOrganizationGridLocal(new GridState() { Parameters = "storageId=" + id }, user);
                model.SectionsGrid = GetStorageSectionGridLocal(new GridState() { Parameters = "storageId=" + id }, user);
                model.BackURL = backURL;

                model.AllowToEdit = storageService.IsPossibilityToEdit(storage, user);
                model.AllowToDelete = storageService.IsPossibilityToDelete(storage, user, false);
                model.AllowToCreateAccountingPriceList = storageService.GetList(user, Permission.AccountingPriceList_Storage_Add).Contains(storage) && user.HasPermission(Permission.AccountingPriceList_Create);

                return model;
            }
        }

        private StorageMainDetailsViewModel GetMainDetails(Storage storage)
        {
            var model = new StorageMainDetailsViewModel();
            model.Id = storage.Id;
            model.AccountOrganizationCount = storage.AccountOrganizationCount.ToString();
            model.Comment = storage.Comment;
            model.Name = storage.Name;
            model.SectionCount = storage.SectionCount.ToString();
            model.TypeName = storage.Type.GetDisplayName();

            return model;
        }

        /// <summary>
        /// Получение значений основных пересчитываемых показателей
        /// </summary>
        private object GetMainChangeableIndicators(Storage storage)
        {
            var j = new
            {
                MainDetails = GetMainDetails(storage)
            };

            return j;
        }

        #endregion

        #region Секции

        #region Список секций

        public GridData GetStorageSectionGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetStorageSectionGridLocal(state, user);
            }
        }

        private GridData GetStorageSectionGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(deriveParams["storageId"].Value.ToString()), user);

            bool allowToDeleteSection = storageService.IsPossibilityToDeleteSection(storage, user);

            GridData model = new GridData();
            if (allowToDeleteSection) { model.AddColumn("Action", "Действие", Unit.Pixel(60)); }
            model.AddColumn("Name", "Название", Unit.Percentage(100), GridCellStyle.PseudoLink);
            model.AddColumn("Square", "Площадь (м2)", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("Volume", "Объем (м3)", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateSection"] = storageService.IsPossibilityToCreateAndEditSection(storage, user);

            var sectionList = storage.Sections.OrderBy(x => x.Name).ToList<StorageSection>();

            var actions = new GridActionCell("Action");
            if (allowToDeleteSection) { actions.AddAction("Удалить", "delete_storageSection_link"); }

            foreach (var item in GridUtils.GetEntityRange(sectionList, state))
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridPseudoLinkCell("Name") { Value = item.Name, Key = "name_link" },
                    new GridLabelCell("Square") { Value = item.Square.ForDisplay(ValueDisplayType.Volume) },
                    new GridLabelCell("Volume") { Value = item.Volume.ForDisplay(ValueDisplayType.Volume) },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "storageSectionId" }
                ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление, редактирование секции

        public StorageSectionEditViewModel CreateSection(short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(storageId, user);

                storageService.CheckPossibilityToCreateAndEditSection(storage, user);

                var model = new StorageSectionEditViewModel();
                model.StorageId = storageId;
                model.Title = "Добавление секции места хранения";
                model.AllowToEdit = true;

                return model;
            }
        }

        public StorageSectionEditViewModel EditSection(short storageSectionId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(storageId, user);
                var section = CheckStorageSectionExistence(storageSectionId, storage);

                bool allowToEdit = storageService.IsPossibilityToCreateAndEditSection(storage, user);

                var model = new StorageSectionEditViewModel();
                model.Id = section.Id;
                model.Name = section.Name;
                model.Square = section.Square.ForEdit();
                model.Volume = section.Volume.ForEdit();
                model.Title = (allowToEdit ? "Редактирование секции места хранения" : "Детали секции места хранения");

                model.AllowToEdit = allowToEdit;

                return model;
            }
        }

        public object SaveSection(StorageSectionEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(model.StorageId, user);

                storageService.CheckPossibilityToCreateAndEditSection(storage, user);

                StorageSection section = null;

                if (model.Id == 0)
                {
                    section = new StorageSection(model.Name);
                    section.Square = ValidationUtils.TryGetDecimal(model.Square);
                    section.Volume = ValidationUtils.TryGetDecimal(model.Volume);

                    storageService.AddSection(storage, section, user);
                }
                else
                {
                    section = CheckStorageSectionExistence(model.Id, storage);

                    section.Name = model.Name;
                    section.Square = ValidationUtils.TryGetDecimal(model.Square);
                    section.Volume = ValidationUtils.TryGetDecimal(model.Volume);

                    storageService.Save(storage, user);
                }

                uow.Commit();

                return GetMainChangeableIndicators(storage);
            }
        }

        public object DeleteSection(short sectionId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(storageId, user);

                var section = CheckStorageSectionExistence(sectionId, storage);

                storageService.DeleteSection(storage, section, user);

                uow.Commit();

                return GetMainChangeableIndicators(storage);
            }
        }

        #endregion

        #endregion

        #region Список связанных организаций

        public GridData GetAccountOrganizationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAccountOrganizationGridLocal(state, user);
            }
        }

        private GridData GetAccountOrganizationGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(deriveParams["storageId"].Value.ToString()), user);

            GridData model = new GridData();

            model.ButtonPermissions["AllowToAddAccountOrganization"] = storageService.IsPossibilityToAddAccountOrganization(storage, user);

            var accountOrganizationList = storage.AccountOrganizations.OrderBy(x => x.ShortName).ToList<AccountOrganization>();

            bool showActionCell = false;    //Признак отображения столбца действия

            foreach (var item in GridUtils.GetEntityRange(accountOrganizationList, state))
            {
                // Проверка бизнес-логикой не выполняется в целях снижения нагрузки на сервер
                bool allowToRemoveAccountOrganization = storageService.IsPossibilityToRemoveAccountOrganization(storage, item, user, false);
                showActionCell = showActionCell || allowToRemoveAccountOrganization;

                var actions = new GridActionCell("Action");
                if (allowToRemoveAccountOrganization)
                {
                    actions.AddAction("Удал. из списка", "delete_accountOrganization_link");
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("ShortName") { Value = item.ShortName },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "accountOrganizationId" }
                ));
            }

            if (showActionCell) { model.AddColumn("Action", "Действие", Unit.Pixel(90)); }
            model.AddColumn("ShortName", "Краткое название", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.State = state;

            return model;
        }

        /// <summary>
        /// Добавление связанной организации
        /// </summary>      
        public object AddAccountOrganization(AccountOrganizationSelectList model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(model.StorageId, user);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(model.SelectedAccountOrganizationId);

                storageService.AddAccountOrganization(storage, accountOrganization, user);

                uow.Commit();

                return GetMainChangeableIndicators(storage);
            }
        }

        /// <summary>
        /// Удаление связанной организации
        /// </summary>        
        public object DeleteAccountOrganization(int accountOrganizationId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.CheckStorageExistence(storageId, user);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                storageService.RemoveAccountOrganization(storage, accountOrganization, user);

                uow.Commit();

                return GetMainChangeableIndicators(storage);
            }
        }

        public AccountOrganizationSelectList GetAvailableAccountOrganizations(short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var storage = storageService.GetById(storageId);

                storageService.CheckPossibilityToAddAccountOrganization(storage, user);

                var accountOrganizations = accountOrganizationService.GetList().OrderBy(x => x.ShortName);

                var model = new AccountOrganizationSelectList();
                model.StorageId = storageId;

                // убираем уже имеющиеся организации
                var availableAccountOrganizations = new List<AccountOrganization>();

                foreach (var accountOrg in accountOrganizations)
                {
                    if (!storage.AccountOrganizations.Contains(accountOrg))
                    {
                        availableAccountOrganizations.Add(accountOrg);
                    }
                }

                model.AccountOrganizationList =
                    availableAccountOrganizations.GetComboBoxItemList(s => s.ShortName, s => s.Id.ToString());

                return model;
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение секции места хранения по id с проверкой его существования
        /// </summary>
        private StorageSection CheckStorageSectionExistence(short sectionId, Storage storage)
        {
            var section = storage.Sections.FirstOrDefault(x => x.Id == sectionId);
            ValidationUtils.NotNull(section, "Секция места хранения не найдена. Возможно, она была удалена.");

            return section;
        }

        /// <summary>
        /// Получение списка всех типов мест хранения для выпадающего списка
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetStorageTypes()
        {
            return ComboBoxBuilder.GetComboBoxItemList<StorageType>();
        }

        #endregion

        #endregion
    }
}