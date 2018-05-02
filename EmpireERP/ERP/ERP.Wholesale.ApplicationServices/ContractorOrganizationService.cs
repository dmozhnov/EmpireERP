using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ContractorOrganizationService : IContractorOrganizationService
    {
        #region Поля

        IContractorOrganizationRepository contractorOrganizationRepository;

        #endregion

        #region Конструкторы

        public ContractorOrganizationService(IContractorOrganizationRepository contractorOrganizationRepository)
        {
            this.contractorOrganizationRepository = contractorOrganizationRepository;
        }

        #endregion

        #region Методы

        public IEnumerable<ContractorOrganization> GetFilteredList(object state, ParameterString parameterString)
        {
            return contractorOrganizationRepository.GetFilteredList(state, parameterString);
        }

        #endregion 
    }
}
