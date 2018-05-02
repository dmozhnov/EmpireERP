using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;

namespace ERP.Wholesale.ApplicationServices
{
    public class StorageService : IStorageService
    {
        #region Поля

        private readonly IStorageRepository storageRepository;
        private readonly ITeamRepository teamRepository;
        private readonly IArticleAvailabilityService articleAvailabilityService;        
        private readonly ISettingRepository settingRepository;

        #endregion

        #region Конструктор

        public StorageService(IStorageRepository storageRepository, ITeamRepository teamRepository, ISettingRepository settingRepository)
        {
            this.storageRepository = storageRepository;
            this.teamRepository = teamRepository;
            this.articleAvailabilityService = IoCContainer.Resolve<IArticleAvailabilityService>();
            this.settingRepository = settingRepository;
        }
        #endregion

        #region Методы

        #region Получение одного места хранения

        // TODO: убрать после реализации прав
        public Storage GetById(short id)
        {
            return storageRepository.GetById(id);
        }

        private Storage GetById(short id, User user, Permission permission)
        {
            var type = user.GetPermissionDistributionType(permission);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var storage = storageRepository.GetById(id);

                if (((type == PermissionDistributionType.Teams || type == PermissionDistributionType.Personal) && user.Teams.SelectMany(x => x.Storages).Contains(storage)) ||
                    type == PermissionDistributionType.All)
                {
                    return storage;
                }

                return null;
            }
        }

        public IEnumerable<Storage> GetStoragesByType(StorageType storageType, User user, Permission permission)
        {
            return FilterByUser(storageRepository.GetStoragesByType(storageType), user, permission);
        }

        /// <summary>
        /// Получение места хранения по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Storage CheckStorageExistence(short id, User user, string message = "")
        {
            return CheckStorageExistence(id, user, Permission.Storage_List_Details, message);
        }

        public Storage CheckStorageExistence(short id, User user, Permission permission, string message = "")
        {
            var storage = GetById(id, user, permission);
            ValidationUtils.NotNull(storage, String.IsNullOrEmpty(message) ? "Место хранения не найдено. Возможно, оно было удалено." : message);

            return storage;
        }

        /// <summary>
        /// Получение места хранения по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Storage CheckStorageExistence(short id, string message = "")
        {
            var storage = storageRepository.GetById(id);
            ValidationUtils.NotNull(storage, String.IsNullOrEmpty(message) ? "Место хранения не найдено. Возможно, оно было удалено." : message);

            return storage;
        }
        #endregion

        #region Список мест хранения

        public IEnumerable<Storage> GetList(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Storage>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.Storages).Distinct();

                case PermissionDistributionType.All:
                    return storageRepository.GetAll();

                default:
                    return null;
            }
        }

        public IEnumerable<Storage> FilterByUser(IEnumerable<Storage> list, User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Storage>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.Storages).Intersect(list).Distinct();

                case PermissionDistributionType.All:
                    return list;

                default:
                    return null;
            }
        }

        public IEnumerable<Storage> GetList()
        {
            return storageRepository.GetAll();
        }

        /// <summary>
        /// Получение списка мест хранения по Id
        /// </summary>        
        public Dictionary<short, Storage> GetList(IEnumerable<short> idList)
        {
            var result = new Dictionary<short, Storage>();
            var listToLoad = new List<short>();

            for (int i = 1; i <= idList.Count(); i++)
            {
                listToLoad.Add(idList.ElementAt(i - 1));

                // делаем выборку 100 строк
                if (i % 100 == 0)
                {
                    result = result.Concat(storageRepository.Query<Storage>().OneOf(x => x.Id, listToLoad).ToList<Storage>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);

                    listToLoad.Clear();
                }
            }

            // добавляем оставшиеся
            result = result.Concat(storageRepository.Query<Storage>().OneOf(x => x.Id, listToLoad).ToList<Storage>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        /// <summary>
        /// Получение списка мест хранения по Id с проверкой существования и прав
        /// </summary>        
        public Dictionary<short, Storage> CheckStorageListExistence(IEnumerable<short> idList, User user, Permission permission, string message = "")
        {
            var type = user.GetPermissionDistributionType(permission);
            if (type == PermissionDistributionType.None)
            {   // если права нет - выдаем ошибку
                throw new Exception(String.IsNullOrEmpty(message) ? "Место хранения не найдено. Возможно, оно было удалено." : message);
            }

            var rawStorages = GetList(idList);
            if (type == PermissionDistributionType.All)
            {   //Права есть - выдаем все
                return rawStorages;
            }

            //проверим существование для каждого переданного ID 
            foreach (var storageId in idList)
            {
                if (rawStorages.ContainsKey(storageId))
                {
                    var storage = rawStorages[storageId];
                    if (!((type == PermissionDistributionType.Teams || type == PermissionDistributionType.Personal) && user.Teams.SelectMany(x => x.Storages).Contains(storage)))
                    {   //нет прав на место хранения
                        throw new Exception(String.IsNullOrEmpty(message) ? "Место хранения не найдено. Возможно, оно было удалено." : message);
                    }
                }
                else
                {   //был передан id несуществующего или удаленного МХ
                    throw new Exception(String.IsNullOrEmpty(message) ? "Место хранения не найдено. Возможно, оно было удалено." : message);
                }
            }

            return rawStorages;
        }

        public IEnumerable<Storage> GetFilteredList(object state, User user, ParameterString param = null)
        {
            if (param == null)
            {
                param = new ParameterString("");
            }

            switch (user.GetPermissionDistributionType(Permission.Storage_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<Storage>();

                case PermissionDistributionType.Teams:
                    var list = user.Teams.SelectMany(x => x.Storages).Select(x => x.Id.ToString()).Distinct().ToList();

                    // если список пуст - то добавляем несуществующее значение
                    if (!list.Any()) { list.Add("0"); }

                    if (param.Keys.Contains("Id"))
                    {
                        param["Id"].Value = list.Intersect(param["Id"].Value as IList<string>);
                    }
                    else
                    {
                        param.Add("Id", ParameterStringItem.OperationType.OneOf, list);
                    }
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return storageRepository.GetFilteredList(state, param);
        }

        #endregion

        #region Редактирование, удаление

        public void Save(Storage storage, User user)
        {
            if (!IsNameUnique(storage.Name, storage.Id)) 
            {
                throw new Exception("Место хранения с таким названием уже существует.");
            }

            // признак необходимости добавления места хранения в команды текущего пользователя 
            bool needAddToTeams = false;
            if (storage.Id == 0)
            {
                needAddToTeams = true;
            }

            storageRepository.Save(storage);

            // при добавлении места хранения добавляем его во все команды пользователя
            if (needAddToTeams)
            {
                foreach (var team in user.Teams)
                {
                    team.AddStorage(storage);
                }
            }
        }

        public bool IsNameUnique(string name, short storageId)
        {
            return storageRepository.IsNameUnique(name, storageId);
        }

        public bool IsSectionNameUnique(string name, short sectionId, short storageId)
        {
            return storageRepository.IsSectionNameUnique(name, sectionId, storageId);
        }

        public void Delete(Storage storage, User user)
        {
            CheckPossibilityToDelete(storage, user);

            storage.DeletionDate = DateTime.Now;
            storageRepository.Delete(storage);

            // отвязываем собственные организации от места хранения
            var organizationList = storage.AccountOrganizations.ToList();
            foreach (var organization in organizationList)
            {
                organization.RemoveStorage(storage);
            }

            // удаляем место хранения из области видимости команды
            foreach (var team in teamRepository.GetAll())
            {
                if (team.Storages.Contains(storage))
                {
                    team.RemoveStorage(storage);
                }
            }
        }

        #endregion

        #region Секции

        public void AddSection(Storage storage, StorageSection section, User user)
        {
            CheckPossibilityToCreateAndEditSection(storage, user);

            ValidationUtils.Assert(storageRepository.IsSectionNameUnique(section.Name, section.Id, storage.Id),
                "Секция с таким названием уже существует.");

            storage.AddSection(section);

            storageRepository.Save(storage);
        }

        public void DeleteSection(Storage storage, StorageSection section, User user)
        {
            CheckPossibilityToDeleteSection(storage, user);

            storage.RemoveSection(section);

            storageRepository.Save(storage);
        }

        #endregion

        #region Связанные организации

        public void AddAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user)
        {
            CheckPossibilityToAddAccountOrganization(storage, user);

            storage.AddAccountOrganization(accountOrganization);
        }

        public void RemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user)
        {
            CheckPossibilityToRemoveAccountOrganization(storage, accountOrganization, user);
            CheckPossibilityToDeleteAccountOrganizationToStorageLink(accountOrganization, storage);

            storage.RemoveAccountOrganization(accountOrganization);
        }

        #endregion

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(Storage storage, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    result = user.Teams.SelectMany(x => x.Storages).Contains(storage);
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(Storage storage, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(storage, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(Storage storage, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(storage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(Storage storage, User user)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_List_Details);
        }
        #endregion

        #region Создание

        public bool IsPossibilityToCreate(User user)
        {
            try
            {
                CheckPossibilityToCreate(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreate(User user)
        {
            user.CheckPermission(Permission.Storage_Create);

            // для SaaS-версии проверяем максимальное кол-во мест хранения
            if (AppSettings.IsSaaSVersion)
            {
                var storageCountLimit = settingRepository.Get().StorageCountLimit;
                ValidationUtils.Assert(storageRepository.GetAll().Count() < storageCountLimit, 
                    String.Format("Невозможно создать место хранения, т.к. их количество для данного аккаунта ограничено {0} шт.", storageCountLimit));
            }
        }
        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(Storage storage, User user)
        {
            try
            {
                CheckPossibilityToEdit(storage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(Storage storage, User user)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_Edit);
        }
        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(Storage storage, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToDelete(storage, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(Storage storage, User user, bool checkLogic = true)
        {
            // права
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_Delete);

            // логика
            if (checkLogic)
            {
                List<ReceiptWaybill> receiptWaybillList = storageRepository.Query<ReceiptWaybill>()
                .Where(x => x.ReceiptStorage.Id == storage.Id)
                .ToList<ReceiptWaybill>().ToList();

                GetExceptionString(receiptWaybillList, 3, "приходной накладной", "и еще в {0} приходных накладных");

                var senderSubquery = storageRepository.SubQuery<MovementWaybill>().Where(y => y.SenderStorage.Id == storage.Id).Select(x => x.SenderStorage);
                var recipientSubquery = storageRepository.SubQuery<MovementWaybill>().Where(y => y.RecipientStorage.Id == storage.Id).Select(x => x.RecipientStorage);

                var movementWaybillList = storageRepository.Query<MovementWaybill>()
                    .Or(x => x.PropertyIn(y => y.SenderStorage, senderSubquery), x => x.PropertyIn(y => y.RecipientStorage, recipientSubquery))
                    .ToList<MovementWaybill>();

                GetExceptionString(movementWaybillList, 3, "накладной перемещения", "и еще в {0} накладных перемещения");

                var writeoffWaybillList = storageRepository.Query<WriteoffWaybill>()
                   .Where(y => y.SenderStorage.Id == storage.Id)
                   .ToList<WriteoffWaybill>();

                GetExceptionString(writeoffWaybillList, 3, "накладной списания", "и еще в {0} накладных списания");

                var expenditureWaybillList = storageRepository.Query<ExpenditureWaybill>()
                   .Where(y => y.SenderStorage.Id == storage.Id)
                   .ToList<ExpenditureWaybill>();

                GetExceptionString(writeoffWaybillList, 3, "накладной реализации", "и еще в {0} накладных реализации");

                var changeOwnerWaybillList = storageRepository.Query<ChangeOwnerWaybill>()
                    .Where(y => y.Storage.Id == storage.Id)
                    .ToList<ChangeOwnerWaybill>();

                GetExceptionString(movementWaybillList, 3, "накладной смены собственника", "и еще в {0} накладных смены собственника");
            }
        }

        /// <summary>
        /// Сформировать и выбросить исключение, перечисляющее, в каких накладных используется место хранения
        /// Текст исключения имеет вид "Невозможно удалить место хранения, так как оно используется в {0} №x, {0} №y {1}"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="waybillList">Список накладных</param>
        /// <param name="maxNum">Количество накладных, чьи номера выводим в тексте исключения</param>
        /// <param name="entityIterateName">Строка 0</param>
        /// <param name="entityTerminateNameFormat">Формат строки 1, куда вместо {0} будет подставлено количество неперечисленных накладных</param>
        private void GetExceptionString<T>(IEnumerable<T> waybillList, int maxNum, string entityIterateName, string entityTerminateNameFormat) where T : BaseWaybill
        {
            if (waybillList.Count() > 0)
            {
                string exceptionString = "Невозможно удалить место хранения, так как оно используется в";
                bool useComma = false;

                int waybillCount = 0;
                foreach (T waybill in waybillList)
                {
                    if (useComma)
                        exceptionString += ",";
                    exceptionString += " " + entityIterateName + " №" + waybill.Number;
                    useComma = true;

                    if (++waybillCount >= maxNum) break;
                }

                if (waybillList.Count() > maxNum)
                    exceptionString += " " + String.Format(entityTerminateNameFormat, waybillList.Count() - maxNum);

                exceptionString += ".";

                throw new Exception(exceptionString);
            }
        }
        #endregion

        #region Добавление связанной организации

        public bool IsPossibilityToAddAccountOrganization(Storage storage, User user)
        {
            try
            {
                CheckPossibilityToAddAccountOrganization(storage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddAccountOrganization(Storage storage, User user)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_AccountOrganization_Add);
        }
        #endregion

        #region Удаление связанной организации

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="accountOrganization"></param>
        /// <param name="user"></param>
        /// <param name="checkLogic">Флаг проверки бизнес-логикой возможность удаления связи места хранения и организации.
        /// При значении false будет выполнена только проверка прав.</param>
        public bool IsPossibilityToRemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToRemoveAccountOrganization(storage, accountOrganization, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="accountOrganization"></param>
        /// <param name="user"></param>
        /// <param name="checkLogic">Флаг проверки бизнес-логикой возможности удаления связи места хранения и организации.
        /// При значении false будет выполнена только проверка прав.</param>
        public void CheckPossibilityToRemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_AccountOrganization_Remove);

            if (checkLogic)
            {
                // Проверяем наличие товаров данной организации на данном складе
                var IsAvailability = articleAvailabilityService.IsExtendedArticleAvailability(storage.Id, accountOrganization.Id, DateTime.Now);
                ValidationUtils.Assert(IsAvailability == false, string.Format("Невозможно удалить связь. На месте хранения «{0}» имеются товары, принадлежащие организации «{1}».",
                    storage.Name, accountOrganization.ShortName));
            }
        }

        /// <summary>
        /// Проверка на возможность удаления
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        private void CheckPossibilityToDeleteAccountOrganizationToStorageLink(AccountOrganization organization, Storage storage)
        {
            List<ReceiptWaybill> receiptWaybillList = storageRepository.Query<ReceiptWaybill>()
              .Where(x => x.AccountOrganization.Id == organization.Id && x.ReceiptStorage.Id == storage.Id)
              .ToList<ReceiptWaybill>().ToList();

            GetExceptionString(receiptWaybillList, 3, "приходной накладной", "и еще в {0} приходных накладных",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var movementSenderSubquery = storageRepository.SubQuery<MovementWaybill>()
                .Where(y => y.Sender.Id == organization.Id && y.SenderStorage.Id == storage.Id).Select(x => x.Id);
            var movementRecipientSubquery = storageRepository.SubQuery<MovementWaybill>()
                .Where(y => y.Recipient.Id == organization.Id && y.RecipientStorage.Id == storage.Id).Select(x => x.Id);

            List<MovementWaybill> movementWaybillList = storageRepository.Query<MovementWaybill>()
                .Or(x => x.PropertyIn(y => y.Id, movementSenderSubquery), x => x.PropertyIn(y => y.Id, movementRecipientSubquery))
                .ToList<MovementWaybill>().Distinct().ToList();

            GetExceptionString(movementWaybillList, 3, "накладной перемещения", "и еще в {0} накладных перемещения",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            List<WriteoffWaybill> writeoffWaybillList = storageRepository.Query<WriteoffWaybill>()
               .Where(y => y.Sender.Id == organization.Id && y.SenderStorage.Id == storage.Id)
               .ToList<WriteoffWaybill>().ToList();

            GetExceptionString(writeoffWaybillList, 3, "накладной списания", "и еще в {0} накладных списания",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            List<ExpenditureWaybill> expenditureWaybillList = storageRepository.Query<ExpenditureWaybill>()
               .Where(x => x.SenderStorage.Id == storage.Id)
               .Restriction<Deal>(x => x.Deal)
               .Restriction<Contract>(x => x.Contract)
               .Where(y => y.AccountOrganization.Id == organization.Id)
               .ToList<ExpenditureWaybill>().ToList();

            GetExceptionString(expenditureWaybillList, 3, "накладной реализации", "и еще в {0} накладных реализации",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            List<ReturnFromClientWaybill> returnFromClientWaybillList = storageRepository.Query<ReturnFromClientWaybill>()
               .Where(x => x.RecipientStorage.Id == storage.Id)
               .Restriction<Deal>(x => x.Deal)
               .Restriction<Contract>(x => x.Contract)
               .Where(y => y.AccountOrganization.Id == organization.Id)
               .ToList<ReturnFromClientWaybill>().ToList();

            GetExceptionString(returnFromClientWaybillList, 3, "накладной возврата от клиента", "и еще в {0} накладных возврата от клиента",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var changeOwnerSenderSubquery = storageRepository.SubQuery<ChangeOwnerWaybill>()
                .Where(y => y.Sender.Id == organization.Id && y.Storage.Id == storage.Id).Select(x => x.Id);
            var changeOwnerRecipientSubquery = storageRepository.SubQuery<ChangeOwnerWaybill>()
                .Where(y => y.Recipient.Id == organization.Id && y.Storage.Id == storage.Id).Select(x => x.Id);

            List<ChangeOwnerWaybill> changeOwnerWaybillList = storageRepository.Query<ChangeOwnerWaybill>()
                .Or(x => x.PropertyIn(y => y.Id, changeOwnerSenderSubquery), x => x.PropertyIn(y => y.Id, changeOwnerRecipientSubquery))
                .ToList<ChangeOwnerWaybill>().Distinct().ToList();

            GetExceptionString(changeOwnerWaybillList, 3, "накладной смены собственника", "и еще в {0} накладных смены собственника",
                "Невозможно удалить связанное место хранения, так как оно используется в");
        }

        /// <summary>
        /// Сформировать и выбросить исключение, перечисляющее, в каких накладных используется место хранения
        /// Текст исключения имеет вид "Невозможно удалить организацию, так как она используется в {0} №x, {0} №y {1}"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="waybillList">Список накладных</param>
        /// <param name="maxNum">Количество накладных, чьи номера выводим в тексте исключения</param>
        /// <param name="entityIterateName">Строка 0</param>
        /// <param name="entityTerminateNameFormat">Формат строки 1, куда вместо {0} будет подставлено количество неперечисленных накладных</param>
        private void GetExceptionString<T>(List<T> waybillList, int maxNum, string entityIterateName, string entityTerminateNameFormat,
            string exceptionString = "Невозможно удалить организацию, так как она используется в") where T : BaseWaybill
        {
            if (waybillList.Count > 0)
            {
                bool useComma = false;

                int waybillCount = 0;
                foreach (T waybill in waybillList)
                {
                    if (useComma)
                        exceptionString += ",";
                    exceptionString += " " + entityIterateName + " №" + waybill.Number;
                    useComma = true;

                    if (++waybillCount >= maxNum) break;
                }

                if (waybillList.Count > maxNum)
                    exceptionString += " " + String.Format(entityTerminateNameFormat, waybillList.Count - maxNum);

                exceptionString += ".";

                throw new Exception(exceptionString);
            }
        }

        #endregion

        #region Добавление/редактирование секции

        public bool IsPossibilityToCreateAndEditSection(Storage storage, User user)
        {
            try
            {
                CheckPossibilityToCreateAndEditSection(storage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreateAndEditSection(Storage storage, User user)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_Section_Create_Edit);
        }
        #endregion

        #region Удаление секции

        public bool IsPossibilityToDeleteSection(Storage storage, User user)
        {
            try
            {
                CheckPossibilityToDeleteSection(storage, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDeleteSection(Storage storage, User user)
        {
            CheckPermissionToPerformOperation(storage, user, Permission.Storage_Section_Delete);
        }
        #endregion


        #endregion

        #endregion
    }
}