using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0007
{
    public class Report0007ExpenditureWaybillItemViewModel
    {
        /// <summary>
        /// Признак заголовка группы
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Заголовок группы
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// Уровень группы (1 - (верняя) наиболее общая)
        /// </summary>
        public int GroupLevel { get; set; }


        #region Данные строки таблицы
        
        /// <summary>
        /// Дата реализации
        /// </summary>
        [DisplayName("Дата")]
        public string Date { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [DisplayName("Номер")]
        public string Number { get; set; }

        /// <summary>
        /// Сумма реализации
        /// </summary>
        [DisplayName("Сумма по накладной")]
        public decimal SaleSum { get; set; }

        /// <summary>
        /// Сумма задолженности по реализации
        /// </summary>
        [DisplayName("Сумма долга")]
        public decimal DebtSum { get; set; }

        /// <summary>
        /// Дата проводки реализации
        /// </summary>
        [DisplayName("Дата проводки")]
        public string AcceptanceDate { get; set; }

        /// <summary>
        /// Дата отгрузки реализации
        /// </summary>
        [DisplayName("Дата отгрузки")]
        public string ShippingDate { get; set; }

        /// <summary>
        /// Требуемая дата оплаты
        /// </summary>
        [DisplayName("Требуемая дата оплаты")]
        public string PaymentDate { get; set; }

        /// <summary>
        /// Отсрочка оплаты
        /// </summary>
        [DisplayName("Отсрочка (дни)")]
        public string PostPaymentDays { get; set; }

        /// <summary>
        /// Просрочка
        /// </summary>
        [DisplayName("Просрочка (дни)")]
        public string DelayPaymentDays { get; set; }

        #endregion
    }
}
