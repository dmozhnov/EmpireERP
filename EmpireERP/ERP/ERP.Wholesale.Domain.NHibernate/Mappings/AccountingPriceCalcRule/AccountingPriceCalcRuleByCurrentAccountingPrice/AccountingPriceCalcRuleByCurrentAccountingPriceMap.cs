using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountingPriceCalcByCurrentAccountingPriceMap : ComponentMap<AccountingPriceCalcByCurrentAccountingPrice>
    {
        public AccountingPriceCalcByCurrentAccountingPriceMap()
        {
            Map(x => x.MarkupPercentValue).Precision(6).Scale(2);

            Component<AccountingPriceDeterminationRule>(x => x.AccountingPriceDeterminationRule);
        }
    }
}
