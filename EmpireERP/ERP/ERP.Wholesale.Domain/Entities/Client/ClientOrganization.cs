
namespace ERP.Wholesale.Domain.Entities
{
    public class ClientOrganization : ContractorOrganization
    {
        #region Свойства
                

        #endregion

        #region Конструкторы

        protected ClientOrganization()
        {
        }

        public ClientOrganization(string shortName, string fullName, EconomicAgent economicAgent) :
            base(shortName, fullName, economicAgent, OrganizationType.ClientOrganization)
        {
        }

        #endregion
    }
}
