using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain
{
    public delegate void ChangeOwnerWaybillEvent(ChangeOwnerWaybill waybill, DateTime date);
    
    /// <summary>
    /// События домена
    /// </summary>
    public static class DomainEvents
    {
        
    }
}
