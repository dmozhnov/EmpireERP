﻿using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderExtraExpensesSheetService
    {
        IList<ProductionOrderExtraExpensesSheet> GetFilteredList(object state, User user, ParameterString parameterString = null);        
    }
}