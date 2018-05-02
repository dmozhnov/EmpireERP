using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ERP.Utils;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0008
{
    /// <summary>
    /// Общая для всех накладных модель-представление. В зависимости от типа накладных используется разное подмножество полей.
    /// </summary>
    /// <remarks>Данное сомнительное архитектурное решение продиктовано стремлением уменьшить дублирование кода</remarks>
    public class Report0008_WaybillItemViewModel
    {
        /// <summary>
        /// Номер накладной
        /// </summary>
        [DisplayName("Номер")]
        public string Number { get; set; }

        /// <summary>
        /// Дата накладной
        /// </summary>
        [DisplayName("Дата")]
        public string Date { get; set; }

        /// <summary>
        /// Сумма в закупочных ценах
        /// </summary>
        public decimal? PurchaseCostSum { get; set; }
        /// <summary>
        /// Сумма в закупочных ценах строкой
        /// </summary>
        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSumString 
        { 
            get 
            {
                return PurchaseCostSum.ForDisplay(ValueDisplayType.Money);
            } 
        }
        
        /// <summary>
        /// Сумма в учетных ценах
        /// </summary>
        public decimal? AccountingPriceSum { get; set; }
        /// <summary>
        /// Сумма в учетных ценах строкой
        /// </summary>
        [DisplayName("Сумма в УЦ")]
        public string AccountingPriceSumString
        {
            get
            {
                return AccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            }
        }
        
        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        public decimal? SalePriceSum { get; set; }
        /// <summary>
        /// Сумма в отпускных ценах строкой
        /// </summary>
        [DisplayName("Сумма в ОЦ")]
        public string SalePriceSumString
        {
            get
            {
                return SalePriceSum.ForDisplay(ValueDisplayType.Money);
            }
        }


        /// <summary>
        /// Сумма возвратов
        /// </summary>
        public decimal? ReturnFromClientSum { get; set; }
        /// <summary>
        /// Сумма возвратов строкой
        /// </summary>
        [DisplayName("Сумма возвратов")]
        public string ReturnFromClientSumString
        {
            get
            {
                return ReturnFromClientSum.ForDisplay(ValueDisplayType.Money);
            }
        }

        /// <summary>
        /// Сумма в учетных ценах отправителя
        /// </summary>
        public decimal? SenderAccountingPriceSum { get; set; }
        /// <summary>
        /// Сумма в учетных ценах отправителя строкой
        /// </summary>
        [DisplayName("Сумма в УЦ отправителя")]
        public string SenderAccountingPriceSumString
        {
            get
            {
                return SenderAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            }
        }

        /// <summary>
        /// Сумма в учетных ценах получателя
        /// </summary>
        public decimal? RecipientAccountingPriceSum { get; set; }
        /// <summary>
        /// Сумма в учетных ценах получателя строкой
        /// </summary>
        [DisplayName("Сумма в УЦ получателя")]
        public string RecipientAccountingPriceSumString
        {
            get
            {
                return RecipientAccountingPriceSum.ForDisplay(ValueDisplayType.Money);
            }
        }

        /// <summary>
        /// Название статуса накладной
        /// </summary>
        [DisplayName("Статус")]
        public string StateName { get; set; }

        /// <summary>
        /// Название поставщика
        /// </summary>
        [DisplayName("Поставщик")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Название МХ-приемщика
        /// </summary>
        [DisplayName("МХ-приемщик")]
        public string RecipientStorageName { get; set; }

        /// <summary>
        /// Название МХ отправителя
        /// </summary>
        [DisplayName("МХ-отправитель")]
        public string SenderStorageName { get; set; }

        /// <summary>
        /// Название МХ
        /// </summary>
        [DisplayName("Место хранения")]
        public string StorageName { get; set; }

        /// <summary>
        /// Название организации-приемщика
        /// </summary>
        [DisplayName("Организация-приемщик")]
        public string RecipientAccountOrganizationName { get; set; }

        /// <summary>
        /// Название организации отправителя
        /// </summary>
        [DisplayName("Организация-отправитель")]
        public string SenderAccountOrganizationName { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        [DisplayName("Организация")]
        public string AccountOrganizationName { get; set; }

        /// <summary>
        /// Название клиента
        /// </summary>
        [DisplayName("Клиент")]
        public string ClientName { get; set; }

        /// <summary>
        /// Договор
        /// </summary>
        [DisplayName("Договор")]
        public string Contract { get; set; }

        /// <summary>
        /// Квота
        /// </summary>
        [DisplayName("Квота")]
        public string Quota { get; set; }

        /// <summary>
        /// Накладная поставщика
        /// </summary>
        [DisplayName("Накладная поставщика")]
        public string ProviderWaybillName { get; set; }

        /// <summary>
        /// Счет-фактура поставщика
        /// </summary>
        [DisplayName("Счет-фактура поставщика")]
        public string ProviderInvoice { get; set; }

        /// <summary>
        /// Название основания для списания
        /// </summary>
        [DisplayName("Основание")]
        public string WriteoffReasonName { get; set; }

        /// <summary>
        /// Движение накладной
        /// </summary>
        [DisplayName("Движение накладной")]
        public string WaybillStateHistory { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [AllowHtml]
        public string Comment { get; set; }

        /// <summary>
        /// Основание для возврата
        /// </summary>
        [DisplayName("Основание")]
        public string ReturnFromClientReasonName { get; set; }

        /// <summary>
        /// Имя куратора накладной
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }

        /// <summary>
        /// Это заголовок для группировки?
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// Уровень группировки
        /// </summary>
        public int GroupLevel { get; set; }
    }
}
