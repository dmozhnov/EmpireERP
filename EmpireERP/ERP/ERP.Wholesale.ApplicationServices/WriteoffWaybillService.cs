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
    public class WriteoffWaybillService : BaseOutgoingWaybillService<WriteoffWaybill>, IWriteoffWaybillService 
    {
        #region Поля

        private readonly ISettingRepository settingRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IStorageRepository storageRepository;
        private readonly IUserRepository userRepository;

        private readonly IArticleMovementService articleMovementService;
        private readonly IArticlePriceService articlePriceService;
        private readonly IFactualFinancialArticleMovementService factualFinancialArticleMovementService;
        private readonly IArticleMovementOperationCountService articleMovementOperationCountService;
        private readonly IReceiptWaybillService receiptWaybillService;

        private readonly IArticleRevaluationService articleRevaluationService;

        #endregion

        #region Конструкторы

        public WriteoffWaybillService(ISettingRepository settingRepository, IWriteoffWaybillRepository writeoffWaybillRepository, IStorageRepository storageRepository, IUserRepository userRepository, IArticleMovementService articleMovementService, IArticlePriceService articlePriceService,                        
            IFactualFinancialArticleMovementService factualFinancialArticleMovementService, IArticleMovementOperationCountService articleMovementOperationCountService,
            IArticleAvailabilityService articleAvailabilityService, IReceiptWaybillService receiptWaybillService, IArticleRevaluationService articleRevaluationService)
            : base(articleAvailabilityService)
        {
            this.settingRepository = settingRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.storageRepository = storageRepository;
            this.userRepository = userRepository;

            this.articleMovementService = articleMovementService;
            this.articlePriceService = articlePriceService;

            this.factualFinancialArticleMovementService = factualFinancialArticleMovementService;
            this.articleMovementOperationCountService = articleMovementOperationCountService;
            this.receiptWaybillService = receiptWaybillService;

            this.articleRevaluationService = articleRevaluationService;
        }

        #endregion

        #region Методы

        #region Получение накладной по Id

        /// <summary>
        /// Получение накладной по Id с учетом прав пользователя
        /// </summary>
        /// <param name="id">Id накладной</param>
        /// <param name="user">Пользователь</param>        
        private WriteoffWaybill GetById(Guid id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.WriteoffWaybill_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var waybill = writeoffWaybillRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return waybill;
                }
                else
                {
                    bool contains = user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage);

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
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override WriteoffWaybill CheckWaybillExistence(Guid id, User user)
        {
            var writeoffWaybill = GetById(id, user);
            ValidationUtils.NotNull(writeoffWaybill, "Накладная списания товаров не найдена. Возможно, она была удалена.");

            return writeoffWaybill;
        }
        #endregion

        #region Список позиций

        /// <summary>
        /// Все списания указанного товара в указанные сроки, у которых МХ-отправитель входит в указанный список
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="storageIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<WriteoffWaybillRow> GetRows(int articleId, IEnumerable<short> storageIds, DateTime startDate, DateTime endDate, bool finallyMovedWaybillsOnly)
        {
            ISubQuery waybillSubQuery;

            if (finallyMovedWaybillsOnly)
            {
                waybillSubQuery = writeoffWaybillRepository.SubQuery<WriteoffWaybill>()
                .OneOf(x => x.SenderStorage.Id, storageIds)
                .Where(x => x.WriteoffDate >= startDate && x.WriteoffDate <= endDate)
                .Select(x => x.Id);
            }
            else
            {
                waybillSubQuery = writeoffWaybillRepository.SubQuery<WriteoffWaybill>()
                .OneOf(x => x.SenderStorage.Id, storageIds)
                .Where(x => x.AcceptanceDate >= startDate && x.AcceptanceDate <= endDate)
                .Select(x => x.Id);
            }

            return writeoffWaybillRepository.Query<WriteoffWaybillRow>()
                .PropertyIn(x => x.WriteoffWaybill, waybillSubQuery)
                .Where(x => x.Article.Id == articleId)
                .ToList<WriteoffWaybillRow>();
        }

        #endregion

        #region Список

        public IList<WriteoffWaybill> GetFilteredList(object state, User user, ParameterString ps)
        {
            switch (user.GetPermissionDistributionType(Permission.WriteoffWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<WriteoffWaybill>();

                case PermissionDistributionType.Personal:
                    ps.Add("Curator", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                    // если область распространения "Только свои", то делаем еще и ограничение по командам пользователя
                    AddSenderStorageParameter(user, ps);
                    break;

                case PermissionDistributionType.Teams:
                    AddSenderStorageParameter(user, ps);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return writeoffWaybillRepository.GetFilteredList(state, ps);
        }

        public IList<WriteoffWaybill> GetFilteredList(object state, User user)
        {
            return GetFilteredList(state, user, new ParameterString(""));
        }

        private void AddSenderStorageParameter(User user, ParameterString param)
        {
            var list = user.Teams.SelectMany(x => x.Storages).Select(x => x.Id.ToString()).Distinct().ToList();

            // если список пуст - то добавляем несуществующее значение
            if (!list.Any()) { list.Add("0"); }

            param.Add("SenderStorage", ParameterStringItem.OperationType.OneOf, list);
        }

       

        #endregion

        #region Report0008

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
        public IEnumerable<WriteoffWaybill> GetList(WriteoffWaybillLogicState logicState, IEnumerable<short> storageIdList, Permission storagePermission, IEnumerable<int> curatorIdList,
            Permission curatorPermission, DateTime startDate, DateTime endDate, int pageNumber, WaybillDateType dateType, DateTime? priorToDate, User user)
        {
            ISubCriteria<Storage> storageSubQuery = null;
            ISubCriteria<User> curatorSubQuery = null;
            ISubCriteria<WriteoffWaybill> writeoffWaybillSubQuery = null;

            switch (user.GetPermissionDistributionType(storagePermission))
            {
                case PermissionDistributionType.All:
                    storageSubQuery = storageRepository.GetStorageSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    storageSubQuery = storageRepository.GetStorageSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<WriteoffWaybill>();
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
                    return new List<WriteoffWaybill>();
            }

            switch (user.GetPermissionDistributionType(Permission.WriteoffWaybill_List_Details))
            {
                case PermissionDistributionType.All:
                    writeoffWaybillSubQuery = writeoffWaybillRepository.GetWriteoffWaybillSubQueryByAllPermission();
                    break;
                case PermissionDistributionType.Teams:
                    writeoffWaybillSubQuery = writeoffWaybillRepository.GetWriteoffWaybillSubQueryByTeamPermission(user.Id);
                    break;
                case PermissionDistributionType.Personal:
                    writeoffWaybillSubQuery = writeoffWaybillRepository.GetWriteoffWaybillSubQueryByPersonalPermission(user.Id);
                    break;
                case PermissionDistributionType.None:
                    return new List<WriteoffWaybill>();
            }

            return writeoffWaybillRepository.GetList(logicState, writeoffWaybillSubQuery, storageIdList, storageSubQuery,
                curatorIdList, curatorSubQuery, startDate, endDate, pageNumber, dateType, priorToDate);
        }

        #endregion

        #region Создание / редактирование

        private bool IsNumberUnique(string number, Guid id, DateTime documentDate, AccountOrganization accountOrganization)
        {
            return writeoffWaybillRepository.IsNumberUnique(number, id, documentDate, accountOrganization);
        }

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="writeoffWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public override void CheckPossibilityToViewDetailsByUser(WriteoffWaybill writeoffWaybill, User curator)
        {
            var storages = curator.Teams.SelectMany(x => x.Storages);
            var result = false;

            switch (curator.GetPermissionDistributionType(Permission.WriteoffWaybill_List_Details))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;
                case PermissionDistributionType.Personal:
                    result = storages.Contains(writeoffWaybill.SenderStorage) && writeoffWaybill.Curator == curator;
                    break;
                case PermissionDistributionType.Teams:
                    result = storages.Contains(writeoffWaybill.SenderStorage);
                    break;
                case PermissionDistributionType.All:
                    result = true;
                    break;
            }
            ValidationUtils.Assert(result, "Куратор накладной должен имееть право на ее просмотр.");
        }

        public void Save(WriteoffWaybill waybill)
        {
            // если номер генерируется автоматически
            if (waybill.Number == "")
            {
                var lastDocumentNumbers = waybill.Sender.GetLastDocumentNumbers(waybill.Date.Year);
                var number = lastDocumentNumbers.WriteoffWaybillLastNumber + 1;

                // пока не найдем уникальный номер
                while (!IsNumberUnique(number.ToString(), Guid.Empty, waybill.Date, waybill.Sender))
                {
                    number = number + 1;
                }

                waybill.Number = number.ToString();
                lastDocumentNumbers.WriteoffWaybillLastNumber = number;
            }
            else
            {
                ValidationUtils.Assert(IsNumberUnique(waybill.Number, waybill.Id, waybill.Date, waybill.Sender),
                String.Format("Накладная с номером {0} уже существует. Укажите другой номер.", waybill.Number));
            }

            writeoffWaybillRepository.Save(waybill);
        }
        #endregion

        #region Удаление

        public void Delete(WriteoffWaybill waybill, User user)
        {
            CheckPossibilityToDelete(waybill, user);

            // удаляем связи с установленными вручную источниками
            articleMovementService.ResetManualSources(waybill);

            writeoffWaybillRepository.Delete(waybill);
        }
        #endregion

        #region Добавление / удаление позиции

        public void AddRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user)
        {
            AddRowLocal(waybill, row, null, user);
        }

        public void AddRow(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            AddRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void AddRowLocal(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            waybill.AddRow(row);

            writeoffWaybillRepository.Save(waybill);

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
        public void AddRowSimply(WriteoffWaybill waybill, Article article, decimal count, User user)
        {
            CheckPossibilityToEdit(waybill, user);

            // распределяем кол-во товара по партиям
            var countDistributionInfo = DistributeCountByBatches(article, waybill.SenderStorage, waybill.Sender, count);

            var batchList = receiptWaybillService.GetRows(countDistributionInfo.Keys);
            foreach (var item in countDistributionInfo)
            {
                var batch = batchList[item.Key];
                var row = new WriteoffWaybillRow(batch, item.Value);

                AddRow(waybill, row, user);
            }
        }

        public void SaveRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user)
        {
            SaveRowLocal(waybill, row, null, user);
        }

        public void SaveRow(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
        {
            SaveRowLocal(waybill, row, sourceDistributionInfo, user);
        }

        private void SaveRowLocal(WriteoffWaybill waybill, WriteoffWaybillRow row, IEnumerable<WaybillRowManualSource> sourceDistributionInfo, User user)
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

            writeoffWaybillRepository.Save(waybill);
        }

        public void DeleteRow(WriteoffWaybill waybill, WriteoffWaybillRow row, User user)
        {
            CheckPossibilityToDeleteRow(row, user);

            if (row.IsUsingManualSource)
            {
                articleMovementService.ResetManualSources(row);
            }

            waybill.DeleteRow(row);
        }
        #endregion

        #region Подготовка / отмена готовности к проводке

        /// <summary>
        /// Подготовка накладной к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void PrepareToAccept(WriteoffWaybill waybill, User user)
        {
            CheckPossibilityToPrepareToAccept(waybill, user);

            waybill.PrepareToAccept();
        }

        /// <summary>
        /// Отменить готовность к проводке
        /// </summary>
        /// <param name="waybill">Накладная</param>
        /// <param name="user">Пользователь</param>
        public void CancelReadinessToAccept(WriteoffWaybill waybill, User user)
        {
            CheckPossibilityToCancelReadinessToAccept(waybill, user);

            waybill.CancelReadinessToAccept();
        }

        #endregion

        #region Проводка / отмена проводки

        /// <summary>
        /// Проводка накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void Accept(WriteoffWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToAccept(waybill, user);

            // получение текущих позиций реестров цен
            var senderPriceLists = articlePriceService.GetArticleAccountingPrices(waybill.SenderStorage.Id,
                writeoffWaybillRepository.GetArticlesSubquery(waybill.Id), currentDateTime);

            // проводим накладную
            waybill.Accept(senderPriceLists, UseReadyToAcceptState, user, currentDateTime);

            // резервирование товаров при проводке
            var reservationInfoList = articleMovementService.AcceptArticles(waybill);

            // Пересчет показателей исходящего проведенного наличия
            articleAvailabilityService.WriteoffWaybillAccepted(waybill, reservationInfoList);
        }

        /// <summary>
        /// Отмена проводки накладной
        /// </summary>
        /// <param name="waybill"></param>
        public void CancelAcceptance(WriteoffWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToCancelAcceptance(waybill, user);

            // отмена резервирования товара при проводке
            var reservationInfoList = articleMovementService.CancelArticleAcceptance(waybill);

            // Пересчет показателей входящего проведенного наличия
            articleAvailabilityService.WriteoffWaybillAcceptanceCanceled(waybill, reservationInfoList,
                articleMovementService.GetIncomingWaybillRowForOutgoingWaybillRow(writeoffWaybillRepository.GetRowsSubQuery(waybill.Id)));

            waybill.CancelAcceptance(UseReadyToAcceptState);

            // удаление связей и пересчет проведенной переоценки
            articleRevaluationService.WriteoffWaybillAcceptanceCancelled(waybill);
        }
        #endregion

        #region Списание / отмена списания

        public void Writeoff(WriteoffWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            CheckPossibilityToWriteoff(waybill, user);

            // списание товара
            waybill.Writeoff(user, currentDateTime);

            // пометка товара как окончательно перемещенного
            articleMovementService.FinallyMoveAcceptedArticles(waybill);

            writeoffWaybillRepository.Save(waybill);

            // Пересчет показателей проведенного исходящего и точного наличия
            articleAvailabilityService.WriteoffWaybillWrittenOff(waybill);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.WriteoffWaybillWrittenOff(waybill);

            // пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalized(waybill);

            // уменьшение показателя точной переоценки
            articleRevaluationService.WriteoffWaybillFinalized(waybill);
        }

        public void CancelWriteoff(WriteoffWaybill waybill, User user, DateTime currentDateTime)
        {
            // регулярная проверка - не появились ли РЦ для переоценки
            articleRevaluationService.CheckAccountingPriceListWithoutCalculatedRevaluation(currentDateTime);

            // Пересчет показателей проведенного исходящего и точного наличия
            articleAvailabilityService.WriteoffWaybillWriteoffCanceled(waybill);

            CheckPossibilityToCancelWriteoff(waybill, user);

            // пересчет финансовых показателей
            factualFinancialArticleMovementService.WriteoffWaybillWriteoffCancelled(waybill);

            // пересчитываем счетчики количеств операций
            articleMovementOperationCountService.WaybillFinalizationCancelled(waybill);

            // увеличение показателя точной переоценки
            articleRevaluationService.WriteoffWaybillFinalizationCancelled(waybill);

            // отменяем списание
            waybill.CancelWriteoff();

            // помечаем товар как проведенный
            articleMovementService.CancelArticleFinalMoving(waybill);
        }
        #endregion

        #region Проверки на возможность совершения операций

        #region Настройки аккаунта

        /// <summary>
        /// Флаг использования статуса "Готовок к проводке"
        /// </summary>
        private bool UseReadyToAcceptState
        {
            get { return settingRepository.Get().UseReadyToAcceptStateForWriteOffWaybill; }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение информации о наличии права на выполнение операции
        /// </summary>        
        private bool IsPermissionToPerformOperation(WriteoffWaybill waybill, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                    result = (waybill.Curator == user) && user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage); // свои + командные
                    break;

                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Storages).Contains(waybill.SenderStorage);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Проверка прав на выполнение операции
        /// </summary>        
        private void CheckPermissionToPerformOperation(WriteoffWaybill waybill, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(waybill, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToEdit, waybill, user);
        }

        public void CheckPossibilityToEdit(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToEdit();
        }
        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToDelete, waybill, user);
        }

        public void CheckPossibilityToDelete(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Delete_Row_Delete);

            // сущность
            waybill.CheckPossibilityToDelete();
        }
        #endregion

        #region Удаление позиции

        public bool IsPossibilityToDeleteRow(WriteoffWaybillRow waybillRow, User user)
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

        public void CheckPossibilityToDeleteRow(WriteoffWaybillRow waybillRow, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybillRow.WriteoffWaybill, user, Permission.WriteoffWaybill_Delete_Row_Delete);

            // сущность
            waybillRow.CheckPossibilityToDelete();
        }
        #endregion

        #region Подготовка к проводке

        public bool IsPossibilityToPrepareToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToPrepareToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false)
        {
            // настройки аккаунта
            ValidationUtils.Assert(UseReadyToAcceptState, "Невозможно выполнить операцию, т.к. опция подготовки накладной к проводке отключена в настройках аккаунта.");

            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Create_Edit);

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

        public bool IsPossibilityToCancelReadinessToAccept(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToCancelReadinessToAccept, waybill, user);
        }

        public void CheckPossibilityToCancelReadinessToAccept(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Create_Edit);

            // сущность
            waybill.CheckPossibilityToCancelReadinessToAccept();
        }

        #endregion

        #region Проводка

        public bool IsPossibilityToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false)
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

        public void CheckPossibilityToAccept(WriteoffWaybill waybill, User user, bool onlyPermission = false)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Accept);

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

        public bool IsPossibilityToCancelAcceptance(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToCancelAcceptance, waybill, user);
        }

        public void CheckPossibilityToCancelAcceptance(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Acceptance_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelAcceptance();
        }

        #endregion

        #region Списание

        public bool IsPossibilityToWriteoff(WriteoffWaybill waybill, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToWriteoff(waybill, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToWriteoff(WriteoffWaybill waybill, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Writeoff);

            if (checkLogic)
            {
                // сущность
                waybill.CheckPossibilityToWriteoff();
            }
        }
        #endregion

        #region Отмена списания

        public bool IsPossibilityToCancelWriteoff(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToCancelWriteoff, waybill, user);
        }

        public void CheckPossibilityToCancelWriteoff(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Writeoff_Cancel);

            // сущность
            waybill.CheckPossibilityToCancelWriteoff();
        }
        #endregion

        #region Возможность печатать формы документов

        public bool IsPossibilityToPrintForms(WriteoffWaybill waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToPrintForms, waybill, user);
        }

        public void CheckPossibilityToPrintForms(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_List_Details);

            // сущность
            if (!waybill.IsWrittenOff)
            {
                throw new Exception("Невозможно распечатать форму, т.к. товар по накладной еще не списан.");
            }
        }
        #endregion

        #region Смена куратора

        public override void CheckPossibilityToChangeCurator(WriteoffWaybill waybill, User user)
        {
            // права
            CheckPermissionToPerformOperation(waybill, user, Permission.WriteoffWaybill_Curator_Change);

            //Сущность
            waybill.CheckPossibilityToChangeCurator();
        }

        #endregion

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Получение статусов позиций накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        public IDictionary<Guid, OutgoingWaybillRowState> GetRowStates(WriteoffWaybill waybill)
        {
            var result = new Dictionary<Guid, OutgoingWaybillRowState>();

            IEnumerable<ArticleBatchAvailabilityShortInfo> articleBatchAvailability = null;

            // если накладная не проведена, то для позиций без ручного указания источников 
            // необходимо найти точное наличие
            if (!waybill.IsAccepted)
            {
                // создаем подзапрос для партий для позиций без ручного указания источников
                var rowWithoutManualSourceBatchSubQuery = writeoffWaybillRepository.GetRowWithoutManualSourceBatchSubquery(waybill.Id);

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

                    outgoingWaybillRowState = (availability != null && availability.Count >= row.WritingoffCount) ?
                        OutgoingWaybillRowState.ReadyToArticleMovement : OutgoingWaybillRowState.ArticlePending;
                }

                result.Add(row.Id, outgoingWaybillRowState);
            }

            return result;
        }

        #endregion

        #endregion

    }
}