using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class OutgoingAcceptedFromExactArticleAvailabilityIndicatorService : 
        BaseArticleAvailabilityIndicatorService<OutgoingAcceptedFromExactArticleAvailabilityIndicator>,
        IOutgoingAcceptedFromExactArticleAvailabilityIndicatorService
    {
        #region Конструкторы

        public OutgoingAcceptedFromExactArticleAvailabilityIndicatorService(IOutgoingAcceptedFromExactArticleAvailabilityIndicatorRepository outgoingAcceptedFromExactArticleAvailabilityIndicatorRepository) :
            base(outgoingAcceptedFromExactArticleAvailabilityIndicatorRepository)
        {            
        }

        #endregion

    }
}
