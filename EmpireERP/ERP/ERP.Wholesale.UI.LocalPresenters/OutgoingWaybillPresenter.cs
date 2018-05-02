using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Infrastructure.UnitOfWork;

namespace ERP.Wholesale.UI.LocalPresenters
{   
    // В данном презентере не используется UnitOfWork, так как его методы вызываются дригими презентарами в их UoW    
    public abstract class OutgoingWaybillPresenter<T> : BaseWaybillPresenter<T> where T : BaseWaybill 
    {
        #region Поля

        protected readonly IStorageService storageService;
        protected readonly IAccountOrganizationService accountOrganizationService;
        protected readonly IArticleService articleService;
        protected readonly IArticlePriceService articlePriceService;
        protected readonly IArticleAvailabilityService articleAvailabilityService;

        #endregion

        #region Конструкторы

        protected OutgoingWaybillPresenter(IUnitOfWorkFactory unitOfWorkFactory, IBaseWaybillService<T> waybillService, IUserService userService,
            IStorageService storageService, IAccountOrganizationService accountOrganizationService, IArticleService articleService,
            IArticlePriceService articlePriceService, IArticleAvailabilityService articleAvailabilityService)
            : base(unitOfWorkFactory, waybillService, userService)
    {
            this.storageService = storageService;
            this.accountOrganizationService = accountOrganizationService;
            this.articleService = articleService;
            this.articlePriceService = articlePriceService;
            this.articleAvailabilityService = articleAvailabilityService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение модели грида товаров для быстрого добавления позиций в исходящие накладные
        /// </summary>        
        protected GridData GetArticlesForWaybillRowsAdditionByListGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState();

            GridData model = new GridData();

            model.AddColumn("IdForDisplay", "Код", Unit.Pixel(38), align: GridColumnAlign.Right);
            model.AddColumn("Number", "Артикул", Unit.Pixel(110));
            model.AddColumn("FullName", "Наименование", Unit.Percentage(100));
            model.AddColumn("AccountingPrice", "Учетная цена", Unit.Pixel(74), align: GridColumnAlign.Right);
            model.AddColumn("MeasureUnitShortName", "Ед. изм.", Unit.Pixel(25), align: GridColumnAlign.Center);
            model.AddColumn("AvailableToReserveFromPendingCount", "Кол-во в ожидании", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("AvailableToReserveFromStorageCount", "Кол-во на складе", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("AvailableToReserveCount", "Доступно к отгрузке", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("MovingCount", "Отгружаемое кол-во", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("MeasureUnitScale", "", Unit.Pixel(0), style: GridCellStyle.Hidden);
            model.AddColumn("ArticleId", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            ParameterString deriveFilter = new ParameterString(state.Filter);

            var actionName = deriveParams["ActionName"].Value as string;
            model.GridPartialViewAction = actionName;

            var storage = storageService.CheckStorageExistence(ValidationUtils.TryGetShort(deriveParams["StorageId"].Value as string), user);
            var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(ValidationUtils.TryGetInt(deriveParams["AccountOrganizationId"].Value as string));

            // исключаем устаревшие товары
            var ps = new ParameterString("");
            ps.Add("IsObsolete", ParameterStringItem.OperationType.Eq, "false");

            // получаем информацию об уже имеющихся в накладной товарах
            var articleTakingsInfo = new Dictionary<int, decimal>();

            if (deriveParams.ContainsNonEmptyString("ArticleTakingsInfo"))
            {
                articleTakingsInfo = deriveParams["ArticleTakingsInfo"].Value.ToString().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split('_')).ToDictionary(x => ValidationUtils.TryGetInt(x[0]), x => ValidationUtils.TryGetDecimal(x[1]));
            }

            bool onlyAvailable = true; //чтобы при начальной загрузке страницы сразу показывались только товары из наличия (а не только после того как пользователь нажмет "искать" в фильтре)

            var onlyAvailableParam = deriveFilter.ContainsNonEmptyString("OnlyAvailable") ? deriveFilter["OnlyAvailable"] : 
                deriveParams.ContainsNonEmptyString("OnlyAvailable") ? deriveParams["OnlyAvailable"] : null; //если нет этого параметра в фильтре, то пробуем искать в коллекции Parameters
            if (onlyAvailableParam != null)
            {
                onlyAvailable = (onlyAvailableParam.Value as string == "1");
                deriveFilter.Delete("OnlyAvailable");
                state.Filter = deriveFilter.ToString();

                deriveParams.Delete("OnlyAvailable");
                deriveParams.Add("OnlyAvailable", ParameterStringItem.OperationType.Eq, onlyAvailableParam.Value.ToString()); //сохраняем в коллекции Parameters, чтобы значение параметра сохранялось при сбросе фильтра
            }            

            IEnumerable<Article> rows = null;
            if(onlyAvailable)
            {
                rows = articleService.GetExtendedAvailableArticles(storage, accountOrganization, DateTime.Now, state);
            }
            else
            {
                rows = articleService.GetFilteredList(state, ps);
            }
            
            var articleIds = rows.Select(x => x.Id);

            state.Parameters = deriveParams.ToString();
            model.State = state;

            var articlePrices = new DynamicDictionary<int, decimal?>();

            // если пользователь может просматривать УЦ на данном МХ
            var hasPermissionToViewNotCommandStorageAccountingPrices = user.HasPermissionToViewStorageAccountingPrices(storage);
            if (hasPermissionToViewNotCommandStorageAccountingPrices)
            {
                articlePrices = articlePriceService.GetAccountingPrice(storage.Id, articleIds, DateTime.Now);
            }

            var articleAvailiabilityList = articleAvailabilityService.GetExtendedArticleBatchAvailability(articleIds, storage, accountOrganization, DateTime.Now);
            
            foreach (var article in rows)
            {
                var articleAvailability = articleAvailiabilityList.Where(x => x.ArticleId == article.Id);

                var availableToReserveCount = articleAvailability.Sum(x => x.AvailableToReserveCount);
                var availableToReserveFromPendingCount = articleAvailability.Sum(x => x.AvailableToReserveFromPendingCount);
                var availableToReserveFromStorageCount = articleAvailability.Sum(x => x.AvailableToReserveFromStorageCount);


                decimal currentTaking;
                articleTakingsInfo.TryGetValue(article.Id, out currentTaking);

                model.AddRow(new GridRow(
                    new GridLabelCell("IdForDisplay") { Value = article.Id.ForDisplay() },
                    new GridLabelCell("Number") { Value = article.Number },
                    new GridLabelCell("FullName") { Value = article.FullName },
                    new GridLabelCell("AccountingPrice") { Value = hasPermissionToViewNotCommandStorageAccountingPrices ? articlePrices[article.Id].ForDisplay() : "---" },
                    new GridLabelCell("MeasureUnitShortName") { Value = article.MeasureUnit.ShortName },
                    new GridLabelCell("AvailableToReserveFromPendingCount") { Value = availableToReserveFromPendingCount.ForDisplay() },
                    new GridLabelCell("AvailableToReserveFromStorageCount") { Value = availableToReserveFromStorageCount.ForDisplay() },
                    new GridLabelCell("AvailableToReserveCount") { Value = availableToReserveCount.ForDisplay() },
                    (currentTaking == 0 && articlePrices[article.Id] != null && availableToReserveCount != 0) ?
                        new GridTextEditorCell("MovingCount") { Value = "" } :
                        (GridCell)new GridLabelCell("MovingCount") { Value = (articlePrices[article.Id] == null ? (decimal?)null : currentTaking).ForDisplay() },
                    new GridHiddenCell("MeasureUnitScale") { Value = article.MeasureUnit.Scale.ToString() },
                    new GridHiddenCell("ArticleId") { Value = article.Id.ToString() }
                ));              
            }

            return model;
        }

        #region Вспомогательные методы

        /// <summary>
        /// Десериализация string в коллекцию объектов WaybillRowManualSource
        /// </summary>
        /// <param name="sourceInfo">Формат: id1_count1_type1;id2_count2_type2;id3_count3_type3;", где id - идентификатор позиции-источника, 
        /// count - количество товара, берущееся из этого источника, type - тип накладной</param>
        /// <returns></returns>
        protected IEnumerable<WaybillRowManualSource> DeserializeWaybillRowManualSourceInfo(string sourceInfoString)
        {
            var result = new List<WaybillRowManualSource>();

            var sourceInfoList = sourceInfoString.Split(';');

            foreach (var sourceInfo in sourceInfoList.Where(x => !String.IsNullOrEmpty(x)))
            {
                var sourceInfoFields = sourceInfo.Split('_');

                var waybillRowId = sourceInfoFields[0];
                var count = sourceInfoFields[1];
                var waybillType = sourceInfoFields[2];

                var item = new WaybillRowManualSource()
                {
                    WaybillRowId = ValidationUtils.TryGetGuid(waybillRowId),
                    Count = ValidationUtils.TryGetDecimal(count),
                    WaybillType = ValidationUtils.TryGetEnum<WaybillType>(waybillType)
                };

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Сериализация коллекции объектов WaybillRowManualSource в string
        /// </summary>
        /// <returns>Формат: id1_count1_type1;id2_count2_type2;id3_count3_type3;", где id - идентификатор позиции-источника, 
        /// count - количество товара, берущееся из этого источника, type - тип накладной</returns>
        protected string SerializeWaybillRowManualSourceInfo(IEnumerable<WaybillRowManualSource> sourcesInfo)
        {
            var sourcesInfoString = new StringBuilder();

            foreach (var sourceInfo in sourcesInfo)
            {
                var waybillRowId = sourceInfo.WaybillRowId.ToString();
                var count = sourceInfo.Count.ForEdit();
                var type = sourceInfo.WaybillType.ValueToString();

                sourcesInfoString.AppendFormat("{0}_{1}_{2};", waybillRowId, count, type);
            }

            return sourcesInfoString.ToString();
        }

        /// <summary>
        /// Получение стиля строк грида позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        protected IDictionary<Guid, GridRowStyle> GetRowsStyle(IDictionary<Guid, OutgoingWaybillRowState> states)
        {
            var result = new Dictionary<Guid, GridRowStyle>();

            foreach (var item in states)
            {
                GridRowStyle rowStyle = GridRowStyle.Normal;

                switch (item.Value)
                {
                    case OutgoingWaybillRowState.Conflicts:
                        rowStyle = GridRowStyle.Error; break;

                    case OutgoingWaybillRowState.ArticlePending:
                        rowStyle = GridRowStyle.Warning; break;

                    default:
                        rowStyle = GridRowStyle.Normal; break;
                }

                result.Add(item.Key, rowStyle);
            }

            return result;
        }

        #endregion

        #endregion
    }
}