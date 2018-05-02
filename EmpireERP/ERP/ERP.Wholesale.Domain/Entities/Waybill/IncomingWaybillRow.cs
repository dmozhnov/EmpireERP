using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Строка входящей накладной
    /// </summary>
    public sealed class IncomingWaybillRow
    {
        /// <summary>
        /// Код накладной
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Тип строки накладной
        /// </summary>
        public WaybillType Type { get; set; }

        /// <summary>
        /// Код родительской накладной
        /// </summary>
        public Guid WaybillId { get; set; }

        /// <summary>
        /// Дата родительской накладной
        /// </summary>
        public DateTime WaybillDate { get; set; }

        /// <summary>
        /// Номер родительской накладной
        /// </summary>
        public string WaybillNumber { get; set; }

        /// <summary>
        /// Наименование родительской накладной (№ "Номер" от "дата")
        /// </summary>
        public string WaybillName { get; set; }

        /// <summary>
        /// Партия товара
        /// </summary>
        public ReceiptWaybillRow Batch { get; set; }

        /// <summary>
        /// Дата прихода партии (дата приходной накладной партии)
        /// </summary>
        public DateTime BatchDate { get; set; }

        /// <summary>
        /// Дата проводки накладной
        /// </summary>
        public DateTime AcceptanceDate { get; set; }

        /// <summary>
        /// Дата перевода позиции накладной в финальный статус
        /// </summary>
        public DateTime? FinalizationDate { get; set; }

        /// <summary>
        /// Количество пришедшего товара
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// Признак - принят ли товар на склад
        /// </summary>
        public bool IsReceipted { get; set; }

        /// <summary>
        /// Проведенное количество
        /// </summary>
        public decimal AcceptedCount { get; set; }

        /// <summary>
        /// Отгруженное количество (исходящая накладная сформирована, отгружена, но не проведена)
        /// </summary>
        public decimal ShippedCount { get; set; }

        /// <summary>
        /// Перемещенное количество (исходящая накладная сформирована и проведена)
        /// </summary>
        public decimal FinallyMovedCount { get; set; }

        /// <summary>
        /// Количество (расширенное) товара, доступное для резервирования исходящим накладными на текущий момент
        /// </summary>
        public decimal AvailableToReserveCount { get; set; }
        
        /// <summary>
        /// Доступное по позиции на складе количество (точное) товара
        /// </summary>
        public decimal AvailableInStorageCount { get; set; }

        /// <summary>
        /// Ожидаемое кол-во товара по позиции
        /// </summary>
        public decimal PendingCount { get; set; }

        /// <summary>
        /// Кол-во товара по позиции с расхождениями
        /// </summary>
        public decimal DivergenceCount { get; set; }

        /// <summary>
        /// Место хранения-получатель
        /// </summary>
        public Storage RecipientStorage { get; set; }

        /// <summary>
        /// Организация-получатель
        /// </summary>
        public AccountOrganization Recipient { get; set; }

        /// <summary>
        /// Счетчик, сколько раз позиция была вручную указана как источник для исходящих накладных
        /// </summary>
        public byte UsageAsManualSourceCount { get; set; }
    }
}
