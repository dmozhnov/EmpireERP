using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    /// <summary>
    /// Базовая модель для трех документов: возврата оплаты клиенту, дебетовая корректировка и кредитовая корректировка сальдо
    /// </summary>
    public class BaseDealPaymentDocumentEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код комадны оплаты
        /// </summary>
        [DisplayName("Команда")]
        [Required(ErrorMessage = "Укажите команду")]
        public short TeamId { get; set; }

        /// <summary>
        /// Список доступных команд
        /// </summary>
        public IEnumerable<SelectListItem> TeamList { get; set; }

        /// <summary>
        /// Организация клиента
        /// </summary>
        [Required(ErrorMessage = "Укажите организацию клиента")]
        [GreaterByConst(0, ErrorMessage = "Укажите организацию клиента")]
        public int ClientOrganizationId { get; set; }
        [DisplayName("Организация клиента")]
        public string ClientOrganizationName { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [Required(ErrorMessage = "Укажите клиента")]
        [GreaterByConst(0, ErrorMessage = "Укажите клиента")]
        public int ClientId { get; set; }
        [DisplayName("Клиент")]
        public string ClientName { get; set; }

        /// <summary>
        /// Сделка
        /// </summary>
        [Required(ErrorMessage = "Укажите сделку")]
        [GreaterByConst(0, ErrorMessage = "Укажите сделку")]
        public int DealId { get; set; }
        [DisplayName("Сделка")]
        public string DealName { get; set; }

        /// <summary>
        /// Показывать ли строчку с названием организации клиента (в данных трех документах она никогда не является ссылкой для выбора)
        /// </summary>
        public bool AllowToViewClientOrganization { get; set; }

        /// <summary>
        /// Показывать ли строчку с названием клиента
        /// </summary>
        public bool AllowToViewClient { get; set; }

        /// <summary>
        /// Делать ли название клиента ссылкой для выбора
        /// </summary>
        public bool AllowToChooseClient { get; set; }

        /// <summary>
        /// Делать ли название сделки ссылкой для выбора (строчка с названием сделки видима всегда)
        /// </summary>
        public bool AllowToChooseDeal { get; set; }

        /// <summary>
        /// Создается документ по клиенту или по организации клиента (влияет на список сделок для выбора)
        /// </summary>
        public bool IsDealSelectedByClient { get; set; }
    }
}
