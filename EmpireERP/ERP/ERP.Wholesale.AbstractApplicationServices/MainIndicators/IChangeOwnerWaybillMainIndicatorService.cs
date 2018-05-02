using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IChangeOwnerWaybillMainIndicatorService
    {
        IDictionary<Guid, ChangeOwnerWaybillRowMainIndicators> GetMainRowsIndicators(ChangeOwnerWaybill waybill, User user, bool calculateValueAddedTaxSum = false);

        ChangeOwnerWaybillRowMainIndicators GetMainRowIndicators(ChangeOwnerWaybillRow row, User user, bool calculateValueAddedTaxSum = false);

        ChangeOwnerWaybillMainIndicators GetMainIndicators(ChangeOwnerWaybill waybill, User user, bool calculateValueAddedTaxes = false);

        decimal CalculateShippingPercent(ChangeOwnerWaybill waybill);
    }
}