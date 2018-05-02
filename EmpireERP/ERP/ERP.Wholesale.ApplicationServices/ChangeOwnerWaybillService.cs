using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ChangeOwnerWaybillService : BaseOutgoingWaybillService<ChangeOwnerWaybill>, IChangeOwnerWaybillService
    {
        #region Поля

        private readonly ISettingRepository settingRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IArticleRepository articleRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;

        private readonly IArticlePriceService articlePriceService;
        private readonly IArticleMovementService articleMovementService;
        private readonly IReceiptWaybillService receiptWaybillService;

        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion
        
        #region Конструкторы

        public ChangeOwnerWaybillService(ISettingRepository settingRepository, IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IArticleRepository articleRepository, 
            IStorageRepository storageRepository,IUserRepository userRepository,
            IArticlePriceService articlePriceService, IArticleAvailabilityService articleAvailabilityService, IArticleMovementService articleMovementService,
            IReceiptWaybillService receiptWaybillService, IArticleRevaluationService articleRevaluationService)
            :base(articleAvailabilityService)
        {
            this.settingRepository = settingRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.articleRepository = articleRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;

            this.articlePriceService = articlePriceService;
            this.articleMovementService = articleMovementService;

            this.receiptWaybillService = receiptWaybillService;
            this.articleRevaluationService = articleRevaluationService;

            articleMovementService.ChangeOwnerWaybillReadyToChangedOwner += new ChangeOwnerWaybillEvent(ChangeOwner);
        }

        #endregion

        #region Методы

        #region Методы репозитория

        /// <summary>
        /// Получение накладной по коду с учетом прав пользователя
        /// </summary>
        /// <param name="id">Код накладной</param>
        /// <param name="user">Пользователь</param>     
        private ChangeOwnerWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.ChangeOwnerWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = changeOwnerWaybillRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return waybill;
                }
                else
                {
                    bool contains = user.Teams.SelectMany(x => x.Storages).Contains(waybill.Storage);

                    if ((type == PermissionDistributionType.Personal && waybill.Curator == user && contains) || // все равно фильтруем еще и по камандам
                        (type == PermissionDistributionType.Teams && contains))
                    {
                        return waybill;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="changeOwnerWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(ChangeOwnerWaybill changeOwnerWaybill, User curator)
        {
            var storages = curator.Teams.SelectMany(x => x.Storages);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.ChangeOwnerWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
                case PermissionDistributionType.Personal:
                    result = storages.Contains(changeOwnerWaybill.Storage) && changeOwnerWaybill.Curator == curator;
                    break;
                case PermissionDistributionType.Teams:
                    result = storages.Contains(changeOwnerWaybill.Storage);
                    break;
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }

        public Guid Save(ChangeOwnerWaybill waybill)
        {
            // если номер генерируется автоматически
            if (waybill.Number == "")
            {
                var lastDocumentNumbers = waybill.Sender.GetLastDocumentNumbers(waybill.Date.Year);

                var number = lastDocumentNumbers.ChangeOwnerWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, waybill.Date, waybill.Sender))
                {
                    number = number + 1;
                }

                waybill.Number = number.ToString();
                lastDocumentNumbers.ChangeOwnerWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(waybill.Number, waybill.Id, waybill.Date, waybill.Sender),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", waybill.Number));
            }

            changeOwnerWaybillRepository.Save(waybill);

            return waybill.Id;
        }

        public void Delete(ChangeOwnerWaybill waybill, User user)
        {
            CheckPossibilityToDelete(waybill, user);

            // удаляем связи с установленными вручную источниками
            articleMovementService.ResetManualSources(waybill);
                        
            changeOwnerWaybillRepository.Delete(waybill);
        }

        public IList<ChangeOwnerWaybill> GetFilteredList(object state, ParameterString param, User user)
        {
            Func<ISubCriteria<ChangeOwnerWaybill>, ISubCriteria<ChangeOwnerWaybill>> cond = null;

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
                            q.Restriction<ChangeOwnerWaybillRow>(x => x.Rows).Where(x => x.Article.Id == articleId);

                            return q.Select(x => x.Id);
                        };
                    }
                    param.Delete("Article");
                }
            }

            switch (user.GetPermissionDistributionType(Permission.ChangeOwnerWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<ChangeOwnerWaybill>();

                case PermissionDistributionType.Personal:
                    param.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    AddStorageParameter(user, param);
                    break;

                case PermissionDistributionType.Teams:
                    AddStorageParameter(user, param);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return changeOwnerWaybillRepository.GetFilteredList(state, param);
        }

        private void AddStorageParameter(User user, ParameterString param)
        {
            var list = user.Teams.SelectMany(x => x.Storages).Select(x => x.Id.ToString()).Distinct().ToList();

            // если список пуст - то добавляем несуществующее значение
            if (!list.Any()) { list.Add("0"); }

            if (param.Keys.Contains("Storage"))
            {
                var storageValue = param["Storage"].Value as string;
                if (!String.IsNullOrEmpty(storageValue))
                {
                    if (!list.Contains(param["Storage"].Value as string))
                    {
                        param["Storage"].Value = "0";   //т.к. указано МХ, на которое у пользователя нет прав
                    }
                }
                else
                {
                    param["Storage"].Value = list;
                }
            }
            else
            {
                param.Add("Storage", ParameterStringItem.OperationType.OneOf, list);
            }
        }

        /// <summary>
        /// Проверка номера накладной на уникальность
        /// </summary>
        /// <param name="number">Номер накладной</param>
        /// <param name="id">Код текущей накладной</param>
        /// <returns>Результат проверки</returns>
        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return changeOwnerWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }

        public ChangeOwnerWaybillRow GetRowById(Guid id)
        {
            return changeOwnerWaybillRepository.GetRowById(id);
        }

        /// <summary>
        /// Добавление позиции в накладную
        /// </summary>
        public virtual void AddRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, User user)
        {
            AddRowLocal(waybill, row, null, user);
        }

        /// <summary>
        /// Добавление позиции в накладную с указанием источников вручную
        /// </summary>
        public virtual void AddRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            AddRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void AddRowLocal(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            waybill.AddRow(row);

            changeOwnerWaybillRepository.Save(waybill);

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
        public void AddRowSimply(ChangeOwnerWaybill waybill, Article article, decimal count, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            // распределяем кол-во товара по партиям
            var countDistributionInfo = DistributeCountByBatches(article, waybill.Storage, waybill.Sender, count);

            var batchList = receiptWaybillService.GetRows(countDistributionInfo.Keys);
            foreach (var item in countDistributionInfo)
            {
                var batch = batchList[item.Key];
                var row = new ChangeOwnerWaybillRow(batch, item.Value, waybill.ValueAddedTax);

                AddRow(waybill, row, user);
            }
        }

        /// <summary>
        /// Сохранение позиции накладной (метод должен вызываться при редактировании строки, при добавлении должен вызываться AddRow)
        /// </summary>
        /// <param name="row"></param>
        public void SaveRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, User user)
        {
            SaveRowLocal(waybill, row, null, user);
        }

        /// <summary>
        /// Сохранение позиции с указанием источников вручную (при редактировании)
        /// </summary>
        public void SaveRow(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            SaveRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void SaveRowLocal(ChangeOwnerWaybill waybill, ChangeOwnerWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }

            if (sourceDistributionInfo != null)
            {
                articleMovementService.SetManualSources(row, sourceDistributionInfo);
            }

            changeOwnerWaybillRepository.Save(waybill);
        }

        public void DeleteRow(ChangeOwnerWaybillRow row, User user)
        {
            CheckPossibilityToDeleteRow(row, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }
            
            row.ChangeOwnerWaybill.DeleteRow(row);            
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
        public IEnumerable<ChangeOwnerWaybill> GetList(ChangeOwnerWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<ChangeOwnerWaybill> changeOwnerWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ChangeOwnerWaybill>();
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
                    return new List<ChangeOwnerWaybill>();
            }

            switch (user.GetPermissionDistributionType(Permission.ChangeOwnerWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    changeOwnerWaybillSubQuery = changeOwnerWaybillRepository.GetChangeOwnerWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    changeOwnerWaybillSubQuery = changeOwnerWaybillRepository.GetChangeOwnerWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    changeOwnerWaybillSubQuery = changeOwnerWaybillRepository.GetChangeOwnerWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<ChangeOwnerWaybill>();
            }

            return changeOwnerWaybillRepository.GetList(logicState, changeOwnerWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, startDate, endDate, pageNumber, dateType, priorToDate);
        }

        #endregion        

        #region Проверка существования

        /// <summary>
        /// Получение накладной по id с проверкой ее существования и проверкой прав
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ChangeOwnerWaybill CheckWaybillExistence(Guid id, User user)
        {
            var changeOwnerWaybill = GetById(id, user);
            if (changeOwnerWaybill == null)
            {
                throw new Exception("Накладная смены собственника не найдена. Возможно, она была удалена.");
            }

            return changeOwnerWaybill;
        }

        /// <summary>
        /// Получение накладной по id с проверкой ее существования, без проверки прав
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ChangeOwnerWaybill CheckWaybillExistence(Guid id)
        {
            var changeOwnerWaybill = changeOwnerWaybillRepository.GetById(id);
            if (changeOwnerWaybill == null)
            {
                throw new Exception("Накладная смены собственника не найдена. Возможно, она была удалена.");
            }

            return changeOwnerWaybill;
        }


        /// <summary>
        /// Получение позиции накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChangeOwnerWaybillRow CheckWaybillRowExistence(Guid id)
        {
            var changeOwnerWaybillRow = GetRowById(id);
            if (changeOwnerWaybillRow == null)
            {
                throw new Exception("Позиция накладной смены собственника не найдена. Возможно, она была удалена.");
            }

            return changeOwnerWaybillRow;
        }

        #endregion

        #region Получение позиций

        /// <summary>
        /// Все позиции с указанным товаром в указанные сроки, у которых МХ входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<ChangeOwnerWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = changeOwnerWaybillRepository.SubQuery<ChangeOwnerWaybill>()
                .OneOf(x => x.Storage.Id, storageIds)
                .Where(x => x.ChangeOwnerDate >= startDate && x.ChangeOwnerDate <= endDate)
                .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = changeOwnerWaybillRepository.SubQuery<ChangeOwnerWaybill>()
                .OneOf(x => x.Storage.Id, storageIds)
                .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                .Select(x => x.Id);
            }

            return changeOwnerWaybillRepository.Query<ChangeOwnerWaybillRow>()
                .PropertyIn(x => x.ChangeOwnerWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<ChangeOwnerWaybillRow>();
        }

        #endregion

        #region Подготовка/отмена готовности к проводке

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void PrepareToAccept(ChangeOwnerWaybill waybill, User user)
        {
            // Проверяем возможность совершения операции
            CheckPossibilityToPrepareToAccept(waybill, user);

            waybill.PrepareToAccept();  // Подготавливаем к проводке
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void CancelReadinessToAccept(ChangeOwnerWaybill waybill, User user)
        {
            // Проверяем возможность совершения операции
            CheckPossibilityToCancelReadinessToAccept(waybill, user);

            waybill.CancelReadinessToAccept();  // Отменяем готовность к проводке
        }

        #endregion

        #region Смена получателя

        public void ChangeRecipient(ChangeOwnerWaybill waybill, AccountOrganization recipient, User user)
        {
            CheckPossibilityToChangeRecipient(waybill, user);

            waybill.ChangeRecipient(recipient);

            Save(waybill);
        }

        #endregion

        #region Проводка накладной / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="changeOwnerWaybill"></param>
        public void Accept(ChangeOwnerWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToAccept(waybill, user);
            
            // получение текущих позиций реестров цен
            var priceLists = articlePriceService.GetArticleAccountingPrices(waybill.Storage.Id, 
                changeOwnerWaybillRepository.GetArticlesSubquery(waybill.Id), currentDateTime);

            waybill.Accept(priceLists, UseReadyToAcceptState, user, currentDateTime);

            // резервирование товаров при проводке
            var reservationInfoList = articleMovementService.AcceptArticles(waybill);

            // Пересчет показателей входящего и исходящего проведенного наличия
            articleAvailabilityService.ChangeOwnerWaybillAccepted(waybill, reservationInfoList);
            
            changeOwnerWaybillRepository.Save(waybill);

            if (waybill.State == ChangeOwnerWaybillState.ReadyToChangeOwner) //Если накладная готова к смене собственника, ...
            {
                ChangeOwner(waybill, currentDateTime);   // ... то меняем собственника накладной
            }
        }

        /// <summary>
        /// Отмена проводки
        /// </summary>
        /// <param name="changeOwnerWaybill"></param>
        public void CancelAcceptance(ChangeOwnerWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            CheckPossibilityToCancelAcceptance(waybill, user);

            var acceptanceDate = waybill.AcceptanceDate.Value;

            if (waybill.IsOwnerChanged) //Если накладная отгружена, то ...
            {
                // ... при отмене проводки сначала выполняем отмену отгрузки
                CancelOwnerChange(waybill, currentDateTime);
            }

            changeOwnerWaybillRepository.Save(waybill);

            // отмена резервирования товара при проводке
            var reservationInfoList = articleMovementService.CancelArticleAcceptance(waybill);

            //  Пересчет показателей входящего и исходящего проведенного наличия
            articleAvailabilityService.ChangeOwnerWaybillAcceptanceCanceled(waybill, reservationInfoList,
                articleMovementService.GetIncomingWaybillRowForOutgoingWaybillRow(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // отменяем проводку
            waybill.CancelAcceptance(UseReadyToAcceptState);

            changeOwnerWaybillRepository.Save(waybill);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.ChangeOwnerWaybillAcceptanceCancelled(waybill);
        }

        #endregion

        #region Смена собственника (отгрузка)

        /// <summary>
        /// Смена собственника (отгрузка)
        /// </summary>
        /// <param name="waybill"></param>
        private void ChangeOwner(ChangeOwnerWaybill waybill, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            // За текущую дату принимаем максимальную из переданной даты и даты проводки.
            // Необходимо, т.к. при приемке прихода задним числом сюда приходит дата приемки прихода. 
            // А она может быть меньше даты проводки накладной смены собственника.
            currentDateTime = currentDateTime > waybill.AcceptanceDate ? currentDateTime : waybill.AcceptanceDate.Value;

            waybill.ChangeOwner(currentDateTime);

            changeOwnerWaybillRepository.Save(waybill);

            articleMovementService.FinallyMoveAcceptedArticles(waybill);

            //Пересчет показателей точного наличия и проведенного наличия
            articleAvailabilityService.ChangeOwnerWaybillOwnerChanged(waybill, 
                articleMovementService.GetOutgoingWaybillRows(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id)));

            // TODO: добавить пересчет финансовых показателей и счетчиков операций

            // расчет переоценок по принятым позициям
            articleRevaluationService.ChangeOwnerWaybillFinalized(waybill);

            changeOwnerWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ChangeOwnerDate);
        }

        /// <summary>
        /// Отмена смены собственника (отмена отгрузки)
        /// </summary>
        /// <param name="waybill"></param>
        private void CancelOwnerChange(ChangeOwnerWaybill waybill, DateTime currentDateTime)
        {
            // регулярная проверка - появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);
            
            waybill.CheckPossibilityToCancelOwnerChange();

            // Пересчет показателей точного и проведенного наличия
            articleAvailabilityService.ChangeOwnerWaybillOwnerChangeCanceled(waybill,
                articleMovementService.GetOutgoingWaybillRows(changeOwnerWaybillRepository.GetRowsSubQuery(waybill.Id)));

            changeOwnerWaybillRepository.Save(waybill);

            // пересчет показателей переоценки
            articleRevaluationService.ChangeOwnerWaybillReceiptCancelled(waybill);

            waybill.ChangeOwnerDate = null; //Сбрасываем дату смены собственника

            articleMovementService.CancelArticleFinalMoving(waybill);

            // TODO: добавить пересчет финансовых показателей и счетчиков операций

            changeOwnerWaybillRepository.Save(waybill);

            articleMovementService.UpdateOutgoingWaybillsStates(waybill, waybill.ChangeOwnerDate);
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(ChangeOwnerWaybill waybill)
        {
            var result = new Dictionary<Guid, OutgoingWaybillRowState>();

            IEnumerable<ArticleBatchAvailabilityShortInfo> articleBatchAvailability = null;

            // если накладная не проведена, то для позиций без ручного указания источников 
            // необходимо найти точное наличие
            if (!waybill.IsAccepted)
            {
                // создаем подзапрос для партий для позиций без ручного указания источников
                var rowWithoutManualSourceBatchSubQuery = changeOwnerWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

                // получаем доступное для резервирования кол-во по партиям
                articleBatchAvailability = articleAvailabilityService.GetAvailableToReserveFromExactArticleAvailability(
                    rowWithoutManualSourceBatchSubQuery, waybill.Storage.Id, waybill.Sender.Id, DateTime.Now);
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

        #region Проверка возможности выполнения операций

        #region Настройки аккаунта

        /// <summary>
        /// Признак возможности использовать статус «Готово к проводке»
        /// </summary>
        private bool UseReadyToAcceptState
        {
            get { return settingRepository.Get().UseReadyToAcceptStateForChangeOwnerWaybill; }
        }

        #endregion

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(ChangeOwnerWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = ((waybill.Curator == user) && user.Teams.SelectMany(x => x.Storages).Contains(waybill.Storage)); // свои + командные
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Storages).Contains(waybill.Storage);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(ChangeOwnerWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }
        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(ChangeOwnerWaybill waybill, User user)
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

        public void CheckPossibilityToViewDetails(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_List_Details);
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(ChangeOwnerWaybill waybill, User user)
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

        public void CheckPossibilityToEdit(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEdit();
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(ChangeOwnerWaybill waybill, User user)
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

        public void CheckPossibilityToDelete(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }

        #endregion

        #region Редактирование позиций

        public bool IsPossibilityToEditRow(ChangeOwnerWaybillRow waybillRow, User user)
        {
            try
            {
                CheckPossibilityToEditRow(waybillRow, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditRow(ChangeOwnerWaybillRow waybillRow, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybillRow.ChangeOwnerWaybill, user, Permission.ChangeOwnerWaybill_Create_Edit);

            // сущность
            waybillRow.CheckPossibilityToEdit();
        }

        #endregion

        #region Удаление позиций

        public bool IsPossibilityToDeleteRow(ChangeOwnerWaybillRow waybillRow, User user)
        {
            try
            {
                CheckPossibilityToDeleteRow(waybillRow, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteRow(ChangeOwnerWaybillRow waybillRow, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybillRow.ChangeOwnerWaybill, user, Permission.ChangeOwnerWaybill_Delete_Row_Delete);

            // сущность
            waybillRow.CheckPossibilityToDelete();
        }

        #endregion

        #region Создание позиций

        public bool IsPossibilityToCreateRow(ChangeOwnerWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToCreateRow(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateRow(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCreateRow();
        }

        #endregion

        #region Подготовка к проводке

        public bool IsPossibilityToPrepareToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToPrepareToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false)
        {
            // настройки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Create_Edit);

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

        #region Отменить готовность к проводке

        public bool IsPossibilityToCancelReadinessToAccept(ChangeOwnerWaybill waybill, User user)
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

        public void CheckPossibilityToCancelReadinessToAccept(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCancelReadinessToAccept();
        }

        #endregion

        #region Проводка

        public bool IsPossibilityToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToAccept(ChangeOwnerWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Accept);

            // Сделано для корректного отображения кнопки на форме
            if (!onlyPermission)
            {
                // сущность
                waybill.CheckPossibilityToAccept(UseReadyToAcceptState);
            }
            else
            {
                // При проверке прав необходимо убедиться, что "следующий шаг" для накладной проводка.
                ValidationUtils.Assert(UseReadyToAcceptState ? waybill.IsReadyToAccept : waybill.IsNew,
                    String.Format("Невозможно провести накладную из состояния «{0}».", waybill.State.GetDisplayName()));
            }
        }

        #endregion

        #region Отмена проводки

        public bool IsPossibilityToCancelAcceptance(ChangeOwnerWaybill waybill, User user)
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

        public void CheckPossibilityToCancelAcceptance(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Acceptance_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelAcceptance();
        }

        #endregion

        #region  Смена получателя

        public bool IsPossibilityToChangeRecipient(ChangeOwnerWaybill waybill, User user)
        {
            try
            {
                CheckPossibilityToChangeRecipient(waybill, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangeRecipient(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Recipient_Change);

            // сущность
            waybill.CheckPossibilityToChangeRecipient();

            // Проверяем наличие резерва по позициям накладной 
            // (проверка реализуется в сервисе, т.к. в сущности это будет проход по массиву в цикле, что сгенерирует кучу запросов в БД)
            var count = changeOwnerWaybillRepository.Query<ChangeOwnerWaybillRow>()
                .Where(x => x.AcceptedCount > 0 || x.ShippedCount > 0 || x.FinallyMovedCount > 0 || x.UsageAsManualSourceCount > 0)
                .Restriction<ChangeOwnerWaybill>(x => x.ChangeOwnerWaybill)
                    .Where(x => x.Id == waybill.Id)
                .CountDistinct();   // Получаем количество позиций накладной, по которым товар зарезервирован
            ValidationUtils.Assert(count == 0, "Невозможно изменить получателя для накладной, по которой имеется отгрузка.");
        }

        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(ChangeOwnerWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_List_Details);

            // сущность
            ValidationUtils.Assert(waybill.IsAccepted, "Невозможно распечатать форму, т.к. накладная еще не проведена.");
        }

        #region Печать форм в УЦ отправителя

        public bool IsPossibilityToPrintFormInSenderAccountingPrices(ChangeOwnerWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInSenderAccountingPrices, waybill, user);
        }

        public void CheckPossibilityToPrintFormInSenderAccountingPrices(ChangeOwnerWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermissionToViewStorageAccountingPrices(waybill.Storage),
                "Недостаточно прав для просмотра учетных цен отправителя.");
        }
        #endregion

        #region Печать форм в ЗЦ

        public bool IsPossibilityToPrintFormInPurchaseCosts(ChangeOwnerWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintFormInPurchaseCosts, waybill, user);
        }

        public void CheckPossibilityToPrintFormInPurchaseCosts(ChangeOwnerWaybill waybill, User user)
        {
            CheckPossibilityToPrintForms(waybill, user);

            ValidationUtils.Assert(user.HasPermission(Permission.PurchaseCost_View_ForEverywhere),
                "Недостаточно прав для просмотра закупочных цен.");
        }
        #endregion

        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(ChangeOwnerWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.ChangeOwnerWaybill_Curator_Change);

            //сущность
            waybill.CheckPossibilityToChangeCurator();
        }
        #endregion

        #endregion

        #endregion
    }
}