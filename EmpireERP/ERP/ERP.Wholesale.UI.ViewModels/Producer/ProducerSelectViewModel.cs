using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Producer
{
    /// <summary>
    /// Модель модальной формы для выбора производителя
    /// </summary>
    public class ProducerSelectViewModel
    {
        /// <summary>
        /// Данные грида
        /// </summary>
        public GridData GridData { get; set; }

        /// <summary>
        /// Заголовок грида
        /// </summary>
        public string Title { get; set; }

        public ProducerSelectViewModel()
        {
            GridData = new GridData();
        }
    }
}