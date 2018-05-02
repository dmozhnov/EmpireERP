using System.Collections.Generic;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using System;
using System.Linq;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.ApplicationServices
{
    public class SaleWaybillService : ISaleWaybillService
    {
        #region Поля

        private readonly ISaleWaybillRepository saleWaybillRepository;
        private readonly IDealRepository dealRepository;
        private readonly ITeamRepository teamRepository;

        private readonly IDealIndicatorService dealIndicatorService;

        #endregion

        #region Конструкторы

        public SaleWaybillService(ISaleWaybillRepository saleWaybillRepository, IDealRepository dealRepository,
            IDealIndicatorService dealIndicatorService, ITeamRepository teamRepository)
        {
            this.saleWaybillRepository = saleWaybillRepository;
            this.dealRepository = dealRepository;
            this.teamRepository = teamRepository;

            this.dealIndicatorService = dealIndicatorService;
        }
        
        #endregion

        #region Методы

        /// <summary>
        /// Получить список не полностью оплаченных проведенных накладных реализации по сделке в рамках команды
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда</param>
        /// <returns>Список накладных</returns>
        public IEnumerable<SaleWaybill> GetSaleWaybillListForDealPaymentDocumentDistribution(Deal deal, Team team)
        {
            return saleWaybillRepository.GetSaleWaybillListForDealPaymentDocumentDistribution(deal.Id, team.Id);
        }

        

       #endregion
    }
}
