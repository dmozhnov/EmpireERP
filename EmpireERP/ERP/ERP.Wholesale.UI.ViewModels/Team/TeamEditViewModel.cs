using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class TeamEditViewModel
    {
        public short Id { get; set; }
        public string Title { get; set; }
        public string BackURL { get; set; }

        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        [DisplayName("Название команды")]
        [Required(ErrorMessage = "Укажите название команды")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }
    }
}
