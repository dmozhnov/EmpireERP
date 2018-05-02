using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Строка исходящей накладной
    /// </summary>
    public sealed class OutgoingWaybillRow : Entity<Guid>
    {
        /// <summary>
        /// Тип накладной
        /// </summary>
        public WaybillType Type { get; set; }

        /// <summary>
        /// Статус строки накладной
        /// </summary>
        public OutgoingWaybillRowState State { get; set; }

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
        /// Партия товара
        /// </summary>
        public ReceiptWaybillRow Batch { get; set; }

        /// <summary>
        /// Количество ушедшего товара
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// Место хранения, с которого уходит товар
        /// </summary>
        public Storage SenderStorage { get; set; }

        /// <summary>
        /// Организация-отправитель
        /// </summary>
        public AccountOrganization Sender { get; set; }

        /// <summary>
        /// Фиксированная учетная цена отправителя
        /// </summary>
        public decimal SenderAccountingPrice { get; set; }

        /// <summary>
        /// Определены ли источники для этой позиции (установлены вручную при добавлении, или же назначены автоматически при проводке).
        /// </summary>
        public bool AreSourcesDetermined { get; set; }

        /// <summary>
        /// Дата проводки позиции
        /// </summary>
        public DateTime? AcceptanceDate { get; set; }

        /// <summary>
        /// Дата перевода позициии в финальный статус
        /// </summary>
        public DateTime? FinalizationDate { get; set; }
    }
}
