using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    /// <summary>
    /// Модель-представление для формы создания партии заказа
    /// </summary>
    public class ProductionOrderBatchEditViewModel
    {
        /// <summary>
        /// Идентификатор партии
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Заголовок модального окна
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public string ProdactionOrderId { get; set; }

        /// <summary>
        /// Название партии
        /// </summary>
        [DisplayName("Название партии")]
        [Required(ErrorMessage = "Укажите название партии.")]
        [StringLength(200, ErrorMessage = "Не более {1} символов.")]
        public string Name {get; set;}

        /// <summary>
        /// Длительность этапа "Создание заказа"
        /// </summary>
        [DisplayName("Длительность этапа «Создание»")]
        [Required(ErrorMessage = "Укажите длительность этапа.")]
        [RegularExpression("[0-9]{0,3}", ErrorMessage = "Введите целое число (до 3 цифр).")]
        [Range(0, 365, ErrorMessage = "Длительность этапа «Создание» должна быть целым числом, от 0 до 365 дней.")]
        public string SystemStagePlannedDuration { get; set; }
    }
}
