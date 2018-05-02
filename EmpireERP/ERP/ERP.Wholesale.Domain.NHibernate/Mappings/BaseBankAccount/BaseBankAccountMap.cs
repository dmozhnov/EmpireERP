using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class BaseBankAccountMap : ClassMap<BankAccount>
    {
        public BaseBankAccountMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.IsMaster).Not.Nullable();
            Map(x => x.Number).Length(34).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            //Map(x => x.Type).CustomType<BankAccountType>().Column("TypeId").Not.Nullable();

            References(x => x.Currency).Column("CurrencyId").Not.Nullable();
            References(x => x.Bank).Column("BankId").Not.Nullable();
            //References(x => x.Organization).Column("OrganizationId").Not.Nullable();
        }
    }
}
