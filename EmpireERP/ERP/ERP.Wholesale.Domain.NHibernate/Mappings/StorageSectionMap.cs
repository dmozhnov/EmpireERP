using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class StorageSectionMap : ClassMap<StorageSection>
    {
        public StorageSectionMap ()
	    {
            Id(x => x.Id).GeneratedBy.Native();
            
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.Square).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.Volume).Not.Nullable().Precision(18).Scale(2);
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            
            References(x => x.Storage).Column("StorageId").Not.Nullable();
            
            Where("DeletionDate is null");
        }        
    }
}
