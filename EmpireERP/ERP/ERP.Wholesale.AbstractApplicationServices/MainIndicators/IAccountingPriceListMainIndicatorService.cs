using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IAccountingPriceListMainIndicatorService
    {
        AccountingPriceListMainIndicators GetMainIndicators(AccountingPriceList accountingPriceList, User user, bool calculateChangesAndMarkups = false);
    }
}