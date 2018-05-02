using System;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountOrganizationDocumentNumbersTest
    {
        private AccountOrganization accountOrganization;

        [TestInitialize]
        public void Initialize()
        { 
            var economicAgent = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            accountOrganization = new AccountOrganization("Тест", "ТестТест", economicAgent);
        }

        /// <summary>
        /// Собственная организация не может быть null
        /// </summary>
        [TestMethod]
        public void AccountOrganizationDocumentNumbers_AccountOrganization_Cant_BeNull()
        {
            try
            {
                var lastNumbers = new AccountOrganizationDocumentNumbers(2012, null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана собственная организация.", ex.Message);
            }
        }

        /// <summary>
        /// Параметры должны правильно устанавливаться после создания (номера документов = 0)
        /// </summary>
        [TestMethod]
        public void AccountOrganizationDocumentNumbers_Numbers_Must_Be_Set_To_Zero()
        {
            var lastNumbers = new AccountOrganizationDocumentNumbers(2012, accountOrganization);

            Assert.AreEqual(0, lastNumbers.ChangeOwnerWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ExpenditureWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.MovementWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ReceiptWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ReturnFromClientWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.WriteoffWaybillLastNumber);
            Assert.AreEqual(2012, lastNumbers.Year);
            Assert.AreEqual(accountOrganization, lastNumbers.AccountOrganization);
        }
    }
}
