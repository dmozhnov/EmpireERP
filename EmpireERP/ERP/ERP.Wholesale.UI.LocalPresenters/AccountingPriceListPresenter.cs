using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.AccountingPriceList;
using ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class AccountingPriceListPresenter : IAccountingPriceListPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IStorageService storageService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IArticleService articleService;
        private readonly IArticleGroupService articleGroupService;
        private readonly IUserService userService;

        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IArticlePriceService articlePriceService;
        private readonly IAccountingPriceCalcService accountingPriceCalcService;
        private readonly IAccountingPriceListService accountingPriceListService;
        private readonly IAccountingPriceCalcRuleService accountingPriceCalcRuleService;

        private readonly IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService;
        private readonly IAccountingPriceListMainIndicatorService accountingPriceListMainIndicatorService;

        #endregion

        #region Конструкторы

        public AccountingPriceListPresenter(IUnitOfWorkFactory unitOfWorkFactory, IStorageService storageService, IReceiptWaybillService receiptWaybillService, IArticleService articleService,
            IArticleGroupService articleGroupService, IArticleAvailabilityService articleAvailabilityService, IArticlePriceService articlePriceService, IAccountingPriceCalcService accountingPriceCalcService,
            IAccountingPriceListService accountingPriceListService, IAccountingPriceCalcRuleService accountingPriceCalcRuleService,
            IArticleAccountingPriceIndicatorService articleAccountingPriceIndicatorService, IAccountingPriceListMainIndicatorService accountingPriceListMainIndicatorService,
            IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.storageService = storageService;
            this.receiptWaybillService = receiptWaybillService;
            this.articleService = articleService;
            this.articleGroupService = articleGroupService;
            this.userService = userService;

            this.articleAvailabilityService = articleAvailabilityService;
            this.articlePriceService = articlePriceService;
            this.accountingPriceCalcService = accountingPriceCalcService;
            this.accountingPriceListService = accountingPriceListService;
            this.accountingPriceCalcRuleService = accountingPriceCalcRuleService;

            this.articleAccountingPriceIndicatorService = articleAccountingPriceIndicatorService;
            this.accountingPriceListMainIndicatorService = accountingPriceListMainIndicatorService;
        }

        #endregion

        #region Методы

        #region Расчет подсказок для редактирования строки реестра

        /// <summary>
        /// Расчет подсказок, выводимых на форму редактирования строки реестра
        /// Для расчета учетной цены методу необходимо знать, какие заданы правила расчета учетной цены и формирования последней цифры, поэтому передается идентификатор реестра
        /// Метод вызывается из представления, поэтому передаются идентификаторы
        /// </summary>
        /// <param name="accountingPriceListId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public ArticleAccountingPriceEditViewModel GetTipsForArticle(Guid accountingPriceListId, int articleId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);
                var article = articleService.CheckArticleExistence(articleId);

                var model = GetArticleAccountingPriceEditViewModel(accountingPriceList, article, user);

                return model;
            }
        }

        private ArticleAccountingPriceEditViewModel GetArticleAccountingPriceEditViewModel(AccountingPriceList accountingPriceList, Article article, User user, Guid? articleAccountingPriceId = null)
        {
            decimal? avgAccPrice, maxAccPrice, minAccPrice, avgPurchaseCost, minPurchaseCost, maxPurchaseCost, lastPurchaseCost;
            accountingPriceListService.GetTipsForArticle(accountingPriceList, article,
                out avgAccPrice, out minAccPrice, out maxAccPrice, out avgPurchaseCost, out minPurchaseCost, out maxPurchaseCost, out lastPurchaseCost, user);

            bool accPriceCalc = false, lastDigitError = false;

            var calculatedAccountingPrice = accountingPriceListService.CalculateDefaultAccountingPriceByRule(accountingPriceList, article, out accPriceCalc, out lastDigitError, user);
            var accountingPrice = accountingPriceListService.GetAccountingPriceForArticle(accountingPriceList, articleAccountingPriceId);

            var rule = accountingPriceList.AccountingPriceCalcRule;
            var accountingPriceRule = rule.GetDisplayName();
            var markupPercent = rule.GetMarkupPercent(article);

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            if (!allowToViewPurchaseCost)
            {
                avgPurchaseCost = minPurchaseCost = maxPurchaseCost = lastPurchaseCost = null;
            }

            var isPossibilityToEditRow = accountingPriceListService.IsPossibilityToEditRow(accountingPriceList, user);
            var isPossibilityToEditPrice = accountingPriceListService.IsPossibilityToEditPrice(accountingPriceList, user);

            var model = new ArticleAccountingPriceEditViewModel
            {
                Id = articleAccountingPriceId ?? Guid.Empty,
                Title = isPossibilityToEditRow || isPossibilityToEditPrice ? "Редактирование товара в реестре цен" : "Детали товара в реестре цен",
                AccountingPriceListId = accountingPriceList.Id,
                ArticleId = article.Id,
                ArticleName = article.FullName,
                ArticleIdForDisplay = article.Id.ToString(),
                ArticleNumber = article.Number,
                AccountingPrice = accountingPrice == null ? "" :
                    isPossibilityToEditPrice ? accountingPrice.Value.ForEdit() : accountingPrice.Value.ForDisplay(ValueDisplayType.Money),
                CalculatedAccountingPrice = calculatedAccountingPrice.ForDisplay(ValueDisplayType.Money),
                AverageAccountingPrice = avgAccPrice.ForDisplay(ValueDisplayType.Money),
                AveragePurchaseCost = avgPurchaseCost.ForDisplay(ValueDisplayType.Money),
                MaxAccountingPrice = maxAccPrice.ForDisplay(ValueDisplayType.Money),
                MaxPurchaseCost = maxPurchaseCost.ForDisplay(ValueDisplayType.Money),
                MinAccountingPrice = minAccPrice.ForDisplay(ValueDisplayType.Money),
                MinPurchaseCost = minPurchaseCost.ForDisplay(ValueDisplayType.Money),
                LastPurchaseCost = lastPurchaseCost.ForDisplay(ValueDisplayType.Money),
                DefaultMarkupPercent = markupPercent.ForDisplay(ValueDisplayType.Percent),
                AccountingPriceRule = accountingPriceRule,
                UsedDefaultRule = (accPriceCalc || lastDigitError) ? "1" : "0",

                AllowToEdit = isPossibilityToEditRow,
                AllowToEditPrice = isPossibilityToEditPrice
            };

            return model;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение данных "шапки" реестра, обновляющихся динамически помимо грида товаров.
        /// Внимание! Должна вызываться после всех изменений, вносимых в сущность реестра.
        /// </summary>
        private object GetMainChangeableIndicators(AccountingPriceList accountingPriceList, User user)
        {
            var indicators = accountingPriceListMainIndicatorService.GetMainIndicators(accountingPriceList, user, calculateChangesAndMarkups: true);

            var j = new
            {
                PurchaseCostSum = indicators.PurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                OldAccountingPriceSum = indicators.OldAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                NewAccountingPriceSum = indicators.NewAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                AccountingPriceDifPercent = indicators.AccountingPriceChangePercent.ForDisplay(ValueDisplayType.Percent),
                AccountingPriceDifSum = indicators.AccountingPriceChangeSum.ForDisplay(ValueDisplayType.Money),
                PurchaseMarkupPercent = indicators.PurchaseMarkupPercent.ForDisplay(ValueDisplayType.Percent),
                PurchaseMarkupSum = indicators.PurchaseMarkupSum.ForDisplay(ValueDisplayType.Money),
                RowCount = accountingPriceList.ArticleAccountingPriceCount,

                AllowToAccept = accountingPriceListService.IsPossibilityToAccept(accountingPriceList, user, false),
                AllowToPrintForms = accountingPriceListService.IsPossibilityToPrintForms(accountingPriceList, user)
            };

            return j;
        }

        #endregion

        #region Список реестров цен

        public AccountingPriceListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountingPriceList_List_Details);

                var model = new AccountingPriceListViewModel();

                model.NewList = GetNewAccountingPriceListGridLocal(new GridState() { Sort = "StartDate=Desc" }, user);
                model.AcceptedList = GetAcceptedAccountingPriceListGridLocal(new GridState() { Sort = "StartDate=Desc" }, user);

                model.FilterData.Items.Add(new FilterDateRangePicker("StartDate", "Дата начала"));
                model.FilterData.Items.Add(new FilterTextEditor("Number", "№ документа"));
                model.FilterData.Items.Add(new FilterComboBox("Reason", "Основание", GetReasonList()));
                model.FilterData.Items.Add(new FilterComboBox("Storage", "Распространение",
                    storageService.GetList(user, Permission.AccountingPriceList_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                    .GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false)));

                return model;
            }
        }

        private IList<SelectListItem> GetReasonList()
        {
            var list = new List<AccountingPriceListReason>(
                (IEnumerable<AccountingPriceListReason>)Enum.GetValues(typeof(AccountingPriceListReason)));
            IList<SelectListItem> result = new List<SelectListItem>();

            result.Add(new SelectListItem() { Text = " ", Value = "" });
            foreach (var value in list)
            {
                if (value == AccountingPriceListReason.ReceiptWaybill)
                {
                    result.Add(new SelectListItem() { Text = "Приход", Value = value.ValueToString() });
                }
                else
                {
                    result.Add(new SelectListItem() { Text = value.GetDisplayName(), Value = value.ValueToString() });
                }
            }

            return result;
        }

        public GridData GetNewAccountingPriceListGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetNewAccountingPriceListGridLocal(state, user);
            }
        }

        public GridData GetAcceptedAccountingPriceListGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAcceptedAccountingPriceListGridLocal(state, user);
            }
        }

        private GridData GetNewAccountingPriceListGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            // Добавляем столбцы
            model.AddColumn("Number", "Номер", Unit.Pixel(60));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(60));
            model.AddColumn("EndDate", "Дата заверш.", Unit.Pixel(60));
            model.AddColumn("Storages", "Распространение", Unit.Percentage(75));
            model.AddColumn("Reason", "Основание", Unit.Percentage(25));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("OldAccountingPriceSum", "Сумма в старых УЦ", Unit.Pixel(93), align: GridColumnAlign.Right);
            model.AddColumn("NewAccountingPriceSum", "Сумма в новых УЦ", Unit.Pixel(93), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptWaybillId", "", GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.AccountingPriceList_Create);

            var filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            state.Filter = "";

            deriveFilter.Add("State", ParameterStringItem.OperationType.Eq, ((int)AccountingPriceListState.New).ToString());

            var rows = accountingPriceListService.GetFilteredList(state, deriveFilter, user);

            state.Filter = filter;
            model.State = state;

            foreach (var item in rows)
            {
                var indicators = accountingPriceListMainIndicatorService.GetMainIndicators(item, user);

                GridCell cellReason = null;
                if (item.Reason == AccountingPriceListReason.ReceiptWaybill && user.HasPermission(Permission.ReceiptWaybill_List_Details))
                {
                    cellReason = new GridLinkCell("Reason") { Value = item.ReasonDescription };
                }
                else
                {
                    cellReason = new GridLabelCell("Reason") { Value = item.ReasonDescription };
                }

                var storages = storageService.FilterByUser(item.Storages, user, Permission.AccountingPriceList_List_Details)
                    .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).Select(x => x.Name);

                var storagesString = storages.Any() ? String.Join(", ", storages) : "---";

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = item.Number.PadLeftZeroes(8) },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = item.EndDate != null ? ((DateTime)item.EndDate).ToShortDateString() : "---" },
                    new GridLabelCell("Storages") { Value = storagesString },
                    cellReason,
                    new GridLabelCell("PurchaseCostSum") { Value = indicators.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("OldAccountingPriceSum") { Value = indicators.OldAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("NewAccountingPriceSum") { Value = indicators.NewAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() },
                    new GridHiddenCell("ReceiptWaybillId")
                    {
                        Value = item.ReasonReceiptWaybillId.HasValue && user.HasPermission(Permission.ReceiptWaybill_List_Details) ?
                            item.ReasonReceiptWaybillId.ToString() : ""
                    }
                ));
            }


            return model;
        }

        private GridData GetAcceptedAccountingPriceListGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            // Добавляем столбцы
            model.AddColumn("Number", "Номер", Unit.Pixel(60));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(60));
            model.AddColumn("EndDate", "Дата заверш.", Unit.Pixel(60));
            model.AddColumn("Storages", "Распространение", Unit.Percentage(75));
            model.AddColumn("Reason", "Основание", Unit.Percentage(25));
            model.AddColumn("PurchaseCostSum", "Сумма в ЗЦ", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("OldAccountingPriceSum", "Сумма в старых УЦ", Unit.Pixel(93), align: GridColumnAlign.Right);
            model.AddColumn("NewAccountingPriceSum", "Сумма в новых УЦ", Unit.Pixel(93), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptWaybillId", "", GridCellStyle.Hidden);

            var filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);
            state.Filter = "";

            deriveFilter.Add("State", ParameterStringItem.OperationType.Eq, ((int)AccountingPriceListState.Accepted).ToString());

            var rows = accountingPriceListService.GetFilteredList(state, deriveFilter, user);

            state.Filter = filter;
            model.State = state;

            foreach (var item in rows)
            {
                var indicators = accountingPriceListMainIndicatorService.GetMainIndicators(item, user);

                GridCell cellReason = null;
                if (item.Reason == AccountingPriceListReason.ReceiptWaybill && user.HasPermission(Permission.ReceiptWaybill_List_Details))
                {
                    cellReason = new GridLinkCell("Reason") { Value = item.ReasonDescription };
                }
                else
                {
                    cellReason = new GridLabelCell("Reason") { Value = item.ReasonDescription };
                }

                var storages = storageService.FilterByUser(item.Storages, user, Permission.AccountingPriceList_List_Details)
                    .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).Select(x => x.Name);

                var storagesString = storages.Any() ? String.Join(", ", storages) : "---";

                model.AddRow(new GridRow(
                    new GridLinkCell("Number") { Value = item.Number.PadLeftZeroes(8) },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = item.EndDate != null ? ((DateTime)item.EndDate).ToShortDateString() : "---" },
                    new GridLabelCell("Storages") { Value = storagesString },
                    cellReason,
                    new GridLabelCell("PurchaseCostSum") { Value = indicators.PurchaseCostSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("OldAccountingPriceSum") { Value = indicators.OldAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("NewAccountingPriceSum") { Value = indicators.NewAccountingPriceSum.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() },
                    new GridHiddenCell("ReceiptWaybillId")
                    {
                        // Почему для грида новых реестров код тут другой???
                        Value = item.ReasonReceiptWaybillId.HasValue ?
                            item.ReasonReceiptWaybillId.ToString() : ""
                    }
                ));

            }


            return model;
        }

        #endregion

        #region Детали реестра цен

        public AccountingPriceListDetailsViewModel Details(Guid accountingPriceListId, UserInfo currentUser, string message = "", string backURL = "")
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);
                accountingPriceListService.CheckPossibilityToViewDetails(accountingPriceList, user);

                var model = new AccountingPriceListDetailsViewModel();

                model.Message = message;
                model.BackURL = backURL;
                model.MainDetails.Id = accountingPriceListId;

                var indicators = accountingPriceListMainIndicatorService.GetMainIndicators(accountingPriceList, user, true);

                model.MainDetails.PurchaseCostSum = indicators.PurchaseCostSum.ForDisplay(ValueDisplayType.Money);
                model.MainDetails.OldAccountingPriceSum = indicators.OldAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                model.MainDetails.NewAccountingPriceSum = indicators.NewAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
                model.MainDetails.AccountingPriceDifPercent = indicators.AccountingPriceChangePercent.ForDisplay(ValueDisplayType.Percent);
                model.MainDetails.AccountingPriceDifSum = indicators.AccountingPriceChangeSum.ForDisplay(ValueDisplayType.Money);
                model.MainDetails.PurchaseMarkupPercent = indicators.PurchaseMarkupPercent.ForDisplay(ValueDisplayType.Percent);
                model.MainDetails.PurchaseMarkupSum = indicators.PurchaseMarkupSum.ForDisplay(ValueDisplayType.Money);

                model.MainDetails.Number = accountingPriceList.Number;
                model.MainDetails.Name = accountingPriceList.Name;
                model.MainDetails.StartDate = accountingPriceList.StartDate == null ? "---" : accountingPriceList.StartDate.ToFullDateTimeString(); // ToShortDateString();
                model.MainDetails.EndDate = accountingPriceList.EndDate.HasValue ? accountingPriceList.EndDate.Value.ToFullDateTimeString() : "---";
                model.MainDetails.RowCount = accountingPriceList.ArticleAccountingPriceCount.ToString();

                model.MainDetails.Reason = accountingPriceList.Reason.ValueToString();
                model.MainDetails.ReasonDescription = accountingPriceList.ReasonDescription;

                model.MainDetails.CuratorId = accountingPriceList.Curator.Id.ToString();
                model.MainDetails.CuratorName = accountingPriceList.Curator.DisplayName;
                model.MainDetails.AllowToViewCuratorDetails = userService.IsPossibilityToViewDetails(accountingPriceList.Curator, user);

                if (accountingPriceList.Reason == AccountingPriceListReason.ReceiptWaybill)
                {
                    try //вот такая проверка, есть ли у юзера права на просмотр деталей этого прихода (IsPossibilityToViewDetails использовать не можем, так как он принимает ReceiptWaybill, а у нас пока только его id)
                    {
                        receiptWaybillService.CheckWaybillExistence(accountingPriceList.ReasonReceiptWaybillId.Value, user);
                        model.MainDetails.AllowToViewReceiptWaybillDetails = true; //эксепшн не вывалился - есть права
                        model.MainDetails.ReasonReceiptWaybillId = accountingPriceList.ReasonReceiptWaybillId.ToString();
                    }
                    catch
                    {
                        model.MainDetails.AllowToViewReceiptWaybillDetails = false; //эксепшн вывалился - прав нет
                        model.MainDetails.ReasonDescription = "---";
                    }
                }
                else
                {
                    model.MainDetails.AllowToViewReceiptWaybillDetails = false;
                }

                model.MainDetails.State = accountingPriceList.State.ValueToString();
                model.MainDetails.StateDescription = accountingPriceList.State.GetDisplayName();

                // создание гридов
                model.ArticleGrid = GetAccountingPriceArticlesGridLocal(new GridState() { Parameters = "AccountingPriceId=" + accountingPriceListId.ToString() }, user);
                model.StorageGrid = GetAccountingPriceStoragesGridLocal(new GridState() { Parameters = "AccountingPriceId=" + accountingPriceListId.ToString() }, user);

                model.AllowToEdit = accountingPriceListService.IsPossibilityToEdit(accountingPriceList, user);
                model.AllowToDelete = accountingPriceListService.IsPossibilityToDelete(accountingPriceList, user);
                model.AllowToAccept = accountingPriceListService.IsPossibilityToAccept(accountingPriceList, user, false);
                model.AllowToCancelAcceptance = accountingPriceListService.IsPossibilityToCancelAcceptation(accountingPriceList, user, false);
                model.AllowToPrintForms = accountingPriceListService.IsPossibilityToPrintForms(accountingPriceList, user);

                return model;
            }
        }

        public GridData GetAccountingPriceArticlesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAccountingPriceArticlesGridLocal(state, user);
            }
        }

        private GridData GetAccountingPriceArticlesGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var accountingPriceListId = ValidationUtils.TryGetGuid(deriveParams["AccountingPriceId"].Value as string);
            var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

            var articlePrices = accountingPriceList.ArticlePrices.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate).ToList();

            GridData model = new GridData();

            model.State = state;
            model.State.TotalRow = articlePrices.Count;

            // Добавляем столбцы
            model.AddColumn("Action", "Действие", Unit.Pixel(80));
            model.AddColumn("ArticleId", "Код", Unit.Pixel(52), align: GridColumnAlign.Right);
            model.AddColumn("Number", "Артикул", Unit.Pixel(105));
            model.AddColumn("Article", "Товар", Unit.Percentage(100));
            model.AddColumn("Price", "Назначенная учетная цена", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "Идентификатор", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = accountingPriceListService.IsPossibilityToAddRow(accountingPriceList, user);

            foreach (var articlePrice in GridUtils.GetEntityRange(articlePrices, state))
            {
                GridActionCell actionCell = new GridActionCell("Action");

                if (accountingPriceListService.IsPossibilityToEditRow(accountingPriceList, user) || accountingPriceListService.IsPossibilityToEditPrice(accountingPriceList, user))
                {
                    actionCell.AddAction("Ред.", "edit_link");
                }
                else
                {
                    actionCell.AddAction("Дет.", "details_link");
                }

                if (accountingPriceListService.IsPossibilityToEditRow(accountingPriceList, user))
                {
                    actionCell.AddAction("Удал.", "delete_link");
                }

                GridCell action = actionCell;

                GridCell articleId = new GridHiddenCell("ArticleId") { Value = articlePrice.Article.Id.ForDisplay() };
                GridCell number = new GridLabelCell("Number") { Value = articlePrice.Article.Number };
                GridCell article = new GridLabelCell("Article") { Value = articlePrice.Article.FullName };
                GridCell price = new GridLabelCell("Price") { Value = articlePrice.AccountingPrice.ForDisplay(ValueDisplayType.Money) };
                GridCell id = new GridHiddenCell("Id") { Value = articlePrice.Id.ToString(), Key = "articlePriceId" };

                GridRowStyle rowStyle;

                if (articlePrice.ErrorAccountingPriceCalculation || articlePrice.ErrorLastDigitCalculation) //если одно из правил применить не удалось
                {
                    rowStyle = GridRowStyle.Warning; //подсвечиваем эту строку в гриде цветом "Предупреждение"
                }
                else
                {
                    rowStyle = GridRowStyle.Normal;
                }

                model.AddRow(new GridRow(action, articleId, number, article, price, id) { Style = rowStyle });
            }

            return model;
        }


        public GridData GetAccountingPriceStoragesGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetAccountingPriceStoragesGridLocal(state, user);
            }
        }

        private GridData GetAccountingPriceStoragesGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var accountingPriceListId = ValidationUtils.TryGetGuid(deriveParams["AccountingPriceId"].Value as string);
            var accountingPrice = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

            var storages = storageService.FilterByUser(accountingPrice.Storages, user, Permission.AccountingPriceList_List_Details).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name);

            GridData model = new GridData();

            model.State = state;

            if (accountingPriceListService.IsPossibilityToRemoveStorage(accountingPrice, user))
            {
                model.AddColumn("Action", "Действие", Unit.Pixel(90));
            }

            model.AddColumn("Name", "Название места хранения", Unit.Percentage(100));
            model.AddColumn("Id", "Идентификатор", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAdd"] = accountingPriceListService.IsPossibilityToAddStorage(accountingPrice, user);

            foreach (var storage in GridUtils.GetEntityRange(storages, state))
            {
                GridCell action = null;

                if (accountingPriceListService.IsPossibilityToRemoveStorage(accountingPrice, user))
                {
                    try
                    {
                        accountingPriceListService.CheckPossibilityToRemoveStorage(accountingPrice, user);
                        user.CheckStorageAvailability(storage, Permission.AccountingPriceList_Storage_Remove);
                        action = new GridActionCell("Action", new GridActionCell.Action("Удал. из списка", "delFromList_link"));
                    }
                    catch
                    {
                        action = new GridHiddenCell("Action");
                    }
                }

                GridCell name = new GridLabelCell("Name") { Value = storage.Name };
                GridCell id = new GridHiddenCell("Id") { Value = storage.Id.ToString(), Key = "storageId" };

                if (action != null)
                {
                    model.AddRow(new GridRow(action, name, id));
                }
                else
                {
                    model.AddRow(new GridRow(name, id));
                }
            }

            return model;
        }

        #endregion

        #region Изменения товаров в реестре цен

        public ArticleAccountingPriceEditViewModel AddArticle(Guid accountingPriceListId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);

                var priceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

                var model = new ArticleAccountingPriceEditViewModel()
                {
                    Title = "Добавление товара в реестр цен",
                    AccountingPriceListId = priceList.Id,
                    ArticleId = 0,
                    ArticleIdForDisplay = "",
                    ArticleName = "Выберите товар",
                    ArticleNumber = "",
                    AccountingPrice = "0",
                    AllowToEdit = true,
                    CalculatedAccountingPrice = "---",
                    AllowToEditPrice = user.HasPermission(Permission.AccountingPriceList_DefaultAccountingPrice_Edit)
                };

                return model;
            }
        }

        public ArticleAccountingPriceEditViewModel EditArticle(Guid accountingPriceListId, Guid articleAccountingPriceId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

                var articleAccountingPrice = accountingPriceList.ArticlePrices.Where(x => x.Id == articleAccountingPriceId).FirstOrDefault();
                ValidationUtils.NotNull(articleAccountingPrice, "Товар в реестре не найден. Возможно, он был удален.");

                var model = GetArticleAccountingPriceEditViewModel(accountingPriceList, articleAccountingPrice.Article, user, articleAccountingPrice.Id);

                return model;
            }
        }

        public object SaveArticle(ArticleAccountingPriceEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNullOrDefault(model.AccountingPriceListId, "Неверное значение входного параметра.");
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(model.AccountingPriceListId, user);

                decimal newAccountingPrice = ValidationUtils.TryGetDecimal(model.AccountingPrice);
                if (newAccountingPrice < 0M)
                {
                    throw new Exception("Учетная цена товара должно быть больше или равна нулю.");
                }

                var newArticle = articleService.CheckArticleExistence(model.ArticleId);
                
                // добавляем
                if (model.Id == Guid.Empty)
                {
                    accountingPriceListService.CheckPossibilityToAddRow(accountingPriceList, user);

                    if (model.CalculatedAccountingPrice.Replace(" ", "") != model.AccountingPrice)
                    {
                        accountingPriceListService.CheckPossibilityToEditPrice(accountingPriceList, user);
                    }

                    ArticleAccountingPrice price = new ArticleAccountingPrice(
                        newArticle,
                        newAccountingPrice);

                    accountingPriceList.AddArticleAccountingPrice(price);
                }
                // редактируем
                else
                {
                    ArticleAccountingPrice oldPrice = accountingPriceList.ArticlePrices.FirstOrDefault(x => x.Id == model.Id);
                    ValidationUtils.NotNull(oldPrice, "Редактируемый товар отсутствует в реестре цен. Возможно, он был удален.");    // Эта ошибка может появиться, если другой пользователь успел удалить товар

                    if (oldPrice.Article != newArticle)
                    {
                        accountingPriceListService.CheckPossibilityToEditRow(accountingPriceList, user);
                    }

                    if ((oldPrice.Article == newArticle && oldPrice.AccountingPrice != ValidationUtils.TryGetDecimal(model.AccountingPrice)) || 
                        (oldPrice.Article != newArticle && model.CalculatedAccountingPrice.Replace(" ", "") != model.AccountingPrice))
                    {
                        accountingPriceListService.CheckPossibilityToEditPrice(accountingPriceList, user);
                    }

                    oldPrice.Article = newArticle;
                    oldPrice.AccountingPrice = newAccountingPrice;

                    oldPrice.ErrorAccountingPriceCalculation = oldPrice.ErrorLastDigitCalculation = false;
                }

                accountingPriceListService.Save(accountingPriceList);

                uow.Commit();

                return GetMainChangeableIndicators(accountingPriceList, user); 
            }
        }

        public object DeleteArticle(Guid accountingPriceListId, Guid articleAccountingPriceId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

                accountingPriceListService.CheckPossibilityToDeleteRow(accountingPriceList, user);

                var articleAccountingPrice = accountingPriceList.ArticlePrices.Where(x => x.Id == articleAccountingPriceId).FirstOrDefault();
                ValidationUtils.NotNull(articleAccountingPrice, "Строка реестра цен не найдена. Возможно, она была удалена.");

                accountingPriceListService.DeleteArticleAccountingPrice(accountingPriceList, articleAccountingPrice);

                uow.Commit();

                return GetMainChangeableIndicators(accountingPriceList, user);
            }
        }

        public ArticleAccountingPriceSetAddViewModel AddArticleAccountingPriceSet(Guid accountingPriceListId, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountingPriceList_ArticleAccountingPrice_Create_Edit);

                var priceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);

                var model = new ArticleAccountingPriceSetAddViewModel()
                {
                    Id = priceList.Id.ToString(),
                    AccountingPriceListId = priceList.Id,
                    AccountingPriceListName = priceList.Name,
                    BackURL = backURL,
                    Title = "Добавление набора товаров в реестр цен",
                    ArticleGroups = articleGroupService.GetList().Where(x => x.Parent != null)
                        .OrderBy(x => x.FullName).ToDictionary(x => x.Id.ToString(), x => x.FullName),
                    Storages = storageService.GetList(user, Permission.Storage_List_Details)
                        .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                };

                return model;
            }
        }

        public object SaveArticleAccountingPriceSet(ArticleAccountingPriceSetAddViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                var user = userService.CheckUserExistence(currentUser.Id);

                ValidationUtils.NotNullOrDefault(model.AccountingPriceListId, "Неверное значение входного параметра.");

                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(model.AccountingPriceListId, user);

                IEnumerable<short> articleGroupIDs;
                IEnumerable<Article> articles;
                IEnumerable<int> articleIdList;
                Dictionary<short, Storage> storages;
                IEnumerable<short> storageIDs = null;

                ValidationUtils.Assert(!String.IsNullOrEmpty(model.ArticleGroupsIDs) || model.AllArticleGroups == "1", "Не выбрано ни одной группы товаров.");

                ValidationUtils.Assert(model.OnlyAvailability != "1" || !(String.IsNullOrEmpty(model.StorageIDs) && model.AllStorages != "1"), "Не выбрано ни одного места хранения.");

                if (model.AllArticleGroups == "1")
                {
                    articleGroupIDs = articleGroupService.GetList().Select(x => x.Id);
                }
                else
                {
                    articleGroupIDs = model.ArticleGroupsIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                }

                if (model.OnlyAvailability == "1")
                {
                    if (model.AllStorages == "1")
                    {
                        storages = storageService.GetList(user, Permission.Storage_List_Details).ToDictionary(x => x.Id, x => x);
                        storageIDs = storages.Select(x => x.Key);
                    }
                    else
                    {
                        storageIDs = model.StorageIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));
                        storages = storageService.CheckStorageListExistence(storageIDs, user, Permission.Storage_List_Details);
                    }
                }

                foreach (var group in articleGroupIDs)
                {
                    if (model.OnlyAvailability == "1")
                    {
                        var exactAvailabilityList = articleAvailabilityService.GetExactArticleAvailability(storageIDs, new List<short> { group }, currentDateTime);
                        var availabilityList = exactAvailabilityList.Select(x => new ArticleAvailabilityInfo(x.BatchId, x.ArticleId, x.PurchaseCost, x.StorageId, x.AccountOrganizationId, x.Count, 0, 0, 0));
                        articleIdList = availabilityList.Select(x => x.ArticleId).Distinct();
                        articles = articleService.GetList(articleIdList);
                        ValidationUtils.Assert(articleIdList.Count() == articles.Count(), "Один из товаров не найден. Возможно, он был удален.");
                    }
                    else
                    {
                        articles = articleService.GetListByArticleGroup(group);
                    }

                    Dictionary<int,bool> accPriceCalc, lastDigitError;
                    var calculatedAccountingPrices = accountingPriceListService.CalculateDefaultAccountingPriceByRule(accountingPriceList, articles, out accPriceCalc, out lastDigitError, user);

                    foreach (var article in articles)
                    {
                        var calculatedAccountingPrice = calculatedAccountingPrices[article.Id];
                        if (calculatedAccountingPrice >= 0M)
                        {
                            var price = new ArticleAccountingPrice(article, calculatedAccountingPrice)
                                {
                                    ErrorAccountingPriceCalculation = accPriceCalc[article.Id],
                                    ErrorLastDigitCalculation = lastDigitError[article.Id]
                                };

                            if (!accountingPriceList.IsAlreadyAddedArticle(price))
                            {
                                accountingPriceList.AddArticleAccountingPrice(price);
                            }
                        }
                    }
                }

                accountingPriceListService.Save(accountingPriceList);
                uow.Commit();

                return GetMainChangeableIndicators(accountingPriceList, user); 
            }
        }

        #endregion

        #region Редактирование и создание нового реестра цен

        public AccountingPriceListEditViewModel Create(string additionalId, AccountingPriceListReason reasonCode, UserInfo currentUser, string backURL = "")
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.AccountingPriceList_Create);

                if (!Enum.IsDefined(typeof(AccountingPriceListReason), reasonCode))
                {
                    throw new Exception("Неизвестное основание создания реестра.");
                }
                AccountingPriceListReason reason = reasonCode;

                var model = new AccountingPriceListEditViewModel();

                model.Title = "Добавление реестра цен";
                model.BackURL = backURL;
                model.AdditionalId = additionalId;
                model.AccountingPriceListId = Guid.Empty;

                model.AllowToEditStorages = true;
                model.AllowToSetByPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                if (reason == AccountingPriceListReason.ReceiptWaybill)
                {
                    ValidationUtils.NotNull(additionalId, "При создании реестра из накладной передан несуществующий идентификатор накладной.");

                    Guid guid = ValidationUtils.TryGetGuid(additionalId);
                    ReceiptWaybill receiptWaybill = receiptWaybillService.CheckWaybillExistence(guid, user);

                    model.Reason = String.Format(reason.GetDisplayName(), receiptWaybill.Number, receiptWaybill.Date.ToShortDateString());
                }
                else
                {
                    model.Reason = reason.GetDisplayName();
                }

                if (reason == AccountingPriceListReason.Storage)
                {
                    model.AllowToEditStorages = false;
                }

                model.ReasonId = ((int)reason).ToString();
                model.Number = GetNextNumber();
                model.StartDate = DateTime.Today.ToShortDateString();
                model.StartTime = DateTime.Now.ToFullTimeString();

                model.AllowToEdit = true;
                model.AllowToSetByPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                FillConstantsForAccountingPriceListEdit(model, user);

                return model;
            }
        }

        public string GetNextNumber()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return accountingPriceListService.GetNextNumber();
            }
        }

        private void FillConstantsForAccountingPriceListEdit(AccountingPriceListEditViewModel model, User user)
        {
            model.NumberIsUnique = 1;

            model.AccountingPriceCalcRuleType_caption1 = AccountingPriceCalcRuleType.ByPurchaseCost.GetDisplayName();
            model.AccountingPriceCalcRuleType_caption2 = AccountingPriceCalcRuleType.ByCurrentAccountingPrice.GetDisplayName();


            model.PurchaseCostDeterminationRuleType_caption1 = PurchaseCostDeterminationRuleType.ByAveragePurchasePrice.GetDisplayName();
            model.PurchaseCostDeterminationRuleType_caption2 = PurchaseCostDeterminationRuleType.ByMinimalPurchaseCost.GetDisplayName();
            model.PurchaseCostDeterminationRuleType_caption3 = PurchaseCostDeterminationRuleType.ByMaximalPurchaseCost.GetDisplayName();
            model.PurchaseCostDeterminationRuleType_caption4 = PurchaseCostDeterminationRuleType.ByLastPurchaseCost.GetDisplayName();


            model.MarkupPercentDeterminationRuleType_caption1 = MarkupPercentDeterminationRuleType.ByArticle.GetDisplayName();
            model.MarkupPercentDeterminationRuleType_caption2 = MarkupPercentDeterminationRuleType.ByArticleGroup.GetDisplayName();
            model.MarkupPercentDeterminationRuleType_caption3 = MarkupPercentDeterminationRuleType.Custom.GetDisplayName();


            model.AccountingPriceDeterminationRuleType_caption1 = AccountingPriceDeterminationRuleType.ByAverageAccountingPrice.GetDisplayName();
            model.AccountingPriceDeterminationRuleType_caption2 = AccountingPriceDeterminationRuleType.ByMinimalAccountingPrice.GetDisplayName();
            model.AccountingPriceDeterminationRuleType_caption3 = AccountingPriceDeterminationRuleType.ByMaximalAccountingPrice.GetDisplayName();
            model.AccountingPriceDeterminationRuleType_caption4 = AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage.GetDisplayName();


            model.MarkupValueRuleType_caption1 = "% наценки";
            model.MarkupValueRuleType_caption2 = "% скидки";


            model.LastDigitCalcRuleType_caption1 = LastDigitCalcRuleType.LeaveAsIs.GetDisplayName();
            model.LastDigitCalcRuleType_caption2 = LastDigitCalcRuleType.RoundDecimalsAndLeaveLastDigit.GetDisplayName();
            model.LastDigitCalcRuleType_caption3 = LastDigitCalcRuleType.LeaveLastDigitFromStorage.GetDisplayName();
            model.LastDigitCalcRuleType_caption4 = LastDigitCalcRuleType.SetCustom.GetDisplayName();
            model.LastDigitCalcRuleType_caption5 = "и копейки:";

            model.StorageTypeList = ComboBoxBuilder.GetComboBoxItemList<AccountingPriceListStorageTypeGroup>();

            model.StorageList = storageService.GetList(user, Permission.AccountingPriceList_List_Details).Where(x => user.HasPermissionToViewStorageAccountingPrices(x)).OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name)
                        .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), sort: false);

            model.Storages = storageService.GetList(user, Permission.AccountingPriceList_Storage_Add)
                .OrderBy(x => x.Type.ValueToString()).ThenBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name);
        }

        public AccountingPriceListEditViewModel Edit(Guid accountingPriceListId, UserInfo currentUser, string backURL = "")
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                AccountingPriceListEditViewModel model = new AccountingPriceListEditViewModel();
                model.Title = "Редактирование реестра цен";
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accountingPriceListId, user);
                if (accountingPriceList == null)
                {
                    throw new Exception("Реестр цен не найден. Возможно, он был удален.");
                }

                model.AccountingPriceCalcRuleType = (short)accountingPriceList.AccountingPriceCalcRule.Type;

                switch (accountingPriceList.AccountingPriceCalcRule.Type)
                {
                    case (AccountingPriceCalcRuleType.ByPurchaseCost):
                        model.AccountingPriceCalcRuleType = (short)AccountingPriceCalcRuleType.ByPurchaseCost;
                        model.PurchaseCostDeterminationRuleType = (short)accountingPriceList.AccountingPriceCalcRule.CalcByPurchaseCost.PurchaseCostDeterminationRuleType;
                        model.MarkupPercentDeterminationRuleType = (short)accountingPriceList.AccountingPriceCalcRule.CalcByPurchaseCost.MarkupPercentDeterminationRule.Type;
                        var markupPercentValue = accountingPriceList.AccountingPriceCalcRule.CalcByPurchaseCost.MarkupPercentDeterminationRule.MarkupPercentValue;
                        if (markupPercentValue.HasValue)
                        {
                            model.CustomMarkupValue = markupPercentValue.Value.ForEdit();
                        }
                        break;
                    case (AccountingPriceCalcRuleType.ByCurrentAccountingPrice):
                        model.AccountingPriceCalcRuleType = (short)AccountingPriceCalcRuleType.ByCurrentAccountingPrice;
                        model.AccountingPriceDeterminationRuleType = (short)accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type;

                        switch (accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Type)
                        {
                            case (AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage):
                                model.StorageId = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Storage.Id.ToString();
                                model.StorageName = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.Storage.Name;
                                break;
                            case (AccountingPriceDeterminationRuleType.ByAverageAccountingPrice):
                                model.StorageTypeId1 = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageType.ValueToString();
                                break;
                            case (AccountingPriceDeterminationRuleType.ByMinimalAccountingPrice):
                                model.StorageTypeId2 = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageType.ValueToString();
                                break;
                            case (AccountingPriceDeterminationRuleType.ByMaximalAccountingPrice):
                                model.StorageTypeId3 = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.AccountingPriceDeterminationRule.StorageType.ValueToString();
                                break;
                        }

                        model.MarkupValueRuleType = (short)(accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.MarkupPercentValue >= 0 ? 1 : 2);
                        var markupValue = accountingPriceList.AccountingPriceCalcRule.CalcByCurrentAccountingPrice.MarkupPercentValue;
                        if (markupValue >= 0)
                        {
                            model.MarkupValuePercent = markupValue.ForEdit();
                        }
                        else
                        {
                            model.DiscountValuePercent = Math.Abs(markupValue).ForEdit();
                        }
                        break;
                }

                model.LastDigitCalcRuleType = (short)accountingPriceList.LastDigitCalcRule.Type;
                if (model.LastDigitCalcRuleType == (short)(LastDigitCalcRuleType.LeaveLastDigitFromStorage))
                {
                    model.StorageTypeId4 = (short)(accountingPriceList.LastDigitCalcRule.Storage.Id);
                }
                model.LastDigitCalcRuleNumber = accountingPriceList.LastDigitCalcRule.LastDigit;
                model.LastDigitCalcRulePenny = accountingPriceList.LastDigitCalcRule.Decimals;

                model.BackURL = backURL;
                model.AccountingPriceListId = (Guid)accountingPriceListId;
                model.Number = accountingPriceList.Number;
                model.StartDate = accountingPriceList.StartDate.ToShortDateString();
                model.StartTime = accountingPriceList.StartDate.ToFullTimeString();
                model.EndDate = accountingPriceList.EndDate.HasValue ? accountingPriceList.EndDate.Value.ToShortDateString() : "";
                model.EndTime = accountingPriceList.EndDate.HasValue ? accountingPriceList.EndDate.Value.ToFullTimeString() : "";
                model.ReasonId = accountingPriceList.Reason.ValueToString();
                model.Reason = accountingPriceList.ReasonDescription;

                model.Number = accountingPriceList.Number;
                model.StartDate = accountingPriceList.StartDate.ToShortDateString();
                model.Name = model.Name;

                model.AllowToEdit = user.HasPermission(Permission.AccountingPriceList_Edit);
                model.AllowToEditStorages = false;
                model.AllowToSetByPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

                FillConstantsForAccountingPriceListEdit(model, user);

                return model;
            }
        }

        public bool IsNumberUnique(string number)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return IsNumberUniqueLocal(number);
            }
        }

        private bool IsNumberUniqueLocal(string number)
        {
            return accountingPriceListService.IsNumberUnique(number);
        }

        public KeyValuePair<object, string> Save(AccountingPriceListEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var currentDateTime = DateTimeUtils.GetCurrentDateTime();

                DateTime startDate;
                DateTime dateForParse;
                DateTime? endDate = null;
                string startDateTime, endDateTime;

                startDateTime = model.StartDate + " " + model.StartTime;
                ValidationUtils.Assert(!String.IsNullOrEmpty(model.StartTime), "Введите время начала действия реестра цен.");
                if (!DateTime.TryParse(startDateTime, out startDate))
                {
                    throw new Exception("Введите начальную дату в правильном формате или выберите из списка.");
                }

                if (!String.IsNullOrEmpty(model.EndDate))
                {
                    ValidationUtils.Assert(!String.IsNullOrEmpty(model.EndTime), "Введите время завершения реестра цен.");
                    endDateTime = model.EndDate + " " + model.EndTime;
                    if (!DateTime.TryParse(endDateTime, out dateForParse))
                    {
                        throw new Exception("Введите конечную дату в правильном формате или выберите из списка.");
                    }
                    endDate = dateForParse;
                }

                AccountingPriceList accountingPriceList = null;

                bool startDateCorrected = false;

                if (model.AccountingPriceListId == Guid.Empty)
                // создание
                {
                    user.CheckPermission(Permission.AccountingPriceList_Create);
                    if (!IsNumberUniqueLocal(model.Number))
                    {
                        throw new Exception("Реестр цен с таким номером уже существует. Введите другой.");
                    }

                    // Реально даты здесь не меняем (они изменятся в конструкторе), но запоминаем, что их следует изменить
                    startDateCorrected = AccountingPriceList.CheckAndCorrectDates(ref startDate, ref endDate, currentDateTime);

                    switch ((AccountingPriceListReason)Convert.ToInt32(model.ReasonId))
                    {
                        case AccountingPriceListReason.ReceiptWaybill:
                            accountingPriceList = CreateByReceiptWaybill(model, user);
                            break;
                        case AccountingPriceListReason.Revaluation:
                            accountingPriceList = CreateByRevaluation(model, user);
                            break;
                        case AccountingPriceListReason.Storage:
                            accountingPriceList = CreateByStorage(model, user);
                            break;
                        default:
                            throw new Exception("Основание для создания реестра цен не указано.");
                    }
                }
                else
                // редактирование
                {
                    accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(model.AccountingPriceListId, user);

                    accountingPriceListService.CheckPossibilityToEdit(accountingPriceList, user);

                    if (accountingPriceList.Number != model.Number && !IsNumberUniqueLocal(model.Number))
                    {
                        throw new Exception(String.Format("Номер реестра цен {0} уже используется. Укажите другой номер.", model.Number));
                    }

                    accountingPriceList.Number = model.Number;
                    accountingPriceList.StartDate = startDate;
                    accountingPriceList.EndDate = endDate;

                    accountingPriceList.AccountingPriceCalcRule = GetAccountingPriceCalcRuleFromViewModel(model, user);
                    accountingPriceList.LastDigitCalcRule = GetLastDigitCalcRuleFromViewModel(model);

                    startDateCorrected = accountingPriceList.CheckAndCorrectDates(currentDateTime);
                }                

                accountingPriceListService.Save(accountingPriceList);

                uow.Commit();

                return new KeyValuePair<object, string>(
                    new { Id = accountingPriceList.Id.ToString() }, startDateCorrected ? "Дата начала действия реестра цен была автоматически изменена на текущую." : "");
            }
        }

        #region Создание реестра по различным основаниям

        private AccountingPriceList CreateByRevaluation(AccountingPriceListEditViewModel model, User user)
        {
            List<Storage> storages = new List<Storage>();

            if (model.StorageIDs != null)
            {
                foreach (var item in model.StorageIDs.Split('_'))
                {
                    storages.Add(storageService.CheckStorageExistence(ValidationUtils.TryGetShort(item), user, Permission.AccountingPriceList_Storage_Add));
                }
            }

            var accountingPriceList = new AccountingPriceList(
                model.Number,
                DateTime.Parse(model.StartDate + " " + model.StartTime),
                String.IsNullOrEmpty(model.EndDate) ? (DateTime?)null : DateTime.Parse(model.EndDate + " " + model.EndTime),
                storages, user, GetAccountingPriceCalcRuleFromViewModel(model, user), GetLastDigitCalcRuleFromViewModel(model));

            return accountingPriceList;
        }

        private AccountingPriceList CreateByReceiptWaybill(AccountingPriceListEditViewModel model, User user)
        {
            var waybill = receiptWaybillService.CheckWaybillExistence(ValidationUtils.TryGetGuid(model.AdditionalId), user);
            var articleList = SortRows(waybill.Rows).Select(x => x.Article);

            AccountingPriceCalcRule emptyAccountingPriceCalcRule = GetAccountingPriceCalcRuleFromViewModel(model, user);
            LastDigitCalcRule emptyLastDigitCalcRule = GetLastDigitCalcRuleFromViewModel(model);

            List<ArticleAccountingPrice> rowList = new List<ArticleAccountingPrice>();

            var articleIdList = articleList.Select(x => x.Id);

            var readyAccountingPriceCalcRule = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(emptyAccountingPriceCalcRule,articleIdList, user);
            var readyLastDigitCalcRule = accountingPriceCalcRuleService.GetReadyLastDigitCalcRule(emptyLastDigitCalcRule, articleIdList, user);

            accountingPriceCalcRuleService.InitializeDefaultRules(readyAccountingPriceCalcRule, readyLastDigitCalcRule, articleIdList, user);                

            var counter = 0;
            foreach (var article in articleList)
            {
                bool priceError = false, digitError = false;
                var calculatedAccountingPrice = accountingPriceCalcService.CalculateAccountingPrice(readyAccountingPriceCalcRule, readyLastDigitCalcRule, article, out priceError, out digitError);

                rowList.Add(new ArticleAccountingPrice(article, calculatedAccountingPrice, ++counter) { ErrorAccountingPriceCalculation = priceError, ErrorLastDigitCalculation = digitError });
            }

            List<Storage> storages = new List<Storage>();           

            if (model.StorageIDs != null)
            {
                var storageIds = model.StorageIDs.Split('_').Select(x => ValidationUtils.TryGetShort(x));

                storages = storageService.CheckStorageListExistence(storageIds, user, Permission.AccountingPriceList_Storage_Add).Select(x => x.Value).ToList();
            }

            var accountingPriceList = new AccountingPriceList(
                model.Number,
                DateTime.Parse(model.StartDate + " " + model.StartTime),
                String.IsNullOrEmpty(model.EndDate) ? (DateTime?)null : DateTime.Parse(model.EndDate + " " + model.EndTime),
                waybill,
                storages,
                rowList,
                user,
                emptyAccountingPriceCalcRule, emptyLastDigitCalcRule);

            return accountingPriceList;
        }

        private AccountingPriceList CreateByStorage(AccountingPriceListEditViewModel model, User user)
        {            
            var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(model.AdditionalId), user, Permission.AccountingPriceList_Storage_Add);

            var resultList = new List<ArticleAccountingPrice>();

            var accountingPriceCalcRule = GetAccountingPriceCalcRuleFromViewModel(model, user);
            var lastDigitCalcRule = GetLastDigitCalcRuleFromViewModel(model);

            var availableStorages = storageService.GetList(user, Permission.Storage_List_Details);

            var articleIdList = accountingPriceListService.GetArticlesListWithNoAccountingPrice(storage, availableStorages);//получаем список товаров, для которых нет учетной цены для этого места хранения

            var articleList = articleService.GetList(articleIdList);

            if (articleList.Any())
            {
                var readyAccountingPriceCalcRule = accountingPriceCalcRuleService.GetReadyAccountingPriceCalcRule(accountingPriceCalcRule, articleIdList, user);
                var readyLastDigitCalcRule = accountingPriceCalcRuleService.GetReadyLastDigitCalcRule(lastDigitCalcRule, articleIdList, user);

                accountingPriceCalcRuleService.InitializeDefaultRules(accountingPriceCalcRule, lastDigitCalcRule, articleIdList, user);             

                foreach (var article in articleList)
                {
                    bool priceError = false, digitError = false;
                    
                    var accountingPrice = accountingPriceCalcService.CalculateAccountingPrice(accountingPriceCalcRule, lastDigitCalcRule, article, out priceError, out digitError);
                    resultList.Add(new ArticleAccountingPrice(article, accountingPrice) { ErrorAccountingPriceCalculation = priceError, ErrorLastDigitCalculation = digitError });
                }
            }

            var accountingPriceList = new AccountingPriceList(
                model.Number,
                DateTime.Parse(model.StartDate + " " + model.StartTime),
                String.IsNullOrEmpty(model.EndDate) ? (DateTime?)null : DateTime.Parse(model.EndDate + " " + model.EndTime),
                storage,
                resultList,
                user,
                accountingPriceCalcRule, lastDigitCalcRule);

            return accountingPriceList;
        }

        #endregion

        #region Правила для расчета учетной цены и последней цифры

        private AccountingPriceCalcRule GetAccountingPriceCalcRuleFromViewModel(AccountingPriceListEditViewModel model, User user)
        {
            AccountingPriceCalcRule rule;
            AccountingPriceCalcRuleType accountingPriceCalcRuleType = (AccountingPriceCalcRuleType)model.AccountingPriceCalcRuleType;
            switch (accountingPriceCalcRuleType)
            {
                case (AccountingPriceCalcRuleType.ByCurrentAccountingPrice):
                    AccountingPriceDeterminationRuleType accountingPriceDeterminationRuleType = (AccountingPriceDeterminationRuleType)model.AccountingPriceDeterminationRuleType;
                    AccountingPriceDeterminationRule accountingPriceDeterminationRule;

                    var storageList = storageService.GetList(user, Permission.Storage_List_Details);

                    switch (accountingPriceDeterminationRuleType)
                    {
                        case (AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage):
                            var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(model.StorageId), user);
                            accountingPriceDeterminationRule = new AccountingPriceDeterminationRule(storage);
                            break;
                        case (AccountingPriceDeterminationRuleType.ByAverageAccountingPrice):
                            accountingPriceDeterminationRule =
                                new AccountingPriceDeterminationRule(accountingPriceDeterminationRuleType, ValidationUtils.TryGetEnum<AccountingPriceListStorageTypeGroup>(model.StorageTypeId1), storageList);
                            break;
                        case (AccountingPriceDeterminationRuleType.ByMinimalAccountingPrice):
                            accountingPriceDeterminationRule =
                                new AccountingPriceDeterminationRule(accountingPriceDeterminationRuleType, ValidationUtils.TryGetEnum<AccountingPriceListStorageTypeGroup>(model.StorageTypeId2), storageList);
                            break;
                        case (AccountingPriceDeterminationRuleType.ByMaximalAccountingPrice):
                            accountingPriceDeterminationRule =
                                new AccountingPriceDeterminationRule(accountingPriceDeterminationRuleType, ValidationUtils.TryGetEnum<AccountingPriceListStorageTypeGroup>(model.StorageTypeId3), storageList);
                            break;
                        default:
                            throw new Exception("Неизвестный тип склада.");
                    }

                    decimal markupPercentValue;
                    switch (model.MarkupValueRuleType)
                    {
                        case 1:
                            markupPercentValue = Convert.ToDecimal(model.MarkupValuePercent.Replace('.', ','));
                            break;
                        case 2:
                            markupPercentValue = -Convert.ToDecimal(model.DiscountValuePercent.Replace('.', ','));
                            break;
                        default:
                            throw new Exception("Неизвестное правило назначения процента наценки/скидки.");
                    }

                    AccountingPriceCalcByCurrentAccountingPrice accountingPriceCalcByCurrentAccountingPrice = new AccountingPriceCalcByCurrentAccountingPrice(accountingPriceDeterminationRule, markupPercentValue);
                    rule = new AccountingPriceCalcRule(accountingPriceCalcByCurrentAccountingPrice);
                    break;
                case (AccountingPriceCalcRuleType.ByPurchaseCost):
                    MarkupPercentDeterminationRuleType markupPercentDeterminationRuleType = (MarkupPercentDeterminationRuleType)model.MarkupPercentDeterminationRuleType;
                    MarkupPercentDeterminationRule markupPercentDeterminationRule;
                    switch (markupPercentDeterminationRuleType)
                    {
                        case (MarkupPercentDeterminationRuleType.Custom):
                            markupPercentDeterminationRule = new MarkupPercentDeterminationRule(Convert.ToDecimal(model.CustomMarkupValue.Replace('.', ',')));
                            break;
                        default:
                            markupPercentDeterminationRule = new MarkupPercentDeterminationRule(markupPercentDeterminationRuleType);
                            break;
                    }
                    PurchaseCostDeterminationRuleType purchaseCostDeterminationRuleType = (PurchaseCostDeterminationRuleType)model.PurchaseCostDeterminationRuleType;
                    AccountingPriceCalcByPurchaseCost accountingPriceCalcByPurchaseCost = new AccountingPriceCalcByPurchaseCost(purchaseCostDeterminationRuleType, markupPercentDeterminationRule);
                    rule = new AccountingPriceCalcRule(accountingPriceCalcByPurchaseCost);
                    break;
                default:
                    throw new Exception("Неизвестный тип правила учетной цены.");
            }

            return rule;
        }

        private LastDigitCalcRule GetLastDigitCalcRuleFromViewModel(AccountingPriceListEditViewModel model)
        {
            LastDigitCalcRule rule;

            var type = (LastDigitCalcRuleType)(model.LastDigitCalcRuleType);

            switch (type)
            {
                case LastDigitCalcRuleType.LeaveAsIs:
                    rule = new LastDigitCalcRule(type);
                    break;
                case LastDigitCalcRuleType.SetCustom:

                    rule = new LastDigitCalcRule(model.LastDigitCalcRuleNumber, model.LastDigitCalcRulePenny);
                    break;
                case LastDigitCalcRuleType.RoundDecimalsAndLeaveLastDigit:
                    rule = new LastDigitCalcRule(type);
                    break;
                case LastDigitCalcRuleType.LeaveLastDigitFromStorage:
                    var storage = storageService.GetById(model.StorageTypeId4);
                    rule = new LastDigitCalcRule(storage);
                    break;
                default:
                    throw new Exception("Неизвестный тип правила расчета последней цифры.");

            }

            return rule;
        }

        #endregion

        /// <summary>
        /// Сортировка строк накладной. В начале выводит строки, добавленные при приемке.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        // Надо перенести в общий сервис оба метода SortRows
        private IEnumerable<ReceiptWaybillRow> SortRows(IEnumerable<ReceiptWaybillRow> list)
        {
            var result1 = new List<ReceiptWaybillRow>();
            var result2 = new List<ReceiptWaybillRow>();

            foreach (var val in list)
            {
                if (val.PendingCount == 0)
                    result1.Add(val);
                else
                    result2.Add(val);
            }

            var result = new List<ReceiptWaybillRow>();
            result.AddRange(result1.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate));
            result.AddRange(result2.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate));

            return result;
        }

        public void Delete(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(id, user);

                accountingPriceListService.CheckPossibilityToDelete(accountingPriceList, user);
                accountingPriceListService.Delete(accountingPriceList, user);

                uow.Commit();
            }
        }

        #endregion

        #region Проводка и отмена проводки

        public string Accept(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(id, user);

                bool startDateCorrected = accountingPriceList.CheckAndCorrectDates(currentDateTime);

                accountingPriceListService.Accept(accountingPriceList, currentDateTime, user);

                uow.Commit();

                return startDateCorrected ? "Дата начала действия реестра цен была автоматически изменена на текущую." : "";
            }
        }

        public void CancelAcceptance(Guid id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(id, user);

                accountingPriceListService.CancelAcceptance(accountingPriceList, currentDateTime, user);

                uow.Commit();
            }
        }

        #endregion

        #region Добавление места хранения

        public AccountingPriceListAddStorageViewModel StoragesList(Guid priceListId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new AccountingPriceListAddStorageViewModel();

                model.StorageList = GetStoragesListForAdding(priceListId, user);
                model.AccountingPriceListId = priceListId;
                model.AllowToSave = true;

                return model;
            }
        }

        /// <summary>
        /// Добавление МХ в распространение, возвращает данные для обновления шапки
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object StoragesList(AccountingPriceListAddStorageViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accPriceList = accountingPriceListService.CheckAccountingPriceListExistence(model.AccountingPriceListId, user);

                if (model.StorageId != null)
                {
                    var storage = storageService.GetById(model.StorageId.Value);
                    var storageList = storageService.GetList();  //этот список нужен для расчета распространения, а распространение рассчитываем исходя из ВСЕХ мест хранений в системе, поэтому юзера не передаем
                    accountingPriceListService.AddStorage(accPriceList, storage, storageList, user);
                }                

                uow.Commit();

                var result = GetMainChangeableIndicators(accPriceList, user);

                return result;
            }
        }

        /// <summary>
        /// Динамическое обновление списка МХ для добавления, вызывается после того как одно МХ добавили, а форма не закрывается и можно добавить еще
        /// </summary>
        /// <param name="priceListId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetListOfStorages(Guid priceListId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var result = GetStoragesListForAdding(priceListId, user);

                return new { List = result.ToList() };
            }
        }

        /// <summary>
        /// Возвращает список мест хранения для заполнения комбобокса в форме "добавление мх в распространение"      
        /// </summary>
        /// <returns>В списке только те места хранения, на которые у пользователя есть права, за исключением ранее добавленных</returns>
        private IEnumerable<SelectListItem> GetStoragesListForAdding(Guid priceListId, User user)
        {
            var accPriceList = accountingPriceListService.CheckAccountingPriceListExistence(priceListId, user);

            accountingPriceListService.CheckPossibilityToAddStorage(accPriceList, user);

            List<Storage> storagesList = storageService.GetList(user, Permission.AccountingPriceList_Storage_Add).OrderBy(z => z.Type.ValueToString()).ThenBy(z => z.Name).ToList();

            foreach (Storage storage in accPriceList.Storages)
            {
                storagesList.Remove(storage);
            }

            return storagesList.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false);
        }

        public object StoragesAddAll(Guid priceListId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accPriceList = accountingPriceListService.CheckAccountingPriceListExistence(priceListId, user);

                accountingPriceListService.CheckPossibilityToAddStorage(accPriceList, user);

                if (accPriceList != null)
                {
                    var storageList = storageService.GetList(user, Permission.AccountingPriceList_Storage_Add)
                        .Concat(accPriceList.Storages).Distinct().ToList();
                    accPriceList.SetStorageList(storageList);
                }                

                accountingPriceListService.Save(accPriceList);

                uow.Commit();

                var result = GetMainChangeableIndicators(accPriceList, user);

                return result;
            }
        }

        public object StoragesAddTradePoint(Guid priceListId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var accPriceList = accountingPriceListService.CheckAccountingPriceListExistence(priceListId, user);

                accountingPriceListService.CheckPossibilityToAddStorage(accPriceList, user);

                var storageList = storageService.GetList(user, Permission.AccountingPriceList_Storage_Add);

                if (accPriceList != null)
                {
                    var tradePointList = storageService.GetStoragesByType(StorageType.TradePoint, user, Permission.AccountingPriceList_Storage_Add).ToList();

                    foreach (var storage in tradePointList)
                    {
                        if (!accPriceList.Storages.Contains(storage))
                        {
                            accountingPriceListService.AddStorage(accPriceList, storage, storageList, user);
                        }
                    }
                }

                accountingPriceListService.Save(accPriceList);

                uow.Commit();

                var result = GetMainChangeableIndicators(accPriceList, user);

                return result;
            }
        }

        #endregion

        #region Удаление места хранения

        public object DeleteStorage(Guid accPriceListId, short storageId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accPriceList = accountingPriceListService.CheckAccountingPriceListExistence(accPriceListId, user);

                var storage = storageService.CheckStorageExistence(storageId, user, Permission.AccountingPriceList_Storage_Remove, "Недостаточно прав для удаления места хранения из списка распространения.");

                var storageList = storageService.GetList(); //этот список нужен для расчета распространения, а распространение рассчитываем исходя из ВСЕХ мест хранений в системе, поэтому юзера не передаем

                accountingPriceListService.DeleteStorage(accPriceList, storage, storageList, user);

                uow.Commit();

                return GetMainChangeableIndicators(accPriceList, user);
            }
        }

        #endregion

        #region Печатные формы

        /// <summary>
        /// Получение модели печатной формы реестра цен
        /// </summary>
        /// <param name="settings">Параметры печатной формы</param>
        /// <returns>Модель печатной формы</returns>
        public AccountingPriceListPrintingFormViewModel GetAccountingPriceListPrintingForm(Guid id, bool detailedMode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var accountingPriceList = accountingPriceListService.CheckAccountingPriceListExistence(id, user);

                accountingPriceListService.CheckPossibilityToPrintForms(accountingPriceList, user);

                DynamicDictionary<short, DynamicDictionary<int, decimal?>> oldAccountingPrices = null;

                var indicators = accountingPriceListMainIndicatorService.GetMainIndicators(accountingPriceList, user);

                var model = new AccountingPriceListPrintingFormViewModel(detailedMode)
                {
                    Comment = "",
                    Date = DateTime.Now.ToShortDateString(),
                    EndDate = accountingPriceList.EndDate.HasValue ? accountingPriceList.EndDate.Value.ToShortDateString() : "---",
                    OrganizationName = "", // TODO: добавить организацию в AccountingPriceList
                    ReasonDescription = accountingPriceList.ReasonDescription,
                    StartDate = accountingPriceList.StartDate.ToShortDateString(),
                    Title = String.Format("Переоценка {0}", accountingPriceList.Name),
                    AccountingPriceDifSum = indicators.AccountingPriceChangeSum.ForDisplay(ValueDisplayType.Money),
                    NewAccountingPriceSum = indicators.NewAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    OldAccountingPriceSum = indicators.OldAccountingPriceSum.ForDisplay(ValueDisplayType.Money),
                    PurchaseCostSum = indicators.PurchaseCostSum.ForDisplay(ValueDisplayType.Money),
                    PurchaseMarkupSum = indicators.PurchaseMarkupSum.ForDisplay(ValueDisplayType.Money),
                };

                // Инициализация набора товаров
                foreach (var listArticle in accountingPriceList.ArticlePrices.OrderBy(x => x.OrdinalNumber).ThenBy(x => x.CreationDate))
                {
                    model.Articles.Add(new AccountingPriceListPrintingFormArticleViewModel
                    {
                        AccountingPrice = listArticle.AccountingPrice.ForDisplay(ValueDisplayType.Money),
                        AccountingPriceValue = listArticle.AccountingPrice,
                        ArticleName = listArticle.Article.FullName,
                        Id = listArticle.Article.Id.ForDisplay(),
                        Number = listArticle.Article.Number,
                        ArticleId = listArticle.Article.Id
                    });
                }
                if (model.DetailedMode)
                {
                    oldAccountingPrices = articlePriceService.GetAccountingPrice(accountingPriceList, (DateTime?)accountingPriceList.StartDate);
                }

                foreach (var storage in accountingPriceList.Storages)
                {
                    var accountingPriceListPrintingFormStorage = new AccountingPriceListPrintingFormStorageViewModel(storage.Name);

                    if (model.DetailedMode)
                    {
                        foreach (var article in model.Articles)
                        {
                            var oldAccountingPrice = oldAccountingPrices[storage.Id][article.ArticleId];
                            var accountingPriceListPrintingFormItem = new AccountingPriceListPrintingFormItemViewModel(article);
                            accountingPriceListPrintingFormItem.OldAccountingPrice = oldAccountingPrice.ForDisplay(ValueDisplayType.Money);
                            accountingPriceListPrintingFormItem.DifferenceInAccountingPrice = oldAccountingPrice.HasValue ?
                                (article.AccountingPriceValue - oldAccountingPrice.Value).ForDisplay(ValueDisplayType.Money) : "---";
                            accountingPriceListPrintingFormStorage.AccountingPriceListItem.Add(accountingPriceListPrintingFormItem);
                        }
                    }
                    model.Storages.Add(accountingPriceListPrintingFormStorage);
                }

                return model;
            }
        }

        #endregion

        #endregion
    }
}