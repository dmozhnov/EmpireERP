using System;
using System.Data;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class EconomicAgentPresenter : IEconomicAgentPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ILegalFormService legalFormService;

        #endregion

        #region Конструктор

        public EconomicAgentPresenter(IUnitOfWorkFactory unitOfWorkFactory, ILegalFormService legalFormService)
        {
            this.legalFormService = legalFormService;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion

        #region Методы

        public JuridicalPersonEditViewModel SelectTypeJuridicalPerson(EconomicAgentTypeSelectorViewModel typeOrg)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var title = "Добавление новой организации";

                if (!typeOrg.IsJuridicalPerson)
                {
                    throw new Exception("Неверный тип параметра.");
                }
                var jp = new JuridicalPersonEditViewModel();
                jp.ActionName = typeOrg.ActionNameForJuridicalPerson;
                jp.ControllerName = typeOrg.ControllerName;
                jp.SuccessFunctionName = typeOrg.SuccessFunctionName;
                jp.Title = title;

                var legalForm = legalFormService.GetJuridicalLegalForms();
                jp.LegalFormList = legalForm.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                return jp;
            }
        }

        public PhysicalPersonEditViewModel SelectTypePhysicalPerson(EconomicAgentTypeSelectorViewModel typeOrg)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var title = "Добавление новой организации";

                if (typeOrg.IsJuridicalPerson)
                {
                    throw new Exception("Неверный тип параметра.");
                }

                var pp = new PhysicalPersonEditViewModel();
                pp.ActionName = typeOrg.ActionNameForPhysicalPerson;
                pp.ControllerName = typeOrg.ControllerName;
                pp.SuccessFunctionName = typeOrg.SuccessFunctionName;
                pp.Title = title;

                var legalForm = legalFormService.GetPhysicalLegalForms();
                pp.LegalFormList = legalForm.GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), true);

                return pp;
            }
        }

        #endregion
    }
}
