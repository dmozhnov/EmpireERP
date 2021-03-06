﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ContractMap : ClassMap<Contract>
    {
        public ContractMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Number).Length(50).Not.Nullable();
            Map(x => x.Date).Not.Nullable();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate);
            Map(x => x.StartDate).Not.Nullable();
            Map(x => x.EndDate);
            Map(x => x.Comment).Length(4000).Not.Nullable();
         
            References(x => x.AccountOrganization).Column("AccountOrganizationId").Not.Nullable();
            References(x => x.ContractorOrganization).Column("ContractorOrganizationId").Not.Nullable();

            HasManyToMany(x => x.Contractors)
                .AsSet().Access.CamelCaseField()
                .Table("ContractorContract")
                .ParentKeyColumn("ContractId")
                .ChildKeyColumn("ContractorId")
                .Inverse();

            Where("DeletionDate is null");
            BatchSize(50);
        }
    }
}
