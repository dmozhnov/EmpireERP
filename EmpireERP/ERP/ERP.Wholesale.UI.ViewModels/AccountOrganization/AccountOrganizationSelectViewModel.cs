
namespace ERP.Wholesale.UI.ViewModels.AccountOrganization
{
    /// <summary>
    /// Модель для модальной формы для выбора организации контрагента
    /// </summary>
    public class AccountOrganizationSelectViewModel
    {
        /// <summary>
        /// Заголовок модальной формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Данные частичного представления с гридом
        /// </summary>
        public AccountOrganizationSelectGridViewModel GridData { get; set; }

        public AccountOrganizationSelectViewModel()
        {
            GridData = new AccountOrganizationSelectGridViewModel();
        }
    }
}