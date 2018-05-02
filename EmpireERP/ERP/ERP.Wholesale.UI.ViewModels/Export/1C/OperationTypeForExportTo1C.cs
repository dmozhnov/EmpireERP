using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Export._1C
{
    /// <summary>
    /// Тип операции для выгрузки в 1С
    /// </summary>
    public enum OperationTypeForExportTo1C : byte
    {
        /// <summary>
        /// Реализация товаров
        /// </summary>
        [EnumDisplayName("Реализация + передача на комиссию")]
        Sale = 1,

        /// <summary>
        /// Внутрискладское перемещение в рамках организации
        /// </summary>
        [EnumDisplayName("Внутрискладское перемещение в рамках организации")]
        Movement = 2,

        /// <summary>
        /// Возврат товаров
        /// </summary>
        [EnumDisplayName("Возвраты от клиентов + возвраты комиссионеров")]
        Return = 3,

        /// <summary>
        /// Поступление на комиссию
        /// </summary>
        [EnumDisplayName("Поступление на комиссию")]
        Incoming = 4
    }
}
