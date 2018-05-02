using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class MarkupPercentDeterminationRuleMap : ComponentMap<MarkupPercentDeterminationRule>
    {
        public MarkupPercentDeterminationRuleMap()
        {
            Map(x => x.MarkupPercentValue).Precision(6).Scale(2).Column("Value");
            Map(x => x.Type).CustomType<MarkupPercentDeterminationRuleType>().Column("TypeId");
        }
    }
}
