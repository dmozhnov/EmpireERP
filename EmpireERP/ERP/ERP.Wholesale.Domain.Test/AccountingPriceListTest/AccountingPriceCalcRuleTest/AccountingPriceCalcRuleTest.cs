using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountingPriceCalcRuleTest
    {
        [TestMethod]
        public void AccountingPriceCalcRule_When_Init_By_AccountingPriceCalcByPurchaseCost_Type_Must_Equal_ByPurchaseCost()
        {
            var mpdRule = new MarkupPercentDeterminationRule(28);
            var apCalc = new AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType.ByMaximalPurchaseCost, mpdRule);
            var apRule = new AccountingPriceCalcRule(apCalc);

            Assert.IsNull(apRule.CalcByCurrentAccountingPrice);
            Assert.AreEqual(apCalc, apRule.CalcByPurchaseCost);
            Assert.AreEqual(AccountingPriceCalcRuleType.ByPurchaseCost, apRule.Type);
        }

        [TestMethod]
        public void AccountingPriceCalcRule_When_Init_By_AccountingPriceCalcByCurrentAccountingPrice_Type_Must_Equal_ByCurrentAccountingPrice()
        {
            var apdRule = new AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.DistributionCenters, new List<Storage>());
            var apCalc = new AccountingPriceCalcByCurrentAccountingPrice(apdRule, 18);
            var apRule = new AccountingPriceCalcRule(apCalc);

            Assert.AreEqual(apCalc, apRule.CalcByCurrentAccountingPrice);
            Assert.IsNull(apRule.CalcByPurchaseCost);
            Assert.AreEqual(AccountingPriceCalcRuleType.ByCurrentAccountingPrice, apRule.Type);
        }
    }
}
