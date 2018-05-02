using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Шаблон жизненного цикла заказа
    /// </summary>
    public class ProductionOrderBatchLifeCycleTemplate : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 200 символов</remarks>
        public virtual string Name { get; set; }

        /// <summary>
        /// Этапы шаблона жизненного цикла заказа
        /// </summary>
        public virtual IEnumerable<ProductionOrderBatchLifeCycleTemplateStage> Stages
        {
            get { return new ImmutableSet<ProductionOrderBatchLifeCycleTemplateStage>(stages); }
        }
        private Iesi.Collections.Generic.ISet<ProductionOrderBatchLifeCycleTemplateStage> stages;

        /// <summary>
        /// Количество этапов
        /// </summary>
        public virtual int StageCount { get { return stages.Count; } }

        #endregion

        #region Конструкторы

        protected ProductionOrderBatchLifeCycleTemplate()
        {
        }

        public ProductionOrderBatchLifeCycleTemplate(string name, ProductionOrderBatchLifeCycleTemplateStage calculationStage,
            ProductionOrderBatchLifeCycleTemplateStage successfullClosingStage, ProductionOrderBatchLifeCycleTemplateStage unsuccessfulClosingStage)
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(name), "Название шаблона не указано.");
            Name = name;

            ValidationUtils.NotNull(calculationStage, "Не указан один из системных этапов.");
            ValidationUtils.NotNull(successfullClosingStage, "Не указан один из системных этапов.");
            ValidationUtils.NotNull(unsuccessfulClosingStage, "Не указан один из системных этапов.");

            stages = new HashedSet<ProductionOrderBatchLifeCycleTemplateStage>();

            AddStage(calculationStage);
            AddStage(successfullClosingStage);
            AddStage(unsuccessfulClosingStage);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Перерасчет порядковых номеров этапов
        /// </summary>
        private void RecalculateOrdinalNumbers()
        {
            var stageList = new List<ProductionOrderBatchLifeCycleTemplateStage>(Stages.OrderBy(x => x.OrdinalNumber));

            short number = 1;

            foreach (var stage in stageList)
            {
                stage.OrdinalNumber = number;
                number++;
            }
        }

        /// <summary>
        /// Проверка правильности следования этапов
        /// </summary>
        public virtual void CheckStageOrder()
        {
            bool isLastStageCalculation = true; // Имел ли предыдущий этап тип "Расчет заказа"
            bool isCurrentStageCalculation; // Имеет ли текущий этап тип "Расчет заказа"
            foreach (var stage in stages.OrderBy(x => x.OrdinalNumber))
            {
                isCurrentStageCalculation = stage.Type == ProductionOrderBatchStageType.Calculation;
                if (isCurrentStageCalculation && !isLastStageCalculation)
                {
                    throw new Exception(String.Format("Этапы с типом «{0}» не могут идти после других этапов.",
                        ProductionOrderBatchStageType.Calculation.GetDisplayName()));
                }

                isLastStageCalculation = isCurrentStageCalculation;
            }
        }

        /// <summary>
        /// Добавить этап в конец списка (применяется только для системных этапов)
        /// </summary>
        /// <param name="stage"></param>
        public virtual void AddStage(ProductionOrderBatchLifeCycleTemplateStage stage)
        {
            RecalculateOrdinalNumbers();
            stages.Add(stage);
            stage.Template = this;
            stage.OrdinalNumber = (short)StageCount;
            CheckStageOrder();
        }

        /// <summary>
        /// Добавить этап после этапа с заданным порядковым номером
        /// </summary>
        /// <param name="stage">Этап</param>
        /// <param name="position">Порядковый номер этапа, после которого осуществляется вставка</param>
        public virtual void AddStage(ProductionOrderBatchLifeCycleTemplateStage stage, short position)
        {
            if (position < 1 || position > StageCount - 2)
            {
                throw new Exception("Невозможно вставить этап на заданную позицию.");
            }

            RecalculateOrdinalNumbers();

            var stageList = new List<ProductionOrderBatchLifeCycleTemplateStage>(Stages.OrderBy(x => x.OrdinalNumber));

            foreach (var productionOrderBatchLifeCycleTemplateStage in stageList)
            {
                if (productionOrderBatchLifeCycleTemplateStage.OrdinalNumber > position)
                    productionOrderBatchLifeCycleTemplateStage.OrdinalNumber++;
            }

            stages.Add(stage);
            stage.Template = this;
            stage.OrdinalNumber = (short)(position + 1);

            CheckStageOrder();
        }

        /// <summary>
        /// Удалить этап
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void DeleteStage(ProductionOrderBatchLifeCycleTemplateStage stage)
        {
            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно удалить системный этап.");
            }

            stage.Template = null;
            stages.Remove(stage);
            RecalculateOrdinalNumbers();
            CheckStageOrder();
        }

        /// <summary>
        /// Переместить этап вверх
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void MoveStageUp(ProductionOrderBatchLifeCycleTemplateStage stage)
        {
            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно переместить системный этап.");
            }

            RecalculateOrdinalNumbers();

            var otherStage = stages.Where(x => x.OrdinalNumber == stage.OrdinalNumber - 1).SingleOrDefault();
            ValidationUtils.NotNull(otherStage, "Невозможно переместить этап.");

            if (otherStage.IsDefault)
            {
                throw new Exception("Невозможно переместить этап.");
            }

            var tempOrdinalNumber = stage.OrdinalNumber;
            stage.OrdinalNumber = otherStage.OrdinalNumber;
            otherStage.OrdinalNumber = tempOrdinalNumber;
            CheckStageOrder();
        }

        /// <summary>
        /// Переместить этап вниз
        /// </summary>
        /// <param name="stage">Этап</param>
        public virtual void MoveStageDown(ProductionOrderBatchLifeCycleTemplateStage stage)
        {
            if (!stages.Contains(stage))
            {
                throw new Exception("Этап не найден. Возможно, он был удален.");
            }

            if (stage.IsDefault)
            {
                throw new Exception("Невозможно переместить системный этап.");
            }

            RecalculateOrdinalNumbers();

            var otherStage = stages.Where(x => x.OrdinalNumber == stage.OrdinalNumber + 1).SingleOrDefault();
            ValidationUtils.NotNull(otherStage, "Невозможно переместить этап.");

            if (otherStage.IsDefault)
            {
                throw new Exception("Невозможно переместить этап.");
            }

            var tempOrdinalNumber = stage.OrdinalNumber;
            stage.OrdinalNumber = otherStage.OrdinalNumber;
            otherStage.OrdinalNumber = tempOrdinalNumber;
            CheckStageOrder();
        }

        #endregion
    }
}
