using System;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация о нераспределенных частях оплат
    /// </summary>
    public class DealPaymentUndistributedPartInfo
    {
        /// <summary>
        /// Платежный документ по сделке, к которому относится информация
        /// </summary>
        public DealPaymentDocument DealPaymentDocument { get; protected set; }

        /// <summary>
        /// Дата появления свободных денег для разнесения (дата платежного документа, либо дата приемки возврата от клиента)
        /// </summary>
        public DateTime AppearenceDate { get; protected set; }

        /// <summary>
        /// Неразнесенная сумма
        /// </summary>
        public decimal Sum { get; protected internal set; }

        /// <summary>
        /// Разнесение на накладную возврата от клиента, которое является источником для данной неразнесенной части оплат 
        /// </summary>
        public DealPaymentDocumentDistributionToReturnFromClientWaybill DealPaymentDocumentDistributionToReturnFromClientWaybill { get; set; }


        public DealPaymentUndistributedPartInfo(DealPaymentDocument dealPaymentDocument, DateTime appearenceDate, decimal sum)
        {
            DealPaymentDocument = dealPaymentDocument;
            AppearenceDate = appearenceDate;
            Sum = sum;
        }
    }
}
