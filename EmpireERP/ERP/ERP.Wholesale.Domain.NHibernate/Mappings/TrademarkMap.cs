﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class TrademarkMap: ClassMap<Trademark>
    {
        public TrademarkMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Column("Name").Length(200).Unique().Not.Nullable();
        }
    }
}
