using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Договор с поставщиком
    /// </summary>
    public class ProviderContract : Contract
    {
        #region Свойства

        #endregion

        #region Конструкторы

        protected ProviderContract() : base()
        {
        }

        public ProviderContract(AccountOrganization accountOrganization, ProviderOrganization providerOrganization, string name, string number, DateTime date, DateTime startDate)
            : base(accountOrganization, name, number, date, startDate)
        {
            if (accountOrganization.Id == providerOrganization.Id)
            {
                throw new Exception("Собственная организация и организация контрагента не могут совпадать.");
            }

            providerOrganization.AddContract(this);
            ContractorOrganization = providerOrganization;
        }

        #endregion
    }
}
