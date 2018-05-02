using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.AccountOrganization
{
    /// <summary>
    /// Модель грида модальной формы для выбора организации контрагента
    /// </summary>
    public class AccountOrganizationSelectGridViewModel
    {
        /// <summary>
        /// Данные грида
        /// </summary>
        public GridData GridData { get; set; }

        /// <summary>
        /// Заголовок грида
        /// </summary>
        public string Title { get; set; }

        public AccountOrganizationSelectGridViewModel()
        {
            GridData = new GridData();
        }
    }
}