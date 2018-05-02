using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IRussianBankService
    {
        RussianBank GetByBIC(string BIC);
        void Save(RussianBank entity);
        void Delete(RussianBank entity);
        IList<RussianBank> GetFilteredList(object state, bool ignoreDeletedRows = true);
        IList<RussianBank> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true);
        RussianBank CheckBankExistence(int id);
        void CheckBankUniqueness(RussianBank bank);
    }
}
