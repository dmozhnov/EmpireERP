using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.ValueObjects;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class PassportInfoMap : ComponentMap<PassportInfo>
    {
        public PassportInfoMap()
        {
            Map(x => x.Series).Not.Nullable().Length(10);
            Map(x => x.Number).Not.Nullable().Length(10);
            Map(x => x.IssueDate);
            Map(x => x.IssuedBy).Not.Nullable().Length(200);
            Map(x => x.DepartmentCode).Not.Nullable().Length(10);
        }
    }

    
}
