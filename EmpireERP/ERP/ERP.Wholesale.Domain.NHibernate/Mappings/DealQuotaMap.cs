using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class DealQuotaMap : ClassMap<DealQuota>
    {
        public DealQuotaMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.IsPrepayment).Not.Nullable();
            Map(x => x.DiscountPercent).Precision(5).Scale(2).Access.CamelCaseField().Not.Nullable();
            Map(x => x.PostPaymentDays);
            Map(x => x.CreditLimitSum).Precision(18).Scale(2);
            
            Where("DeletionDate is null");

            BatchSize(50);
        }
    }
}
