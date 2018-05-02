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
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Article;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ArticlePresenter : IArticlePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IArticleService articleService;
        private readonly ICountryService countryService;
        private readonly IManufacturerService manufacturerService;
        private readonly ITrademarkService trademarkService;
        private readonly IStorageService storageService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly IMeasureUnitService measureUnitService;
        private readonly IArticleGroupService articleGroupService;
        private readonly IArticleCertificateService articleCertificateService;

        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IArticlePriceService articlePriceService;
        private readonly IArticleSaleService articleSaleService;
        private readonly IReturnFromClientService returnFromClientService;

        private readonly IDealService dealService;
        private readonly ITeamService teamService;
        private readonly IUserService userService;

        #endregion

        #region Конструктор

        public ArticlePresenter(IUnitOfWorkFactory unitOfWorkFactory, IArticleService articleService, ICountryService countryService, IManufacturerService manufacturerService, ITrademarkService trademarkService,
            IStorageService storageService, IAccountOrganizationService accountOrganizationService, IMeasureUnitService measureUnitService, IArticleGroupService articleGroupService,
            IArticleAvailabilityService articleAvailabilityService, IArticlePriceService articlePriceService, IDealService dealService, IExpenditureWaybillService expenditureWaybillService,
            IArticleCertificateService articleCertificateService, IArticleSaleService articleSaleService, IReturnFromClientService returnFromClientService, IUserService userService, ITeamService teamService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.articleService = articleService;
            this.dealService = dealService;
            this.countryService = countryService;
            this.manufacturerService = manufacturerService;
            this.trademarkService = trademarkService;
            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.measureUnitService = measureUnitService;
            this.articleGroupService = articleGroupService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.articleCertificateService = articleCertificateService;
            this.userService = userService;
            this.returnFromClientService = returnFromClientService;
            this.teamService = teamService;

            this.articleAvailabilityService = articleAvailabilityService;
            this.articlePriceService = articlePriceService;
            this.articleSaleService = articleSaleService;
        }

        #endregion

        #region Методы

        #region Список

        public ArticleListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Article_List_Details);

                var model = new ArticleListViewModel()
                {
                    ActualArticleGrid = GetArticleGridLocal(new GridState() { Sort = "Id=Asc" }, user, false),
                    ObsoleteArticleGrid = GetArticleGridLocal(new GridState() { Sort = "Id=Asc" }, user, true)
                };

                var productionCountryList = countryService.GetList()
                    .GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                var manufacturerList = manufacturerService.GetList()
                    .GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                var trademarkList = trademarkService.GetList()
                    .GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                model.FilterData.Items.Add(new FilterTextEditor("Number", "Артикул"));
                model.FilterData.Items.Add(new FilterHyperlink("ArticleGroup", "Группа товара", "Выберите группу"));
                model.FilterData.Items.Add(new FilterTextEditor("FullName", "Наименование"));
                model.FilterData.Items.Add(new FilterComboBox("Manufacturer", "Фабрика-изготовитель", manufacturerList));
                model.FilterData.Items.Add(new FilterTextEditor("Id", "Код товара"));
                model.FilterData.Items.Add(new FilterComboBox("Trademark", "Торговая марка", trademarkList));

                return model;
            }
        }

        public GridData GetActualArticleGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleGridLocal(state, user, false);
            }
        }

        public GridData GetObsoleteArticleGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleGridLocal(state, user, true);
            }
        }

        private GridData GetArticleGridLocal(GridState state, User user, bool isObsolete)
        {
            if (state == null)
                state = new GridState() { Sort = "Id=Asc" };

            bool allowToCreate = user.HasPermission(Permission.Article_Create);
            bool allowToEdit = user.HasPermission(Permission.Article_Edit);
            bool allowToDelete = user.HasPermission(Permission.Article_Delete);

            GridData model = new GridData() { State = state };
            model.AddColumn("Action", "Действие", Unit.Pixel(120));
            model.AddColumn("Id", "Код", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("Number", "Артикул", Unit.Pixel(80));
            model.AddColumn("FullName", "Наименование", Unit.Percentage(100));

            ParameterString deriveFilter = new ParameterString(state.Filter);

            if (isObsolete)
            {
                deriveFilter.Add("IsObsolete", ParameterStringItem.OperationType.Eq, "true");
            }
            else
            {
                deriveFilter.Add("IsObsolete", ParameterStringItem.OperationType.Eq, "false");
                model.ButtonPermissions["AllowToCreate"] = allowToCreate;
            }

            var rows = articleService.GetFilteredList(state, deriveFilter);

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction(allowToEdit ? "Ред." : "Дет.", "edit_link");
            if (allowToDelete)
            {
                actions.AddAction("Удал.", "delete_link");
            }
            if (allowToCreate && allowToEdit)
            {
                actions.AddAction("Копир.", "copy_link");
            }

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Id") { Value = item.Id.ToString() },
                    new GridLabelCell("Number") { Value = item.Number },
                    new GridLabelCell("FullName") { Value = item.FullName }
                ));
            }

            return model;
        }

        #endregion

        #region Создание, редактирование, копирование, удаление

        public ArticleEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Article_Create);

                var model = new ArticleEditViewModel();
                model.Title = "Добавление товара";
                model.IsCurrentArticleGroupLevelCorrect = 1;
                model.ArticleGroupName = "Выбрать группу товара";
                model.IsSalaryPercentFromGroup = true.ForDisplay();
                model.SalaryPercentFromGroup = 0.ForDisplay();
                model.IsObsolete = false.ForDisplay();
                model.ManufacturerName = "Выбрать фабрику-изготовителя";
                model.TrademarkName = "Выбрать торговую марку";
                model.MeasureUnitName = "Выбрать единицу измерения";
                model.CertificateName = "Выбрать сертификат товара";
                model.TrademarkId = 0;
                model.ProductionCountryList = countryService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

                model.AllowToEdit = true;
                model.AllowToClearCertificate = false;
                model.AllowToAddCountry = user.HasPermission(Permission.Country_Create);

                return model;
            }
        }

        public ArticleEditViewModel Edit(int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var article = articleService.CheckArticleExistence(id);

                return GetArticleViewModel(article, user);
            }
        }

        public ArticleEditViewModel Copy(int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Article_Create);
                user.CheckPermission(Permission.Article_Edit);

                var originalArticle = articleService.CheckArticleExistence(id);

                var model = GetArticleViewModel(originalArticle, user);
                model.Id = 0;

                return model;
            }
        }

        public object Save(ArticleEditViewModel model, UserInfo currentUser)
        {            
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var articleGroup = articleGroupService.CheckArticleGroupExistence(model.ArticleGroupId);
                var measureUnit = measureUnitService.CheckMeasureUnitExistence(model.MeasureUnitId);
                bool isSalaryPercentFromGroup = ValidationUtils.TryGetBool(model.IsSalaryPercentFromGroup);

                Article article;

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Article_Create);

                    article = new Article(model.FullArticleName, articleGroup, measureUnit, isSalaryPercentFromGroup);
                }
                else
                {
                    user.CheckPermission(Permission.Article_Edit);

                    article = articleService.CheckArticleExistence(model.Id);

                    article.FullName = model.FullArticleName;
                    article.ArticleGroup = articleGroup;
                    article.MeasureUnit = measureUnit;
                    article.IsSalaryPercentFromGroup = isSalaryPercentFromGroup;
                }

                article.ShortName = model.ShortName;
                article.Number = model.Number;
                article.ManufacturerNumber = model.ManufacturerNumber;
                article.Comment = StringUtils.ToHtml(model.Comment);
                article.MarkupPercent = ValidationUtils.TryGetDecimal(model.MarkupPercent);
                article.PackSize = ValidationUtils.TryGetDecimal(model.PackSize);
                article.PackVolume = ValidationUtils.TryGetDecimal(model.PackVolume);

                article.PackLength = ValidationUtils.TryGetInt(model.PackLength, "Указанная длина упаковки товара не является целым числом.", true);
                article.PackHeight = ValidationUtils.TryGetInt(model.PackHeight, "Указанная высота упаковки товара не является целым числом.", true);
                article.PackWidth = ValidationUtils.TryGetInt(model.PackWidth, "Указанная ширина упаковки товара не является целым числом.", true);

                article.ProductionCountry = (model.ProductionCountryId > 0 ? countryService.CheckExistence((short)model.ProductionCountryId) : null);
                article.Trademark = (model.TrademarkId > 0 ? trademarkService.CheckExistence((short)model.TrademarkId) : null);
                article.Manufacturer = (model.ManufacturerId > 0 ? manufacturerService.CheckExistence((short)model.ManufacturerId) : null);
                article.SalaryPercent = isSalaryPercentFromGroup ? articleGroup.SalaryPercent : ValidationUtils.TryGetDecimal(model.SalaryPercent);
                article.PackWeight = ValidationUtils.TryGetDecimal(model.PackWeight);
                article.Certificate = (model.CertificateId > 0 ? articleCertificateService.CheckArticleCertificateExistence(model.CertificateId) : null);
                article.IsObsolete = ValidationUtils.TryGetBool(model.IsObsolete);

                articleService.Save(article);

                uow.Commit();

                return new { IsObsolete = article.IsObsolete };
            }            
        }

        public void Delete(int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var article = articleService.CheckArticleExistence(id);
                var user = userService.CheckUserExistence(currentUser.Id);

                articleService.Delete(article, user);

                uow.Commit();
            }
        }

        #endregion

        #region Выбор товара

        public GridData GetArticleSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleSelectGridLocal(state, user);
            }
        }

        private GridData GetArticleSelectGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(58));
            model.AddColumn("Id", "Код", Unit.Pixel(68), align: GridColumnAlign.Right);
            model.AddColumn("Id2", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("Number", "Артикул", Unit.Pixel(145));
            model.AddColumn("FullName", "Наименование", Unit.Percentage(100));
            model.AddColumn("MeasureUnitShortName", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("MeasureUnitScale", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            IEnumerable<Article> rows;
            if (deriveParams["storageId"] != null)
            {
                var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(deriveParams["storageId"].Value as string), user);
                var organization = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(deriveParams["senderId"].Value as string));
                
                rows = articleService.GetExtendedAvailableArticles(storage, organization, DateTime.Now, state);
            }
            // выбора товара для возврата
            else if (deriveParams["dealId"] != null && deriveParams["teamId"] != null)
            {
                var deal = dealService.CheckDealExistence(ValidationUtils.TryGetShort(deriveParams["dealId"].Value.ToString()), user, Permission.ReturnFromClientWaybill_Create_Edit);
                var team = teamService.CheckTeamExistence(ValidationUtils.TryGetShort(deriveParams["teamId"].Value.ToString()));
                rows = returnFromClientService.GetAvailableToReturnArticleFilteredList(state, deal, team);
            }
            else
            {
                rows = articleService.GetFilteredList(state);
            }
            
            model.State = state;

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "article_select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Id") { Value = item.Id.ForDisplay() },
                    new GridHiddenCell("Id2") { Value = item.Id.ToString(), Key = "articleId" }, // Поменять с большой буквы
                    new GridLabelCell("Number") { Value = item.Number, Key = "articleNumber" },
                    new GridLabelCell("FullName") { Value = item.FullName, Key = "articleFullName" },
                    new GridHiddenCell("MeasureUnitShortName") { Value = item.MeasureUnit.ShortName },
                    new GridHiddenCell("MeasureUnitScale") { Value = item.MeasureUnit.Scale.ToString() }
                ));
            }

            return model;
        }

        private ArticleSelectViewModel SelectArticle(GridState state, User user)
        {
            var model = new ArticleSelectViewModel();

            ParameterString deriveParams = new ParameterString(state.Parameters);

            model.FilterData.Items.Add(new FilterTextEditor("Number", "Артикул"));
            model.FilterData.Items.Add(new FilterHyperlink("ArticleGroup", "Группа товара", "Выберите группу"));
            model.FilterData.Items.Add(new FilterTextEditor("FullName", "Наименование"));

            if (deriveParams["storageId"] == null)  //Если выборка товаров по расширенному наличию, то пропускаем изготовителя ...
            {
                var manufacturerList = manufacturerService.GetList()
                .GetComboBoxItemList(x => x.Name, x => x.Id.ToString());
                
                model.FilterData.Items.Add(new FilterComboBox("Manufacturer", "Фабрика-изготовитель", manufacturerList));
            }

            model.FilterData.Items.Add(new FilterTextEditor("Id", "Код товара"));

            if (deriveParams["storageId"] == null)  // ... и торговую марку
            {
                var trademarkList = trademarkService.GetList()
                .GetComboBoxItemList(x => x.Name, x => x.Id.ToString());
                
                model.FilterData.Items.Add(new FilterComboBox("Trademark", "Торговая марка", trademarkList));
            }

            //ХАК: если эти поля (storageId и senderId) в State не указаны, то значит форма была вызвана для прихода или возврата, 
            //а в этих случаях источники выбрать невозможно
            //Таким образом, разрешаем выбрать источники только если указаны они оба
            model.AllowToSelectSources = (deriveParams["storageId"] != null && deriveParams["senderId"] != null);
           
            model.Data = GetArticleSelectGridLocal(state, user);

            return model;
        }

        public ArticleSelectViewModel SelectArticle(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return SelectArticle(new GridState() { PageSize = 5 }, user);
            }
        }

        public ArticleSelectViewModel SelectArticleFromStorage(short storageId, int senderId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return SelectArticle(new GridState() { PageSize = 5, Parameters = "storageId=" + storageId + ";senderId=" + senderId }, user);
            }
        }

        /// <summary>
        /// Получить список товаров для возврата
        /// </summary>
        public ArticleSelectViewModel SelectArticleToReturn(int dealId, int teamId, int recipientId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return SelectArticle(new GridState() { PageSize = 5, Parameters = "dealId=" + dealId + ";teamId=" + teamId + ";recipientId=" + recipientId }, user);
            }
        }

        #endregion

        #region Выбор партии товара

        public GridData GetArticleBatchSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleBatchSelectGridLocal(state, user);
            }
        }

        private GridData GetArticleBatchSelectGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(50));
            model.AddColumn("BatchName", "Партия", Unit.Pixel(130));
            model.AddColumn("PurchaseCost", "Закупочная цена", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("AvailableInStorageCount", "Кол-во на складе", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("PendingCount", "Кол-во в ожидании", Unit.Pixel(80), align: GridColumnAlign.Right);            
            model.AddColumn("ReservedCount", "Кол-во в отгрузках", Unit.Pixel(80), align: GridColumnAlign.Right);
            model.AddColumn("AvailableToReserveCount", "Доступно для товародвижения", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("ReceiptWaybillRowId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("MeasureUnitScale", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("AvailableToReserveFromStorageCount", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("AvailableToReserveFromPendingCount", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            ValidationUtils.NotNull(deriveParams["ArticleId"], "Не указан товар.");
            ValidationUtils.NotNull(deriveParams["StorageId"], "Не указано место хранения отправителя.");
            ValidationUtils.NotNull(deriveParams["SenderId"], "Не указана организация отправителя.");

            int articleId = ValidationUtils.TryGetInt(deriveParams["ArticleId"].Value.ToString());
            short recipientStorageId = ValidationUtils.TryGetShort(deriveParams["StorageId"].Value.ToString());

            var senderId = ValidationUtils.TryGetInt(deriveParams["SenderId"].Value.ToString());
            var sender = accountOrganizationService.CheckAccountOrganizationExistence(senderId);

            Guid? articleBatchToExcludeId = null;

            if (deriveParams["ArticleBatchToExcludeId"] != null)
            {
                articleBatchToExcludeId = new Guid(deriveParams["ArticleBatchToExcludeId"].Value as string);
            }

            var article = articleService.CheckArticleExistence(articleId);
            var senderStorage = storageService.CheckStorageExistence(recipientStorageId, user, "Место хранения отправителя не найдено.");

            var rows = articleAvailabilityService.GetExtendedArticleBatchAvailability(article, senderStorage, sender, DateTime.Now)
                .Where(x => x.ArticleBatchId != articleBatchToExcludeId && x.AvailableToReserveCount > 0);

            model.State = state;

            bool allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            GridActionCell actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "articleBatch_select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("BatchName") { Value = item.BatchName.ToString(), Key = "batchName" },
                    new GridLabelCell("PurchaseCost") { Value = (allowToViewPurchaseCosts ? item.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "---"), Key = "purchaseCost" },
                    new GridLabelCell("AvailableInStorageCount") { Value = item.AvailableInStorageCount.ForDisplay() },
                    new GridLabelCell("PendingCount") { Value = item.PendingCount.ForDisplay() },                    
                    new GridLabelCell("ReservedCount") { Value = item.ReservedCount.ForDisplay() },
                    new GridLabelCell("AvailableToReserveCount") { Value = item.AvailableToReserveCount.ForDisplay() },
                    new GridLabelCell("ReceiptWaybillRowId") { Value = item.ArticleBatchId.ToString() },
                    new GridHiddenCell("MeasureUnitScale") { Value = item.ArticleMeasureUnitScale.ToString() },
                    new GridHiddenCell("AvailableToReserveFromStorageCount") { Value = item.AvailableToReserveFromStorageCount.ForDisplay() },
                    new GridHiddenCell("AvailableToReserveFromPendingCount") { Value = item.AvailableToReserveFromPendingCount.ForDisplay() }
                ));
            }

            return model;
        }

        public ArticleBatchSelectViewModel SelectArticleBatch(int articleId, short senderStorageId, short recipientStorageId, int senderId, UserInfo currentUser, string date = "", Guid? articleBatchToExcludeId = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var _date = (date == "" ? DateTime.Now : DateTime.Parse(date));

                var article = articleService.CheckArticleExistence(articleId);

                var senderStorage = storageService.GetById(senderStorageId);

                if (senderStorageId != 0)
                {
                    ValidationUtils.NotNull(senderStorage, "Место хранения отправителя не найдено.");
                }

                var recipientStorage = storageService.GetById(recipientStorageId);

                if (recipientStorageId != 0)
                {
                    ValidationUtils.NotNull(recipientStorage, "Место хранения получателя не найдено.");
                }

                bool allowToViewSenderAccPrices = senderStorageId != 0;
                bool allowToViewRecipientAccPrices = recipientStorageId != 0; 
                bool allowToViewBothAccPrices = allowToViewRecipientAccPrices && allowToViewSenderAccPrices;

                /* ---------------------------------------- */
                DynamicDictionary<int, decimal?> result;
                if (senderStorageId != recipientStorageId && recipientStorageId != 0)
                {
                    allowToViewSenderAccPrices = allowToViewSenderAccPrices && user.HasPermissionToViewStorageAccountingPrices(senderStorage);
                    allowToViewRecipientAccPrices = allowToViewRecipientAccPrices && user.HasPermissionToViewStorageAccountingPrices(recipientStorage);
                    allowToViewBothAccPrices = allowToViewRecipientAccPrices && allowToViewSenderAccPrices;

                    result = articlePriceService.GetAccountingPrice(new List<short>() { senderStorageId, recipientStorageId }, article.Id);
                }
                else
                {
                    result = articlePriceService.GetAccountingPrice(new List<short>() { senderStorageId }, article.Id);
                }
                /* ---------------------------------------- */

                if (senderStorageId != 0)
                {
                    ValidationUtils.NotNull(result[senderStorage.Id], "Не установлена учетная цена отправителя на данный товар.");
                }

                if (recipientStorageId != 0)
                {
                    ValidationUtils.NotNull(result[recipientStorage.Id], "Не установлена учетная цена получателя на данный товар.");
                }

                var model = new ArticleBatchSelectViewModel();
                model.ArticleId = article.Id.ToString();
                model.ArticleName = article.FullName;
                model.ArticleNumber = article.Number;
                model.AvailableToMoveTotalCount = "0";

                if (allowToViewSenderAccPrices && result[senderStorage.Id].Value == 0)
                {
                    model.MovementMarkupPercent = "0";
                }
                else
                {
                    model.MovementMarkupPercent = (allowToViewBothAccPrices ?
                        (((result[recipientStorage.Id].Value - result[senderStorage.Id].Value) / result[senderStorage.Id].Value) * 100) :
                        (decimal?)null).ForDisplay(ValueDisplayType.Percent);
                }

                model.MovementMarkupSum = (allowToViewBothAccPrices ? (result[recipientStorage.Id].Value - result[senderStorage.Id].Value) : (decimal?)null).ForDisplay(ValueDisplayType.Money);
                model.RecipientAccountingPrice = (allowToViewRecipientAccPrices && result.ContainsKey(recipientStorageId) ? result[recipientStorageId].Value : (decimal?)null).ForDisplay(ValueDisplayType.Money);
                model.RecipientAccountingPriceValue = (allowToViewRecipientAccPrices && result.ContainsKey(recipientStorageId) ? result[recipientStorageId].Value : (decimal?)null).ForEdit();
                model.SenderAccountingPrice = (allowToViewSenderAccPrices && result.ContainsKey(senderStorageId) ? result[senderStorageId].Value : (decimal?)null).ForDisplay(ValueDisplayType.Money);
                model.SenderAccountingPriceValue = (allowToViewSenderAccPrices && result.ContainsKey(senderStorageId) ? result[senderStorageId].Value : (decimal?)null).ForEdit();
                // выбираем партии, пришедшие на место хранения отправителя (SenderStorage)
                model.BatchGrid = GetArticleBatchSelectGridLocal(new GridState()
                    {
                        Parameters = "ArticleId=" + articleId + ";StorageId=" + senderStorageId + ";SenderId=" + senderId.ToString() + ";Date=" + _date.ToString() +
                        (articleBatchToExcludeId != null ? ";ArticleBatchToExcludeId=" + articleBatchToExcludeId.Value.ToString() : ""),
                        Sort = "CreationDate=Asc"
                    }, user);

                return model;
            }
        }

        public ArticleBatchSelectViewModel SelectArticleBatchByStorage(int articleId, short storageId, int senderId, UserInfo currentUser, string date = "", Guid? articleBatchToExcludeId = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var _date = (date == "" ? DateTime.Now : DateTime.Parse(date));

                var article = articleService.CheckArticleExistence(articleId);
                var storage = storageService.CheckStorageExistence(storageId, user);

                decimal? accountingPrice = null;

                if (user.HasPermissionToViewStorageAccountingPrices(storage))
                {
                    accountingPrice = articlePriceService.GetAccountingPrice(article, storage);
                    ValidationUtils.NotNull(accountingPrice, "Не установлена учетная цена места хранения на данный товар.");
                }

                var model = new ArticleBatchSelectViewModel();
                model.ArticleId = article.Id.ToString();
                model.ArticleName = article.FullName;
                model.ArticleNumber = article.Number;
                model.AvailableToMoveTotalCount = "0";

                model.SenderAccountingPrice = accountingPrice.ForDisplay(ValueDisplayType.Money);

                // выбираем партии, пришедшие на место хранения отправителя (SenderStorage)
                model.BatchGrid = GetArticleBatchSelectGridLocal(new GridState()
                {
                    Parameters = "ArticleId=" + articleId + ";StorageId=" + storageId + ";SenderId=" + senderId.ToString() + ";Date=" + _date.ToString() +
                    (articleBatchToExcludeId != null ? ";ArticleBatchToExcludeId=" + articleBatchToExcludeId.Value.ToString() : ""),
                    Sort = "CreationDate=Asc"
                }, user);

                return model;
            }
        }

        #endregion

        #region Выбор партий накладных реализации

        public ArticleSaleSelectViewModel SelectArticleSale(int articleId, int dealId, short teamId, int recipientId, short storageId, UserInfo currentUser, Guid? articleSaleToExcludeId = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var article = articleService.CheckArticleExistence(articleId);
                //var recipient = accountOrganizationService.CheckAccountOrganizationExistence(recipientId);
                var deal = dealService.CheckDealExistence(dealId, user, Permission.ReturnFromClientWaybill_Create_Edit);
                var storage = storageService.GetById(storageId);    // независимо от прав
                var team = teamService.CheckTeamExistence(teamId);

                ValidationUtils.NotNull(storage, "Место хранения не найдено. Возможно, оно было удалено.");

                var accountingPrice = articlePriceService.GetAccountingPrice(article, storage);
                ValidationUtils.NotNull(accountingPrice, "Не установлена учетная цена места хранения на данный товар.");

                var rows = articleSaleService.GetArticleSaleAvailability(article, deal, team).Where(x => x.SaleWaybillRow.Id != articleSaleToExcludeId);
                var availableToReturnTotalCount = rows.Sum(x => x.AvailableToReturnCount);

                var model = new ArticleSaleSelectViewModel()
                {
                    AccountingPrice = (user.HasPermissionToViewStorageAccountingPrices(storage) ? accountingPrice.Value.ForDisplay(ValueDisplayType.Money) : "---"),
                    ArticleId = article.Id.ToString(),
                    ArticleName = article.FullName,
                    ArticleNumber = article.Number,
                    AvailableToReturnTotalCount = availableToReturnTotalCount.ForDisplay()
                };

                // выбираем партии, пришедшие на место хранения отправителя (SenderStorage)
                model.SaleGrid = GetArticleSaleSelectGridLocal(new GridState()
                {
                    Parameters = "ArticleId=" + articleId + ";RecipientId=" + recipientId.ToString() + ";StorageId=" + storageId + ";DealId=" + dealId.ToString() +
                    ";TeamId=" + team.Id + (articleSaleToExcludeId != null ? ";ArticleSaleToExcludeId=" + articleSaleToExcludeId.Value.ToString() : ""),
                    Sort = "CreationDate=Asc"
                }, user);

                return model;
            }
        }

        /// <summary>
        /// Получить грид партий для возврата товара
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetArticleSaleSelectGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleSaleSelectGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получить грид партий для возврата товара
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetArticleSaleSelectGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            ParameterString deriveParams = new ParameterString(state.Parameters);

            ValidationUtils.NotNull(deriveParams["ArticleId"], "Не указан товар.");
            ValidationUtils.NotNull(deriveParams["RecipientId"], "Не указана организация-приемщик.");
            ValidationUtils.NotNull(deriveParams["DealId"], "Не указана сделка.");
            ValidationUtils.NotNull(deriveParams["StorageId"], "Не указано место хранения-приемщик.");

            var storageId = ValidationUtils.TryGetShort(deriveParams["StorageId"].Value.ToString());
            var teamId = ValidationUtils.TryGetShort(deriveParams["TeamId"].Value.ToString());
            var articleId = ValidationUtils.TryGetInt(deriveParams["ArticleId"].Value.ToString());
            var recipientId = ValidationUtils.TryGetInt(deriveParams["RecipientId"].Value.ToString());
            var dealId = ValidationUtils.TryGetInt(deriveParams["DealId"].Value.ToString());

            var article = articleService.CheckArticleExistence(articleId);
            //var recipient = accountOrganizationService.CheckAccountOrganizationExistence(recipientId);
            var deal = dealService.CheckDealExistence(dealId, user, Permission.ReturnFromClientWaybill_Create_Edit);
            var team = teamService.CheckTeamExistence(teamId);
            var storage = storageService.GetById(storageId);    // независимо от прав
            ValidationUtils.NotNull(storage, "Место хранения не найдено. Возможно, оно было удалено.");

            Guid? ArticleSaleToExcludeId = null;
            if (deriveParams["ArticleSaleToExcludeId"] != null)
            {
                ArticleSaleToExcludeId = new Guid(deriveParams["ArticleSaleToExcludeId"].Value as string);
            }

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(50));
            model.AddColumn("SaleWaybillName", "Документ реализации", Unit.Percentage(30));
            model.AddColumn("SalePrice", "Отпускная цена", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("SoldCount", "Кол-во реализовано", Unit.Pixel(100), align: GridColumnAlign.Right);
            model.AddColumn("ReturnedCount", "Возврат по другим накладным", Unit.Pixel(120), align: GridColumnAlign.Right);
            model.AddColumn("AvailableToReturnCount", "Доступно к возврату", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("SaleWaybillRowId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("SaleWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("PurchaseCost", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountingPrice", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("MeasureUnitScale", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            var rows = articleSaleService.GetArticleSaleAvailability(article, deal, team).Where(x => x.SaleWaybillRow.Id != ArticleSaleToExcludeId);

            decimal? accountingPrice = null;

            // если пользователь имеет возможность просмотреть УЦ - получаем ее
            if (user.HasPermissionToViewStorageAccountingPrices(storage))
            {
                accountingPrice = articlePriceService.GetAccountingPrice(article, storage);
            }

            model.State = state;

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "articleSale_select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    expenditureWaybillService.IsPossibilityToViewDetails(item.SaleWaybillRow.SaleWaybill.As<ExpenditureWaybill>(), user) ?
                        (GridCell)new GridLinkCell("SaleWaybillName") { Value = item.SaleWaybillRow.SaleWaybill.Name } :
                        new GridLabelCell("SaleWaybillName") { Value = item.SaleWaybillRow.SaleWaybill.Name },
                    new GridLabelCell("SalePrice") { Value = item.SaleWaybillRow.SalePrice.ForDisplay(ValueDisplayType.Money) },
                    new GridLabelCell("SoldCount") { Value = item.SaleWaybillRow.SellingCount.ForDisplay() },
                    new GridLabelCell("ReturnedCount") { Value = item.ReturnedCount.ForDisplay() },
                    new GridLabelCell("AvailableToReturnCount") { Value = item.AvailableToReturnCount.ForDisplay() },
                    new GridHiddenCell("SaleWaybillRowId") { Value = item.SaleWaybillRow.Id.ToString() },
                    new GridHiddenCell("SaleWaybillId") { Value = item.SaleWaybillRow.SaleWaybill.Id.ToString() },
                    new GridHiddenCell("PurchaseCost")
                    {
                        Value = (user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ?
                            item.SaleWaybillRow.PurchaseCost.ForDisplay(ValueDisplayType.Money) : "---")
                    },
                    new GridHiddenCell("AccountingPrice") { Value = accountingPrice.ForDisplay(ValueDisplayType.Money) },
                    new GridHiddenCell("MeasureUnitScale") { Value = item.SaleWaybillRow.ArticleMeasureUnitScale.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение ViewModel для Article
        /// </summary>
        /// <param name="value">Объект класса Article</param>
        /// <returns>ViewModel для Article</returns>        
        private ArticleEditViewModel GetArticleViewModel(Article value, User user)
        {
            var allowToEdit = user.HasPermission(Permission.Article_Edit);
            var model = new ArticleEditViewModel();

            model.Id = value.Id;
            model.FullArticleName = value.FullName;
            model.ShortName = value.ShortName;
            model.Number = value.Number;
            model.ManufacturerNumber = value.ManufacturerNumber;

            if (value.ArticleGroup != null)
                model.ArticleGroupId = value.ArticleGroup.Id;

            if (value.Trademark != null)
            {
                model.TrademarkId = value.Trademark.Id;
                model.TrademarkName = value.Trademark.Name;
            }
            else
            {
                model.TrademarkId = 0;
                model.TrademarkName = allowToEdit ? "Выбрать торговую марку" : "---";
            }

            if (value.Manufacturer != null)
            {
                model.ManufacturerId = value.Manufacturer.Id;
                model.ManufacturerName = value.Manufacturer.Name;
            }
            else
            {
                model.ManufacturerName = allowToEdit ? "Выбрать фабрику-изготовителя" : "---";
            }
            if (value.ProductionCountry != null)
                model.ProductionCountryId = value.ProductionCountry.Id;

            model.MeasureUnitId = value.MeasureUnit.Id;
            model.MeasureUnitName = value.MeasureUnit.FullName;
            model.MeasureUnitShortName = value.MeasureUnit.ShortName;
            model.MeasureUnitScale = value.MeasureUnit.Scale.ToString();

            if (value.Certificate != null)
            {
                model.CertificateId = value.Certificate.Id;
                model.CertificateName = value.Certificate.Name;
            }
            else
            {
                model.CertificateId = 0;
                model.CertificateName = allowToEdit ? "Выбрать сертификат товара" : "---";
            }

            model.PackSize = value.PackSize.ForEdit();
            model.PackWeight = value.PackWeight.ForEdit();

            model.PackHeight = value.PackHeight > 0 ? value.PackHeight.ToString() : "";
            model.PackLength = value.PackLength > 0 ? value.PackLength.ToString() : "";
            model.PackWidth = value.PackWidth > 0 ? value.PackWidth.ToString() : "";

            model.PackVolume = value.PackVolume.ForEdit();
            model.IsSalaryPercentFromGroup = value.IsSalaryPercentFromGroup.ForDisplay();
            model.SalaryPercentFromGroup = value.ArticleGroup.SalaryPercent.ForEdit();
            model.SalaryPercent = value.IsSalaryPercentFromGroup ? value.ArticleGroup.SalaryPercent.ForEdit() : value.SalaryPercent.ForEdit();
            model.MarkupPercent = value.MarkupPercent.ForEdit();
            model.Comment = value.Comment;
            model.IsCurrentArticleGroupLevelCorrect = 1;
            model.IsObsolete = value.IsObsolete.ForDisplay();

            model.ArticleGroupName = (value.ArticleGroup.Id == 0 ? "Выбрать группу товара" : value.ArticleGroup.Name);
            model.ProductionCountryList = countryService.GetList().GetComboBoxItemList(x => x.Name, x => x.Id.ToString());

            model.Title = (value.Id == 0 ? "Добавление товара" : (user.HasPermission(Permission.Article_Edit) ? "Редактирование товара" : "Детали товара"));

            model.AllowToEdit = allowToEdit;
            model.AllowToClearCertificate = value.Certificate != null;
            model.AllowToAddCountry = user.HasPermission(Permission.Country_Create);

            return model;
        }

        #endregion

        /// <summary>
        /// Получить словарь товаров по идентификатору группы товара
        /// </summary>
        /// <param name="articleGroupId">Идентификатор группы товара</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Dictionary[id,название товара]</returns>
        public Dictionary<string, string> GetArticleFromArticleGroup(short articleGroupId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var articleGroup = articleGroupService.CheckArticleGroupExistence(articleGroupId);

                return articleService.GetListByArticleGroup(articleGroup.Id).ToDictionary(x => x.Id.ToString(), x => x.FullName);
            }
        }
        #endregion
    }
}