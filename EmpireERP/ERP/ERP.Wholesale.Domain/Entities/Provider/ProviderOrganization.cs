
namespace ERP.Wholesale.Domain.Entities
{
    public class ProviderOrganization : ContractorOrganization
    {
        #region Свойства

        #endregion

        #region Конструкторы

        protected ProviderOrganization()
        {
        }

        public ProviderOrganization(string shortName, string fullName, EconomicAgent economicAgent) :
            base(shortName, fullName, economicAgent, OrganizationType.ProviderOrganization)
        {
        }

        #endregion
    }
}
