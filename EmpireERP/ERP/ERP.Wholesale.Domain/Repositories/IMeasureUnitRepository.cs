﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IMeasureUnitRepository : IRepository<MeasureUnit, short>, IFilteredRepository<MeasureUnit>
    {
    }
}
