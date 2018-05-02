using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IReturnFromClientWaybillMainIndicatorService
    {
        ReturnFromClientWaybillMainIndicators CalculateMainRowIndicators(ReturnFromClientWaybillRow row, User user);

        decimal CalculateShippingPercent(ReturnFromClientWaybill returnFromClientWaybill);

        decimal CalculateAccountingPriceSum(ReturnFromClientWaybill waybill);
    }
}