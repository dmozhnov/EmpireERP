using System;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class AccountOrganizationTest
    {
        [TestMethod]
        public void AccountOrganization_InitialParameters_Must_Be_Set()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org = new AccountOrganization_Accessor("Тест", "ТестТест", jp);
            
            Assert.AreEqual("Тест", org.ShortName);
            Assert.AreEqual("ТестТест", org.FullName);
            Assert.AreEqual(String.Empty, org.Address);
            Assert.IsNotNull(org.RussianBankAccounts);
            Assert.AreEqual(String.Empty, org.Comment);
            Assert.AreEqual(0, org.ContractCount);
            Assert.IsNotNull(org.Contracts);
            Assert.AreEqual(DateTime.Today.Date, org.CreationDate.Date);
            Assert.IsNull(org.DeletionDate);
            Assert.IsNotNull(org.EconomicAgent);
            Assert.AreEqual(0, ((AccountOrganization)org.Target).Id);
            Assert.AreEqual(0, org.StorageCount);
            Assert.IsNotNull(org.Storages);
            Assert.AreEqual(EconomicAgentType.JuridicalPerson, org.EconomicAgent.Type);
            Assert.IsNotNull(org.documentNumbers);
            Assert.AreEqual(0, org.documentNumbers.Count);
        }
        
        #region Проверка перегруженного равенства, неравенства, Equals и GetHashCode

        #region Проверка перегруженных операторов равенства

        [TestMethod]
        public void AccountOrganization_NullsMustBeEqual()
        {
            AccountOrganization orgNull1 = null;
            AccountOrganization orgNull2 = null;
            bool areEqual = (orgNull1 == orgNull2);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_NullMustNotBeEqualToNotNull()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org1 = new AccountOrganization("Тест", "Тест", jp) { Id = 5 };
            AccountOrganization orgNull = null;
            bool areEqual = (orgNull == org1);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_NotNullMustNotBeEqualToNull()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org1 = new AccountOrganization("Тест", "Тест", jp) { Id = 5 };
            AccountOrganization orgNull = null;
            bool areEqual = (org1 == orgNull);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_DifferentIdMustNotBeEqual()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест", "Тест", jp1) { Id = 5 };
            var org2 = new AccountOrganization("Тест", "Тест", jp1) { Id = 6 };
            bool areEqual = (org1 == org2);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_SameIdMustBeEqual()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            var jp2 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 2 };
            var org2 = new AccountOrganization("Тест", "Тест2", jp2) { Id = 5 };
            bool areEqual = (org1 == org2);

            Assert.IsTrue(areEqual);
        }

        #endregion

        #region Проверка перегруженных операторов неравенства

        [TestMethod]
        public void AccountOrganization_NullsMustNotBeNonEqual()
        {
            AccountOrganization orgNull1 = null;
            AccountOrganization orgNull2 = null;
            bool areEqual = (orgNull1 != orgNull2);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_NullMustBeNonEqualToNotNull()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org1 = new AccountOrganization("Тест", "Тест", jp) { Id = 5 };
            AccountOrganization orgNull = null;
            bool areEqual = (orgNull != org1);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_NotNullMustBeNonEqualToNull()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org1 = new AccountOrganization("Тест", "Тест", jp) { Id = 5 };
            AccountOrganization orgNull = null;
            bool areEqual = (org1 != orgNull);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_DifferentIdMustBeNonEqual()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест", "Тест", jp1) { Id = 5 };
            var org2 = new AccountOrganization("Тест", "Тест", jp1) { Id = 6 };
            bool areEqual = (org1 != org2);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void AccountOrganization_SameIdMustNotBeNonEqual()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            var jp2 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 2 };
            var org2 = new AccountOrganization("Тест", "Тест2", jp2) { Id = 5 };
            bool areEqual = (org1 != org2);

            Assert.IsFalse(areEqual);
        }

        #endregion

        #region Проверка перегруженного метода Equals

        [TestMethod]
        public void AccountOrganization_MustNotBeEqualToNull()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            AccountOrganization accountOrganizationNull = null;

            Assert.IsFalse(org1.Equals(accountOrganizationNull));
            Assert.IsFalse(org1.Equals(null));
        }

        [TestMethod]
        public void AccountOrganization_MustNotBeEqualToNonAccountOrganization()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            DateTime nonAccountOrganization = new DateTime(2007, 12, 5);

            Assert.IsFalse(org1.Equals(nonAccountOrganization));
        }

        [TestMethod]
        public void AccountOrganization_DifferentIdMustHaveEqualsFalse()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест", "Тест", jp1) { Id = 5 };
            var org2 = new AccountOrganization("Тест", "Тест", jp1) { Id = 6 };

            Assert.IsFalse(org1.Equals(org2));
            Assert.IsFalse(org2.Equals(org1));
        }

        [TestMethod]
        public void AccountOrganization_SameIdMustHaveEqualsTrue()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            var jp2 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 2 };
            var org2 = new AccountOrganization("Тест", "Тест2", jp2) { Id = 5 };

            Assert.IsTrue(org1.Equals(org2));
            Assert.IsTrue(org2.Equals(org1));
        }

        #endregion

        #region Проверка перегруженного метода GetHashCode

        [TestMethod]
        public void AccountOrganization_SameIds_MustHaveEqual_HashCodes()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 5 };
            var jp2 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 2 };
            var org2 = new AccountOrganization("Тест", "Тест2", jp2) { Id = 5 };

            Assert.IsTrue(org1.GetHashCode() == org2.GetHashCode());
        }

        [TestMethod]
        public void AccountOrganization_DifferentIds_MustHaveDifferent_HashCodes()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест", "Тест", jp1) { Id = 5 };
            var org2 = new AccountOrganization("Тест", "Тест", jp1) { Id = 6 };

            Assert.IsFalse(org1.GetHashCode() == org2.GetHashCode());
        }

        [TestMethod]
        public void AccountOrganization_ZeroIds_DifferentValues_ShouldHaveDifferent_HashCodes()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест1", "Тест1", jp1) { Id = 0 };
            var jp2 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 2 };
            var org2 = new AccountOrganization("Тест2", "Тест2", jp2) { Id = 0 };

            Assert.IsFalse(org1.GetHashCode() == org2.GetHashCode());
        }

        [TestMethod]
        public void AccountOrganization_2CallsOfGetHashCode_MustReturnSameValues()
        {
            var jp1 = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 1 };
            var org1 = new AccountOrganization("Тест", "Тест", jp1) { Id = 5 };

            var hashCode1 = org1.GetHashCode();
            var hashCode2 = org1.GetHashCode();

            Assert.IsTrue(hashCode1 == hashCode2);
        }

        #endregion

        #endregion

        [TestMethod]
        public void AccountOrganization_Storage_Additon_And_Removing_Must_Be_Ok()
        {
            var storage = new Storage("Склад", StorageType.DistributionCenter);
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org = new AccountOrganization("Тест", "Тест", jp);

            org.AddStorage(storage);

            Assert.AreEqual(1, org.StorageCount);
            Assert.AreEqual(1, storage.AccountOrganizationCount);

            org.RemoveStorage(storage);

            Assert.AreEqual(0, org.StorageCount);
            Assert.AreEqual(0, storage.AccountOrganizationCount);
        }

        [TestMethod]
        public void AccountOrganization_Repeat_Storage_Additon_Must_Throw_Exception()
        {
            try
            {
                var storage = new Storage("Склад", StorageType.DistributionCenter);
                var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
                var org = new AccountOrganization("Тест", "Тест", jp);

                org.AddStorage(storage);
                org.AddStorage(storage);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данное место хранения уже связано с этой организацией.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountOrganization_Deletion_Not_Added_Storage_Must_Throw_Exception()
        {
            try
            {
                var storage = new Storage("Склад", StorageType.DistributionCenter);
                var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
                var org = new AccountOrganization("Тест", "Тест", jp);

                org.RemoveStorage(storage);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данное место хранения не связано с этой организацией. Возможно, оно было удалено.", ex.Message);
            }
        }

        /// <summary>
        /// Метод GetLastDocumentNumbers всегда должен возвращать объект с дефолтными параметрами
        /// </summary>
        [TestMethod]
        public void AccountOrganization_DocumentsNumber_Must_Add_New_Year_Automatic_And_Set_Numbers_To_0()
        {
            var jp = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson));
            var org = new AccountOrganization_Accessor("Тест", "ТестТест", jp);

            var lastNumbers = org.GetLastDocumentNumbers(2012);

            Assert.IsNotNull(lastNumbers);
            Assert.AreEqual(0, lastNumbers.ChangeOwnerWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ExpenditureWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.MovementWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ReceiptWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.ReturnFromClientWaybillLastNumber);
            Assert.AreEqual(0, lastNumbers.WriteoffWaybillLastNumber);
            Assert.AreEqual(2012, lastNumbers.Year);
            Assert.AreEqual(org, lastNumbers.AccountOrganization);
        }
    }
}
