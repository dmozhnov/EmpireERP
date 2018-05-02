using System;
using System.Collections.Generic;
using ERP.Infrastructure.IoC;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;
using ERP.Infrastructure.Repositories.Criteria;
using System.Linq;

namespace ERP.Wholesale.Domain.Services
{
    public class ClientContractIndicatorService : IClientContractIndicatorService
    {
        #region Поля

        private readonly IClientContractRepository clientContractRepository;
        private readonly ISaleWaybillRepository saleWaybillRepository;

        private readonly IDealIndicatorService dealIndicatorService;

        #endregion

        #region Конструктор

        public ClientContractIndicatorService(IClientContractRepository clientContractRepository, ISaleWaybillRepository saleWaybillRepository, IDealIndicatorService dealIndicatorService)
        {
            this.clientContractRepository = clientContractRepository;
            this.saleWaybillRepository = saleWaybillRepository;

            this.dealIndicatorService = dealIndicatorService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Расчет превышения максимально допустимой суммы наличных расчетов ПО ОПЛАТАМ ОТ КЛИЕНТА для договора с клиентом.
        /// То есть, считает сумму наличных расчетов по оплатам от клиента для всех сделок указанного договора и вычитает из нее максимально допустимую сумму.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        public decimal CalculateCashPaymentLimitExcessByPaymentsFromClient(ClientContract clientContract)
        {
            decimal totalCashPaymentSum = CalculateClientContractCashPaymentSum(clientContract);

            // из суммы сделки вычитаем сумму лимита наличных расчетов для договора
            return totalCashPaymentSum - AppSettings.MaxCashPaymentSum;
        }

        /// <summary>
        /// Расчет суммы текущих взаиморасчетов за наличный расчет по договору
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        public decimal CalculateClientContractCashPaymentSum(ClientContract clientContract)
        {
            decimal totalCashPaymentSum = 0;

            var deals = clientContractRepository.GetDeals(clientContract);

            foreach (var deal in deals)
            {
                totalCashPaymentSum += deal.CashDealPaymentSum;
            }

            return totalCashPaymentSum;
        }

        #endregion
    }
}
