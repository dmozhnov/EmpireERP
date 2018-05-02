using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ForeignBankMap : SubclassMap<ForeignBank>
    {
        public ForeignBankMap()
        {
            KeyColumn("Id");
            Map(x => x.ClearingCode).Length(9).Not.Nullable();
            Map(x => x.ClearingCodeType).CustomType<ClearingCodeType?>().Column("ClearingCodeTypeId");
            Map(x => x.SWIFT).Length(11).Unique().Not.Nullable();
        }
    }
}
