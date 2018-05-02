using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class BankAccountMap : SubclassMap<RussianBankAccount>
    {
        public BankAccountMap()
        {
            KeyColumn("Id");
            Map(x => x.DeletionDate);
            References(x => x.Organization).Column("OrganizationId").Not.Nullable();
        }
    }
}
