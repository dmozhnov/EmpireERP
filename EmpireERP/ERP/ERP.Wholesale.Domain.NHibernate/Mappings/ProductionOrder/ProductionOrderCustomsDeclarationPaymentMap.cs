using ERP.Wholesale.Domain.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace ERP.Wholesale.Domain.NHibernate.Mappings
{
    public class ProductionOrderCustomsDeclarationPaymentMap : SubclassMap<ProductionOrderCustomsDeclarationPayment>
    {
        public ProductionOrderCustomsDeclarationPaymentMap()
        {
            KeyColumn("Id");

            Map(x => x.DeletionDate).Access.CamelCaseField();

            References(x => x.CustomsDeclaration).Column("CustomsDeclarationId").Not.Nullable();
        }
    }
}
