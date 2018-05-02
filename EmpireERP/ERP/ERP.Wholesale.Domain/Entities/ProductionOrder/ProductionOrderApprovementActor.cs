using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип действующего лица, утверждающего или отменяющего утверждение партии заказа
    /// </summary>
    public enum ProductionOrderApprovementActor : byte
    {
        /// <summary>
        /// Линейный руководитель
        /// </summary>
        [EnumDisplayName("Рук.")]
        LineManager = 1,

        /// <summary>
        /// Финансовый отдел
        /// </summary>
        [EnumDisplayName("Фин.")]
        FinancialDepartment,

        /// <summary>
        /// Отдел продаж
        /// </summary>
        [EnumDisplayName("Прод.")]
        SalesDepartment,

        /// <summary>
        /// Аналитический отдел
        /// </summary>
        [EnumDisplayName("Аналит.")]
        AnalyticalDepartment,

        /// <summary>
        /// Руководитель проекта
        /// </summary>
        [EnumDisplayName("РП")]
        ProjectManager
    }
}
