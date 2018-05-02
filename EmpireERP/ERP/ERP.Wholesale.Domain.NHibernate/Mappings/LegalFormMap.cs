using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class LegalFormMap : ClassMap<LegalForm>
    {
        public LegalFormMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(15).Unique().Not.Nullable();
            Map(x => x.EconomicAgentType).CustomType<EconomicAgentType>().Column("EconomicAgentTypeId").Not.Nullable();
        }
    }
}
