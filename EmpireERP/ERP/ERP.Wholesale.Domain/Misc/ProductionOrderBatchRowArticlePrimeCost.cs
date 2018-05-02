using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация о рассчитанной себестоимости для одной позиции заказа
    /// </summary>
    public class ProductionOrderBatchRowArticlePrimeCost
    {
        #region Свойства

        /// <summary>
        /// Позиция заказа
        /// </summary>
        public ProductionOrderBatchRow ProductionOrderBatchRow { get; set; }

        /// <summary>
        /// Объем позиции заказа
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Вес позиции заказа
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Стоимость производства позиции в рублях
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal RowProductionCostInBaseCurrency { get; set; }

        /// <summary>
        /// Стоимость транспортировки позиции в рублях
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal TransportationCostInBaseCurrency { get; set; }

        #region Таможенная стоимость

        /// <summary>
        /// Стоимость таможенных расходов по позиции (всех). Может быть установлена только она, если стоит опция "Не разделять таможенные затраты"
        /// </summary>
        public decimal CustomsExpensesCostSum { get; set; }

        /// <summary>
        /// Сумма ввозных таможенных пошлин по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal ImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма вывозных таможенных пошлин по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal ExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма НДС по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Акциз по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal ExciseSum { get; set; }

        /// <summary>
        /// Сумма таможенных сборов по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal CustomsFeesSum { get; set; }

        /// <summary>
        /// Сумма КТС (корректировка таможенной стоимости) по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal CustomsValueCorrection { get; set; }

        #endregion

        /// <summary>
        /// Величина дополнительных расходов по позиции
        /// </summary>
        /// <remarks>вещественное (18, 2)</remarks>
        public decimal ExtraExpensesSumInBaseCurrency { get; set; }

        /// <summary>
        /// Общая себестоимость позиции в рублях. Округляется, чтобы при расчете приходной накладной сумма сходилась
        /// </summary>
        public decimal RowCostInBaseCurrency
        {
            get
            {
                return Math.Round(RowProductionCostInBaseCurrency + TransportationCostInBaseCurrency + CustomsExpensesCostSum + ExtraExpensesSumInBaseCurrency, 6);
            }
        }

        /// <summary>
        /// Общая себестоимость единицы товара в рублях
        /// </summary>
        public decimal CostInBaseCurrency { get { return RowCostInBaseCurrency / ProductionOrderBatchRow.Count; } }

        #endregion
    }
}
