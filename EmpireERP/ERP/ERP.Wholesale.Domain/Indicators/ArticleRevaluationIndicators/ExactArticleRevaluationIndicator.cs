using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель точной переоценки
    /// </summary>
    public class ExactArticleRevaluationIndicator : BaseArticleRevaluationIndicator
    {
        #region Конструкторы

        public ExactArticleRevaluationIndicator()
        {
        }

        public ExactArticleRevaluationIndicator(DateTime startDate, short storageId, int accountOrganizationId, decimal revaluationSum) 
            : base(startDate, storageId, accountOrganizationId, revaluationSum)
        {
        }

        #endregion
    }
}
