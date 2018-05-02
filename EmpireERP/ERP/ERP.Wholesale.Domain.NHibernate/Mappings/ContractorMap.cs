using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ContractorMap : ClassMap<Contractor>
    {
        public ContractorMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Name).Length(200).Not.Nullable();
            Map(x => x.ContractorType).CustomType<ContractorType>().Column("ContractorTypeId").Not.Nullable();
            Map(x => x.Rating).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            HasManyToMany(x => x.Organizations)
                .AsSet().Access.CamelCaseField()
                .Table("ContractorOrganizationContractor")
                .ParentKeyColumn("ContractorId")
                .ChildKeyColumn("ContractorOrganizationId").Cascade.All().BatchSize(50);

            HasManyToMany(x => x.Contracts)
                .AsSet().Access.CamelCaseField()
                .Table("ContractorContract")
                .ParentKeyColumn("ContractorId")
                .ChildKeyColumn("ContractId").Cascade.All().BatchSize(50);

            Where("DeletionDate is null");

            Cache.ReadWrite();
        }
    }
}
