using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ArticleCertificate
{
    public class ArticleCertificateEditViewModel
    {
        public virtual int Id { get; set; }

        [DisplayName("Название")]
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(500, ErrorMessage = "Не более {1} символов")]
        public virtual string Name { get; set; }

        [DisplayName("Дата начала действия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string StartDate { get; set; }

        [DisplayName("Дата завершения действия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string EndDate { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Разрешено ли редактировать
        /// </summary>
        public bool AllowToEdit { get; set; }
    }
}