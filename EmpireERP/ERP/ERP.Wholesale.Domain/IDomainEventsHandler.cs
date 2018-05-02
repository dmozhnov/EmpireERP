using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain
{
    public interface IDomainEventsHandler
    {
        void OnChangeOwnerWaybillChangedOwner(ChangeOwnerWaybill waybill, DateTime date);
    }
}
