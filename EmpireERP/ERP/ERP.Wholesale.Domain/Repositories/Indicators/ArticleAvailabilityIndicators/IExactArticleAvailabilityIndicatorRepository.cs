﻿using System;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IExactArticleAvailabilityIndicatorRepository : IRepository<ExactArticleAvailabilityIndicator, Guid>
    {
    }
}
