using System;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Этап партии заказа на производство по умолчанию
    /// </summary>
    public class DefaultProductionOrderBatchStage : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name;

        /// <summary>
        /// Тип
        /// </summary>
        public virtual ProductionOrderBatchStageType Type
        {
            get { return type; }
            set
            {
                if (!Enum.IsDefined(typeof(ProductionOrderBatchStageType), value))
                {
                    throw new Exception("Недопустимый тип этапа.");
                }
                if (value == ProductionOrderBatchStageType.Closed)
                {
                    throw new Exception(String.Format("Невозможно присвоить этапу тип «{0}».", value.GetDisplayName()));
                }

                type = value;
            }
        }
        private ProductionOrderBatchStageType type;

        #endregion

        #region Конструкторы

        protected DefaultProductionOrderBatchStage()
        {
        }

        #endregion
    }
}
