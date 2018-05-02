namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008ViewModel : BaseReportViewModel
    {        
        /// <summary>
        /// Начало интервала построения отчета
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Конец интервала построения отчета
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Статусы выведенных накладных
        /// </summary>
        public string ShownStates { get; set; }

        /// <summary>
        /// Код типа накладных отчета
        /// </summary>
        public int WaybillType { get; set; }

        /// <summary>
        /// Название типа накладных по которым строится отчет
        /// </summary>
        public string WaybillTypeName { get; set; }

        /// <summary>
        /// Значение поля "До даты"
        /// </summary>
        public string PriorToDateString { get; set; }

        /// <summary>
        /// Название типа даты
        /// </summary>
        public string DateTypeName { get; set; }

        /// <summary>
        /// Название типа даты для сортировки
        /// </summary>
        public string SortDateTypeName { get; set; }

        /// <summary>
        /// Модель для приходов
        /// </summary>
        public Report0008_ReceiptWaybillTableViewModel ReceiptWaybillModel { get; set; }

        /// <summary>
        /// Модель для внутреннего перемещения
        /// </summary>
        public Report0008_MovementWaybillTableViewModel MovementWaybillModel { get; set; }

        /// <summary>
        /// Модель для смены собственника
        /// </summary>
        public Report0008_ChangeOwnerWaybillTableViewModel ChangeOwnerWaybillModel { get; set; }

        /// <summary>
        /// Модель для списания
        /// </summary>
        public Report0008_WriteoffWaybillTableViewModel WriteoffWaybillModel { get; set; }

        /// <summary>
        /// Модель для реализации
        /// </summary>
        public Report0008_ExpenditureWaybillTableViewModel ExpenditureWaybillModel { get; set; }

        /// <summary>
        /// Модель для возвратов
        /// </summary>
        public Report0008_ReturnFromClientWaybillTableViewModel ReturnFromClientWaybillModel { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0008ViewModel()
        {
            ReceiptWaybillModel = new Report0008_ReceiptWaybillTableViewModel();
            MovementWaybillModel = new Report0008_MovementWaybillTableViewModel();
            ChangeOwnerWaybillModel = new Report0008_ChangeOwnerWaybillTableViewModel();
            WriteoffWaybillModel = new Report0008_WriteoffWaybillTableViewModel();
            ExpenditureWaybillModel = new Report0008_ExpenditureWaybillTableViewModel();
            ReturnFromClientWaybillModel = new Report0008_ReturnFromClientWaybillTableViewModel();
        }
    }
}
