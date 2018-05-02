using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ReceiptWaybillTest
    {
        #region Инициализация и конструкторы

        private readonly Storage storage;
        private readonly JuridicalPerson juridicalPerson;
        private readonly PhysicalPerson physicalPerson;
        private readonly AccountOrganization accountOrganization;
        private readonly Provider provider;
        private readonly ArticleGroup articleGroup;
        private readonly MeasureUnit measureUnit;
        private readonly MeasureUnit measureUnitM;
        private readonly Article articleA;
        private readonly Article articleB;
        private readonly Article articleC;
        private readonly Article articleM;
        private readonly List<ArticleAccountingPrice> priceLists;
        private readonly ProviderOrganization providerOrganization;
        private readonly LegalForm juridicalLegalForm, physicalLegalForm;
        private readonly ProviderContract providerContract;
        private readonly User user;
        private readonly ValueAddedTax valueAddedTax18;
        private readonly String customDeclarationNumber;

        public ReceiptWaybillTest()
        {
            storage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
            juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            physicalLegalForm = new LegalForm("ИП", EconomicAgentType.PhysicalPerson);

            juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            physicalPerson = new PhysicalPerson(physicalLegalForm) { Id = 2 };

            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 3 };
            providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", physicalPerson) { Id = 4 };

            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrganization);

            providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "4645", DateTime.Now, DateTime.Now);
            provider.AddProviderContract(providerContract);

            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            measureUnitM = new MeasureUnit("м.", "Метр", "124", 3) { Id = 1 };
            articleA = new Article("Тестовый товар А", articleGroup, measureUnit, true) { Id = 100 };
            articleB = new Article("Тестовый товар B", articleGroup, measureUnit, true) { Id = 101 };
            articleC = new Article("Тестовый товар C", articleGroup, measureUnit, true) { Id = 102 };
            articleM = new Article("Тестовый товар M", articleGroup, measureUnitM, true) { Id = 103 };

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100), new ArticleAccountingPrice(articleB, 200),
                new ArticleAccountingPrice(articleC, 300), new ArticleAccountingPrice(articleM, 10M) };

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);

            valueAddedTax18 = new ValueAddedTax("18%", 18M);

            customDeclarationNumber = new String('0',25);
        }

        #endregion

        [TestMethod]
        public void ReceiptWaybill_Initial_Parameters_Must_Be_Set()
        {  
            var currentDateTime = DateTime.Now;
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(1), storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, currentDateTime);
            
            Assert.AreEqual(receiptWaybill.Number, "123АБВ");
            Assert.AreEqual(receiptWaybill.Date, DateTime.Today.AddDays(1));
            Assert.AreEqual(receiptWaybill.ReceiptStorage.Name, "Тестовое хранилище");
            Assert.AreEqual(receiptWaybill.AccountOrganization.FullName, "Тестовое юридическое лицо");
            Assert.IsNotNull(receiptWaybill.Provider);
            Assert.AreEqual(receiptWaybill.PendingSum, 100.05M);
            Assert.AreEqual(receiptWaybill.PendingValueAddedTax.Name, "18%");
            Assert.AreEqual(receiptWaybill.PendingValueAddedTax.Value, 18);

            Assert.AreEqual(receiptWaybill.Id, Guid.Empty);
            Assert.AreEqual(providerContract, receiptWaybill.ProviderContract);
            Assert.IsNotNull(receiptWaybill.CreationDate);
            Assert.AreEqual(receiptWaybill.CurrentSum, receiptWaybill.PendingSum);
            Assert.AreEqual(receiptWaybill.CustomsDeclarationNumber, "0000000000000000000000000");
            Assert.IsNull(receiptWaybill.DeletionDate);
            Assert.AreEqual(receiptWaybill.DiscountPercent, 0);
            Assert.AreEqual(receiptWaybill.PendingDiscountSum, 0);
            Assert.AreEqual(receiptWaybill.DiscountSum, 0);
            Assert.IsNull(receiptWaybill.ApprovedSum);
            Assert.IsNull(receiptWaybill.ProviderDate);
            Assert.IsNull(receiptWaybill.ProviderInvoiceDate);
            Assert.AreEqual(receiptWaybill.ProviderInvoiceNumber, "");
            Assert.AreEqual(receiptWaybill.ProviderNumber, "");
            Assert.IsNotNull(receiptWaybill.Rows);
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.New);
            Assert.IsNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
            Assert.AreEqual(user, receiptWaybill.Curator);
            Assert.AreEqual(user, receiptWaybill.CreatedBy);
            Assert.AreEqual(receiptWaybill.CreationDate, currentDateTime);
        }
        
        [TestMethod]
        public void ReceiptWaybillRow_Initial_Parameters_Must_Be_Set()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 151.23M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            Assert.AreEqual(receiptWaybillRow.PendingCount, 99);
            Assert.AreEqual(receiptWaybillRow.Article.FullName, "Тестовый товар А");
            Assert.IsNotNull(receiptWaybillRow.CreationDate);
            Assert.AreEqual(receiptWaybillRow.PendingSum, 151.23M);
            Assert.AreEqual(1.527576M, receiptWaybillRow.PurchaseCost);

            var receiptWaybillRow2 = new ReceiptWaybillRow(articleB, 99, receiptWaybill.PendingValueAddedTax, 1.527577M);
            receiptWaybill.AddRow(receiptWaybillRow2);

            Assert.AreEqual(receiptWaybillRow2.PendingCount, 99);
            Assert.AreEqual(receiptWaybillRow2.Article.FullName, "Тестовый товар B");
            Assert.IsNotNull(receiptWaybillRow2.CreationDate);
            Assert.AreEqual(receiptWaybillRow2.PendingSum, 151.23M);
            Assert.AreEqual(1.527577M, receiptWaybillRow2.PurchaseCost);
        }

        [TestMethod]
        public void ReceiptWaybillRow_Init_Negative_PendingCount_Throws_Exception()
        {
            try
            {
                var receiptWaybillRow = new ReceiptWaybillRow(articleA, -100, 45, valueAddedTax18);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно создать партию с отрицательным ожидаемым количеством.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybillRow_Init_Negative_PendingSum_Throws_Exception()
        {
            try
            {
                var receiptWaybillRow = new ReceiptWaybillRow(articleA, 100, -45, valueAddedTax18);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно создать партию с отрицательной ожидаемой суммой.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybill_No_Parent_CurrentSum_MustThrowException()
        {
            try
            {
                var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 151.23M, valueAddedTax18);
                var sum = receiptWaybillRow.CurrentSum;
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Object reference not set to an instance of an object.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybill_Receipt_With_Count_And_Sum_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 100M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 98;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);

            Assert.AreEqual(receiptWaybill.ReceiptedSum, 150M);
            Assert.AreEqual(receiptWaybill.Rows.Sum(x => x.ProviderSum.Value), 150M);
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.ReceiptedWithSumAndCountDivergences);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        #region Смена поставщика и места хранения

        #region Смена поставщика

        [TestMethod]
        public void ReceiptWaybill_ChangeProvider_ContractWithProviderMustAddOk()
        {
            var contract = new ProviderContract(accountOrganization, (ProviderOrganization)provider.Organizations.First(), "Договор", "1945", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(contract);

            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybill.ChangeProvider(provider, contract);
            Assert.AreEqual(receiptWaybill.Provider.Organizations.First().FullName, providerOrganization.FullName);
            Assert.AreEqual(receiptWaybill.Provider, provider);
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeProvider_NullContractMustChangeOk()
        {
            var providerOrg = new ProviderOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson);
            providerOrg.Id = 9;

            var provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrg);
            provider.AddProviderContract(providerContract);
            provider.Id = 9;

            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybill.ChangeProvider(provider, null);
            Assert.AreEqual("Тестовое юридическое лицо", receiptWaybill.Provider.Organizations.First().FullName);
            Assert.AreEqual(receiptWaybill.Provider, provider);
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeProvider_ContractMustBeInProvider()
        {
            try
            {
                var contract = new ProviderContract(accountOrganization, (ProviderOrganization)provider.Organizations.First(), "Договор", "1945", DateTime.Now, DateTime.Today);
                var providerBad = new Provider("Тестовый поставщик", new ProviderType("Тип"), ProviderReliability.Medium, 5) { Id = 99 };

                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeProvider(providerBad, contract);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Указан договор, не связанный с поставщиком.");
            }
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeProvider_ContractAndProviderIdsCanBeZero()
        {
            var contract = new ProviderContract(accountOrganization, (ProviderOrganization)provider.Organizations.First(), "Договор", "1945", DateTime.Now, DateTime.Today);
            contract.ContractorOrganization.Id = 0;
            var providerBad = new Provider("Тестовый поставщик", new ProviderType("Тип"), ProviderReliability.Medium, 5) { Id = 0 };
            providerBad.AddProviderContract(contract);

            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybill.ChangeProvider(providerBad, contract);

            Assert.AreEqual(receiptWaybill.Provider.Id, 0);
            Assert.AreEqual(receiptWaybill.ProviderContract.Id, 0);
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeProvider_ProviderMustNotBeNull()
        {
            try
            {
                var contract = new ProviderContract(accountOrganization, (ProviderOrganization)provider.Organizations.First(), "Договор", "1945", DateTime.Now, DateTime.Today);
                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeProvider(null, contract);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Поставщик не указан.");
            }
        }

        #endregion

        #region Смена места хранения

        [TestMethod]
        public void ReceiptWaybill_ChangeStorage_StorageWithOrganizationMustAddOk()
        {
            //var newStorage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
            //newStorage.Id = 5;
            //var newAccountOrganization = new AccountOrganization("Юрик", "Юрик", juridicalPerson);
            //newAccountOrganization.Id = 9;
            
            //newAccountOrganization.AddStorage(newStorage);

            //var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, new ValueAddedTax("18%", 18), providerContract);
            //receiptWaybill.ChangeReceiptStorage(newStorage, newAccountOrganization);

            //Assert.AreEqual(5, receiptWaybill.ReceiptStorage.Id);
            //Assert.AreEqual(9, receiptWaybill.AccountOrganization.Id);
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeStorage_StorageAndOrganizationMustBeLinked()
        {
            try
            {
                var newStorage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
                newStorage.Id = 5;
                var newBadStorage = new Storage("Неподходящее тестовое хранилище", StorageType.DistributionCenter);
                newBadStorage.Id = 55;
                var newAccountOrganization = new AccountOrganization("Юрик", "Юрик", juridicalPerson);
                newAccountOrganization.Id = 9;

                newAccountOrganization.AddStorage(newStorage);

                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeReceiptStorage(newBadStorage, newAccountOrganization);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreNotEqual(ex.Message, "Исключения не было.");
            }
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeStorage_StorageAndOrganizationMustNotBothBeNull()
        {
            try
            {
                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeReceiptStorage(null, null);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreNotEqual(ex.Message, "Исключения не было.");
            }
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeStorage_OrganizationMustNotBeNull()
        {
            try
            {
                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeReceiptStorage(storage, null);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreNotEqual(ex.Message, "Исключения не было.");
            }
        }

        [TestMethod]
        public void ReceiptWaybill_ChangeStorage_StorageMustNotBeNull()
        {
            try
            {
                var newStorage = new Storage("Тестовое хранилище", StorageType.DistributionCenter);
                var newAccountOrganization = new AccountOrganization("Юрик", "Юрик", juridicalPerson);

                newAccountOrganization.AddStorage(newStorage);

                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
                receiptWaybill.ChangeReceiptStorage(null, newAccountOrganization);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreNotEqual(ex.Message, "Исключения не было.");
            }
        }

        #endregion

        #endregion

        [TestMethod]
        public void ReceiptWaybill_Receipt_With_Count_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 5;
            receiptWaybillRow.ProviderCount = receiptWaybillRow.PendingCount;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);

            Assert.AreEqual(receiptWaybill.ReceiptedSum, 150M);
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.ReceiptedWithCountDivergences);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Accept_With_Internal_Error_Sum()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var article2 = new Article("Тестовый товар #2", articleGroup, measureUnit, true) { Id = 2 };

            receiptWaybill.AddRow(new ReceiptWaybillRow(articleA, 99, 100M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 5, ProviderCount = 6, ProviderSum = 100M });
            receiptWaybill.AddRow(new ReceiptWaybillRow(article2, 99, 100M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 5, ProviderCount = 6, ProviderSum = 100M });
            
            priceLists.Add(new ArticleAccountingPrice(article2, 150));
            try
            {
                receiptWaybill.Accept(priceLists, user, DateTime.Now);                
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма накладной не совпадает с суммой по позициям.", ex.Message);
            }

            Assert.AreEqual(ReceiptWaybillState.New, receiptWaybill.State);
        }

        [TestMethod]
        public void ReceiptWaybill_Receipt_With_Sum_Divergences_By_WaybillSum()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 99;
            receiptWaybillRow.ProviderSum = 160M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            Assert.AreEqual(receiptWaybill.ReceiptedSum, 160M);
            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithSumDivergences, receiptWaybill.State);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Approve_Without_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 99;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);

            Assert.AreEqual(150M, receiptWaybill.ReceiptedSum);
            Assert.AreEqual(ReceiptWaybillState.ApprovedWithoutDivergences, receiptWaybill.State);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNotNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Approve_Count_Article()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillRow.ReceiptedCount = 90;
            receiptWaybillRow.ProviderCount = 90;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);   //Приемка накладной
            // согласование
            receiptWaybillRow.ApprovedCount = 99;
            receiptWaybillRow.ApprovedSum = 150M;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            receiptWaybill.Approve(150M, user, DateTime.Now, priceLists);    // согласуем накладную
            
            Assert.AreEqual(receiptWaybill.ApprovedSum, 150M);
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.ApprovedFinallyAfterDivergences);                        
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNotNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Approve_Sum()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 99;
            receiptWaybillRow.ProviderSum = 160M;

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            receiptWaybillRow.ApprovedSum = 150M;
            receiptWaybillRow.ApprovedCount = 99;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            receiptWaybill.Approve(150M, user, DateTime.Now, priceLists);

            Assert.AreEqual(receiptWaybill.ApprovedSum, 150M);
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.ApprovedFinallyAfterDivergences);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNotNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Approve_Divergences_Internal_Sum()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 90;
            receiptWaybillRow.ProviderCount = 90;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);   //Приемка накладной
            // проводка
            receiptWaybillRow.ApprovedCount = 99;
            receiptWaybillRow.ApprovedSum = 140M;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            try
            {
                receiptWaybill.Approve(150M, user, DateTime.Now);    // согласуем накладную
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма накладной не сходится с суммой по позициям.", ex.Message);
            }
            
            Assert.AreEqual(receiptWaybill.State, ReceiptWaybillState.ReceiptedWithCountDivergences);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_Sum_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 90, ProviderSum = 149M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(149M, user, DateTime.Now);
            receiptWaybill.CancelReceipt();

            Assert.AreEqual(ReceiptWaybillState.AcceptedDeliveryPending, receiptWaybill.State);
            Assert.AreEqual(0M, receiptWaybill.ReceiptedSum);
            Assert.IsNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_Count_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 5, ProviderCount = 5, ProviderSum = 150M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);
            receiptWaybill.CancelReceipt();

            Assert.AreEqual(ReceiptWaybillState.AcceptedDeliveryPending, receiptWaybill.State);
            Assert.AreEqual(0M, receiptWaybill.ReceiptedSum);
            Assert.IsNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_Sum_Count_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 5, ProviderCount = 5, ProviderSum = 160M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);
            receiptWaybill.CancelReceipt();

            Assert.AreEqual(ReceiptWaybillState.AcceptedDeliveryPending, receiptWaybill.State);
            Assert.AreEqual(0M, receiptWaybill.ReceiptedSum);
            Assert.IsNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        /// <summary>
        /// При попытке отмены приемки на склад с позициями, по которым была отгрузка, должно происходить исключение.
        /// </summary>
        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_WithShippedPositions_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 300M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRowWithDivergences = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRowWithDivergences);
            receiptWaybillRowWithDivergences.ReceiptedCount = 98;
            receiptWaybillRowWithDivergences.ProviderCount = 97;
            receiptWaybillRowWithDivergences.ProviderSum = 149;
            var receiptWaybillRowWithoutDivergences = new ReceiptWaybillRow(articleB, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRowWithoutDivergences);
            receiptWaybillRowWithoutDivergences.ReceiptedCount = 99;
            receiptWaybillRowWithoutDivergences.ProviderCount = 99;
            receiptWaybillRowWithoutDivergences.ProviderSum = 150;
            receiptWaybillRowWithoutDivergences.SetOutgoingArticleCount(0, 50, 0);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(299M, user, DateTime.Now);

            var beforeState = receiptWaybill.State;

            try
            {
                receiptWaybill.CancelReceipt();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить приемку накладной на склад, так как товар из нее используется в других документах.", ex.Message);
            }

            Assert.AreEqual(beforeState, receiptWaybill.State);
        }

        /// <summary>
        /// При попытке отмены приемки на склад с позициями, по которым было окончательно перемещение товара, должно происходить исключение.
        /// </summary>
        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_WithFinallyMovedPositions_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 300M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRowWithDivergences = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRowWithDivergences);
            receiptWaybillRowWithDivergences.ReceiptedCount = 98;
            receiptWaybillRowWithDivergences.ProviderCount = 97;
            receiptWaybillRowWithDivergences.ProviderSum = 149;
            var receiptWaybillRowWithoutDivergences = new ReceiptWaybillRow(articleB, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRowWithoutDivergences);
            receiptWaybillRowWithoutDivergences.ApprovedCount = 99;
            receiptWaybillRowWithoutDivergences.ReceiptedCount = 99;
            receiptWaybillRowWithoutDivergences.ProviderCount = 99;
            receiptWaybillRowWithoutDivergences.ProviderSum = 150M;
            receiptWaybillRowWithoutDivergences.SetOutgoingArticleCount(0, 0, 50);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(299M, user, DateTime.Now);

            var beforeState = receiptWaybill.State;

            try
            {
                receiptWaybill.CancelReceipt();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить приемку накладной на склад, так как товар из нее используется в других документах.", ex.Message);
            }

            Assert.AreEqual(beforeState, receiptWaybill.State);
        }

        /// <summary>
        /// Отмена приемки должна удалять позиции, добавленные при приемке (с ожидаемым количеством, равным 0).
        /// </summary>
        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_Must_Delete_Rows_Added_During_Receipt()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 200M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRowWithDivergences = new ReceiptWaybillRow(articleA, 99, 100M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 98, ProviderCount = 97, ProviderSum = 50 };
            var receiptWaybillRowWithoutDivergences = new ReceiptWaybillRow(articleB, 99, 100M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 99, ProviderSum = 50M };
            var receiptWaybillRowAddedDuringReceipt = new ReceiptWaybillRow(articleC, 0, 0M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 5, ProviderCount = 5, ProviderSum = 50M };

            receiptWaybill.AddRow(receiptWaybillRowWithDivergences);
            receiptWaybill.AddRow(receiptWaybillRowWithoutDivergences);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);

            receiptWaybill.AddRow(receiptWaybillRowAddedDuringReceipt);

            receiptWaybill.Receipt(150M, user, DateTime.Now);

            Assert.AreEqual(3, receiptWaybill.Rows.Count());

            receiptWaybill.CancelReceipt();

            Assert.AreEqual(2, receiptWaybill.Rows.Count());
            Assert.IsTrue(receiptWaybill.Rows.Contains(receiptWaybillRowWithDivergences));
            Assert.IsTrue(receiptWaybill.Rows.Contains(receiptWaybillRowWithoutDivergences));
            Assert.IsFalse(receiptWaybill.Rows.Contains(receiptWaybillRowAddedDuringReceipt));
            Assert.AreEqual(ReceiptWaybillState.AcceptedDeliveryPending, receiptWaybill.State);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_ApprovedWithoutDivergences_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 99;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now); // накладная без расхождений - после приемки автоматически происходит согласование

            var beforeState = receiptWaybill.State;

            try
            {
                receiptWaybill.CancelReceipt();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception Ex)
            {
                Assert.AreEqual("Невозможно отменить приемку окончательно согласованной накладной.", Ex.Message);
            }

            Assert.AreEqual(beforeState, receiptWaybill.State);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Receipt_On_Unapproved_Must_Throw_exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;

            var beforeState = receiptWaybill.State;

            try
            {
                receiptWaybill.CancelReceipt();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception Ex)
            {
                Assert.AreEqual("Невозможно отменить приемку непринятой накладной.", Ex.Message);
            }

            Assert.AreEqual(beforeState, receiptWaybill.State);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approve_Without_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 99, ProviderSum = 150M };
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now); 
            receiptWaybill.CancelApprovement();

            Assert.AreEqual(ReceiptWaybillState.AcceptedDeliveryPending, receiptWaybill.State);
            Assert.IsNull(receiptWaybill.ApprovedSum);
            Assert.AreEqual(0M, receiptWaybill.ReceiptedSum);
            Assert.IsNull(receiptWaybill.Rows.ElementAt(0).ApprovedSum);
            Assert.IsNull(receiptWaybill.Rows.ElementAt(0).ApprovedCount);
            Assert.IsNull(receiptWaybill.Rows.ElementAt(0).ApprovedPurchaseCost);
            Assert.IsNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approve_Count_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 5;
            receiptWaybillRow.ProviderCount = 5;
            receiptWaybillRow.ProviderSum = 150M;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now);

            receiptWaybillRow.ApprovedSum = 150M;
            receiptWaybillRow.ApprovedCount = 4;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            receiptWaybill.Approve(150M, user, DateTime.Now, priceLists);
            receiptWaybill.CancelApprovement();

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithCountDivergences, receiptWaybill.State);
            Assert.IsNull(receiptWaybill.ApprovedSum);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approve_Sum_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99M, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillRow.ReceiptedCount = 99M;
            receiptWaybillRow.ProviderCount = 99M;
            receiptWaybillRow.ProviderSum = 160M;

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            receiptWaybillRow.ApprovedSum = 150M;
            receiptWaybillRow.ApprovedCount = 99M;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            receiptWaybill.Approve(150M, user, DateTime.Now, priceLists);
            receiptWaybill.CancelApprovement();

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithSumDivergences, receiptWaybill.State);
            Assert.IsNull(receiptWaybill.ApprovedSum);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNull(receiptWaybill.ApprovementDate);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approve_Sum_Count_Divergences()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillRow.ReceiptedCount = 5;
            receiptWaybillRow.ProviderCount = 99;
            receiptWaybillRow.ProviderSum = 160;

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            receiptWaybillRow.ApprovedSum = 150M;
            receiptWaybillRow.ApprovedCount = 4;
            receiptWaybillRow.ApprovedPurchaseCost = Math.Round(receiptWaybillRow.ApprovedSum.Value / receiptWaybillRow.ApprovedCount.Value, 6);

            receiptWaybill.Approve(150M, user, DateTime.Now, priceLists);
            receiptWaybill.CancelApprovement();

            Assert.AreEqual(ReceiptWaybillState.ReceiptedWithSumAndCountDivergences, receiptWaybill.State);
            Assert.IsNull(receiptWaybill.ApprovedSum);
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approvement_On_Unapproved_Must_Throw_exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRow.ReceiptedCount = 99;
            receiptWaybillRow.ProviderCount = 0;
            receiptWaybillRow.ProviderSum = 160;

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            var beforeState = receiptWaybill.State;

            try
            {
                receiptWaybill.CancelApprovement();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception Ex)
            {
                Assert.AreEqual("Невозможно отменить согласование несогласованной накладной.", Ex.Message);
            }

            Assert.AreEqual(beforeState, receiptWaybill.State);

        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approvement_WithShippedPositions_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 99, ProviderSum = 150M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now); // происходит автоматическое согласование всей накладной
            receiptWaybillRow.SetOutgoingArticleCount(0, 45, 0);

            try
            {
                receiptWaybill.CancelApprovement();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить согласование накладной, так как товар из нее используется в других документах.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybill_Cancel_Approvement_WithFinallyMovedPositions_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 99, ProviderSum = 150M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(150M, user, DateTime.Now); // происходит автоматическое согласование всей накладной
            receiptWaybillRow.SetOutgoingArticleCount(0, 0, 45);

            try
            {
                receiptWaybill.CancelApprovement();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить согласование накладной, так как товар из нее используется в других документах.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybill_Add_Row_To_Approved_Must_Throw_Exception()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 150M, receiptWaybill.PendingValueAddedTax) { ReceiptedCount = 99, ProviderCount = 0, ProviderSum = 160M };

            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(160M, user, DateTime.Now);

            var receiptWaybillRowForFail = new ReceiptWaybillRow(new Article("Холодильник", articleGroup, measureUnit, true) { Id = 543 }, 20, 30, receiptWaybill.PendingValueAddedTax);
            
            var rowsList = receiptWaybill.Rows;

            try
            {
                receiptWaybill.AddRow(receiptWaybillRowForFail);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception Ex)
            {
                Assert.AreEqual("Невозможно добавить позицию к проведенной накладной.", Ex.Message);
            }

            Assert.AreEqual(rowsList.Count(), receiptWaybill.Rows.Count());
        }

        [TestMethod]
        public void ReceiptWaybill_Deletion_Must_SetDateAtChildren()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var receiptWaybillRowA = new ReceiptWaybillRow(articleA, 99, 151.23M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRowA.Id = Guid.NewGuid();
            var receiptWaybillRowB = new ReceiptWaybillRow(articleB, 109, 15.23M, receiptWaybill.PendingValueAddedTax);
            receiptWaybillRowB.Id = Guid.NewGuid();
            receiptWaybill.AddRow(receiptWaybillRowA);
            receiptWaybill.AddRow(receiptWaybillRowB);

            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(receiptWaybill.DeletionDate);

            receiptWaybill.DeletionDate = curDate;

            Assert.AreEqual(curDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[0].DeletionDate);
            Assert.AreEqual(curDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[1].DeletionDate);

            receiptWaybill.DeletionDate = nextDate;

            Assert.AreEqual(curDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[0].DeletionDate);
            Assert.AreEqual(curDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[1].DeletionDate);

            Assert.AreNotEqual(nextDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[0].DeletionDate);
            Assert.AreNotEqual(nextDate, receiptWaybill.Rows.ToArray<ReceiptWaybillRow>()[1].DeletionDate);
        }

        [TestMethod]
        public void ReceiptWaybill_ReDeletion_Must_Not_Work()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(receiptWaybill.DeletionDate);

            receiptWaybill.DeletionDate = curDate;

            Assert.AreEqual(curDate, receiptWaybill.DeletionDate);

            receiptWaybill.DeletionDate = nextDate;

            Assert.AreEqual(curDate, receiptWaybill.DeletionDate);
            Assert.AreNotEqual(nextDate, receiptWaybill.DeletionDate);
        }

        [TestMethod]
        public void ReceiptWaybillRow_ReDeletion_Must_Not_Work()
        {
            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 99, 151.23M, valueAddedTax18);
            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(receiptWaybillRow.DeletionDate);

            receiptWaybillRow.DeletionDate = curDate;

            Assert.AreEqual(curDate, receiptWaybillRow.DeletionDate);

            receiptWaybillRow.DeletionDate = nextDate;

            Assert.AreEqual(curDate, receiptWaybillRow.DeletionDate);
            Assert.AreNotEqual(nextDate, receiptWaybillRow.DeletionDate);
        }

        [TestMethod]
        public void ReceiptWaybill_If_Date_Is_Today_Hours_Minutes_Seconds_Must_Not_Be_0()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            Assert.AreEqual(0, receiptWaybill.Date.Hour + receiptWaybill.Date.Minute + receiptWaybill.Date.Second);            
        }

        [TestMethod]
        public void ReceiptWaybill_If_Date_Is_Later_Than_Today_Hours_Minutes_Seconds_Must_0()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(5), storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            Assert.AreEqual(0, receiptWaybill.Date.Hour + receiptWaybill.Date.Minute + receiptWaybill.Date.Second);
        }

        [TestMethod]
        public void ReceiptWaybill_If_Date_Is_Earlier_Than_Today_Hours_Minutes_Seconds_Must_0()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(-5), storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            Assert.AreEqual(0, receiptWaybill.Date.Hour + receiptWaybill.Date.Minute + receiptWaybill.Date.Second);
        }

        [TestMethod]
        public void ReceiptWaybillRow_Set_Of_Any_Count_ToLessThan0_MustThrowException()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(-5), storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var rowB = new ReceiptWaybillRow(articleB, 40, 40M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(rowB);

            Assert.IsTrue(rowB.CurrentCount > 0);
                        

            try
            {
                rowB.SetOutgoingArticleCount(-1, 0, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }

            try
            {
                rowB.SetOutgoingArticleCount(0, -1, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }

            try
            {
                rowB.SetOutgoingArticleCount(0, 0, -1);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество проведенного, отгруженного или окончательно перемещенного товара не может быть меньше 0.", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybillRow_Set_Of_Count_TooMuch_MustThrowException()
        {
            var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(-5), storage, accountOrganization, provider, 150M, 0M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            var rowB = new ReceiptWaybillRow(articleB, 40, 40M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(rowB);

            Assert.AreEqual(40, rowB.CurrentCount);
            
            try
            {
                rowB.SetOutgoingArticleCount(56, 0, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше текущего количества товара (ожидаемого или принятого).", ex.Message);
            }

            try
            {
                rowB.SetOutgoingArticleCount(0, 57, 0);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше текущего количества товара (ожидаемого или принятого).", ex.Message);
            }

            try
            {
                rowB.SetOutgoingArticleCount(0, 0, 58);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше текущего количества товара (ожидаемого или принятого).", ex.Message);
            }

            rowB.SetOutgoingArticleCount(0, 15, 0);

            try
            {
                rowB.SetOutgoingArticleCount(0, 15, 35);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Сумма проведенного, отгруженного и окончательно перемещенного товара не может быть больше текущего количества товара (ожидаемого или принятого).", ex.Message);
            }
        }

        [TestMethod]
        public void ReceiptWaybill_ReceiptWaybillRowInfo_Must_Set_Initial_Fields()
        {
            Guid guid = Guid.NewGuid();
            var receiptWaybillRowInfo = new ReceiptWaybillRowInfo(guid, 11M, 2M, 33M);

            Assert.AreEqual(guid, receiptWaybillRowInfo.Id);
            Assert.AreEqual(11M, receiptWaybillRowInfo.PendingSum);
            Assert.AreEqual(2M, receiptWaybillRowInfo.Count);
            Assert.AreEqual(33M, receiptWaybillRowInfo.PurchaseCost);

            receiptWaybillRowInfo.PurchaseCost = 32M;

            Assert.AreEqual(guid, receiptWaybillRowInfo.Id);
            Assert.AreEqual(11M, receiptWaybillRowInfo.PendingSum);
            Assert.AreEqual(2M, receiptWaybillRowInfo.Count);
            Assert.AreEqual(32M, receiptWaybillRowInfo.PurchaseCost);
        }

        [TestMethod]
        public void ReceiptWaybill_DistributeDiscountSum_Must_Set_PurchaseCost()
        {
           
            var receiptWaybill = new ReceiptWaybill_Accessor("123АБВ", DateTime.Today, storage, accountOrganization,
                provider, 1M, 0.01M, new ValueAddedTax("10%", 10), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            var receiptWaybillRow = new ReceiptWaybillRow(articleM, 1.111M, 1M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            receiptWaybillRow.ReceiptedCount = 1.111M;
            receiptWaybillRow.ProviderCount = 1.111M;
            receiptWaybillRow.ProviderSum = 1M;

            receiptWaybill.Accept(priceLists, user, DateTime.Now);
            receiptWaybill.Receipt(1M, user, DateTime.Now);

            Assert.AreEqual(ReceiptWaybillState.ApprovedWithoutDivergences, receiptWaybill.State);
            Assert.IsNotNull(receiptWaybill.ReceiptDate);
            Assert.IsNotNull(receiptWaybill.ApprovementDate);

            Assert.AreEqual(0.891089M, receiptWaybill.Rows.FirstOrDefault().PurchaseCost);
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void ReceiptWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                var receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(1), storage, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, null, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }
    }
}
