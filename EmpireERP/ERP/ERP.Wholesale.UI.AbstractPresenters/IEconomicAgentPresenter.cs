using ERP.Wholesale.UI.ViewModels.EconomicAgent;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IEconomicAgentPresenter
    {
        JuridicalPersonEditViewModel SelectTypeJuridicalPerson(EconomicAgentTypeSelectorViewModel typeOrg);
        PhysicalPersonEditViewModel SelectTypePhysicalPerson(EconomicAgentTypeSelectorViewModel typeOrg);
    }
}
