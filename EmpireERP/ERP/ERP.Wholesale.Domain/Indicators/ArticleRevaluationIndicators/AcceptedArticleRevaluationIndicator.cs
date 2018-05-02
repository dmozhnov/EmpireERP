using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.Domain.Indicators
{
    /// <summary>
    /// Показатель проведенной переоценки
    /// </summary>
    public class AcceptedArticleRevaluationIndicator : BaseArticleRevaluationIndicator
    {
        #region Конструкторы

        public AcceptedArticleRevaluationIndicator()
        {
        }

        public AcceptedArticleRevaluationIndicator(DateTime startDate, short storageId, int accountOrganizationId, decimal revaluationSum) 
            : base(startDate, storageId, accountOrganizationId, revaluationSum)
        {
        }

        #endregion
    }
}
