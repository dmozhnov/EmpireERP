using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Misc
{
    public class InitialBalanceInfo
    {
        public int AccountOrganizationId { get; set; }
        public int ClientId { get; set; }
        public int ClientOrganizationId { get; set; }
        public short ContractId { get; set; }
        public short TeamId { get; set; }
        public decimal Sum { get; set; }

        public InitialBalanceInfo(int accountOrganizationId, int clientId, int clientOrganizationId, short contractId, short teamId, decimal sum)
        {
            AccountOrganizationId = accountOrganizationId;
            ClientId = clientId;
            ClientOrganizationId = clientOrganizationId;
            ContractId = contractId;
            TeamId = teamId;
            Sum = sum;
        }
    }
}
