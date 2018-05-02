using System;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class DealQuotaTest
    {
        [TestMethod]
        public void DealQuota_With_Prepayment_InitialParameters_Must_Be_Set()
        {
            var quota = new DealQuota("Тестовая квота", 50);

            Assert.AreEqual("Тестовая квота", quota.Name);
            Assert.AreEqual(50, quota.DiscountPercent);
            Assert.AreEqual(DateTime.Today, quota.CreationDate.Date);
            Assert.IsNull(quota.CreditLimitSum);            
            Assert.IsNull(quota.DeletionDate);
            Assert.IsNull(quota.EndDate);
            Assert.AreEqual(0, quota.Id);
            Assert.IsTrue(quota.IsPrepayment);
            Assert.IsNull(quota.PostPaymentDays);
            Assert.AreEqual(DateTime.Today, quota.StartDate.Date);
        }

        [TestMethod]
        public void DealQuota_Without_Prepayment_InitialParameters_Must_Be_Set()
        {
            var quota = new DealQuota("Тестовая квота", 20, 15, 10000);   

            Assert.AreEqual(20, quota.DiscountPercent);
            Assert.AreEqual(10000, quota.CreditLimitSum);
            Assert.AreEqual((short)15, quota.PostPaymentDays);
            Assert.IsFalse(quota.IsPrepayment);
        }
    }
}
