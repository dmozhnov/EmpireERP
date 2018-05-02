using ERP.Wholesale.Domain.Entities;
using System.Linq;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;
using System;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IWriteoffWaybillMainIndicatorService
    {
        WriteoffWaybillMainIndicators GetMainIndicators(WriteoffWaybill waybill, User user, bool calculateReceivelessProfit = false);

        IDictionary<Guid, WriteoffWaybillRowMainIndicators> GetMainRowsIndicators(WriteoffWaybill waybill, User user);

        WriteoffWaybillRowMainIndicators GetMainRowIndicators(WriteoffWaybillRow row, User user);
    }
}