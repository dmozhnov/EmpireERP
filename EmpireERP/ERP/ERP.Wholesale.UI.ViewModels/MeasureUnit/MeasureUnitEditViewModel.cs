using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.MeasureUnit
{
    public class MeasureUnitEditViewModel
    {
        [DisplayName("Код")]
        public virtual short Id { get; set; }

        [DisplayName("Полное наименование")]
        [Required(ErrorMessage = "Укажите полное наименование")]
        [StringLength(20, ErrorMessage = "Не более {1} символов")]
        public virtual string FullName { get; set; }

        [DisplayName("Краткое наименование")]
        [Required(ErrorMessage = "Укажите краткое наименование")]
        [StringLength(7, ErrorMessage = "Не более {1} символов")]
        public virtual string ShortName { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(500, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public virtual string Comment { get; set; }

        [DisplayName("Кол-во знаков после запятой")]
        [Required(ErrorMessage = "Укажите количество знаков")]
        [RegularExpression("[0-9]{1,2}", ErrorMessage = "Укажите целое число")]
        public virtual byte Scale { get; set; }

        [DisplayName("Цифровой код")]
        [Required(ErrorMessage = "Укажите цифровой код")]
        [StringLength(3, ErrorMessage = "Не более {1} символов")]
        public virtual string NumericCode { get; set; }

        public string Title { get; set; }

        public bool AllowToEdit { get; set; }

        public MeasureUnitEditViewModel()
        {
            FullName = string.Empty;
            ShortName = string.Empty;
            Comment = string.Empty;
            Scale = 0;
        }
    }
}