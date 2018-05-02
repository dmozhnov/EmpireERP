using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Договор с клиентом
    /// </summary>
    public class ClientContract : Contract
    {
        #region Конструкторы

        protected ClientContract() : base()
        {     
        }

        public ClientContract(AccountOrganization accountOrganization, ClientOrganization clientOrganization, string name, string number, DateTime date, DateTime startDate) :
            base(accountOrganization, name, number, date, startDate)
        {
            if (accountOrganization.Id == clientOrganization.Id)
            {
                throw new Exception("Собственная организация и организация контрагента не могут совпадать.");
            }

            clientOrganization.AddContract(this);
            ContractorOrganization = clientOrganization;            
        }        

        #endregion
    }
}
