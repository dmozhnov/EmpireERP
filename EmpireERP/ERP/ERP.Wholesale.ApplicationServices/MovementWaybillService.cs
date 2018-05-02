using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class MovementWaybillService : BaseOutgoingWaybillService<MovementWaybill>, IMovementWaybillService
    {
        #region Поля

        private readonly ISettingRepository settingRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;
        private readonly IArticlePriceService articlePriceService;
        private readonly IArticleMovementService articleMovementService;
        
        private readonly IFactualFinancialArticleMovementService factualFinancialArticleMovementService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IReceiptWaybillService receiptWaybillService;

        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion

        #region Конструктор

        public MovementWaybillService(ISettingRepository settingRepository, IMovementWaybillRepository movementWaybillRepository, IStorageRepository storageRepository,IUserRepository userRepository,
            IArticlePriceService articlePriceService,
            IArticleMovementService articleMovementService, IArticleAvailabilityService articleAvailabilityService,            
            IFactualFinancialArticleMovementService factualFinancialArticleMovementService, IArticleMovementOperationCountService articleMovementOperationCountService,
            IReceiptWaybillService receiptWaybillService, IArticleRevaluationService articleRevaluationService)
            : base(articleAvailabilityService)
        {
            this.settingRepository = settingRepository;

            this.movementWaybillRepository = movementWaybillRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;

            this.articlePriceService = articlePriceService;
            this.articleMovementService = articleMovementService;

            this.factualFinancialArticleMovementService = factualFinancialArticleMovementService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;

            this.receiptWaybillService = receiptWaybillService;
            this.articleRevaluationService = articleRevaluationService;
        }

        #endregion

        #region Методы

        #region Получение накладной по Id

        private MovementWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.MovementWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = movementWaybillRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return waybill;
                }
                else
                {
                    bool contains = (user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage) ||
                        user.Teams.SelectMany(x => x.Storages).Contains(waybill.RecipientStorage));

                    if ((type == PermissionDistributionType.Personal && waybill.Curator == user && contains) ||
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return waybill;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override MovementWaybill CheckWaybillExistence(Guid id, User user)
        {
            var movementWaybill = GetById(id, user);
            ValidationUtils.NotNull(movementWaybill, "Накладная перемещения не найдена. Возможно, она была удалена.");

            return movementWaybill;
        }
        #endregion

        #region Список накладных

        public IEnumerable<MovementWaybill> GetFilteredList(object state, User user, ParameterString param = null)
        {
            Func<ISubCriteria<MovementWaybill>, ISubCriteria<MovementWaybill>> cond = null;

            var articleId = 0;

            if (param == null)
            {
                param = new ParameterString("");
            }
            else
            {
                if (param.Keys.Contains("Article"))
                {
                    if (!String.IsNullOrEmpty((param["Article"].Value as List<string>)[0]))
                    {
                        articleId = ValidationUtils.TryGetInt((param["Article"].Value as List<string>)[0]);
                        cond = crit =>
                        {
                            var q = crit;
                            q.Restriction<MovementWaybillRow>(x => x.Rows)
                                .Where(x => x.Article.Id == articleId);

                            return q.Select(x => x.Id);
                        };
                    }
                    param.Delete("Article");
                }
            }

            switch (user.GetPermissionDistributionType(Permission.MovementWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<MovementWaybill>();

                case PermissionDistributionType.Personal:
                    param.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    AddReceiptAndSenderStorageParameter(user, param);
                    break;

                case PermissionDistributionType.Teams:
                    AddReceiptAndSenderStorageParameter(user, param);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return movementWaybillRepository.GetFilteredList(state, param, cond: cond);
        }

        private void AddReceiptAndSenderStorageParameter(User user, ParameterString param)
        {
            var list = user.Teams.SelectMany(x => x.Storages).Select(x => x.Id.ToString()).Distinct().ToList();

            // если список пуст - то добавляем несуществующее значение
            if (!list.Any()) { list.Add("0"); }

            param.Add(ParameterStringItem.OperationType.Or,
                new ParameterStringItem("SenderStorage", ParameterStringItem.OperationType.OneOf, list),
                new ParameterStringItem("RecipientStorage", ParameterStringItem.OperationType.OneOf, list));
        }

        /// <summary>
        /// Получение списка накладных
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="storageIdList">Список кодов мест хранения</param>
        /// <param name="storagePermission">Право, которым определяются доступные места хранения</param>
        /// <param name="curatorIdList">Список кодов кураторов</param>
        /// <param name="curatorPermission">Право, которым определяются доступные пользователи</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <param name="pageNumber">Номер страницы, первая 1.</param>
        /// <param name="dateType">Тип даты</param>
        /// <param name="priorToDate">Параметр "До даты"</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<MovementWaybill> GetList(MovementWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, 
            IEnumerable<int> curatorIdList, Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, 
            WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<MovementWaybill> movementWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<MovementWaybill>();
            }

            switch (user.GetPermissionDistributionType(curatorPermission))
            {
                case PermissionDistributionType.All:
                    curatorSubQuery = userRepository.GetUserSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    curatorSubQuery = userRepository.GetUserSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<MovementWaybill>();
            }

            switch (user.GetPermissionDistributionType(Permission.MovementWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    movementWaybillSubQuery = movementWaybillRepository.GetMovementWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    movementWaybillSubQuery = movementWaybillRepository.GetMovementWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    movementWaybillSubQuery = movementWaybillRepository.GetMovementWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<MovementWaybill>();
            }

            return movementWaybillRepository.GetList(logicState, movementWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, startDate, endDate, pageNumber, dateType, priorToDate);
        }

        #endregion

        #region Получение позиций

        /// <summary>
        /// Все позиции перемещений с указанным товаром и в указанные сроки, у которых либо МХ-отправитель, либо МХ-получатель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<MovementWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            var senderSubQuery = movementWaybillRepository.SubQuery<MovementWaybill>().OneOf(x => x.SenderStorage.Id, storageIds).Select(x => x.Id);
            var recipientSubQuery = movementWaybillRepository.SubQuery<MovementWaybill>().OneOf(x => x.RecipientStorage.Id, storageIds).Select(x => x.Id);

            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = movementWaybillRepository.SubQuery<MovementWaybill>()
                   .Or(x => x.PropertyIn(y => y.Id, senderSubQuery), x => x.PropertyIn(y => y.Id, recipientSubQuery))
                   .Where(x => x.ReceiptDate >= startDate && x.ReceiptDate <= endDate)
                   .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = movementWaybillRepository.SubQuery<MovementWaybill>()
                   .Or(x => x.PropertyIn(y => y.Id, senderSubQuery), x => x.PropertyIn(y => y.Id, recipientSubQuery))
                   .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                   .Select(x => x.Id);
            }

            return movementWaybillRepository.Query<MovementWaybillRow>()
                .PropertyIn(x => x.MovementWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<MovementWaybillRow>();
        }

        #endregion

        #region Добавление / удаление позиций

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(MovementWaybill waybill, MovementWaybillRow row, User user)
        {
            AddRowLocal(waybill, row, null, user);
        }

        /// <summary>
        /// Добавление позиции в накладную с указанием источников вручную
        /// </summary>
        public virtual void AddRow(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            AddRowLocal(waybill, row, sourceDistributionInfo, user);                        
        }

        private void AddRowLocal(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            waybill.AddRow(row);

            movementWaybillRepository.Save(waybill);

            if (sourceDistributionInfo != null)
            {
                articleMovementService.SetManualSources(row, sourceDistributionInfo);
            }            
        }

        /// <summary>
        /// Упрощенное добавление позиции в накладную
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="article">Товар</param>
        /// <param name="count">Кол-во резервируемого товара</param>
        /// <param name="user">Пользователь</param>        
        public void AddRowSimply(MovementWaybill waybill, Article article, decimal count, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            // распределяем кол-во товара по партиям
            var countDistributionInfo = DistributeCountByBatches(article, waybill.SenderStorage, waybill.Sender, count);

            var batchList = receiptWaybillService.GetRows(countDistributionInfo.Keys);
            foreach (var item in countDistributionInfo)
            {
                var batch = batchList[item.Key];
                var row = new MovementWaybillRow(batch, item.Value, waybill.ValueAddedTax);

                AddRow(waybill, row, user);
            }
        }

        public void SaveRow(MovementWaybill waybill, MovementWaybillRow row, User user)
        {
            SaveRowLocal(waybill, row, null, user);
        }

        /// <summary>
        /// Сохранение позиции с указанием источников вручную (при редактировании)
        /// </summary>
        public void SaveRow(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            SaveRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void SaveRowLocal(MovementWaybill waybill, MovementWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            // если у позиции источники уже установлены вручную, то сначала сбрасываем источники
            // TODO: в будущем добавить параметр о необходимости сброса источников в метод SetManualSources
            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }
            
            if (sourceDistributionInfo != null)
            {
                articleMovementService.SetManualSources(row, sourceDistributionInfo);
            }
            
            movementWaybillRepository.Save(waybill);
        }

        /// <summary>
        /// Удаление позиции из накладной
        /// </summary>        
        public virtual void DeleteRow(MovementWaybill waybill, MovementWaybillRow row, User user)
        {
            CheckPossibilityToDeleteRow(row, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }

            waybill.DeleteRow(row);
        }

        #endregion

        #region Подготовка к проводке/отмена готовности к проводке

        /// <summary>
        /// Подготовка к проводке накладной
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void PrepareToAccept(MovementWaybill waybill, User user)
        {
            // Проверяем права
            CheckPossibilityToPrepareToAccept(waybill, user);

            waybill.PrepareToAccept();
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void CancelReadinessToAccept(MovementWaybill waybill, User user)
        {
            // Проверяем права
            CheckPossibilityToCancelReadinessToAccept(waybill, user);

            waybill.CancelReadinessToAccept();
        }

        #endregion

        #region Проводка / отмена проводки

        public void Accept(MovementWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToAccept(waybill, user);

            // получение текущих позиций реестров цен
            var recipientArticleAccountingPrices = articlePriceService.GetArticleAccountingPrices(waybill.RecipientStorage.Id,
                movementWaybillRepository.GetArticlesSubquery(waybill.Id), currentDateTime);
            var senderArticleAccountingPrices = articlePriceService.GetArticleAccountingPrices(waybill.SenderStorage.Id,
                movementWaybillRepository.GetArticlesSubquery(waybill.Id), currentDateTime);

            // проводка накладной
            waybill.Accept(senderArticleAccountingPrices, recipientArticleAccountingPrices, UseReadyToAcceptState, user, currentDateTime);
            
            // резервирование товаров при проводке
            var reservationInfoList = articleMovementService.AcceptArticles(waybill);
            
            //Пересчет показателей входящего и исходящего проведенного наличия
            articleAvailabilityService.MovementWaybillAccepted(waybill, reservationInfoList);
        }

        public void CancelAcceptance(MovementWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToCancelAcceptance(waybill, user);

            // отмена резервирования товара при проводке
            var reservationInfoList = articleMovementService.CancelArticleAcceptance(waybill);

            // Пересчет показателей входящего и исходящего проведенного наличия
            articleAvailabilityService.MovementWaybillAcceptanceCanceled(waybill, reservationInfoList,
                articleMovementService.GetIncomingWaybillRowForOutgoingWaybillRow(movementWaybillRepository.GetRowsSubQuery(waybill.Id)));

            waybill.CancelAcceptance(UseReadyToAcceptState);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.MovementWaybillAcceptanceCancelled(waybill);
        }

        #endregion

        #region Отгрузка / отмена отгрузки

        /// <summary>
        /// Отгрузить
        /// </summary> 
        public virtual void Ship(MovementWaybill waybill, User user, DateTime currentDateTime)
        {
            CheckPossibilityToShip(waybill, user);

            articleMovementService.ShipAcceptedArticles(waybill);

            waybill.Ship(user, currentDateTime);
        }

        /// <summary>
        /// Отменить отгрузку
        /// </summary>
        public virtual void CancelShipping(MovementWaybill waybill, User user)
        {
            CheckPossibilityToCancelShipping(waybill, user);

            articleMovementService.CancelArticleShipping(waybill);

            waybill.CancelShipping();
        }

        #endregion

        #region Приемка / отмена приемки

        /// <summary>
        /// Принять
        /// </summary>
        public virtual void Receipt(MovementWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToReceipt(waybill, user);

            // приемка товара
            waybill.Receipt(user, currentDateTime);

            movementWaybillRepository.Save(waybill);

            // пометка товара как окончательно перемещенного
            articleMovementService.FinallyMoveShippedArticles(waybill);

            // Пересчет показателей точного и проведенного наличия
            articleAvailabilityService.MovementWaybillReceipted(waybill);
            
            // пересчет финансовых показателей
            factualFinancialArticleMovementService.MovementWaybillReceipted(waybill);

            // пересчет счетчиков количеств операций
            articleMovementOperationCountService.WaybillFinalized(waybill);

            // расчет переоценок по принятым позициям
            articleRevaluationService.MovementWaybillFinalized(waybill);

            movementWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        /// <summary>
        /// Отмена приемки
        /// </summary>
        public virtual void CancelReceipt(MovementWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToCancelReceipt(waybill, user);

            // Пересчет показателей точного и проведенного наличия
            articleAvailabilityService.MovementWaybillReceiptCanceled(waybill,
                articleMovementService.GetOutgoingWaybillRows(movementWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // пересчет показателей переоценки
            articleRevaluationService.MovementWaybillReceiptCancelled(waybill);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.MovementWaybillReceiptCancelled(waybill);

            //Пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);

            waybill.CancelReceipt();

            articleMovementService.CancelArticleFinalMoving(waybill);

            movementWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ReceiptDate);
        }

        #endregion

        #region Удаление

        public void Delete(MovementWaybill waybill, User user)
        {
            CheckPossibilityToDelete(waybill, user);

            // удаляем связи с установленными вручную источниками
            articleMovementService.ResetManualSources(waybill);

            movementWaybillRepository.Delete(waybill);
        }

        #endregion

        #region Смена получателя

        public void ChangeRecipientStorage(MovementWaybill movementWaybill, Storage newRecipientStorage, AccountOrganization newRecipient)
        {
            var articleList = movementWaybill.Rows.Select(x => x.Article);
            var accPriceList = articlePriceService.GetAccountingPrice(newRecipientStorage.Id, movementWaybillRepository.GetArticlesSubquery(movementWaybill.Id));

            foreach (var article in articleList)
            {
                if (!accPriceList[article.Id].HasValue)
                {
                    throw new Exception(String.Format("На месте хранения «{0}» не установлена учетная цена для одного или нескольких товаров.", newRecipientStorage.Name));
                }
            }

            movementWaybill.RecipientStorage = newRecipientStorage;
            movementWaybill.Recipient = newRecipient;
        } 
        #endregion

        #region Сохранение

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="movementWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(MovementWaybill movementWaybill, User curator)
        {
            var storages = curator.Teams.SelectMany(x => x.Storages);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.MovementWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
                case PermissionDistributionType.Personal:
                    result=(storages.Contains(movementWaybill.SenderStorage) || storages.Contains(movementWaybill.RecipientStorage)) && 
                        movementWaybill.Curator == curator;
                    break;
                case PermissionDistributionType.Teams:
                    result=storages.Contains(movementWaybill.SenderStorage) || storages.Contains(movementWaybill.RecipientStorage);
                    break;
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }
        
        public void Save(MovementWaybill waybill)
        {
            // если номер генерируется автоматически
            if (waybill.Number == "")
            {
                var lastDocumentNumbers = waybill.Sender.GetLastDocumentNumbers(waybill.Date.Year);
                var number = lastDocumentNumbers.MovementWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, waybill.Date, waybill.Sender))
                {
                    number = number + 1;
                }

                waybill.Number = number.ToString();
                lastDocumentNumbers.MovementWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(waybill.Number, waybill.Id, waybill.Date, waybill.Sender),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", waybill.Number));
            }

            movementWaybillRepository.Save(waybill);
        }

        /// <summary>
        /// Проверка номера накладной на уникальность
        /// </summary>
        /// <param name="number">Номер накладной</param>
        /// <param name="id">Код текущей накладной</param>
        /// <returns>Результат проверки</returns>
        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return movementWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }
        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(MovementWaybill waybill)
        {
            var result = new Dictionary<Guid, OutgoingWaybillRowState>();

            IEnumerable<ArticleBatchAvailabilityShortInfo> articleBatchAvailability = null; 

            // если накладная не проведена, то для позиций без ручного указания источников 
            // необходимо найти точное наличие
            if (!waybill.IsAccepted)
            {
                // создаем подзапрос для партий для позиций без ручного указания источников
                var rowWithoutManualSourceBatchSubQuery = movementWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

                // получаем доступное для резервирования кол-во по партиям
                articleBatchAvailability = articleAvailabilityService.GetAvailableToReserveFromExactArticleAvailability(
                    rowWithoutManualSourceBatchSubQuery, waybill.SenderStorage.Id, waybill.Sender.Id, DateTime.Now);
            }
            
            // вычисляем статусы всех позиций
            foreach (var row in waybill.Rows)
            {
                var outgoingWaybillRowState = OutgoingWaybillRowState.ArticlePending;
                
                // если накладная проведена или по позиции известны источники - то статус уже известен
                if (waybill.IsAccepted || row.IsUsingManualSource)
                {
                    outgoingWaybillRowState = row.OutgoingWaybillRowState;
                }
                // если накладная не проведена и источники для позиции не указаны
                else
                {
                    // находим точное наличие по партии
                    var availability = articleBatchAvailability.Where(x => x.BatchId == row.ReceiptWaybillRow.Id).FirstOrDefault();
                    
                    outgoingWaybillRowState = (availability != null && availability.Count >= row.MovingCount) ? 
                        OutgoingWaybillRowState.ReadyToArticleMovement : OutgoingWaybillRowState.ArticlePending;
                }
                
                result.Add(row.Id, outgoingWaybillRowState);
            }

            return result;
        }

        #endregion

        #region Настройки аккаунта

        /// <summary>
        /// Разрешение использовать подготовку к проведению
        /// </summary>
        /// <returns></returns>
        private bool UseReadyToAcceptState
        {
            get { return settingRepository.Get().UseReadyToAcceptStateForMovementWaybill; }
        }

        #endregion

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(MovementWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = (waybill.Curator == user);
                    break;

                case PermissionDistributionType.Teams:
                    result = (user.Teams.SelectMany(x => x.Storages).Contains(waybill.RecipientStorage) ||
                        user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage));
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(MovementWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }
        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_List_Details);
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToEdit(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Create_Edit);

            // редактировать можно только накладную, исходящую из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEdit();
        }
        #endregion

        #region Смена получателя

        public bool IsPossibilityToEditRecipientAndRecipientStorage(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToEditRecipientAndRecipientStorage(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditRecipientAndRecipientStorage(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Create_Edit);

            // редактировать можно только накладную, исходящую из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEditRecipientAndRecipientStorage();
        }
        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToDelete(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Delete_Row_Delete);

            // удалить можно только накладную, исходящую из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }
        #endregion

        #region Удаление позиции

        public bool IsPossibilityToDeleteRow(MovementWaybillRow row, User user)
        {
            try
            {
                CheckPossibilityToDeleteRow(row, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteRow(MovementWaybillRow row, User user)
        {
            // права
            CheckPermissionToPerformOperation(row.MovementWaybill, user, Permission.MovementWaybill_Delete_Row_Delete);

            // удалить можно только позицию накладной, исходящей из места хранения пользователя
            user.CheckStorageAvailability(row.MovementWaybill.SenderStorage, Permission.MovementWaybill_Delete_Row_Delete);

            // сущность
            row.CheckPossibilityToDelete();
        }
        #endregion

        #region Подготовка к проводке

        public bool IsPossibilityToPrepareToAccept(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToPrepareToAccept(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToPrepareToAccept(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            // настроки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Create_Edit);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToPrepareToAccept();
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной подготовка к проводке. 
                ValidationUtils.Assert(waybill.IsDraft, String.Format("Невозможно подготовить к проводке накладную со статусом «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отмена подготовки к проводке

        public bool IsPossibilityToCancelReadinessToAccept(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelReadinessToAccept(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelReadinessToAccept(MovementWaybill waybill, User user)
        {
            // настроки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCancelReadinessToAccept();
        }

        #endregion

        #region Проводка

        public bool IsPossibilityToAccept(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToAccept(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAccept(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Accept);

            // провести можно только накладную, исходящую из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Accept);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToAccept(UseReadyToAcceptState);
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной проводка.
                ValidationUtils.Assert(UseReadyToAcceptState? waybill.IsReadyToAccept: waybill.IsNew, 
                    String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отмена проводки

        public bool IsPossibilityToCancelAcceptance(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelAcceptance(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelAcceptance(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Acceptance_Cancel);

            // отменить проводку можно только для накладной, исходящей из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Acceptance_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelAcceptance();
        }
        #endregion

        #region Отгрузка

        public bool IsPossibilityToShip(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            try
            {
                CheckPossibilityToShip(waybill, user, onlyPermission);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToShip(MovementWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Ship);

            // отгрузить можно только накладную, исходящую из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Ship);

            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToShip();
            }
            else
            {
                ValidationUtils.Assert(waybill.State == MovementWaybillState.ReadyToShip,
                    String.Format("Отгрузить товар можно только для накладной со статусом «{0}».", MovementWaybillState.ReadyToShip.GetDisplayName()));
            }
        }
        #endregion

        #region Отмена отгрузки

        public bool IsPossibilityToCancelShipping(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelShipping(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelShipping(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Shipping_Cancel);

            // отменить отгрузку можно только для накладной, исходящей из места хранения пользователя
            user.CheckStorageAvailability(waybill.SenderStorage, Permission.MovementWaybill_Shipping_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelShipping();
        }
        #endregion

        #region Приемка

        public bool IsPossibilityToReceipt(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToReceipt(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToReceipt(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Receipt);

            // принять можно только накладную, входящую на место хранения пользователя
            user.CheckStorageAvailability(waybill.RecipientStorage, Permission.MovementWaybill_Receipt);

            // сущность
            waybill.CheckPossibilityToReceipt();
        }
        #endregion

        #region Отмена приемки

        public bool IsPossibilityToCancelReceipt(MovementWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCancelReceipt(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCancelReceipt(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Receipt_Cancel);

            // отменить приемку можно только для накладной, входящей на место хранения пользователя
            user.CheckStorageAvailability(waybill.RecipientStorage, Permission.MovementWaybill_Receipt_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelReceipt();
        }
        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(MovementWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_List_Details);

            // сущность
            ValidationUtils.Assert(waybill.IsAccepted, "Невозможно распечатать форму, т.к. накладная еще не проведена.");
        }

        #region Печатная форма накладной
        
        public bool IsPossibilityToPrintWaybillForm(MovementWaybill waybill, bool printSenderPrices, bool printRecipientPrices, User user)
        {
            try
            {
                CheckPossibilityToPrintWaybillForm(waybill, printSenderPrices, printRecipientPrices, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToPrintWaybillForm(MovementWaybill waybill, bool printSenderPrices, bool printRecipientPrices, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            if (printSenderPrices)
            {
                ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage), "Недостаточно прав для просмотра учетных цен отправителя.");
            }

            if (printRecipientPrices)
            {
                ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage), "Недостаточно прав для просмотра учетных цен получателя.");
            }
        } 
        #endregion

        #region Печать форм в УЦ отправителя
        
        public bool IsPossibilityToPrintFormInSenderAccountingPrices(MovementWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInSenderAccountingPrices, waybill, user);            
        }

        public void CheckPossibilityToPrintFormInSenderAccountingPrices(MovementWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.SenderStorage),
                "Недостаточно прав для просмотра учетных цен отправителя.");            
        } 
        #endregion

        #region Печать форм в УЦ получателя

        public bool IsPossibilityToPrintFormInRecipientAccountingPrices(MovementWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInRecipientAccountingPrices, waybill, user);            
        }

        public void CheckPossibilityToPrintFormInRecipientAccountingPrices(MovementWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.RecipientStorage),
                "Недостаточно прав для просмотра учетных цен получателя.");
        }
        #endregion

        #region Печать форм в ЗЦ

        public bool IsPossibilityToPrintFormInPurchaseCosts(MovementWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInPurchaseCosts, waybill, user);
        }

        public void CheckPossibilityToPrintFormInPurchaseCosts(MovementWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                "Недостаточно прав для просмотра закупочных цен.");
        }
        #endregion

        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(MovementWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.MovementWaybill_Curator_Change);

            //сущность
            waybill.CheckPossibilityToChangeCurator();
        }
        #endregion

        #endregion

        #endregion
    }
}