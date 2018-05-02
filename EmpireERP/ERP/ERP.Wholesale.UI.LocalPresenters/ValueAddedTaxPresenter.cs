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
using ERP.Wholesale.UI.ViewModels.ValueAddedTax;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ValueAddedTaxPresenter : BaseDictionaryPresenter<ValueAddedTax>, IValueAddedTaxPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IValueAddedTaxService valueAddedTaxService;

        #endregion

        #region Конструктор

        public ValueAddedTaxPresenter(IUnitOfWorkFactory unitOfWorkFactory, IValueAddedTaxService valueAddedTaxService, IUserService userService)
            : base(valueAddedTaxService, userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.valueAddedTaxService = valueAddedTaxService;
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

        public GridData GetValueAddedTaxGrid(GridState state, UserInfo currentUser)
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

            bool allowToDelete = user.HasPermission(valueAddedTaxService.DeletionPermission);
            bool allowToEdit = user.HasPermission(valueAddedTaxService.EditingPermission);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Name", "Наименование", Unit.Percentage(100));
            model.AddColumn("Value", "Значение", Unit.Pixel(55));
            model.AddColumn("IsDefault", "Используется по умолчанию", Unit.Pixel(100));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(valueAddedTaxService.CreationPermission);

            var rows = valueAddedTaxService.GetFilteredList(state);
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
                                 new GridLabelCell("Name") {Value = item.Name, Key = "Name"},
                                 new GridLabelCell("Value")
                                     {Value = item.Value.ForDisplay(ValueDisplayType.Percent), Key = "Value"},
                                 new GridLabelCell("IsDefault")
                                     {Value = item.IsDefault ? "Да" : "Нет", Key = "IsDefault"},
                                 new GridHiddenCell("Id") {Value = item.Id.ToString(), Key = "Id"}
                                 ));
            }

            return model;
        }

        #endregion

        #region Создание / редактирование / удаление

        public ValueAddedTaxEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ValueAddedTax_Create);

                var model = new ValueAddedTaxEditViewModel()
                {
                    AllowToEditValue = true,
                    Title = "Добавление ставки НДС",
                    AllowToEdit = true,
                    IsDefault = "0",
                    Value = 0M.ForDisplay(ValueDisplayType.Percent)
                };

                return model;
            }
        }

        public ValueAddedTaxEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var valueAddedTax = valueAddedTaxService.CheckExistence(id);
                var allowToEdit = user.HasPermission(Permission.ValueAddedTax_Edit);

                var model = new ValueAddedTaxEditViewModel
                                {
                                    Id = valueAddedTax.Id,
                                    AllowToEditValue = false,
                                    Name = valueAddedTax.Name,
                                    Value = valueAddedTax.Value.ForDisplay(ValueDisplayType.Percent),
                                    AllowToEdit = allowToEdit,
                                    IsDefault = valueAddedTax.IsDefault ? "1" : "0",
                                    Title = allowToEdit ? "Редактирование ставки НДС" : "Детали ставки НДС"
                                };

                return model;
            }
        }

        public object Save(ValueAddedTaxEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var value = ValidationUtils.TryGetDecimal(model.Value);
                var isDefault = ValidationUtils.TryGetBool(model.IsDefault);
                
                ValueAddedTax valueAddedTax;

                if (model.Id == 0)
                {
                    user.CheckPermission(valueAddedTaxService.CreationPermission);

                    valueAddedTax = new ValueAddedTax(model.Name, value, isDefault);
                }
                else
                {
                    user.CheckPermission(valueAddedTaxService.EditingPermission);

                    valueAddedTax = valueAddedTaxService.CheckExistence(model.Id);

                    valueAddedTax.Name = model.Name;
                    valueAddedTax.IsDefault = isDefault;
                }

                valueAddedTaxService.Save(valueAddedTax);

                uow.Commit();

                return new
                {
                    Name = valueAddedTax.Name,
                    Id = valueAddedTax.Id
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