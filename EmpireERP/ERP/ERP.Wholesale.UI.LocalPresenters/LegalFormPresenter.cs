using System.Data;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.LegalForm;
using ERP.Utils.Mvc;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class LegalFormPresenter : BaseDictionaryPresenter<LegalForm>, ILegalFormPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly ILegalFormService legalFormService;

        #endregion

        #region Конструктор

        public LegalFormPresenter(IUnitOfWorkFactory unitOfWorkFactory, ILegalFormService legalFormService, IUserService userService)
            : base(legalFormService, userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.legalFormService = legalFormService;
        }

        #endregion

        #region Методы

        #region Список

        public BaseDictionaryListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.ListBaseDictionary(currentUser);

                return model;
            }
        }

        public GridData GetLegalFormGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionaryGrid(state, currentUser);

                return model;
            }
        }

        protected override GridData GetBaseDictionaryGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState() { Sort = "Name=Asc" };

            bool allowToDelete = user.HasPermission(legalFormService.DeletionPermission);
            bool allowToEdit = user.HasPermission(legalFormService.EditingPermission);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("EconomicAgentType", "Тип хозяйствующего субъекта", Unit.Pixel(215));
            model.AddColumn("Name", "Наименование", Unit.Percentage(100));
            
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(legalFormService.CreationPermission);

            var rows = legalFormService.GetFilteredList(state);
            model.State = state;

            var actions = new GridActionCell("Action");
            if (allowToEdit)
            {
                actions.AddAction("Ред.", "edit_link");
            }
            else
            {
                actions.AddAction("Дет.", "edit_link");
            }

            if (allowToDelete)
            {
                actions.AddAction("Удал.", "delete_link");
            }

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                                 actions,
                                 new GridLabelCell("EconomicAgentType") { Value = item.EconomicAgentType.GetDisplayName(), Key = "EconomicAgentType" },
                                 new GridLabelCell("Name") {Value = item.Name, Key = "Name"},
                                 
                                 new GridHiddenCell("Id") {Value = item.Id.ToString(), Key = "Id"}
                                 ));
            }

            return model;
        }

        #endregion

        #region Создание / редактирование / удаление

        public LegalFormEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.LegalForm_Create);

                var model = new LegalFormEditViewModel()
                {
                    AllowToEditEconomicAgentType = true,
                    Title = "Добавление организационно-правовой формы",
                    AllowToEdit = true,
                    EconomicAgentType = "0",
                    EconomicAgentTypeList = ComboBoxBuilder.GetComboBoxItemList<EconomicAgentType>()
                };

                return model;
            }
        }

        public LegalFormEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var legalForm = legalFormService.CheckExistence(id);
                var allowToEdit = user.HasPermission(Permission.LegalForm_Edit);

                var model = new LegalFormEditViewModel
                                {
                                    Id = legalForm.Id,
                                    Name = legalForm.Name,
                                    EconomicAgentType = legalForm.EconomicAgentType.ValueToString(),
                                    EconomicAgentTypeList = ComboBoxBuilder.GetComboBoxItemList<EconomicAgentType>(),
                                    AllowToEdit = allowToEdit,
                                    AllowToEditEconomicAgentType = legalFormService.IsPossibilityToEdit(legalForm, user, true),
                                    Title = allowToEdit ? "Редактирование организационно-правовой формы" : "Детали организационно-правовой формы"
                                };

                return model;
            }
        }

        public object Save(LegalFormEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var economicAgentType = ValidationUtils.TryGetEnum<EconomicAgentType>(model.EconomicAgentType);

                LegalForm legalForm;

                if (model.Id == 0)
                {
                    user.CheckPermission(legalFormService.CreationPermission);

                    legalForm = new LegalForm(model.Name, economicAgentType);
                }
                else
                {
                    user.CheckPermission(legalFormService.EditingPermission);

                    legalForm = legalFormService.CheckExistence(model.Id);

                    legalForm.Name = model.Name;
                    var allowToEditEconomicAgentType = legalFormService.IsPossibilityToEdit(legalForm, user, true);

                    if (allowToEditEconomicAgentType)
                    {
                        legalForm.EconomicAgentType = economicAgentType;
                    }
                }

                legalFormService.Save(legalForm);

                uow.Commit();

                return new
                {
                    Name = legalForm.Name,
                    Id = legalForm.Id
                };
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                base.DeleteBaseDictionary(id, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        public void CheckNameUniqueness(string name, short id)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                base.CheckBaseDictionaryNameUniqueness(name, id);
            }
        }

        #endregion

        #endregion
    }
}