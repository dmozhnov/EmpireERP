using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class AccountOrganizationMap : SubclassMap<AccountOrganization>
    {
        public AccountOrganizationMap()
        {
            KeyColumn("Id");

            HasMany<AccountOrganizationDocumentNumbers>(Reveal.Member<AccountOrganization>("documentNumbers")).AsSet()
                .KeyColumn("AccountOrganizationId").Inverse().Cascade.All();

            Map(x => x.SalesOwnArticle).Not.Nullable();

            HasMany(x => x.Contracts)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("AccountOrganizationId").Inverse().Cascade.SaveUpdate().Where("DeletionDate is null");

            HasManyToMany(x => x.Storages)
                .AsSet().Access.CamelCaseField()
                .Table("AccountOrganizationStorage")
                .ParentKeyColumn("AccountOrganizationId")
                .ChildKeyColumn("StorageId").Inverse();

            BatchSize(50);
        }
    }
}
