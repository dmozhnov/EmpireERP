using System;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Правило определения % наценки
    /// </summary>
    public class MarkupPercentDeterminationRule
    {
        /// <summary>
        /// Тип правила определения % наценки
        /// </summary>
        public virtual MarkupPercentDeterminationRuleType Type
        {
            get { return type; }
            protected internal set
            {
                if (Type !=0 && value == MarkupPercentDeterminationRuleType.Custom)
                {
                    throw new Exception("Невозможно установить данный тип правила.");
                }

                type = value;
            }
        }
        private MarkupPercentDeterminationRuleType type;

        /// <summary>
        /// Значение % наценки (если с минусом - % скидки)
        /// </summary>
        public virtual decimal? MarkupPercentValue { get; set; }


        #region Конструкторы
        protected MarkupPercentDeterminationRule()
        {
        }

        public MarkupPercentDeterminationRule(decimal markupPercentValue)
        {
            MarkupPercentValue = markupPercentValue;
            type = MarkupPercentDeterminationRuleType.Custom;
        }

        public MarkupPercentDeterminationRule(MarkupPercentDeterminationRuleType type)
        {
            Type = type;
            MarkupPercentValue = null;
        }
        #endregion
    }
}