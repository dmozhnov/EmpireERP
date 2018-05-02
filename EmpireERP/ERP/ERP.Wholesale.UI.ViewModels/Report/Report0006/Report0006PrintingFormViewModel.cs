using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Модель акта сверки взаиморасчетов по одной собственной организации
    /// </summary>
    public class Report0006PrintingFormViewModel
    {
        /// <summary>
        /// Имя собственной организации
        /// </summary>
        public string AccountOrganizationName { get; set; }

        /// <summary>
        /// Является ли собственная организация юридическим лицом
        /// </summary>
        public bool IsJuridicalPerson { get; set; }

        #region Поля для юридического лица

        /// <summary>
        /// Должность директора
        /// </summary>
        public string DirectorPost { get; set; }

        /// <summary>
        /// ФИО директора
        /// </summary>
        public string DirectorName { get; set; }

        /// <summary>
        /// ФИО гл. бухгалтера
        /// </summary>
        public string MainBookkeeperName { get; set; }

        #endregion

        #region Поля для физического лица

        /// <summary>
        /// ФИО физического лица
        /// </summary>
        public string OwnerName { get; set; }

        #endregion

        /// <summary>
        /// Таблица 1 - "Состояние взаимных расчетов по данным учета"
        /// </summary>
        public List<Report0006BalanceItemViewModel> BalanceDocumentSummary;

        /// <summary>
        /// Таблица 2 - "Развернутая информация по документам учета"
        /// </summary>
        public List<Report0006BalanceItemViewModel> BalanceDocumentFullInfo;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Report0006PrintingFormViewModel()
        {
            BalanceDocumentSummary = new List<Report0006BalanceItemViewModel>();
            BalanceDocumentFullInfo = new List<Report0006BalanceItemViewModel>();
        }
    }
}
