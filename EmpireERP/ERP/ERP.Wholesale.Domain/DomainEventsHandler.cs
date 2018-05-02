using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain
{
    /// <summary>
    /// Обработчик событий домена
    /// </summary>
    public class DomainEventsHandler : IDomainEventsHandler
    {
        public void OnChangeOwnerWaybillChangedOwner(ChangeOwnerWaybill waybill, DateTime date) { }
    }
}
