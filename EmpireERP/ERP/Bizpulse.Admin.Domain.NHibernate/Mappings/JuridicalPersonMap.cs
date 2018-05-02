using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.ValueObjects;
using FluentNHibernate.Mapping;

namespace Bizpulse.Admin.Domain.NHibernate.Mappings
{
    public class JuridicalPersonMap : SubclassMap<JuridicalPerson>
    {
        public JuridicalPersonMap()
        {
            KeyColumn("Id");
            Map(x => x.ShortName).Length(100).Not.Nullable();
            Map(x => x.INN).Length(10).Not.Nullable();
            Map(x => x.KPP).Length(9).Not.Nullable();
            Map(x => x.OGRN).Length(13).Not.Nullable();
            Map(x => x.OKPO).Length(10).Not.Nullable();
            Map(x => x.DirectorPost).Length(100).Not.Nullable();
            Map(x => x.DirectorName).Length(100).Not.Nullable();
            Map(x => x.DirectorEmail).Length(50).Not.Nullable();

            Component<Address>(x => x.JuridicalAddress, y =>
            {
                y.Map(x => x.PostalIndex).Not.Nullable().Length(6).Column("JuridicalPostalIndex");
                y.Map(x => x.LocalAddress).Not.Nullable().Length(100).Column("JuridicalLocalAddress");

                // т.к. ValueObject Address маппится 2 раза, необходимо жестко задать имя внешнего ключа (а то они оба равны FK_Address_City)
                y.References(x => x.City).Column("JuridicalCityId").Not.Nullable().ForeignKey("FK_JuridicalAddress_City");
            });
        }
    }
}
