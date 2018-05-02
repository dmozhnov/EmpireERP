using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Перемещение товара между строками накладных
    /// </summary>
    public class WaybillRowArticleMovement : Entity<Guid>
    {
        #region Свойства

        /// <summary>
        /// Код строки - источника
        /// </summary>
        public virtual Guid SourceWaybillRowId { get; set; }

        /// <summary>
        /// Тип накладной, которой принадлежит строка-источник
        /// </summary>
        public virtual WaybillType SourceWaybillType { get; set; }

        /// <summary>
        /// Код строки - приемника
        /// </summary>
        public virtual Guid DestinationWaybillRowId { get; set; }

        /// <summary>
        /// Тип накладной, которой принадлежит строка-приемник
        /// </summary>
        public virtual WaybillType DestinationWaybillType { get; set; }

        /// <summary>
        /// Перемещаемое количество
        /// </summary>
        public virtual decimal MovingCount { get; set; }

        /// <summary>
        /// Источник для связи был указан вручную
        /// </summary>
        public virtual bool IsManuallyCreated { get; set; }

        #endregion

        #region Конструкторы

        protected WaybillRowArticleMovement()
        {
        }

        public WaybillRowArticleMovement(Guid sourceWaybillRowId, WaybillType sourceWaybillType,
            Guid destinationWaybillRowId, WaybillType destinationWaybillType, decimal movingCount)
        {
            SourceWaybillRowId = sourceWaybillRowId;
            SourceWaybillType = sourceWaybillType;
            DestinationWaybillRowId = destinationWaybillRowId;
            DestinationWaybillType = destinationWaybillType;            

            MovingCount = movingCount;
            IsManuallyCreated = false;
        }

        #endregion
    }
}
