using System;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class BlockingService : IBlockingService
    {
        #region Классы

        /// <summary>
        /// Набор проверок для выполнения
        /// </summary>
        private struct CheckSet
        {
            /// <summary>
            /// Проверка на блокировки по просрочке платежа и по связям с заблокированным клиентом
            /// </summary>
            public bool CheckForPaymentDelayBlocking { get; set; }

            /// <summary>
            /// Проверка на ручную блокировку клиента
            /// </summary>
            public bool CheckForManualBlocking { get; set; }

            /// <summary>
            /// Проверка на блокировку по кредитному лимиту
            /// </summary>
            public bool CheckForCreditLimitBlocking { get; set; }
        }

        #endregion

        #region Поля

        private readonly IDealRepository dealRepository;

        private readonly IClientContractIndicatorService clientContractIndicatorService;
        private readonly IDealIndicatorService dealIndicatorService;

        #endregion

        #region Конструктор

        public BlockingService(IDealRepository dealRepository, IDealIndicatorService dealIndicatorService, IClientContractIndicatorService clientContractIndicatorService)
        {
            this.dealRepository = dealRepository;

            this.clientContractIndicatorService = clientContractIndicatorService;
            this.dealIndicatorService = dealIndicatorService;
        }

        #endregion

        #region Методы

        #region Методы для обработки операций

        /// <summary>
        /// Проверка, не мешают ли блокировки операции по данной сделке
        /// </summary>
        /// <param name="operation">Тип проводимой операции</param>
        /// <param name="deal">Сделка, по которой проверяются блокировки</param>
        public void CheckForBlocking(BlockingDependentOperation operation, Deal deal, SaleWaybill transientSaleWaybill)
        {
            CheckSet checkSet;

            switch (operation)
            {
                case BlockingDependentOperation.CreateExpenditureWaybill:
                    checkSet = new CheckSet { CheckForManualBlocking = true };
                    break;

                case BlockingDependentOperation.SavePrePaymentExpenditureWaybillRow:
                    checkSet = new CheckSet { CheckForManualBlocking = true };
                    break;

                case BlockingDependentOperation.SavePostPaymentExpenditureWaybillRow:
                    checkSet = new CheckSet { CheckForManualBlocking = true };
                    break;

                case BlockingDependentOperation.AcceptPrePaymentExpenditureWaybill:
                    checkSet = new CheckSet { CheckForManualBlocking = true, CheckForPaymentDelayBlocking = true };
                    break;

                case BlockingDependentOperation.AcceptPostPaymentExpenditureWaybill:
                    checkSet = new CheckSet
                    {
                        CheckForPaymentDelayBlocking = true,
                        CheckForManualBlocking = true,
                        CheckForCreditLimitBlocking = true,
                    };
                    break;
                
                case BlockingDependentOperation.ShipUnpaidPostPaymentExpenditureWaybill:
                    checkSet = new CheckSet { CheckForPaymentDelayBlocking = true, CheckForManualBlocking = true };
                    break;
                
                default:
                    throw new Exception("Неизвестный тип операции.");
            };

            PerformCheck(checkSet, deal, transientSaleWaybill);
        }

        /// <summary>
        /// Выполнение проверок на блокировки.
        /// Проверки всегда выполняются в определенном порядке:
        /// 1. Проверка на просрочку/связи с просрочившей организацией
        /// 2. Проверка на ручную блокировку
        /// 3. Проверка на блокировку по кредитному лимиту
        /// </summary>
        /// <param name="checkSet">Набор проверок для выполнения</param>
        /// <param name="deal">Сделка, по которой проверяются блокировки</param>
        private void PerformCheck(CheckSet checkSet, Deal deal, SaleWaybill transientSaleWaybill)
        {
            if (checkSet.CheckForPaymentDelayBlocking)
            {
                CheckForPaymentDelayBlocking(deal);
            }
            if (checkSet.CheckForManualBlocking)
            {
                CheckForManualBlocking(deal);
            }
            if (checkSet.CheckForCreditLimitBlocking)
            {
                CheckForCreditLimitBlocking(deal, transientSaleWaybill);
            }
        }

        #endregion

        #region Проверка блокировок
        
        /// <summary>
        /// Проверка на блокировки по просрочке платежа и по связям с заблокированным клиентом
        /// </summary>
        /// <param name="deal">Сделка</param>
        private void CheckForPaymentDelayBlocking(Deal deal)
        {
            // Можно было бы банально вызывать расчет показателей для клиента и организации, а потом посмотреть, не равна ли 0 просрочка,
            // и вывести сумму (всей) просрочки и период (максимальный в днях). Но тогда мы бы не смогли вывести имя сделки.

            // Проходим все сделки данного клиента, в том числе и текущую
            foreach (Deal clientDeal in deal.Client.Deals)
            {
                CheckDealForPaymentDelayBlocking(clientDeal);
            }

            // Проходим все сделки с данной организацией через других клиентов
            var clientContractSQ = dealRepository.SubQuery<ClientContract>().Where(x => x.ContractorOrganization.Id == deal.Contract.ContractorOrganization.Id)
                .Select(x => x.Id);
            var clientOrganizationDealList = dealRepository.Query<Deal>()
                .Restriction<ClientContract>(x => x.Contract).PropertyIn(x => x.Id, clientContractSQ).ToList<Deal>();

            foreach (Deal clientOrganizationDeal in clientOrganizationDealList)
            {
                if (!deal.Client.Deals.Contains(clientOrganizationDeal)) // не проходим сделки того же клиента повторно
                {
                    CheckDealForPaymentDelayBlocking(clientOrganizationDeal);
                }
            }
        }

        private void CheckDealForPaymentDelayBlocking(Deal deal)
        {
            decimal paymentDelayPeriod = dealIndicatorService.CalculatePaymentDelayPeriod(deal);
            if (paymentDelayPeriod > 0)
            {
                throw new Exception(String.Format("Данная операция невозможна, т.к. имеется просрочка платежа по сделке «{0}» сроком {1} дн.",
                    deal.Name, paymentDelayPeriod.ForDisplay()));
            }
        }

        /// <summary>
        /// Проверка, не заблокирован ли вручную клиент, к которому относится сделка
        /// </summary>
        /// <param name="deal">Сделка</param>
        private void CheckForManualBlocking(Deal deal)
        {
            if (deal.Client.IsBlockedManually)
            {
                throw new Exception(String.Format("Клиент «{0}» был заблокирован пользователем «{1}» {2}.", deal.Client.Name, deal.Client.ManualBlocker.DisplayName,
                    deal.Client.ManualBlockingDate.Value.ToShortDateString()));
            }
        }

        /// <summary>
        /// Проверка, не заблокирована ли сделка по кредитному лимиту
        /// Выполняется только при проведении операций по непринятым накладным реализации с отсрочкой платежа.
        /// Таким образом, накладная реализации всегда имеет квоту с отсрочкой платежа.
        /// </summary>
        /// <param name="deal">Сделка</param>
        private void CheckForCreditLimitBlocking(Deal deal, SaleWaybill transientSaleWaybill)
        {
            decimal? currentCreditLimitSum;
            decimal creditLimitRemainder = dealIndicatorService.CalculateCreditLimitRemainder(deal, out currentCreditLimitSum, transientSaleWaybill);

            ValidationUtils.NotNull(currentCreditLimitSum, "Текущая квота по сделке не предусматривает отсрочку платежа.");

            if (currentCreditLimitSum != 0) // Безлимитный кредит соответствует значению 0, при нем блокировка невозможна
            {
                if (creditLimitRemainder < 0)
                {
                    throw new Exception(String.Format("Данная операция невозможна, произошло превышение кредитного лимита по сделке на {0} р.",
                        (-creditLimitRemainder).ForDisplay(ValueDisplayType.Money)));
                }
            }
        }
        
        #endregion

        #endregion
    }
}
