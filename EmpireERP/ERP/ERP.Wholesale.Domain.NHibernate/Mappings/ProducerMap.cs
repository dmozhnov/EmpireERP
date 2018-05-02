using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProducerMap : SubclassMap<Producer>
    {
        public ProducerMap()
        {
            KeyColumn("Id");

            Map(x => x.VATNo).Not.Nullable();
            Map(x => x.ManagerName).Length(100).Not.Nullable();
            Map(x => x.Email).Length(100).Not.Nullable();
            Map(x => x.MobilePhone).Length(100).Not.Nullable();
            Map(x => x.Skype).Length(100).Not.Nullable();
            Map(x => x.MSN).Length(100).Not.Nullable();
            
            References(x => x.Curator).Column("CuratorId").Not.Nullable();

            HasManyToMany(x => x.Manufacturers)
                .AsSet().Access.CamelCaseField()
                .Table("ProducerManufacturer")
                .ParentKeyColumn("ProducerId")
                .ChildKeyColumn("ManufacturerId").Cascade.All();
        }
    }
}
