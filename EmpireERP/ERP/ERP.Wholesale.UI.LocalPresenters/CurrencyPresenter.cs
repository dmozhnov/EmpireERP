using System;
using System.Collections.Generic;
using System.Data;
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
using ERP.Wholesale.UI.ViewModels.Currency;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class CurrencyPresenter : ICurrencyPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ICurrencyService currencyService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public CurrencyPresenter(IUnitOfWorkFactory unitOfWorkFactory, ICurrencyService currencyService,
            ICurrencyRateService currencyRateService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.currencyService = currencyService;
            this.currencyRateService = currencyRateService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        /// <summary>
        /// Список всех валют
        /// </summary>
        /// <returns></returns>
        public CurrencyListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new CurrencyListViewModel();
                var user = userService.CheckUserExistence(currentUser.Id);

                model.Title = "Валюты";
                model.CurrencyGrid = GetCurrencyGridLocal(new GridState() { Sort = "Name=Asc", PageSize = 25 }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование грида валют
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetCurrencyGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetCurrencyGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида валют
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private GridData GetCurrencyGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var list = currencyService.GetFilteredList(state);

            var model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            model.AddColumn("Name", "Название валюты", Unit.Percentage(100));
            model.AddColumn("LiteralCode", "Символьный код", Unit.Pixel(80));
            model.AddColumn("NumericCode", "Цифровой код", Unit.Pixel(80));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Currency_Create);

            var action = new GridActionCell("Action");

            if (user.HasPermission(Permission.Currency_Edit))
            {
                action.AddAction("Ред.", "edit");
            }
            else
            {
                action.AddAction("Дет.", "details");
            }

            if (user.HasPermission(Permission.Currency_Delete))
            {
                action.AddAction("Удал.", "delete");
            }


            foreach (var row in list)
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("Name") { Value = row.Name },
                    new GridLabelCell("LiteralCode") { Value = row.LiteralCode },
                    new GridLabelCell("NumericCode") { Value = row.NumericCode },
                    new GridHiddenCell("Id") { Value = row.Id.ToString() }
                    ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Добавление / Редактирование / Удаление

        public CurrencyEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Currency_Create);

                var model = new CurrencyEditViewModel();

                model.Title = "Добавление валюты";
                model.CurrencyId = "0";
                model.LiteralCode = "";
                model.Name = "";
                model.NumericCode = "";
                model.AllowToEdit = true;
                model.CurrencyId = "0";
                model.CurrencyRateGrid = GetCurrencyRateGridLocal(new GridState() { Parameters = "CurrencyId=0", PageSize = 5 }, user);

                return model;
            }
        }

        public CurrencyEditViewModel Edit(short currencyId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var currency = currencyService.CheckCurrencyExistence(currencyId);
                var model = new CurrencyEditViewModel();

                model.Title = "Редактирование валюты";
                model.CurrencyId = currency.Id.ToString();
                model.LiteralCode = currency.LiteralCode;
                model.Name = currency.Name;
                model.NumericCode = currency.NumericCode;
                model.CurrencyId = currency.Id.ToString();
                model.AllowToEdit = user.HasPermission(Permission.Currency_Edit);

                model.CurrencyRateGrid = GetCurrencyRateGridLocal(new GridState() { Parameters = "CurrencyId=" + currency.Id.ToString(), PageSize = 5 }, user);

                return model;
            }
        }

        /// <summary>
        /// Формирование курсов валют
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public GridData GetCurrencyRateGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetCurrencyRateGridLocal(state, user);
            }
        }

        private GridData GetCurrencyRateGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var ps = new ParameterString(state.Parameters);
            var currencyId = ValidationUtils.TryGetShort(ps["CurrencyId"].Value as string);
            Currency currency = null;
            if (currencyId > 0)
            {
                currency = currencyService.CheckCurrencyExistence(currencyId);
            }

            var hasPermissonForAction = user.HasPermission(Permission.Currency_AddRate);

            var model = new GridData();

            if (hasPermissonForAction)
            {
                model.AddColumn("Action", "Действие", Unit.Pixel(70), GridCellStyle.Action);
            }
            model.AddColumn("Rate", "Курс", Unit.Pixel(120), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(120));
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(120));
            model.AddColumn("CurrencyRateId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Currency_AddRate);

            if (currency != null)
            {
                foreach (var row in GridUtils.GetEntityRange(currency.Rates.OrderByDescending(x => x).ToList(), state))
                {
                    var action = new GridActionCell("Action");
                    if (hasPermissonForAction)
                    {
                        var isPossibilityToEdit = currencyRateService.IsPossibilityToEditCurrencyRate(row, user, true);
                        var isPossibilityToDelete = currencyRateService.IsPossibilityToDeleteCurrencyRate(row, user, true);

                        if (isPossibilityToEdit)
                        {
                            action.AddAction(new GridActionCell.Action("Ред.", "edit"));
                        }
                        if (isPossibilityToDelete)
                        {
                            action.AddAction(new GridActionCell.Action("Удал.", "delete"));
                        }
                    }
                    model.AddRow(new GridRow(
                        action,
                        new GridLabelCell("Rate") { Value = row.Rate.ForDisplay(ValueDisplayType.CurrencyRate) },
                        new GridLabelCell("StartDate") { Value = row.StartDate.ToShortDateString() },
                        new GridLabelCell("EndDate") { Value = row.EndDate != null ? row.EndDate.Value.ToShortDateString() : "---" },
                        new GridHiddenCell("CurrencyRateId") { Value = row.Id.ToString() }
                        ));
                }

            }
            model.State = state;

            return model;
        }

        public object Save(CurrencyEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var id = ValidationUtils.TryGetShort(model.CurrencyId);

                Currency currency;

                if (id == 0)
                {
                    user.CheckPermission(Permission.Currency_Create);
                    currency = new Currency(model.NumericCode, model.LiteralCode, model.Name);

                }
                else
                {
                    user.CheckPermission(Permission.Currency_Edit);

                    currency = currencyService.CheckCurrencyExistence(id);
                    currency.NumericCode = model.NumericCode;
                    currency.LiteralCode = model.LiteralCode;
                    currency.Name = model.Name;
                }

                currencyService.Save(currency);

                uow.Commit();

                return new { Id = currency.Id };
            }
        }

        public void Delete(short currencyId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Currency_Delete);

                var currency = currencyService.CheckCurrencyExistence(currencyId);
                currencyService.Delete(currency, user);

                uow.Commit();
            }
        }

        #endregion

        #region Добавление курса

        public CurrencyRateEditViewModel CreateRate(short currencyId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Currency_AddRate);

                var model = new CurrencyRateEditViewModel()
                {
                    CurrencyId = currencyId.ToString(),
                    Rate = "",
                    CurrencyRateId = "0",
                    StartDate = DateTimeUtils.GetCurrentDateTime().ToShortDateString(),
                    Title = "Добавление курса валюты",
                    AllowToEditCurrencyRate = true
                };

                return model;
            }
        }

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель данных формы редактирования</returns>
        public CurrencyRateEditViewModel EditRate(int currencyRateId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Currency_AddRate);

                var currencyRate = currencyRateService.CheckCurrencyRateExistence(currencyRateId);

                var model = new CurrencyRateEditViewModel()
                {
                    CurrencyId = currencyRate.Currency.Id.ToString(),
                    Rate = currencyRate.Rate.ForEdit(ValueDisplayType.CurrencyRate),
                    CurrencyRateId = currencyRate.Id.ToString(),
                    StartDate = currencyRate.StartDate.ToShortDateString(),
                    Title = "Редактирование курса валюты",

                    AllowToEditCurrencyRate = currencyRateService.IsPossibilityToEditCurrencyRate(currencyRate, user, true),
                };

                return model;
            }
        }

        /// <summary>
        /// Сохранение курса валюты
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="currentUser">Пользователь</param>
        public void SaveRate(CurrencyRateEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Currency_AddRate);

                var currency = currencyService.CheckCurrencyExistence(ValidationUtils.TryGetShort(model.CurrencyId));
                var baseCurrency = currencyService.GetCurrentBaseCurrency();

                ValidationUtils.NotNull(currency, "Валюта не найдена. Возможно она была удалена.");
                ValidationUtils.NotNull(baseCurrency, "Базовая валюта не найдена. Возможно она была удалена.");

                var rateValue = ValidationUtils.TryGetDecimal(model.Rate, "Указанный курс не корректен.");
                var startDate = ValidationUtils.TryGetDate(model.StartDate, "Указанная дата не корректна.");
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                CurrencyRate rate;
                var rateId = ValidationUtils.TryGetInt(model.CurrencyRateId);

                // Проверяем, создается ли новый курс
                if (rateId == 0)
                {
                    // Да, создается новый курс
                    rate = new CurrencyRate(currency, baseCurrency, rateValue, startDate, currentDate); // Создаем курс валюты
                    currencyRateService.AddRate(currency, rate, user);
                }
                else
                {
                    //Нет, редактируется имеющийся
                    rate = currencyRateService.CheckCurrencyRateExistence(rateId);  // Получаем курс валюты
                    currencyRateService.EditRate(rate, startDate, rateValue, user);
                }

                currencyService.Save(currency); // Сохраняем изменения курса в валюте

                uow.Commit();
            }
        }

        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <param name="currentUser">Пользователь</param>
        public void DeleteRate(int currencyRateId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var rate = currencyRateService.CheckCurrencyRateExistence(currencyRateId);

                currencyRateService.DeleteRate(rate, user);

                uow.Commit();
            }
        }

        /* Метод пока не используется
        public object ImportCurrencyRate(short currencyId)
        {
            var currency = currencyService.CheckCurrencyExistence(currencyId);
            var rate = currencyService.ImportCurrencyRate(currency);

            var model = new CurrencyRateEditViewModel();

            model.CurrencyId = currencyId.ToString();
            model.Rate = rate.ForEdit();
            model.Title = "Добавление курса валюты";

            return model;
        }*/

        #endregion

        #region Выбор курса валюты

        public SelectCurrencyRateViewModel SelectCurrencyRate(short currencyId, string selectFunctionName)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currency = currencyService.CheckCurrencyExistence(currencyId);

                var model = new SelectCurrencyRateViewModel();

                model.Title = String.Format("Курсы валюты");
                model.CurrencyRateGrid = GetSelectCurrencyRateGridLocal(new GridState()
                {
                    Parameters = "CurrencyId=" + currency.Id.ToString() + ";",
                    PageSize = 5,
                    Sort = "StartDate=Desc;"
                });

                model.Filter = new FilterData()
                {
                    Items = new List<FilterItem>()
                    {
                        new FilterDateRangePicker("Date","Курсы за период")
                    }
                };

                model.SelectFunctionName = selectFunctionName;

                return model;
            }
        }

        public GridData GetSelectCurrencyRateGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetSelectCurrencyRateGridLocal(state);
            }
        }

        private GridData GetSelectCurrencyRateGridLocal(GridState state)
        {
            if (state == null)
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var ps = new ParameterString(state.Parameters);
            var currencyId = ValidationUtils.TryGetShort(ps["CurrencyId"].Value as string);

            var list = currencyService.GetCurrencyRateFilteredList(currencyId, state);

            var model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("Rate", "Курс", Unit.Percentage(33), GridCellStyle.Label, GridColumnAlign.Right);
            model.AddColumn("StartDate", "Дата начала", Unit.Percentage(33));
            model.AddColumn("EndDate", "Дата завершения", Unit.Percentage(33));
            model.AddColumn("RateForEdit", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "selectCurrencyRate");

            foreach (var row in list)
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("Rate") { Value = row.Rate.ForDisplay(ValueDisplayType.CurrencyRate) },
                        new GridLabelCell("StartDate") { Value = row.StartDate.ToShortDateString() },
                        new GridLabelCell("EndDate") { Value = row.EndDate.ForDisplay() },
                        new GridHiddenCell("RateForEdit") { Value = row.Rate.ForEdit() },
                        new GridHiddenCell("Id") { Value = row.Id.ToString() }
                        ));
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Получение курса валюты

        public object GetCurrentCurrencyRate(short currencyId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currency = currencyService.CheckCurrencyExistence(currencyId);
                var currencyRate = currencyService.GetCurrentCurrencyRate(currency);

                object result = new
                {
                    CurrencyRate = currencyRate != null ? currencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate) : "---",
                    CurrencyRateForEdit = currencyRate != null ? currencyRate.Rate.ForEdit() : "",
                    CurrencyRateId = currencyRate != null ? currencyRate.Id.ToString() : "",
                    CurrencyRateStartDate = currencyRate != null ? currencyRate.StartDate.ToShortDateString() : "---"
                };

                return result;
            }
        }

        public object GetCurrencyRate(int currencyRateId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var currencyRate = currencyRateService.CheckCurrencyRateExistence(currencyRateId);

                object result = new
                {
                    CurrencyRate = currencyRate.Rate.ForDisplay(ValueDisplayType.CurrencyRate),
                    CurrencyRateForEdit = currencyRate.Rate.ForEdit(),
                    CurrencyRateId = currencyRate.Id.ToString(),
                    CurrencyRateStartDate = currencyRate.StartDate.ToShortDateString()
                };

                return result;
            }
        }

        #endregion

        #endregion

    }
}