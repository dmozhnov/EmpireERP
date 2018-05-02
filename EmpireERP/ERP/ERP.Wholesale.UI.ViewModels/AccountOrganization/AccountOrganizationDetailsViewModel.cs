using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.AccountOrganization
{
    public class AccountOrganizationDetailsViewModel
    {
        /// <summary>
        /// Название собственной организации
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Таблица расчетных счетов
        /// </summary>
        public GridData BankAccountGrid { get; set; }

        /// <summary>
        /// Таблица расчетных счетов
        /// </summary>
        public GridData ForeignBankAccountGrid { get; set; }

        /// <summary>
        /// Таблица связанных мест хранения
        /// </summary>
        public GridData StorageGrid { get; set; }

        /// <summary>
        /// Шапка с деталями собственной организации
        /// </summary>
        public AccountOrganizationMainDetailsViewModel MainDetails { get; set; }

        /// <summary>
        /// Идентификатор собственной организации
        /// </summary>
        public int AccountOrganizationId { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToDelete { get; set; }
       
    }
}