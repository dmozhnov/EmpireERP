using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;
using ERP.Wholesale.Domain.ValueObjects;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class JuridicalPersonMap : SubclassMap<JuridicalPerson>
    {
        public JuridicalPersonMap()
        {
            KeyColumn("Id");
            Map(x => x.INN).Length(10).Not.Nullable(); 
            Map(x => x.KPP).Length(9).Not.Nullable(); 
            Map(x => x.OGRN).Length(13).Not.Nullable();
            Map(x => x.OKPO).Length(10).Not.Nullable(); 
            Map(x => x.DirectorName).Length(100).Not.Nullable();
            Map(x => x.DirectorPost).Length(100).Not.Nullable();
            Map(x => x.MainBookkeeperName).Length(100).Not.Nullable();
            Map(x => x.CashierName).Length(100).Not.Nullable();
        }
    }    
}
