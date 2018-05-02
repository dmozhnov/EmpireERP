using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IProductionOrderPlannedPaymentService
    {
        IEnumerable<ProductionOrderPlannedPayment> GetFilteredList(object state, User user, ParameterString parameterString = null);
    }
}
