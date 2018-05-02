using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.DealPaymentDocument
{
    /// <summary>
    /// Модель модальной формы для ручного разнесения платежного документа
    /// Все поля, которые приходят с предыдущей формы (редактирования), здесь не валидируются, ставится только ConvertEmptyStringToNull
    /// </summary>
    public class BaseDestinationDocumentSelectViewModel
    {
        /// <summary>
        /// Контроллер, принимающий POST запрос
        /// </summary>
        public string DestinationDocumentSelectorControllerName { get; set; }

        /// <summary>
        /// Метод контроллера, принимающий POST запрос
        /// </summary>
        public string DestinationDocumentSelectorActionName { get; set; }
        
        /// <summary>
        /// Заголовок модальной формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код команды, для которой создается оплата
        /// </summary>
        [DisplayName("Команда")]
        [Required(ErrorMessage = "Укажите команду")]
        public short TeamId { get; set; }

        /// <summary>
        /// Список доступных команд
        /// </summary>
        public IEnumerable<SelectListItem> TeamList { get; set; }

        /// <summary>
        /// Код платежного документа (не установлен при создании, установлен при переразнесении)
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DealPaymentDocumentId { get; set; }

        /// <summary>
        /// Введенная на предыдущем этапе сумма
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SumValue { get; set; }

        /// <summary>
        /// Нераспределенная сумма
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UndistributedSumValue { get; set; }
        
        /// <summary>
        /// Текущий внутренний порядковый номер, который будет присвоен следующему документу, выбранному пользователем
        /// </summary>
        public int CurrentOrdinalNumber { get; set; }

        /// <summary>
        /// Данные грида доступных к разнесению накладных реализации
        /// </summary>
        public GridData SaleWaybillGridData { get; set; }

        /// <summary>
        /// Данные грида доступных к разнесению дебетовых корректировок сальдо
        /// </summary>
        public GridData DealDebitInitialBalanceCorrectionGridData { get; set; }

        /// <summary>
        /// Информация о распределении по документам (устанавливается флажками в гридах)
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DistributionInfo { get; set; }

        public BaseDestinationDocumentSelectViewModel()
        {
            SaleWaybillGridData = new GridData();
            DealDebitInitialBalanceCorrectionGridData = new GridData();
            TeamList = new List<SelectListItem>();
        }
    }
}