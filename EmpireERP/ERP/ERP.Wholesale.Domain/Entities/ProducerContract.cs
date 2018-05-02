using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Контракт с производителем
    /// </summary>
    public class ProducerContract : Contract
    {
        #region Свойства

        #endregion

        #region Конструкторы

        protected ProducerContract() : base()
        {
        }

        public ProducerContract(AccountOrganization accountOrganization, ProducerOrganization producerOrganization, string name, string number, DateTime date, DateTime startDate)
            : base(accountOrganization, name, number, date, startDate)
        {
            if (accountOrganization.Id == producerOrganization.Id)
            {
                throw new Exception("Собственная организация и организация производителя не могут совпадать.");
            }

            producerOrganization.AddContract(this);
            ContractorOrganization = producerOrganization;
        }

        #endregion
    }
}
