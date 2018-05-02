using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealQuota
{
    public class DealQuotaEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор квоты
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название квоты
        /// </summary>              
        [Required(ErrorMessage="Укажите название квоты")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [DisplayName("Название квоты")]
        public string Name { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>                
        [DisplayName("Дата начала действия")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата завершения действия
        /// </summary>                
        [DisplayName("Дата завершения действия")]
        public string EndDate { get; set; }

        /// <summary>
        /// Клиентская скидка (%)
        /// </summary>                
        [DisplayName("Клиентская скидка")]
        [Required(ErrorMessage = "Укажите клиентскую скидку")]
        [RegularExpression(@"[0-9]{1,3}([,.][0-9]{1,2})?", ErrorMessage = "Введите положительное число от 0 до 100, не более 2 знаков после запятой")]
        public string DiscountPercent { get; set; }

        /// <summary>
        /// Срок отсрочки платежа
        /// </summary>                
        [DisplayName("Срок отсрочки платежа")]
        [RegularExpression(@"[0-9]*", ErrorMessage = "Введите целое число")]
        [RequiredByRadioButton("IsPrepayment", 0, ErrorMessage="Укажите срок отсрочки")]
        [Range(0, 32767, ErrorMessage = "Введите число не больше 32 767")]
        public string PostPaymentDays { get; set; }

        /// <summary>
        /// Максимальный кредитный лимит
        /// </summary>                
        [DisplayName("Макс. кредитный лимит")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Введите положительное число, не более 2 знаков после запятой")]
        [RequiredByRadioButton("IsPrepayment", 0, ErrorMessage = "Укажите кредитный лимит")]
        public string CreditLimitSum { get; set; }

        /// <summary>
        /// Действующая ли квота
        /// </summary>
        [DisplayName("Действует")]
        public string IsActive { get; set; }

        /// <summary>
        /// Форма взаиморасчетов (1 - по предоплате, 0 - нет)
        /// </summary>        
        public byte IsPrepayment { get; set; }

        /// <summary>
        /// Опция, соответствующая значению "false" радиокнопки выбора формы взаиморасчетов
        /// </summary>
        public readonly string IsPrepayment_false;

        /// <summary>
        /// Опция, соответствующая значению "true" радиокнопки выбора формы взаиморасчетов
        /// </summary>
        public readonly string IsPrepayment_true;

        public bool AllowToEdit { get; set; }
        public bool IsPossibilityToEdit { get; set; }
        
        public DealQuotaEditViewModel()
        {
            CreditLimitSum = "0";
            PostPaymentDays = "0";
            
            IsPrepayment_false = "Отсрочка платежа";
            IsPrepayment_true = "Предоплата";

            IsActive = "1";
        }
    }
}