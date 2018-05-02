using System;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Wholesale.Domain.Misc;
using System.Collections.Generic;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountingPriceCalcByCurrentAccountingPriceTest
    {
        [TestMethod]
        public void AccountingPriceCalcByCurrentAccountingPrice_Initial_Parameters_Must_Be_Set()
        {
            var detRule = new AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.All, new List<Storage>());
            var apCalc = new AccountingPriceCalcByCurrentAccountingPrice(detRule, 10);

            Assert.AreEqual(10, apCalc.MarkupPercentValue);
            Assert.AreEqual(detRule, apCalc.AccountingPriceDeterminationRule);
        }

        [TestMethod]
        public void AccountingPriceCalcByCurrentAccountingPrice_Create_With_Rule_Set_To_Null_Must_Throw_Exception()
        {
            var detRule = new AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.All, new List<Storage>());
            try
            {
                var apCalc = new AccountingPriceCalcByCurrentAccountingPrice(null, 10);
                Assert.Fail("Нельзя устанавливать правило в null.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно установить правило определения учетной цены в null.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceCalcByCurrentAccountingPrice_Set__Rule_To_Null_Must_Throw_Exception()
        {
            var detRule = new AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType.ByAverageAccountingPrice, AccountingPriceListStorageTypeGroup.All, new List<Storage>());
            var apCalc = new AccountingPriceCalcByCurrentAccountingPrice(detRule, 10);
            try
            {
                apCalc.AccountingPriceDeterminationRule = null;
                Assert.Fail("Нельзя устанавливать правило в null.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно установить правило определения учетной цены в null.", ex.Message);
            }
        }
    }
}
