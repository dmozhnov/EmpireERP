using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class LastDigitCalcRuleMap : ComponentMap<LastDigitCalcRule>
    {
        public LastDigitCalcRuleMap()
        {
            Map(x => x.Type).CustomType<LastDigitCalcRuleType>().Column("TypeId").Not.Nullable();
            Map(x => x.LastDigit);
            Map(x => x.Decimals);

            References(x => x.Storage).Column("StorageId");
        }
    }
}
