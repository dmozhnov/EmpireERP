using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using System;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderEditViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string BackUrl { get; set; }

        [DisplayName("Название заказа")]
        [Required(ErrorMessage = "Укажите название заказа")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        [DisplayName("Куратор")]
        [Required(ErrorMessage = "Укажите куратора")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите куратора")]
        public string CuratorId { get; set; }

        public string CuratorName { get; set; }

        [DisplayName("Производитель")]
        [Required(ErrorMessage = "Укажите производителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите производителя")]
        public string ProducerId { get; set; }

        public string ProducerName { get; set; }

        [DisplayName("Валюта")]
        [Required(ErrorMessage = "Укажите валюту")]
        [Range(1, short.MaxValue, ErrorMessage = "Укажите валюту")]
        public short CurrencyId { get; set; }
        public IEnumerable<SelectListItem> CurrencyList { get; set; }

        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите место хранения")]
        [Range(1, short.MaxValue, ErrorMessage = "Укажите место хранения")]
        public short StorageId { get; set; }
        public IEnumerable<SelectListItem> StorageList { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        [DisplayName("Дата начала")]
        public string Date { get; set; }

        /// <summary>
        /// Название текущего этапа
        /// </summary>
        [DisplayName("Этап")]
        public string CurrentStageName { get; set; }

        [DisplayName("Длительность")]
        [RegularExpression("[0-9]{0,3}", ErrorMessage = "Введите целое число (до 3 цифр)")]
        [Range(0, 365, ErrorMessage = "Длительность этапа «Создание заказа» должна быть целым числом, от 0 до 365 дней.")]
        //[Required(ErrorMessage = "Укажите длительность или конечную дату этапа")]
        public string SystemStagePlannedDuration { get; set; }

        /// <summary>
        /// Дата завершения системного этапа "Расчет заказа"
        /// </summary>
        [DisplayName("Дата завершения")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string SystemStagePlannedEndDate { get; set; }

        /// <summary>
        /// Способ расчета закупочных цен (а именно транспортных расходов) в приходах, созданных по данному заказу
        /// </summary>
        [DisplayName("Способ расчета ЗЦ")]
        [Required(ErrorMessage = "Укажите способ расчета ЗЦ")]
        public string ArticleTransportingPrimeCostCalculationType { get; set; }

        /// <summary>
        /// Перечень возможных способов расчета закупочных цен
        /// </summary>
        [DisplayName("по транспортировке в связанных приходах")]
        public IEnumerable<SelectListItem> ArticleTransportingPrimeCostCalculationTypeList { get; set; }

        /// <summary>
        /// Является ли конкретный день недели рабочим днем
        /// </summary>
        [DisplayName("Рабочие дни")]
        public bool MondayIsWorkDay { get; set; }
        public bool TuesdayIsWorkDay { get; set; }
        public bool WednesdayIsWorkDay { get; set; }
        public bool ThursdayIsWorkDay { get; set; }
        public bool FridayIsWorkDay { get; set; }
        public bool SaturdayIsWorkDay { get; set; }
        public bool SundayIsWorkDay { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Разрешено ли выбирать куратора
        /// </summary>
        public bool AllowToChangeCurator { get; set; }

        /// <summary>
        /// Разрешено ли изменять производителя
        /// </summary>
        public bool AllowToChangeProducer { get; set; }

        /// <summary>
        /// Разрешено ли изменять валюту
        /// </summary>
        public bool AllowToChangeCurrency { get; set; }

        /// <summary>
        /// Разрешено ли изменять способ расчета закупочных цен (а именно транспортных расходов) в приходах
        /// </summary>
        public bool AllowToChangeArticleTransportingPrimeCostCalculationType { get; set; }

        /// <summary>
        /// Разрешено ли изменять место хранения
        /// </summary>
        public bool AllowToChangeStorage { get; set; }

        /// <summary>
        /// Показывать ли название текущего этапа (если он один)
        /// </summary>
        public bool ShowCurrentStageName { get; set; }

        /// <summary>
        /// Разрешено ли редактировать график рабочих дней в заказе
        /// </summary>
        public bool AllowToEditWorkDaysPlan { get; set; }

        /// <summary>
        /// Разрешено ли редактировать поля с информацией о системном этапе
        /// </summary>
        public bool AllowToEditSystemStage { get; set; }

        /// <summary>
        /// Разрешено ли редактировать вообще
        /// </summary>
        public bool AllowToEdit { get; set; }
    }
}
