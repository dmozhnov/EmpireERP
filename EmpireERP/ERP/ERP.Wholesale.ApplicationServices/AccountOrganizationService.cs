using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;

namespace ERP.Wholesale.ApplicationServices
{
    public class AccountOrganizationService : IAccountOrganizationService
    {
        #region Поля
        
        private readonly IAccountOrganizationRepository accountOrganizationRepository;
        private readonly ISettingRepository settingRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IMovementWaybillRepository movementWaybillRepository;
        private readonly IChangeOwnerWaybillRepository changeOwnerWaybillRepository;
        private readonly IWriteoffWaybillRepository writeoffWaybillRepository;
        private readonly IExpenditureWaybillRepository expenditureWaybillRepository;
        private readonly IReturnFromClientWaybillRepository returnFromClientWaybillRepository;
        private readonly IContractRepository contractRepository;
        
        private readonly IArticleAvailabilityService articleAvailabilityService;
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        
        #endregion

        #region Конструкторы

        public AccountOrganizationService(IAccountOrganizationRepository accountOrganizationRepository, ISettingRepository settingRepository,
            IReceiptWaybillRepository receiptWaybillRepository, IMovementWaybillRepository movementWaybillRepository, 
            IChangeOwnerWaybillRepository changeOwnerWaybillRepository, IWriteoffWaybillRepository writeoffWaybillRepository, 
            IExpenditureWaybillRepository expenditureWaybillRepository, IReturnFromClientWaybillRepository returnFromClientWaybillRepository,
            IContractRepository contractRepository, IArticleAvailabilityService articleAvailabilityService,
            IOrganizationService organizationService, IUserService userService)
        {
            this.accountOrganizationRepository = accountOrganizationRepository;

            this.settingRepository = settingRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;
            this.movementWaybillRepository = movementWaybillRepository;
            this.changeOwnerWaybillRepository = changeOwnerWaybillRepository;
            this.writeoffWaybillRepository = writeoffWaybillRepository;
            this.expenditureWaybillRepository = expenditureWaybillRepository;
            this.returnFromClientWaybillRepository = returnFromClientWaybillRepository;
            this.contractRepository = contractRepository;

            this.articleAvailabilityService = articleAvailabilityService;
            this.organizationService = organizationService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Получение по Id
        
        /// <summary>
        /// Получение собственной организации по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AccountOrganization CheckAccountOrganizationExistence(int id, string message = "")
        {
            var accountOrganization = accountOrganizationRepository.GetById(id);
            ValidationUtils.NotNull(accountOrganization, String.IsNullOrEmpty(message) ? "Собственная организация не найдена. Возможно, она была удалена." : message);

            return accountOrganization;
        } 
        #endregion

        #region Список
        
        public IEnumerable<AccountOrganization> GetList()
        {
            return accountOrganizationRepository.GetAll();
        }

        /// <summary>
        /// Получение списка собственных организаций по Id
        /// </summary>
        public Dictionary<int, AccountOrganization> GetList(IEnumerable<int> idList)
        {
            var result = new Dictionary<int, AccountOrganization>();
            var listToLoad = new List<int>();

            for (int i = 1; i <= idList.Count(); i++)
            {
                listToLoad.Add(idList.ElementAt(i - 1));

                // делаем выборку 100 строк
                if (i % 100 == 0)
                {
                    result = result.Concat(accountOrganizationRepository.Query<AccountOrganization>()
                        .OneOf(x => x.Id, listToLoad).ToList<AccountOrganization>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);

                    listToLoad.Clear();
                }
            }

            // добавляем оставшиеся
            result = result.Concat(accountOrganizationRepository.Query<AccountOrganization>()
                .OneOf(x => x.Id, listToLoad).ToList<AccountOrganization>().ToDictionary(x => x.Id, x => x)).ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        public IList<AccountOrganization> GetFilteredList(object state, ParameterString param = null)
        {
            if (param == null)
            {
                param = new ParameterString("");
            }

            return accountOrganizationRepository.GetFilteredList(state, param, true);
        }
 
        #endregion

        public int Save(AccountOrganization organization)
        {
            organizationService.CheckOrganizationUniqueness(organization);

            accountOrganizationRepository.Save(organization);

            return organization.Id;
        }

        #region Удаление

        public void Delete(AccountOrganization organization, User user)
        {
            CheckPossibilityToDelete(organization, user);

            foreach (var storage in organization.Storages.ToList())
            {
                storage.RemoveAccountOrganization(organization);
            }

            organization.DeletionDate = DateTime.Now;
            accountOrganizationRepository.Delete(organization);
        }

        /// <summary>
        /// Проверка на возможность удаления
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        private void CheckPossibilityToDelete(AccountOrganization organizationToDelete, User user)
        {
            userService.CheckPermission(user, Permission.AccountOrganization_Delete);
            
            var receiptWaybillList = receiptWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(receiptWaybillList, 3, "приходной накладной", "и еще в {0} приходных накладных");
            
            var movementWaybillList = movementWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(movementWaybillList, 3, "накладной перемещения", "и еще в {0} накладных перемещения");

            var changeOwnerWaybillList = changeOwnerWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(changeOwnerWaybillList, 3, "накладной смены собственника", "и еще в {0} накладных смены собственника");

            var writeoffWaybillList = writeoffWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(writeoffWaybillList, 3, "накладной списания", "и еще в {0} накладных списания");

            var expenditureWaybillList = expenditureWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(expenditureWaybillList, 3, "накладной реализации", "и еще в {0} накладных реализации");

            var returnFromClientWaybillList = returnFromClientWaybillRepository.GetList(organizationToDelete);
            GetExceptionString(returnFromClientWaybillList, 3, "накладной возврата от клиента", "и еще в {0} накладных возврата от клиента");

            // Проверяем наличие договоров с участием организации
            ValidationUtils.Assert(!contractRepository.AnyContracts(organizationToDelete), "Невозможно удалить организацию, так как она используется в договорах.");
        }

        /// <summary>
        /// Проверка на возможность удаления связи "Собственная организация - МХ"
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        private void CheckPossibilityToDeleteAccountOrganizationToStorageLink(AccountOrganization organization, Storage storage)
        {
            // Проверяем наличие товаров данной организации на данном складе
            var isAvailability = articleAvailabilityService.IsExtendedArticleAvailability(storage.Id, organization.Id, DateTime.Now);
            ValidationUtils.Assert(isAvailability == false, string.Format("Невозможно удалить связь. На месте хранения «{0}» имеются товары, принадлежащие организации «{1}».", storage.Name, organization.ShortName));

            var receiptWaybillList = receiptWaybillRepository.GetList(organization, storage);
            GetExceptionString(receiptWaybillList, 3, "приходной накладной", "и еще в {0} приходных накладных",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var movementWaybillList = movementWaybillRepository.GetList(organization, storage);
            GetExceptionString(movementWaybillList, 3, "накладной перемещения", "и еще в {0} накладных перемещения",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var changeOwnerWaybillList = changeOwnerWaybillRepository.GetList(organization, storage);
            GetExceptionString(changeOwnerWaybillList, 3, "накладной смены собственника", "и еще в {0} накладных смены собственника",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var writeoffWaybillList = writeoffWaybillRepository.GetList(organization, storage);
            GetExceptionString(writeoffWaybillList, 3, "накладной списания", "и еще в {0} накладных списания",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var expenditureWaybillList = expenditureWaybillRepository.GetList(organization, storage);
            GetExceptionString(expenditureWaybillList, 3, "накладной реализации", "и еще в {0} накладных реализации",
                "Невозможно удалить связанное место хранения, так как оно используется в");

            var returnFromClientWaybillList = returnFromClientWaybillRepository.GetList(organization, storage);
            GetExceptionString(returnFromClientWaybillList, 3, "накладной возврата от клиента", "и еще в {0} накладных возврата от клиента",
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
        private void GetExceptionString<T>(IEnumerable<T> waybillList, int maxNum, string entityIterateName, string entityTerminateNameFormat,
            string exceptionString = "Невозможно удалить организацию, так как она используется в") where T : BaseWaybill
        {
            if (waybillList.Count() > 0)
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

                if (waybillList.Count() > maxNum)
                    exceptionString += " " + String.Format(entityTerminateNameFormat, waybillList.Count() - maxNum);

                exceptionString += ".";

                throw new Exception(exceptionString);
            }
        }

        #endregion

        #region Места хранения
        
        public void AddStorage(AccountOrganization organization, Storage storage, User user)
        {
            user.CheckStorageAvailability(storage, Permission.Storage_AccountOrganization_Add);
                
            organization.AddStorage(storage);
        }

        public void RemoveStorage(AccountOrganization organization, Storage storage, User user)
        {
            user.CheckStorageAvailability(storage, Permission.Storage_AccountOrganization_Remove);
            // Проверяем накладные данной организации на данном складе
            CheckPossibilityToDeleteAccountOrganizationToStorageLink(organization, storage);

            organization.RemoveStorage(storage);
        }
        
        #endregion

        #region Счета в российских банках

        public void AddRussianBankAccount(AccountOrganization organization, RussianBankAccount bankAccount)
        {
            organization.AddRussianBankAccount(bankAccount);
        }

        public void DeleteRussianBankAccount(AccountOrganization organization, RussianBankAccount bankAccount)
        {
            organization.DeleteRussianBankAccount(bankAccount);
        } 
        #endregion

        #region Счета в иностранных банках

        public void AddForeignBankAccount(AccountOrganization organization, ForeignBankAccount bankAccount)
        {
            organization.AddForeignBankAccount(bankAccount);
        }

        public void DeleteForeignBankAccount(AccountOrganization organization, ForeignBankAccount bankAccount)
        {
            organization.DeleteForeignBankAccount(bankAccount);
        } 
        #endregion

        #region Права на совершение операций

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
            user.CheckPermission(Permission.AccountOrganization_Create);

            // для SaaS-версии проверяем максимальное кол-во собственных организаций
            if (AppSettings.IsSaaSVersion)
            {
                var accountOrganizationCountLimit = settingRepository.Get().AccountOrganizationCountLimit;
                ValidationUtils.Assert(accountOrganizationRepository.GetAll().Count() < accountOrganizationCountLimit, 
                    String.Format("Невозможно создать организацию, т.к. их количество для данного аккаунта ограничено {0} шт.", accountOrganizationCountLimit));
            }
        }
        #endregion

        #endregion

        #endregion
    }
}
