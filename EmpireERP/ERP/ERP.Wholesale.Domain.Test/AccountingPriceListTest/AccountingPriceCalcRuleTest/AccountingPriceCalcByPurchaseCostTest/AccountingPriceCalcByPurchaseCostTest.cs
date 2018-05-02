using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountingPriceCalcByPurchaseCostTest
    {
        [TestMethod]
        public void AccountingPriceCalcByPurchaseCost_Initial_Parameters_Must_Be_Set()
        {
            var mpRule = new MarkupPercentDeterminationRule(MarkupPercentDeterminationRuleType.ByArticle);
            var apCalc = new AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType.ByAveragePurchasePrice, mpRule);

            Assert.AreEqual(PurchaseCostDeterminationRuleType.ByAveragePurchasePrice, apCalc.PurchaseCostDeterminationRuleType);
            Assert.AreEqual(mpRule, apCalc.MarkupPercentDeterminationRule);
        }
    }
}
