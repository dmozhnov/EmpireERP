using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12
{
    public class TORG12PrintingFormViewModel
    {
        /// <summary>
        /// ID документа
        /// </summary>
        public string WaybillId { get; set; }

        /// <summary>
        /// Тип цен для печати отчета
        /// </summary>
        public string PriceTypeId { get; set; }

        /// <summary>
        /// Учитывать возвраты
        /// </summary>
        public bool? ConsiderReturns { get; set; }

        /// <summary>
        /// Адрес запроса к контроллеру за содержимым строк печатной формы
        /// </summary>
        public string RowsContentURL { get; set; }

        /// <summary>
        /// Организация - грузоотправитель
        /// </summary>
        [DisplayName("Организация - грузоотправитель")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Структурное подразделение
        /// </summary>
        [DisplayName("Структурное подразделение")]
        public string Department { get; set; }

        /// <summary>
        /// Грузополучатель
        /// </summary>
        [DisplayName("Грузополучатель")]
        public string Recepient { get; set; }

        /// <summary>
        /// Поставщик
        /// </summary>
        [DisplayName("Поставщик")]
        public string Sender { get; set; }

        /// <summary>
        /// Плательщик
        /// </summary>
        [DisplayName("Плательщик")]
        public string Payer { get; set; }

        /// <summary>
        /// Основание
        /// </summary>
        [DisplayName("Основание")]
        public string Reason { get; set; }

        /// <summary>
        /// Организация по ОКПО
        /// </summary>
        [DisplayName("по ОКПО")]
        public string OrganizationOKPO { get; set; }

        /// <summary>
        /// Вид деятельности организации по ОКДП
        /// </summary>
        [DisplayName("Вид деятельности по ОКДП")]
        public string OrganizationOKDP { get; set; }

        /// <summary>
        /// Получатель по ОКПО
        /// </summary>
        [DisplayName("по ОКПО")]
        public string RecepientOKPO { get; set; }

        /// <summary>
        /// Отправитель по ОКПО
        /// </summary>
        [DisplayName("по ОКПО")]
        public string SenderOKPO { get; set; }

        /// <summary>
        /// Плательщик по ОКПО
        /// </summary>
        [DisplayName("по ОКПО")]
        public string PayerOKPO { get; set; }

        /// <summary>
        /// Номер транспортной накладной
        /// </summary>
        [DisplayName("номер")]
        public string ShippingWaybillNumber { get; set; }

        /// <summary>
        /// Дата транспортной накладной
        /// </summary>
        [DisplayName("дата")]
        public string ShippingWaybillDate { get; set; }

        /// <summary>
        /// Номер основания
        /// </summary>
        [DisplayName("номер")]
        public string ReasonNumber { get; set; }

        /// <summary>
        /// Дата  основания
        /// </summary>
        [DisplayName("дата")]
        public string ReasonDate { get; set; }

        /// <summary>
        /// Дата составления
        /// </summary>
        [DisplayName("Дата составления")]
        public string Date { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        [DisplayName("Номер документа")]
        public string Number { get; set; }                        
    }
}