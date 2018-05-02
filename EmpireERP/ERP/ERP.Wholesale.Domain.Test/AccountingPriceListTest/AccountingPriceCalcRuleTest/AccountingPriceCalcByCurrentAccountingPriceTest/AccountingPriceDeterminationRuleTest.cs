using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountingPriceDeterminationRuleTest
    {
        [TestMethod]
        public void AccountingPriceDeterminationRule_Initial_Parameters_Must_Be_Set()
        {
            var apRule = new AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.All, new List<Storage>());

            Assert.AreEqual(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, apRule.Type);
            Assert.AreEqual(AccountingPriceListStorageTypeGroup.All, apRule.StorageType);
            Assert.IsNull(apRule.Storage);
        }

        [TestMethod]
        public void AccountingPriceDeterminationRule_Must_Have_Type_Equal_To_ByAccountingPriceOnFirstFrom_When_StorageId_Set()
        {
            var testStorage = new Storage("ТЕСТОВЫЙ СКЛАД", StorageType.DistributionCenter) {Id = 17};
            var apRule = new AccountingPriceDeterminationRule(testStorage);

            Assert.AreEqual(AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage, apRule.Type);
            Assert.AreEqual(testStorage, apRule.Storage);
        }

        
    }
}
