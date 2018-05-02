using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class RussianBankMap : SubclassMap<RussianBank>
    {
        public RussianBankMap()
        {
            KeyColumn("Id");
            Map(x => x.CorAccount).Length(20).Not.Nullable();
            Map(x => x.BIC).Length(9).Unique().Not.Nullable();
        }
    }
}
