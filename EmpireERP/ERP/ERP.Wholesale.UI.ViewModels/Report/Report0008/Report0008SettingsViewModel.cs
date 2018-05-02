using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    public class Report0008SettingsViewModel
    {
        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Начальная дата отчета
        /// </summary>
        [DisplayName("Период построения отчета")]
        [IsDate(ErrorMessage = "Дата начала диапазона указана неверно")]
        public string StartDate{ get; set; }

        /// <summary>
        /// Конечная дата отчета
        /// </summary>
        [IsDate(ErrorMessage = "Дата конца диапазона указана неверно")]
        public string EndDate { get; set; }

        /// <summary>
        /// До даты
        /// </summary>
        [DisplayName("До даты")]
        [IsDate(ErrorMessage = "Дата в поле «До даты» указана неверно")]
        public string PriorToDate { get; set; }

        /// <summary>
        /// Тип накладных, по которым строится отчет
        /// </summary>
        [DisplayName("Тип накладных")]
        public string WaybillTypeId { get; set; }
        /// <summary>
        /// Доступный перечень типов накладных
        /// </summary>
        public IEnumerable<SelectListItem> WaybillTypeList { get; set; }

        /// <summary>
        /// Тип даты, которая должна попадать в диапозон
        /// </summary>
        [DisplayName("В диапазон должна попадать")]
        public string DateTypeId { get; set; }
        /// <summary>
        /// Доступный перечень типов дат
        /// </summary>
        public IEnumerable<SelectListItem> DateTypeList { get; set; }

        /// <summary>
        /// Признак: исключить ли расхождения
        /// </summary>
        [DisplayName("Исключить расхождения")]
        public string ExcludeDivergences { get; set; }

        /// <summary>
        /// Признак вывода подробной информации
        /// </summary>
        [DisplayName("Выводить развернутую информацию")]
        public string ShowAdditionInfo { get; set; }

        /// <summary>
        /// Настройка состояния накладных для вывода
        /// </summary>
        [DisplayName("Выводить накладные")]
        public string WaybillOptionId { get; set; }
        public IEnumerable<SelectListItem> WaybillOptionList { get; set; }

        /// <summary>
        /// Настройка сортировки накладных по дате
        /// </summary>
        [DisplayName("Сортировать по")]
        public string SortDateTypeId { get; set; }
        public IEnumerable<SelectListItem> SortDateTypeList { get; set; }

        /// <summary>
        /// Список доступных группировок информации
        /// </summary>
        [DisplayName("Добавить группировку информации по")]
        public IEnumerable<SelectListItem> GroupByCollection { get; set; }   
        public string GroupByCollectionIDs { get; set; }

        /// <summary>
        /// Строка кодов мест хранения
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageIDs { get; set; }

        /// <summary>
        /// Список доступных МХ
        /// </summary>
        public Dictionary<string, string> StorageList { get; set; }

        /// <summary>
        /// Признак выбора всех МХ
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllStorages { get; set; }

        /// <summary>
        /// Строка кодов клиентов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ClientIDs { get; set; }

        /// <summary>
        /// Признак выбора всех клиентов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllClients{ get; set; }

        /// <summary>
        /// Строка кодов поставщиков
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ProviderIDs { get; set; }

        /// <summary>
        /// Признак выбора всех поставщиков
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllProviders { get; set; }

        /// <summary>
        /// Строка кодов кураторов накладной
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CuratorIDs { get; set; }

        /// <summary>
        /// Список доступных кураторов накладной
        /// </summary>
        public Dictionary<string, string> CuratorList { get; set; }

        /// <summary>
        /// Признак выбора всех кураторов
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AllCurators { get; set; }
    }
}
