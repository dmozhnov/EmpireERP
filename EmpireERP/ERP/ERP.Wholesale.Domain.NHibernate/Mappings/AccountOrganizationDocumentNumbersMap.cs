using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountOrganizationDocumentNumbersMap: ClassMap<AccountOrganizationDocumentNumbers>
    {
        public AccountOrganizationDocumentNumbersMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            
            Map(x => x.Year).Not.Nullable();
            Map(x => x.ReceiptWaybillLastNumber).Not.Nullable();
            Map(x => x.ChangeOwnerWaybillLastNumber).Not.Nullable();
            Map(x => x.ExpenditureWaybillLastNumber).Not.Nullable();
            Map(x => x.MovementWaybillLastNumber).Not.Nullable();
            Map(x => x.ReturnFromClientWaybillLastNumber).Not.Nullable();
            Map(x => x.WriteoffWaybillLastNumber).Not.Nullable();

            References(x => x.AccountOrganization).Column("AccountOrganizationId").Cascade.None().Not.Nullable();
        }
    }
}
