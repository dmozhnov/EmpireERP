using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.ValueObjects;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class EconomicAgentMap : ClassMap<EconomicAgent>
    {
        public EconomicAgentMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            References(x => x.LegalForm).Column("LegalFormId").Access.CamelCaseField().Not.Nullable();
            Map(x => x.Type).CustomType<EconomicAgentType>().Column("EconomicAgentTypeId").Not.Nullable();

            //Component<JuridicalPerson>(x => x.JuridicalPerson).ColumnPrefix("JuridicalPerson");
            //Component<PhysicalPerson>(x => x.PhysicalPerson).ColumnPrefix("PhysicalPerson");
        }
    }
}
