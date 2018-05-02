using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IMovementWaybillMainIndicatorService
    {
        MovementWaybillMainIndicators GetMainIndicators(MovementWaybill waybill, User user, bool calculateMarkups = false, bool calculateValueAddedTaxSums = false);

        IDictionary<Guid, MovementWaybillRowMainIndicators> GetMainRowIndicators(MovementWaybill waybill, User user, bool calculateValueAddedTaxSums = false,
            bool calculateMarkups = false);
        MovementWaybillRowMainIndicators GetMainRowIndicators(MovementWaybillRow row, User user, bool calculateValueAddedTaxSums = false, bool calculateMarkups = false);

        decimal CalculateShippingPercent(MovementWaybill waybill);
    }
}