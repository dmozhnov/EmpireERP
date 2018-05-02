using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ContractorOrganization
{
    /// <summary>
    /// Модель для модальной формы для выбора организации контрагента
    /// </summary>
    public class ContractorOrganizationSelectViewModel
    {
        /// <summary>
        /// Заголовок модальной формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Текст на ссылке "Создать новую организацию"
        /// </summary>
        public string NewOrganizationLinkName { get; set; }

        public bool AllowToCreateNewOrganization { get; set; }

        /// <summary>
        /// Код контрагента
        /// </summary>
        public string ContractorId { get; set; }

        #region Вызываемые в представлении методы

        /// <summary>
        /// Метод контроллера, вызываемый по методу GET для получения формы создания организации
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Имя контроллера, вызываемого по методу GET для получения формы создания организации
        /// </summary>
        public string ControllerName { get; set; }

        #endregion

        /// <summary>
        /// Данные частичного представления с гридом
        /// </summary>
        public GridData GridData { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }
        
    }
}