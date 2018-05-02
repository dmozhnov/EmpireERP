using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.ValueObjects;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class PhysicalPersonMap : SubclassMap<PhysicalPerson>
    {        
        public PhysicalPersonMap()
        {
            KeyColumn("Id");
            Map(x => x.INN).Length(12).Not.Nullable();
            Map(x => x.OGRNIP).Length(15).Not.Nullable();
            Map(x => x.OwnerName).Length(100).Not.Nullable();
            Component<PassportInfo>(x => x.Passport).ColumnPrefix("Passport");
        }
    }
}
