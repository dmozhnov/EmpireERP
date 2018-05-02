﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProductionOrderExtraExpensesSheetRepository : IRepository<ProductionOrderExtraExpensesSheet, Guid>,
        IFilteredRepository<ProductionOrderExtraExpensesSheet>, IGetAllRepository<ProductionOrderExtraExpensesSheet>
    {
    }
}
