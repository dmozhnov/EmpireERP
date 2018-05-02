using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealInitialBalanceCorrectionMap : SubclassMap<DealInitialBalanceCorrection>
    {
        public DealInitialBalanceCorrectionMap()
        {
            KeyColumn("Id");

            Map(x => x.CorrectionReason).Length(140).Not.Nullable();

            BatchSize(10);
        }
    }
}
