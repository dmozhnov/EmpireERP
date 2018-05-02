using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.IoC;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProviderOrganizationService : IProviderOrganizationService
    {
        #region Поля

        private readonly IProviderOrganizationRepository providerOrganizationRepository;
        private readonly IArticlePriceService articlePriceService;
        private readonly IStorageService storageService;
        private readonly IOrganizationService organizationService;
        private readonly IArticlePurchaseService articlePurchaseService;

        #endregion

        #region Конструкторы

        public ProviderOrganizationService(IProviderOrganizationRepository providerOrganizationRepository, 
            IStorageService storageService, IOrganizationService organizationService, IArticlePurchaseService articlePurchaseService)
        {
            this.providerOrganizationRepository = providerOrganizationRepository;

            this.articlePriceService = IoCContainer.Resolve<IArticlePriceService>();

            this.organizationService = organizationService;
            this.storageService = storageService;
            this.articlePurchaseService = articlePurchaseService;
        }

        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение организации поставщика по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProviderOrganization CheckProviderOrganizationExistence(int id, string message = "")
        {
            var providerOrganization = providerOrganizationRepository.GetById(id);
            ValidationUtils.NotNull(providerOrganization, String.IsNullOrEmpty(message) ? "Организация поставщика не найдена. Возможно, она была удалена." : message);

            return providerOrganization;
        }
        #endregion

        public void Save(ProviderOrganization providerOrganization)
        {
            organizationService.CheckOrganizationUniqueness(providerOrganization);

            providerOrganizationRepository.Save(providerOrganization);
        }

        public IEnumerable<ProviderOrganization> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return providerOrganizationRepository.GetFilteredList(state, parameterString, ignoreDeletedRows);
        }

        public void DeleteRussianBankAccount(ProviderOrganization providerOrganization, RussianBankAccount bankAccount)
        {
            CheckRussianBankAccountDeletionPossibility(providerOrganization, bankAccount);

            providerOrganization.DeleteRussianBankAccount(bankAccount);

            providerOrganizationRepository.Save(providerOrganization);
        }

        public void DeleteForeignBankAccount(ProviderOrganization providerOrganization, ForeignBankAccount bankAccount)
        {
            CheckForeignBankAccountDeletionPossibility(providerOrganization, bankAccount);

            providerOrganization.DeleteForeignBankAccount(bankAccount);

            providerOrganizationRepository.Save(providerOrganization);
        }

        public void Delete(ProviderOrganization providerOrganization, User user)
        {
            user.CheckPermission(Permission.ProviderOrganization_Delete);

            CheckPossibilityToDeleteProviderOrganization(providerOrganization, user);

            // Удаляем все расчетные счета из организации
            var bankAccountList = new List<RussianBankAccount>(providerOrganization.RussianBankAccounts);
            foreach (var bankAccount in bankAccountList)
            {
                providerOrganization.DeleteRussianBankAccount(bankAccount);
            }

            // Удаляем организацию из всех поставщиков, где она фигурировала
            var providerOrganizationContractorList = new List<Contractor>(providerOrganization.Contractors);
            foreach (var contractor in providerOrganizationContractorList)
            {
                Provider provider = contractor.As<Provider>();
                provider.RemoveContractorOrganization(providerOrganization);
            }

            // Удаляем саму организацию
            providerOrganizationRepository.Delete(providerOrganization);
        }

        /// <summary>
        /// Проверка возможности удаления расчетного счета, принадлежащего организации поставщика
        /// </summary>
        /// <param name="providerOrganization">Организация поставщика, которой принадлежит расчетный счет</param>
        /// <param name="bankAccount">Расчетный счет</param>
        private void CheckRussianBankAccountDeletionPossibility(ProviderOrganization providerOrganization, RussianBankAccount bankAccount)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета, принадлежащего организации поставщика
            //organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }

        /// <summary>
        /// Проверка возможности удаления расчетного счета, принадлежащего организации поставщика
        /// </summary>
        /// <param name="providerOrganization">Организация поставщика, которой принадлежит расчетный счет</param>
        /// <param name="bankAccount">Расчетный счет</param>
        private void CheckForeignBankAccountDeletionPossibility(ProviderOrganization providerOrganization, ForeignBankAccount bankAccount)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета, принадлежащего организации поставщика
            //organizationService.CheckBankAccountDeletionPossibility(bankAccount);
        }

        /// <summary>
        /// Проверка возможности удаления организации поставщика
        /// </summary>
        /// <param name="providerOrganization">Организация поставщика</param>
        private void CheckPossibilityToDeleteProviderOrganization(ProviderOrganization providerOrganization, User user)
        {
            // TODO: сделать проверки на возможность удаления расчетного счета, принадлежащего организации поставщика
            // TODO: когда можно будет связывать один договор с несколькими поставщиками, сделать доп. проверки на возможность их удаления
            var contractCount = providerOrganization.ContractCount;
            if (contractCount > 0)
            {
                string message = String.Format("Невозможно удалить организацию поставщика, так как с ней заключены договоры в количестве {0} шт.",
                    contractCount);

                decimal purchaseCostSum = articlePurchaseService.GetTotalPurchaseCostSum(providerOrganization, user);
                if (purchaseCostSum > 0 && user.HasPermission(Permission.PurchaseCost_View_ForEverywhere))
                {
                    message += String.Format(", для которых существуют приходные накладные на сумму {0} р.",
                        purchaseCostSum.ForDisplay(ValueDisplayType.Money));
                }

                throw new Exception(message);
            }
        }       

        #endregion
    }
}
