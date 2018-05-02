using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IClientContractIndicatorService
    {
        /// <summary>
        /// Расчет превышения максимально допустимой суммы наличных расчетов ПО ОПЛАТАМ ОТ КЛИЕНТА для договора с клиентом.
        /// То есть, считает сумму наличных расчетов по оплатам от клиента для всех сделок указанного договора и вычитает из нее максимально допустимую сумму.
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        decimal CalculateCashPaymentLimitExcessByPaymentsFromClient(ClientContract clientContract);

        /// <summary>
        /// Расчет суммы текущих взаиморасчетов за наличный расчет по договору
        /// </summary>
        /// <param name="clientContract">Договор с клиентом.</param>
        decimal CalculateClientContractCashPaymentSum(ClientContract clientContract);
    }
}
