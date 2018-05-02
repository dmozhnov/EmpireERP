using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate
{
    public class ProductionOrderBatchLifeCycleTemplateEditViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название шаблона жизненного цикла заказа
        /// </summary>
        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название шаблона")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }
    }
}