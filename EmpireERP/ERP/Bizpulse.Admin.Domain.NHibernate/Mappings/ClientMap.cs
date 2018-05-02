using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.ValueObjects;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class ClientMap : ClassMap<Client>
    {
        public ClientMap()
        {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.CreationDate).Not.Nullable();
            Map(x => x.Type).CustomType<ClientType>().Column("ClientTypeId").Not.Nullable();
            Map(x => x.Phone).Length(20).Not.Nullable();
            Map(x => x.AdminEmail).Length(50).Not.Nullable();
            Map(x => x.PromoCode).Length(10).Not.Nullable();
            Map(x => x.Comment).Length(4000).Not.Nullable();
            Map(x => x.DBServerName).Length(100).Not.Nullable();
            Map(x => x.DBName).Length(20).Not.Nullable();
            Map(x => x.BlockingDate);
            Map(x => x.DeletionDate);
            Map(x => x.PrepaymentSum).Precision(18).Scale(2).Not.Nullable();
            Map(x => x.LastActivityDate).Not.Nullable();

            References(x => x.BlockedBy).Column("BlockedById");
            References(x => x.DeletedBy).Column("DeletedById");

            HasMany(x => x.Users)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("ClientId")
                .Inverse().Cascade.SaveUpdate()
                .Where("DeletionDate is null");

            HasMany(x => x.ServiceSets)
                .AsSet().Access.CamelCaseField()
                .KeyColumn("ClientId")
                .Inverse().Cascade.SaveUpdate()
                .Where("DeletionDate is null");
            
            Component<Address>(x => x.PostalAddress, y =>
            {
                y.Map(x => x.PostalIndex).Length(6).Column("PostalPostalIndex");
                y.Map(x => x.LocalAddress).Length(100).Column("PostalLocalAddress");

                // т.к. ValueObject Address маппится 2 раза, необходимо жестко задать имя внешнего ключа (а то они оба равны FK_Address_City)
                y.References(x => x.City).Column("PostalCityId").ForeignKey("FK_PostalAddress_City");
            });

            Where("DeletionDate is null");
        }
    }
}
