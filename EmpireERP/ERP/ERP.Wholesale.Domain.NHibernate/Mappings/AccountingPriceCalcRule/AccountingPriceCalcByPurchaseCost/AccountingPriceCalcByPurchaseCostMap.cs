using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceCalcByPurchaseCostMap : ComponentMap<AccountingPriceCalcByPurchaseCost>
    {
        public AccountingPriceCalcByPurchaseCostMap()
        {
            Map(x => x.PurchaseCostDeterminationRuleType).CustomType<PurchaseCostDeterminationRuleType>().Column("PurchaseCostDeterminationRuleTypeId");

            Component<MarkupPercentDeterminationRule>(x => x.MarkupPercentDeterminationRule).ColumnPrefix("MarkupPercentDeterminationRule");            
        }
    }
}
