using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ForeignBankAccountMap : SubclassMap<ForeignBankAccount>
    {
        public ForeignBankAccountMap()
        {
            KeyColumn("Id");
            Map(x => x.DeletionDate);
            Map(x => x.IBAN).Length(34).Not.Nullable();
            References(x => x.Organization).Column("OrganizationId").Not.Nullable();
        }
    }
}
