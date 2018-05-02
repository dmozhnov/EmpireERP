using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceCalcRuleMap : ComponentMap<AccountingPriceCalcRule>
    {
        public AccountingPriceCalcRuleMap()
        {
            Map(x => x.Type).CustomType<AccountingPriceCalcRuleType>().Column("TypeId").Not.Nullable();

            Component<AccountingPriceCalcByPurchaseCost>(x => x.CalcByPurchaseCost).ColumnPrefix("ByPurchaseCost");
            Component<AccountingPriceCalcByCurrentAccountingPrice>(x => x.CalcByCurrentAccountingPrice).ColumnPrefix("ByCurrentAccountingPrice");
        }
    }
}
