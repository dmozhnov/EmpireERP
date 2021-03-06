﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Misc.ExportTo1CDataModels
{
    /// <summary>
    /// Класс для выгрузки в 1С накладной перемещения товаров
    /// </summary>
    public class MovementWaybillExportTo1CDataModel
    {
        /// <summary>
        /// Ид места хранения-отправителя
        /// </summary>
        public short SenderStorageId { get; set; }

        /// <summary>
        /// Название места хранения-отправителя
        /// </summary>
        public string SenderStorageName { get; set; }

        /// <summary>
        /// Ид места хранения-получателя
        /// </summary>
        public short RecipientStorageId { get; set; }

        /// <summary>
        /// Название места хранения-получателя
        /// </summary>
        public string RecipientStorageName { get; set; }

        /// <summary>
        /// Название собственной организации
        /// </summary>
        public string SenderShortName { get; set; }

        /// <summary>
        /// Полное название собственной организации
        /// </summary>
        public string SenderFullName { get; set; }

        /// <summary>
        /// ИНН собственной организации
        /// </summary>
        public string SenderINN { get; set; }

        /// <summary>
        /// КПП юридического лица (если собственная организация - юридическое лицо)
        /// </summary>
        public string SenderKPP { get; set; }

        /// <summary>
        ///Признак принадлежности товара продавцу (1 - товар принадлежит продавцу, 
        ///0 - товар взят на комиссию)
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Номер накладной перемещения
        /// </summary>
        public string MovementWaybillNumber { get; set; }

        /// <summary>
        /// Дата накладной перемещения
        /// </summary>
        public DateTime MovementWaybillDate { get; set; }

        /// <summary>
        /// Сумма по накладной перемещения
        /// </summary>
        public decimal MovementWaybillSalePriceSum { get; set; }

        /// <summary>
        /// Название группы товаров 
        /// </summary>
        public string ArticleGroupName { get; set; }

        /// <summary>
        /// Количество товара по позиции накладной (по группе товаров)
        /// </summary>
        public decimal ArticleCount { get; set; }

        /// <summary>
        ///Средняя отпускная цена единицы товара по группе
        /// </summary>
        public decimal AccountingPrice { get; set; }

        /// <summary>
        /// Сумма с НДС по группе товаров
        /// </summary>
        public decimal AccountingSum { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public decimal ValueAddedTax { get; set; }

        /// <summary>
        /// Сумма НДС по группе товаров
        /// </summary>
        public decimal ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Числовой код единицы измерения
        /// </summary>
        public string MeasureUnitNumericCode { get; set; }

        /// <summary>
        /// Краткое название единицы измерения
        /// </summary>
        public string MeasureUnitShortName { get; set; }

    }
}
