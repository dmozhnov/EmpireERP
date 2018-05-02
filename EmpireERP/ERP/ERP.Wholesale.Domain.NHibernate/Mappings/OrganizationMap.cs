using ERP.Wholesale.Domain.Entities;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class OrganizationMap : ClassMap<Organization>
    {  
        public OrganizationMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Type).CustomType<OrganizationType>().Column("OrganizationTypeId").Not.Nullable();
            Map(x => x.Address).Length(250).Not.Nullable();
            Map(x => x.ShortName).Length(100).Not.Nullable();
            Map(x => x.FullName).Length(250).Not.Nullable();
            Map(x => x.Phone).Length(20).Not.Nullable();
            Map(x => x.Fax).Length(20).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.DeletionDate).Access.CamelCaseField();
            Map(x => x.Comment).Length(4000).Not.Nullable();

            References(x => x.EconomicAgent).Column("EconomicAgentId").Cascade.All();

            HasMany(x => x.RussianBankAccounts)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("OrganizationId").Where("DeletionDate is null").Inverse().Cascade.All();

            HasMany(x => x.ForeignBankAccounts)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("OrganizationId").Where("DeletionDate is null").Inverse().Cascade.All();

            Where("DeletionDate is null");
        }
    }
}
