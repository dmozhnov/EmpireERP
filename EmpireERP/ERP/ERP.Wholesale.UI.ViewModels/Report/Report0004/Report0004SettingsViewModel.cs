using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0004
{
    public class Report0004SettingsViewModel
    {
        public string BackURL { get; set; }

        [DisplayName("Товар, движение которого отобразить")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Нужна ли сводная таблица сальдо по МХ на начало периода
        /// </summary>
        [DisplayName("Сальдо на начало по местам хранения?")]
        public string ShowStartQuantityByStorage { get; set; }

        /// <summary>
        /// Нужна ли сводная таблица сальдо по организациям на начало периода
        /// </summary>
        [DisplayName("Сальдо на начало по организациям?")]
        public string ShowStartQuantityByOrganization { get; set; }

        /// <summary>
        /// Нужна ли сводная таблица сальдо по МХ на конец периода
        /// </summary>
        [DisplayName("Сальдо на конец по местам хранения?")]
        public string ShowEndQuantityByStorage { get; set; }

        /// <summary>
        /// Нужна ли сводная таблица сальдо по организациям на конец периода
        /// </summary>
        [DisplayName("Сальдо на конец по организациям?")]
        public string ShowEndQuantityByOrganization { get; set; }

        /// <summary>
        /// Определяет вывод столбца "Партия" в отчете.
        /// Если стоит нет, то в случае когда в одной накладной (кроме прихода) указанный товар участвует двумя позициями (по разным приходным партиям), 
        /// то такие позиции объединяются в одну строку отчета, а количество движения выводится суммарно по всем строчкам данного товара в накладной.
        /// Если стоит да, то для каждой строчки каждой накладной выводится отдельная строка отчета.
        /// </summary>
        [DisplayName("Указать партии товаров?")]
        public string ShowBatches { get; set; }

        /// <summary>
        /// Регулируется правом "Просмотр ЗЦ", определяет вывод столбца "ЗЦ" в отчете. 
        /// Если стоит нет, то таблица отчета "Приходы" не расширяется соответствующим столбцом, иначе - расширяется.
        /// </summary>
        [DisplayName("Указать ЗЦ в приходах?")]
        public string ShowPurchaseCosts { get; set; }       

        /// <summary>
        /// Определяет вывод столбца "УЦ приемки".
        /// Регулируется правом "Просмотр УЦ получателя" если МХ-получатель не командный.
        /// </summary>
        [DisplayName("Указать УЦ приемки (входящие накладные)?")]
        public string ShowRecipientAccountingPrices { get; set; }

        /// <summary>
        /// Определяет вывод столбца "УЦ отправки".
        /// Регулируется правом "Просмотр УЦ отправителя" если МХ-получатель не командный.
        /// </summary>
        [DisplayName("Указать УЦ отправки (исходящие накладные)?")]
        public string ShowSenderAccountingPrices { get; set; }

        /// <summary>
        /// Определяет, будут ли попадать в отчет все накладные или только накладные, составляющие точно наличие.
        /// </summary>
        [DisplayName("Только закрытые накладные?")]
        public string ShowOnlyExactAvailability { get; set; }

        /// <summary>
        /// Список мест хранения
        /// </summary>
        public Dictionary<string, string> Storages { get; set; }

        /// <summary>
        /// Строка кодов выбранных мест хранения
        /// </summary>
        public string StorageIDs { get; set; }
        public string AllStorages { get; set; }

        /// <summary>
        /// Дата начала отчета
        /// </summary>
        [DisplayName("с")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string StartDate { get; set; }

        /// <summary>
        /// Дата конца отчета
        /// </summary>
        [DisplayName("по")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string EndDate { get; set; }

        /// <summary>
        /// Разрешено ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        /// <summary>
        /// Разрешено ли просматривать учетные цены отправки
        /// </summary>
        public bool AllowToViewSenderAccountingPrices { get; set; }

        /// <summary>
        /// Разрешено ли просматривать учетные цены приемки
        /// </summary>
        public bool AllowToViewRecipientAccountingPrices { get; set; }

        public Report0004SettingsViewModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString();
            EndDate = DateTime.Now.Date.ToShortDateString();

            ShowStartQuantityByStorage = "1";
            ShowStartQuantityByOrganization = "1";
            ShowEndQuantityByStorage = "1";
            ShowEndQuantityByOrganization = "1";
            
            ShowBatches = ShowPurchaseCosts = ShowRecipientAccountingPrices = ShowSenderAccountingPrices = ShowOnlyExactAvailability = "0";
        }
    }
}
