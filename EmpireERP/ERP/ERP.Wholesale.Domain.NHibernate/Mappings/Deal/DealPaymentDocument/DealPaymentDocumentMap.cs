using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealPaymentDocumentMap : ClassMap<DealPaymentDocument>
    {
        public DealPaymentDocumentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb().Not.Nullable();

            Map(x => x.Date).Not.Nullable();
            Map(x => x.Sum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.DistributedSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.IsFullyDistributed).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.Type).CustomType<DealPaymentDocumentType>().Column("DealPaymentDocumentTypeId").Access.CamelCaseField().Not.Nullable();

            References(x => x.Deal).Column("DealId").Not.Nullable();
            References(x => x.Team).Column("TeamId").Not.Nullable();
            References(x => x.User).Column("UserId").Not.Nullable();

            Where("DeletionDate is null");

            BatchSize(10);
        }
    }
}
