using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProviderService : IProviderService
    {
        #region Поля

        private readonly IProviderRepository providerRepository;
        private readonly IProviderContractRepository providerContractRepository;
        private readonly IStorageService storageService;
        private readonly ITaskRepository taskRepository;

        #endregion

        #region Конструктор

        public ProviderService(IProviderRepository providerRepository, IProviderContractRepository providerContractRepository, IStorageService storageService,
            ITaskRepository taskRepository)
        {
            this.providerRepository = providerRepository;
            this.providerContractRepository = providerContractRepository;

            this.storageService = storageService;
            this.taskRepository = taskRepository;
        }
        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение поставщика по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Provider CheckProviderExistence(int id, string message = "")
        {
            var provider = providerRepository.GetById(id);
            ValidationUtils.NotNull(provider, String.IsNullOrEmpty(message) ? "Поставщик не найден. Возможно, он был удален." : message);

            return provider;
        }
        #endregion

        #region Список

        public IEnumerable<Provider> GetList(User user)
        {
            if (user.HasPermission(Permission.Provider_List_Details))
            {
                return providerRepository.GetAll();
            }

            return new List<Provider>();
        }

        public IList<Provider> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return providerRepository.GetFilteredList(state, ignoreDeletedRows);
        }
        #endregion

        public void Save(Provider provider)
        {
            providerRepository.Save(provider);
        }

        #region Добавление / удаление организации

        public void AddContractorOrganization(Provider provider, ProviderOrganization providerOrganization)
        {
            if (providerRepository.Query<AccountOrganization>().Where(x => x.Id == providerOrganization.Id).Count() > 0)
            {
                throw new Exception("Организация не является организацией, доступной для поставщиков. Она включена в список собственных организаций.");
            }

            provider.AddContractorOrganization(providerOrganization);
        }

        public void RemoveProviderOrganization(Provider provider, ProviderOrganization providerOrganization)
        {
            CheckPossibilityToRemoveProviderOrganization(providerOrganization, provider);

            provider.RemoveContractorOrganization(providerOrganization);

            providerRepository.Save(provider);
        }

        private void CheckPossibilityToRemoveProviderOrganization(ProviderOrganization providerOrganization, Provider provider)
        {
            int contractCount = providerOrganization.Contracts.Count(x => x.Contractors.Any(z => z.Id == provider.Id));
            if (contractCount > 0)
            {
                throw new Exception(String.Format("Невозможно удалить организацию поставщика. Для данного поставщика существуют договоры с ее участием в количестве {0}.",
                    contractCount));
            }
        }
        #endregion

        #region Добавление / удаление договора

        public void AddProviderContract(Provider provider, ProviderContract providerContract)
        {
            provider.AddProviderContract(providerContract);

            providerContractRepository.Save(providerContract);
        }

        public void DeleteProviderContract(Provider provider, ProviderContract providerContract, User user)
        {
            CheckPossibilityToDeleteContract(provider, providerContract, user);

            provider.RemoveProviderContract(providerContract);

            // TODO: если будет возможность связать с одним договором несколько поставщиков, удалять его совсем только после последнего (Providers.Count == 0)
            providerContractRepository.Delete(providerContract);
        }

        #endregion

        #region Удаление

        public void Delete(Provider provider, User user)
        {
            user.CheckPermission(Permission.Provider_Delete);
            CheckPossibilityToDelete(provider);

            var contractList = new List<Contract>(provider.Contracts);
            foreach (Contract contract in contractList)
            {
                ProviderContract providerContract = contract.As<ProviderContract>();

                provider.RemoveProviderContract(providerContract);

                providerContractRepository.Delete(providerContract);
            }

            var contractorOrganizationList = new List<ContractorOrganization>(provider.Organizations);
            foreach (ContractorOrganization contractorOrganization in contractorOrganizationList)
            {
                ProviderOrganization providerOrganization = contractorOrganization.As<ProviderOrganization>();

                provider.RemoveContractorOrganization(providerOrganization);
            }

            providerRepository.Delete(provider);
        }

        /// <summary>
        /// Проверка возможности удаления поставщика
        /// </summary>
        /// <param name="provider">Поставщик</param>
        private void CheckPossibilityToDelete(Provider provider)
        {
            var contractIdList = provider.Contracts.ToList().ConvertAll<short>(x => x.Id);

            // TODO: если будет возможность связать с одним договором несколько поставщиков, то можно будет
            // удалить поставщика, даже если по некоторым договорам есть накладные

            int receiptWaybillCount = providerRepository.Query<ReceiptWaybill>().Where(x => x.Provider.Id == provider.Id).Count();
            if (receiptWaybillCount > 0)
            {
                throw new Exception(String.Format("Невозможно удалить поставщика, так как для него созданы приходные накладные в количестве {0} шт.",
                    receiptWaybillCount));
            }

            var countOfLinkedTask = taskRepository.GetTaskCountForContractor(provider.Id);
            ValidationUtils.Assert(countOfLinkedTask == 0, String.Format("Невозможно удалить поставщика, так как с ним связаны мероприятия и задачи в количестве {0} шт.", countOfLinkedTask));
        }
        #endregion     

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(Provider provider, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(Provider provider, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(provider, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Редактирование организаций договора

        /// <summary>
        /// Можно ли изменять организации, указанные в договоре
        /// </summary>
        public bool IsPossibilityToEditOrganization(Provider provider, ProviderContract contract, User user)
        {
            try
            {
                CheckPossibilityToEditOrganization(provider, contract, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditOrganization(Provider provider, ProviderContract contract, User user)
        {
            // права
            CheckPermissionToPerformOperation(provider, user, Permission.Provider_ProviderContract_Edit);

            ValidationUtils.Assert(!providerContractRepository.AnyReceipts(contract), "Невозможно редактировать организации в договоре, по которому уже есть приходы.");
        }

        #endregion

        #region Удаление договора

        /// <summary>
        /// Можно ли удалить договор
        /// </summary>
        /// <param name="deal"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsPossibilityToDeleteContract(Provider provider, ProviderContract contract, User user)
        {
            try
            {
                CheckPossibilityToDeleteContract(provider, contract, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка возможности удаления договора
        /// </summary>
        /// <param name="providerContract">код договора</param>
        /// <param name="provider">код поставщика, которому принадлежит договор</param>
        public void CheckPossibilityToDeleteContract(Provider provider, ProviderContract providerContract, User user)
        {
            // права
            CheckPermissionToPerformOperation(provider, user, Permission.Provider_ProviderContract_Delete);

            ValidationUtils.Assert(!providerContractRepository.AnyReceipts(providerContract), "Невозможно удалить договор, так как по нему существуют приходные накладные.");
        }

        #endregion

        #endregion

        #endregion
    }
}
