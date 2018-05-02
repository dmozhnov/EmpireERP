using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Export._1C
{
    public class ExportTo1CSettingsViewModel
    {       
        /// <summary>
        /// Начальная дата периода выгрузки
        /// </summary>
        [DisplayName("Период выгрузки")]
        [IsDate(ErrorMessage = "Дата начала периода указана неверно")]
        public string StartDate { get; set; }

        /// <summary>
        /// Конечная дата периода выгрузки
        /// </summary>
        [IsDate(ErrorMessage = "Дата конца периода указана неверно")]
        public string EndDate { get; set; }

        /// <summary>
        /// Тип операции
        /// </summary>
        [DisplayName("Тип операции")]
        [Required(ErrorMessage = "Выберите тип операции")]
        public string OperationTypeId { get; set; }
        public IEnumerable<SelectListItem> OperationTypes { get; set; }

        /// <summary>
        /// Список собственных организаций для которых выгружаются данные
        /// </summary>
        public Dictionary<string, string> AccountOrganizationList { get; set; }

        /// <summary>
        /// Строка кодов организаций для которых выгружаются данные
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AccountOrganizationIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllAccountOrganizations { get; set; }

        #region Реализация
        /// <summary>
        /// Добавить в выгрузку по реализации передачу на комиссию
        /// </summary>
        [DisplayName("Добавить в выгрузку передачу на комиссию")]
        public string AddTransfersToCommission { get; set; }

        /// <summary>
        /// Строка кодов организаций передачу на комиссию которым нужно выгрузить
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CommissionaireOrganizationsIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllCommissionaireOrganizations { get; set; } 
        #endregion

        #region Возвраты
        /// <summary>
        /// Добавить в выгрузку по возвратам принятые от комиссионеров
        /// </summary>
        [DisplayName("Добавить в выгрузку возвраты, принятые от комиссионеров")]
        public string AddReturnsFromCommissionaires { get; set; }

        /// <summary>
        /// Строка кодов организаций комиссионеров возвраты от которых надо добавить в выгрузку по возвратам
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReturnsFromCommissionairesOrganizationsIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllReturnsFromCommissionairesOrganizations { get; set; }

        /// <summary>
        /// Добавить в выгрузку по возвратам возвраты принятые комиссионерами
        /// </summary>
        [DisplayName("Добавить в выгрузку возвраты, принятые комиссионерами")]
        public string AddReturnsAcceptedByCommissionaires { get; set; }

        /// <summary>
        /// Строка кодов организаций комиссионеров по которым необходимо добавить возвраты клиентов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReturnsAcceptedByCommissionairesOrganizationsIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllReturnsAcceptedByCommissionairesOrganizations { get; set; } 
        #endregion

        #region Поступления на комиссию
        /// <summary>
        /// Строка кодов организаций которые передают товар на комиссию
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ConsignorOrganizationsIDs { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllConsignorOrganizations { get; set; } 
        #endregion

        public ExportTo1CSettingsViewModel()
        {
            AddReturnsAcceptedByCommissionaires = "0";
            AddReturnsFromCommissionaires = "0";
            AddTransfersToCommission = "0";
        }
    }

}
