using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class BankMap : ClassMap<Bank>
    {
        public BankMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Address).Length(250).Not.Nullable();
            Map(x => x.Name).Length(250).Unique().Not.Nullable();
            Map(x => x.DeletionDate);
        }
    }
}
