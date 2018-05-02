using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Deal
{
    public class DealEditViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название сделки
        /// </summary>     
        [DisplayName("Название сделки")]
        [Required(ErrorMessage="Укажите название сделки")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }
        
        [Required(ErrorMessage = "Укажите клиента")]
        public string ClientId { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }
        [Required(ErrorMessage = "Укажите куратора")]
        public string CuratorId { get; set; }

        /// <summary>
        /// Ожидаемый бюджет
        /// </summary>
        [DisplayName("Ожидаемый бюджет")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Введите положительное число, не более 2 знаков после запятой")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ExpectedBudget { get; set; }

        /// <summary>
        /// Этап
        /// </summary>
        [DisplayName("Этап")]
        public string StageName { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }
    }
}