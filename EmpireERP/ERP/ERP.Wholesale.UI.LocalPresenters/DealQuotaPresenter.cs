using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealQuota;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class DealQuotaPresenter : IDealQuotaPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IDealQuotaService dealQuotaService;
        private readonly IUserService userService;
        private readonly IDealService dealService;

        #endregion

        #region Конструктор

        public DealQuotaPresenter(IUnitOfWorkFactory unitOfWorkFactory, IDealQuotaService dealQuotaService, IDealService dealService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.dealQuotaService = dealQuotaService;
            this.userService = userService;
            this.dealService = dealService;
        }

        #endregion

        #region Методы

        public DealQuotaListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.DealQuota_List_Details);

                var model = new DealQuotaListViewModel();
                model.ActiveDealQuotaGrid = GetActiveDealQuotaGridLocal(new GridState { Sort = "CreationDate=Desc" }, user);
                model.InactiveDealQuotaGrid = GetInactiveDealQuotaGridLocal(new GridState { Sort = "CreationDate=Desc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.Filter.Items.Add(new FilterTextEditor("DiscountPercent", "Процент скидки"));
                model.Filter.Items.Add(new FilterTextEditor("PostPaymentDays", "Отсрочка платежа (дн.)"));
                model.Filter.Items.Add(new FilterTextEditor("CreditLimitSum", "Кредитный лимит"));

                return model;
            }
        }

        public GridData GetActiveDealQuotaGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetActiveDealQuotaGridLocal(state, user);
            }
        }

        private GridData GetActiveDealQuotaGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("QuotaName", "Название квоты", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(70));
            model.AddColumn("PostPaymentDays", "Отсрочка платежа", Unit.Pixel(110), align: GridColumnAlign.Right);
            model.AddColumn("CreditLimit", "Кредитный лимит", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("DiscountPercent", "Скидка", Unit.Pixel(65), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveFilter = new ParameterString(state.Filter);

            deriveFilter.Add("EndDate", ParameterStringItem.OperationType.IsNull);

            model.ButtonPermissions["AllowToCreate"] = dealQuotaService.IsPossibilityToCreate(user);

            var dealQuotas = dealQuotaService.GetFilteredList(state, deriveFilter, user);

            //проверяем только права, так как галочку "действует/не действует" по бизнес-логике можно изменять всегда
            var allowToEdit = user.HasPermission(Permission.DealQuota_Edit);

            foreach (var quota in dealQuotas)
            {
                var actions = new GridActionCell("Action");

                if (allowToEdit)
                {
                    actions.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actions.AddAction("Дет.", "details_link");
                }

                if (dealQuotaService.IsPossibilityToDelete(quota, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("QuotaName") { Value = quota.Name },
                    new GridLabelCell("Date") { Value = quota.StartDate.ToShortDateString() },
                    new GridLabelCell("PostPaymentDays") { Value = quota.PostPaymentDays.HasValue ? quota.PostPaymentDays.Value.ForDisplay() : "---" },
                    new GridLabelCell("CreditLimit") { Value = quota.CreditLimitSum.HasValue ? quota.CreditLimitSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("DiscountPercent") { Value = quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = quota.Id.ToString(), Key = "quotaId" }
                    ));
            }

            model.State = state;
            model.Title = "Действующие квоты";

            return model;
        }

        public GridData GetInactiveDealQuotaGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetInactiveDealQuotaGridLocal(state, user);
            }
        }

        private GridData GetInactiveDealQuotaGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("QuotaName", "Название квоты", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(70));
            model.AddColumn("PostPaymentDays", "Отсрочка платежа", Unit.Pixel(110), align: GridColumnAlign.Right);
            model.AddColumn("CreditLimit", "Кредитный лимит", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("DiscountPercent", "Скидка", Unit.Pixel(65), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);

            deriveFilter.Add("EndDate", ParameterStringItem.OperationType.IsNotNull);

            var dealQuotas = dealQuotaService.GetFilteredList(state, deriveFilter, user);

            //проверяем только права, так как галочку "действует/не действует" по бизнес-логике можно изменять всегда
            var allowToEdit = dealQuotas.Any() ? dealQuotaService.IsPossibilityToEdit(dealQuotas.First(), user, false) : false;

            foreach (var quota in dealQuotas)
            {
                var actions = new GridActionCell("Action");

                if (allowToEdit)
                {
                    actions.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actions.AddAction("Дет.", "details_link");
                }

                if (dealQuotaService.IsPossibilityToDelete(quota, user))
                {
                    actions.AddAction("Удал.", "delete_link");
                }

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("QuotaName") { Value = quota.Name },
                    new GridLabelCell("Date") { Value = quota.StartDate.ToShortDateString() },
                    new GridLabelCell("PostPaymentDays") { Value = quota.PostPaymentDays.HasValue ? quota.PostPaymentDays.Value.ForDisplay() : "---" },
                    new GridLabelCell("CreditLimit") { Value = quota.CreditLimitSum.HasValue ? quota.CreditLimitSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("DiscountPercent") { Value = quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = quota.Id.ToString(), Key = "quotaId" }
                    ));
            }

            model.State = state;
            model.Title = "Недействующие квоты";

            return model;
        }

        public DealQuotaEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                dealQuotaService.CheckPossibilityToCreate(user);

                DealQuotaEditViewModel model = new DealQuotaEditViewModel();

                model.Name = "";
                model.Id = 0;
                model.Title = "Создание новой квоты";
                model.StartDate = DateTime.Today.ToShortDateString();
                model.EndDate = "---";
                model.AllowToEdit = true;
                model.IsPossibilityToEdit = true;

                return model;
            }
        }

        public DealQuotaEditViewModel Edit(int id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var quota = dealQuotaService.CheckDealQuotaExistence(id, user);

                var model = GetQuotaViewModel(quota, false, user);

                return model;
            }
        }

        /// <summary>
        /// Заполнение ViewModel на основе квоты
        /// </summary>
        /// <param name="quota">Квота</param>
        /// <param name="forCreation">Если false, модель создается не для редактирования/просмотра деталей данной квоты, а для создания новой на ее основе</param>
        /// <returns></returns>
        private DealQuotaEditViewModel GetQuotaViewModel(DealQuota quota, bool forCreation, User user)
        {
            var model = new DealQuotaEditViewModel();

            bool possibilityToEdit = dealQuotaService.IsPossibilityToEdit(quota, user, false); //проверяем только права (для галки "активно/не активно")
            bool allowToEdit = dealQuotaService.IsPossibilityToEdit(quota, user);

            model.Id = quota.Id;
            model.Name = quota.Name;
            model.Title = allowToEdit ? "Редактирование квоты" : "Детали квоты";
            model.DiscountPercent = allowToEdit || forCreation ? quota.DiscountPercent.ForEdit() : quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent);
            model.StartDate = quota.StartDate.ToShortDateString();
            model.EndDate = quota.EndDate.HasValue ? quota.EndDate.Value.ToShortDateString() : "---";

            if (!quota.IsPrepayment)
            {
                model.CreditLimitSum = allowToEdit || forCreation ? quota.CreditLimitSum.Value.ForEdit() : quota.CreditLimitSum.Value.ForDisplay(ValueDisplayType.Money);
                model.PostPaymentDays = allowToEdit || forCreation ? quota.PostPaymentDays.Value.ToString() : quota.PostPaymentDays.Value.ForDisplay();
            }
            else
            {
                model.CreditLimitSum = "";
                model.PostPaymentDays = "";
            }

            model.IsPrepayment = quota.IsPrepayment ? (byte)1 : (byte)0;
            model.AllowToEdit = allowToEdit;
            model.IsPossibilityToEdit = possibilityToEdit;

            model.IsActive = (quota.IsActive == true ? "1" : "0");

            return model;
        }

        public object Save(DealQuotaEditViewModel model, UserInfo currentUser)
        {
            DealQuota quota = null;

            ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                if (model.Id == 0)
                {
                    dealQuotaService.CheckPossibilityToCreate(user);

                    decimal discountPercent = String.IsNullOrEmpty(model.DiscountPercent) ? 0 : ValidationUtils.TryGetDecimal(model.DiscountPercent);

                    switch (model.IsPrepayment)
                    {
                        case 0: // Без предоплаты                               
                            quota = new DealQuota(model.Name, discountPercent, ValidationUtils.TryGetShort(model.PostPaymentDays), ValidationUtils.TryGetDecimal(model.CreditLimitSum));
                            break;
                        case 1: // С предоплатой
                            quota = new DealQuota(model.Name, discountPercent);
                            break;
                        default:
                            throw new Exception("Неизвестный тип квоты.");
                    }

                    quota.IsActive = (model.IsActive == "1" ? true : false);
                }
                else
                {
                    dealQuotaService.CheckPossibilityToEdit(quota, user, false);

                    quota = dealQuotaService.CheckDealQuotaExistence(model.Id, user);

                    quota.Name = model.Name;

                    if (quota.IsActive && model.IsActive == "0")
                    {
                        dealQuotaService.RemoveDealQuotaFromAllDeals(quota);
                    }

                    quota.IsActive = (model.IsActive == "1" ? true : false);

                    if (dealQuotaService.IsPossibilityToEdit(quota, user))
                    {
                        quota.IsPrepayment = (model.IsPrepayment == 1);

                        quota.CreditLimitSum = (model.CreditLimitSum != "" && !quota.IsPrepayment) ? ValidationUtils.TryGetDecimal(model.CreditLimitSum) : (decimal?)null;
                        quota.PostPaymentDays = (model.PostPaymentDays != "" && !quota.IsPrepayment) ? ValidationUtils.TryGetShort(model.PostPaymentDays) : (short?)null;

                        quota.DiscountPercent = ValidationUtils.TryGetDecimal(model.DiscountPercent);                        
                    }
                }

                dealQuotaService.Save(quota, user);

                uow.Commit();
            }

            return new { IsActive = quota.IsActive };
        }

        public void Delete(int id, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var quota = dealQuotaService.CheckDealQuotaExistence(id, user);

                dealQuotaService.Delete(quota, user);

                uow.Commit();
            }
        }

        /// <summary>
        /// Возвращает грид квот, которые можно добавить в сделку, то есть:
        /// - действующие
        /// - не добавленные в эту сделку
        /// </summary>
        /// <param name="dealId"></param>
        /// <returns></returns>
        public DealQuotaSelectViewModel SelectDealQuota(int dealId, UserInfo currentUser, string mode)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);
                
                dealService.CheckPossibilityToAddQuota(deal, user); // Проверяем права на добавление квоты

                var model = new DealQuotaSelectViewModel();

                model.Title = "Выбор квоты";

                model.FilterData = new FilterData();
                model.FilterData.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.FilterData.Items.Add(new FilterTextEditor("DiscountPercent", "Процент скидки"));
                model.FilterData.Items.Add(new FilterTextEditor("PostPaymentDays", "Отсрочка платежа (дн.)"));
                model.FilterData.Items.Add(new FilterTextEditor("CreditLimitSum", "Кредитный лимит"));

                model.DealQuotasGrid = GetDealQuotaSelectGridLocal(new GridState()
                {
                    Parameters = "Mode=" + mode + ";DealId=" + dealId.ToString(),
                    PageSize = 5,
                    Sort = "CreationDate=Desc",
                }, user);

                return model;
            }
        }

        public GridData GetDealQuotaSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetDealQuotaSelectGridLocal(state, user);
            }
        }

        private GridData GetDealQuotaSelectGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();

            var model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(60));
            model.AddColumn("QuotaName", "Название квоты", Unit.Percentage(100));
            model.AddColumn("Date", "Дата", Unit.Pixel(70));
            model.AddColumn("PostPaymentDays", "Отсрочка платежа", Unit.Pixel(110), align: GridColumnAlign.Right);
            model.AddColumn("CreditLimit", "Кредитный лимит", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("DiscountPercent", "Скидка", Unit.Pixel(65), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("IsPrepayment", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("QuotaFullName", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var deriveParameters = new ParameterString(state.Parameters);

            string mode = deriveParameters["Mode"].Value as string;
            string dealId = deriveParameters["DealId"].Value as string;

            ParameterString ps = new ParameterString(state.Filter);
            ps.Add("EndDate", ParameterStringItem.OperationType.IsNull);
            ps.Add("DealId", ParameterStringItem.OperationType.Eq, dealId);
            ps.Add("Mode", ParameterStringItem.OperationType.Eq, mode);

            var dealQuotas = dealQuotaService.GetFilteredList(state, ps, user);

            foreach (var quota in dealQuotas)
            {
                var actions = new GridActionCell("Action");
                actions.AddAction("Выбрать", "dealQuota_select_link");

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("QuotaName") { Value = quota.Name, Key = "quotaName" },
                    new GridLabelCell("Date") { Value = quota.StartDate.ToShortDateString() },
                    new GridLabelCell("PostPaymentDays") { Value = quota.PostPaymentDays.HasValue ? quota.PostPaymentDays.Value.ForDisplay() : "---" },
                    new GridLabelCell("CreditLimit") { Value = quota.CreditLimitSum.HasValue ? quota.CreditLimitSum.Value.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("DiscountPercent") { Value = quota.DiscountPercent.ForDisplay(ValueDisplayType.Percent) + " %" },
                    new GridHiddenCell("Id") { Value = quota.Id.ToString(), Key = "quotaId" },
                    new GridHiddenCell("IsPrepayment") { Value = quota.IsPrepayment ? "1" : "0", Key = "isPrepayment" },
                    new GridHiddenCell("QuotaFullName") { Value = quota.FullName, Key = "quotaFullName" }
                    ));
            }

            model.State = state;            

            if (mode == "Deal")
            {
                model.Title = "Активные квоты";
            }
            else
                if (mode == "Sale")
                {
                    model.Title = "Квоты сделки";
                }

            return model;
        }

        #endregion
    }
}