using ERP.Wholesale.Domain.Entities.Security;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings.Security
{
    public class SettingMap : ClassMap<Setting>
    {
        public SettingMap()
        {
            Id(x => x.Id);

            Map(x => x.UseReadyToAcceptStateForChangeOwnerWaybill).Not.Nullable();
            Map(x => x.UseReadyToAcceptStateForExpenditureWaybill).Not.Nullable();
            Map(x => x.UseReadyToAcceptStateForMovementWaybill).Not.Nullable();
            Map(x => x.UseReadyToAcceptStateForReturnFromClientWaybill).Not.Nullable();
            Map(x => x.UseReadyToAcceptStateForWriteOffWaybill).Not.Nullable();

            Map(x => x.ActiveUserCountLimit).Not.Nullable();
            Map(x => x.TeamCountLimit).Not.Nullable();
            Map(x => x.StorageCountLimit).Not.Nullable();
            Map(x => x.AccountOrganizationCountLimit).Not.Nullable();
            Map(x => x.GigabyteCountLimit).Not.Nullable();

            Cache.ReadWrite();
        }
    }
}
