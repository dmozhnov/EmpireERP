using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.Services
{
    public class ReceiptedReturnFromClientIndicatorService : BaseReturnFromClientIndicatorService<ReceiptedReturnFromClientIndicator>,
                                                            IReceiptedReturnFromClientIndicatorService
    {
        #region Конструкторы

        public ReceiptedReturnFromClientIndicatorService(IReceiptedReturnFromClientIndicatorRepository receiptedReturnFromClientIndicatorRepository, IArticleRepository articleRepository)
            : base(receiptedReturnFromClientIndicatorRepository, articleRepository)
        {
        }

        #endregion
    }
}