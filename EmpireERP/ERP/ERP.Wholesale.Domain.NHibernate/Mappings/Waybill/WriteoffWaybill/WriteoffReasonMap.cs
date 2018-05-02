using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class WriteoffReasonMap : ClassMap<WriteoffReason>
    {
        public WriteoffReasonMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Length(200).Unique().Not.Nullable();
        }
    }
}
