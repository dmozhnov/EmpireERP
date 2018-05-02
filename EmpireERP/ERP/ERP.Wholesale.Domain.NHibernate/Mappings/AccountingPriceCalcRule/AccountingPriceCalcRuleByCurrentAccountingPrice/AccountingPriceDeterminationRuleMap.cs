using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceDeterminationRuleMap : ComponentMap<AccountingPriceDeterminationRule>
    {
        public AccountingPriceDeterminationRuleMap()
        {
            Map(x => x.Type).CustomType<AccountingPriceDeterminationRuleType>().Column("TypeId");
            Map(x => x.StorageType).CustomType<AccountingPriceListStorageTypeGroup>().Column("StorageTypeId");

            References(x => x.Storage).Column("StorageId");
        }
    }
}
