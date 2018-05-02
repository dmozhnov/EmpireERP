using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.ValueObjects;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class PhysicalPersonMap : SubclassMap<PhysicalPerson>
    {
        public PhysicalPersonMap()
        {
            KeyColumn("Id");
            Map(x => x.LastName).Length(100).Not.Nullable();
            Map(x => x.FirstName).Length(100).Not.Nullable();
            Map(x => x.Patronymic).Length(100).Not.Nullable();
            Map(x => x.INN).Length(12).Not.Nullable();
            Map(x => x.OGRNIP).Length(15).Not.Nullable();

            Component<Address>(x => x.RegistrationAddress, y =>
            {
                y.Map(x => x.PostalIndex).Not.Nullable().Length(6).Column("RegistrationPostalIndex");
                y.Map(x => x.LocalAddress).Not.Nullable().Length(100).Column("RegistrationLocalAddress");

                // т.к. ValueObject Address маппится 2 раза, необходимо жестко задать имя внешнего ключа (а то они оба равны FK_Address_City)
                y.References(x => x.City).Column("RegistrationCityId").Not.Nullable().ForeignKey("FK_RegistrationAddress_City");
            });
        }
    }
}
