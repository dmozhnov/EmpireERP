using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealPaymentDocument;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class DealPaymentDocumentPresenter : IDealPaymentDocumentPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IClientOrganizationService clientOrganizationService;
        private readonly IDealPaymentDocumentService dealPaymentDocumentService;
        private readonly IDealService dealService;
        private readonly IUserService userService;

        private readonly IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator;

        #endregion

        #region Конструктор

        public DealPaymentDocumentPresenter(IUnitOfWorkFactory unitOfWorkFactory, IClientOrganizationService clientOrganizationService,
            IDealPaymentDocumentService dealPaymentDocumentService, IDealService dealService,
            IUserService userService, IDealPaymentDocumentPresenterMediator dealPaymentDocumentPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.clientOrganizationService = clientOrganizationService;
            this.dealPaymentDocumentService = dealPaymentDocumentService;
            this.dealService = dealService;
            this.userService = userService;

            this.dealPaymentDocumentPresenterMediator = dealPaymentDocumentPresenterMediator;
        }

        #endregion

        #region Методы

        #region Получение гридов разнесения в деталях платежных документов

        public GridData GetSaleWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return dealPaymentDocumentPresenterMediator.GetSaleWaybillGridLocal(state, user);
            }
        }

        public GridData GetDealDebitInitialBalanceCorrectionGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return dealPaymentDocumentPresenterMediator.GetDealDebitInitialBalanceCorrectionGridLocal(state, user);
            }
        }

        #endregion

        #endregion
    }
}