using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ContractorOrganizationMap : SubclassMap<ContractorOrganization>
    {
        public ContractorOrganizationMap()
        {
            KeyColumn("Id");

            HasMany(x => x.Contracts)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("ContractorOrganizationId").Inverse().Cascade.SaveUpdate()
                .Where("DeletionDate is null")
                .BatchSize(50);

            HasManyToMany(x => x.Contractors)
                .AsSet().Access.CamelCaseField()
                .Table("ContractorOrganizationContractor")
                .ParentKeyColumn("ContractorOrganizationId")
                .ChildKeyColumn("ContractorId").Inverse()
                .BatchSize(50);

            BatchSize(25);
        }
    }
}
