using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IForeignBankService
    {
        void Save(ForeignBank entity);
        void Delete(ForeignBank entity);
        ForeignBank GetBySWIFT(string swift);
        IList<ForeignBank> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true);
        ForeignBank CheckBankExistence(int id);
        void CheckBankUniqueness(ForeignBank bank);
    }
}
