using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealDebitInitialBalanceCorrectionMap : SubclassMap<DealDebitInitialBalanceCorrection>
    {
        public DealDebitInitialBalanceCorrectionMap()
        {
            KeyColumn("Id");

            HasMany(x => x.ConcreteDistributions)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("DestinationDealPaymentDocumentId")
                .Inverse().Cascade.AllDeleteOrphan()
                .BatchSize(50);

            BatchSize(10);
        }
    }
}
