using System.Collections.Generic;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using System;

namespace ERP.Wholesale.UI.LocalPresenters
{
    /// <summary>
    /// Базовый абстрактный класс, представляющий презентер для типовых справочников. Реализует основной общий функционал
    /// </summary>
    /// <typeparam name="T">Тип сущности для справочника</typeparam>
    public abstract class BaseDictionaryPresenter<T> : IBaseDictionaryPresenter<T> where T : BaseDictionary, new()
    {
        #region Поля

        protected readonly IBaseDictionaryService<T> baseDictionaryService;
        protected readonly IUserService userService;

        #endregion

        #region Конструктор

        protected BaseDictionaryPresenter(IBaseDictionaryService<T> baseDictionaryService, IUserService userService)
        {
            this.baseDictionaryService = baseDictionaryService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        /// <summary>
        /// Базовый метод для получения грида для списка
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected BaseDictionaryListViewModel ListBaseDictionary(UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            var model = new BaseDictionaryListViewModel()
            {
                Data = GetBaseDictionaryGridLocal(new GridState() { Sort = "Name=Asc" }, user)
            };

            return model;
        }

        /// <summary>
        /// Базовый метод для обновления грида в списке
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected GridData GetBaseDictionaryGrid(GridState state, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            return GetBaseDictionaryGridLocal(state, user);
        }

        /// <summary>
        /// Базовый метод заполнения грида для списка
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual GridData GetBaseDictionaryGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState() { Sort = "Name=Asc" };

            bool allowToDelete = user.HasPermission(baseDictionaryService.DeletionPermission);
            bool allowToEdit = user.HasPermission(baseDictionaryService.EditingPermission);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Name", "Наименование", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(baseDictionaryService.CreationPermission);

            var rows = baseDictionaryService.GetFilteredList(state);
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
                    new GridLabelCell("Name") { Value = item.Name, Key = "Name" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        }

        /// <summary>
        /// Базовый метод получения сущностей для ComboBox
        /// </summary>
        /// <returns></returns>
        protected object GetBaseDictionarySelectList()
        {
            var types = baseDictionaryService.GetList().GetComboBoxItemList(p => p.Name, p => p.Id.ToString(), false);

            return new { List = types };
        }

        #endregion

        #region Создание / редактирование / удаление

        /// <summary>
        /// Базовый метод создания получения модели для создания сущности
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected BaseDictionaryEditViewModel CreateBaseDictionary(UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            user.CheckPermission(baseDictionaryService.CreationPermission);

            var model = new BaseDictionaryEditViewModel()
            {
                AllowToEdit = true
            };

            return model;
        }

        /// <summary>
        /// Базовый метод получения модели для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected BaseDictionaryEditViewModel EditBaseDictionary(short id, UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);
            var dictionaryElement = baseDictionaryService.CheckExistence(id);

            bool allowToEdit = user.HasPermission(baseDictionaryService.EditingPermission);

            var model = new BaseDictionaryEditViewModel()
            {
                AllowToEdit = allowToEdit,
                Name = dictionaryElement.Name,
                Id = dictionaryElement.Id,
            };

            return model;
        }

        /// <summary>
        /// Базовый метод сохранения сущности
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected T SaveBaseDictionary(BaseDictionaryEditViewModel model, UserInfo currentUser, Action<T> additionalAction = null)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            T dictionary;

            if (model.Id == 0)
            {
                user.CheckPermission(baseDictionaryService.CreationPermission);

                dictionary = new T();
                dictionary.Name = model.Name;
            }
            else
            {
                user.CheckPermission(baseDictionaryService.EditingPermission);

                dictionary = baseDictionaryService.CheckExistence(model.Id);

                dictionary.Name = model.Name;
            }

            if (additionalAction != null)
            {
                additionalAction(dictionary);
            }

            baseDictionaryService.Save(dictionary);

            return dictionary;
        }

        /// <summary>
        /// Базовый метод удаления сущности
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        protected void DeleteBaseDictionary(short id, UserInfo currentUser)
        {
            var dictionary = baseDictionaryService.CheckExistence(id);
            var user = userService.CheckUserExistence(currentUser.Id);

            baseDictionaryService.Delete(dictionary, user);
        }

        #endregion

        #region Выбор

        /// <summary>
        /// Базовый метод получения модели грида для выбора сущности
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        protected BaseDictionarySelectViewModel SelectBaseDictionary(UserInfo currentUser)
        {
            var user = userService.CheckUserExistence(currentUser.Id);

            var model = new BaseDictionarySelectViewModel()
            {
                Title = "Выбор",
                AllowToCreate = user.HasPermission(baseDictionaryService.CreationPermission),
                Filter = new FilterData()
                {
                    Items = new List<FilterItem>()
                    {
                        new FilterTextEditor("Name", "Наименование")
                    }
                },
                Grid = GetBaseDictionarySelectGridLocal(new GridState() { PageSize = 5 })
            };

            return model;
        }

        /// <summary>
        /// Базовый метод обновления гради в окне выбора
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected GridData GetBaseDictionarySelectGrid(GridState state)
        {
            return GetBaseDictionarySelectGridLocal(state);
        }

        private GridData GetBaseDictionarySelectGridLocal(GridState state)
        {
            if (state == null) state = new GridState() { Sort = "Name=Asc" };

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("Name", "Наименование", Unit.Percentage(100));
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            var rows = baseDictionaryService.GetFilteredList(state);
            model.State = state;

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridPseudoLinkCell("Name") { Value = item.Name, Key = "Name" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименвоание</param>
        /// <param name="id">id сущности</param>
        protected void CheckBaseDictionaryNameUniqueness(string name, short id)
        {
            baseDictionaryService.CheckNameUniqueness(id, name);
        }

        #endregion

        #endregion
    }
}