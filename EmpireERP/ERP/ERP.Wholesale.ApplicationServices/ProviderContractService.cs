using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ProviderContractService : IProviderContractService
    {
        #region Поля

        private readonly IProviderContractRepository providerContractRepository;

        #endregion

        #region Конструкторы

        public ProviderContractService(IProviderContractRepository providerContractRepository)
        {
            this.providerContractRepository = providerContractRepository;
        }

        #endregion

        #region Методы

        public IEnumerable<ProviderContract> GetList()
        {
            return providerContractRepository.GetAll();
        }

        public void Save(ProviderContract providerContract)
        {
            providerContractRepository.Save(providerContract);
        }

        public ProviderContract CheckProviderContractExistence(short id)
        {
            var providerContract = providerContractRepository.GetById(id);
            ValidationUtils.NotNull(providerContract, "Договор с поставщиком не найден. Возможно, он был удален.");

            return providerContract;
        }

        #endregion

    }
}