using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Test
{
    /// <summary>
    /// Тесты для сущности "Реестр цен"
    /// </summary>
    [TestClass]
    public class AccountingPriceListTest
    {
        #region Инициализация и общая часть
                
        private ArticleGroup articleGroup = null;
        private MeasureUnit measureUnit = null;
        private Article articleA = null;
        private Article articleB = null;
        private Article articleC = null;
        private ArticleAccountingPrice articleAccountingPriceA1 = null;
        private ArticleAccountingPrice articleAccountingPriceA2 = null;
        private ArticleAccountingPrice articleAccountingPriceA3 = null;
        private ArticleAccountingPrice articleAccountingPriceB = null;
        private ArticleAccountingPrice articleAccountingPriceC = null;
        private List<ArticleAccountingPrice> articleAccountingPriceWrongListOnlyA = null;
        private List<ArticleAccountingPrice> articleAccountingPriceCorrectList1 = null;
        private Storage storage1 = null;
        private Storage storage2 = null;
        private Storage storage3 = null;
        private List<Storage> storageList1 = null;
        private JuridicalPerson juridicalPerson;
        private AccountOrganization accountOrganization;
        private Provider provider;
        private ReceiptWaybill receiptWaybill;
        private AccountingPriceCalcRule priceRule;
        private LastDigitCalcRule digitRule;
        private ProviderOrganization providerOrganization;
        private PhysicalPerson physicalPerson;
        private ProviderContract providerContract;
        private User user;

        [TestInitialize]
        public void Init()
        {
            articleGroup = new ArticleGroup("Бытовая техника", "Бытовая техника");
            articleGroup.SalaryPercent = 15;
            articleGroup.Id = 8;

            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0);
            measureUnit.Id = 17;

            articleA = new Article("Пылесос", articleGroup, measureUnit, true) { Id = 29, Number = "ПЫЛ" };
            articleB = new Article("Холодильник", articleGroup, measureUnit, true) { Id = 38, Number = "ХО-1" };
            articleC = new Article("Плита газовая", articleGroup, measureUnit, true) { Id = 48, Number = "ПГ1" };

            articleAccountingPriceA1 = new ArticleAccountingPrice(articleA, 1M);
            articleAccountingPriceA2 = new ArticleAccountingPrice(articleA, 1001M);
            articleAccountingPriceA3 = new ArticleAccountingPrice(articleA, 1192.45M);
            articleAccountingPriceB = new ArticleAccountingPrice(articleB, 150M);
            articleAccountingPriceC = new ArticleAccountingPrice(articleC, 180M);

            articleAccountingPriceWrongListOnlyA = new List<ArticleAccountingPrice>();
            articleAccountingPriceWrongListOnlyA.Add(articleAccountingPriceA1);
            articleAccountingPriceWrongListOnlyA.Add(articleAccountingPriceA2);
            articleAccountingPriceWrongListOnlyA.Add(articleAccountingPriceA3);

            articleAccountingPriceCorrectList1 = new List<ArticleAccountingPrice>();
            articleAccountingPriceCorrectList1.Add(articleAccountingPriceA2);
            articleAccountingPriceCorrectList1.Add(articleAccountingPriceB);
            articleAccountingPriceCorrectList1.Add(articleAccountingPriceC);

            storage1 = new Storage("Торговая точка номер 1", StorageType.TradePoint);
            storage2 = new Storage("Доп. склад северный", StorageType.ExtraStorage);
            storage3 = new Storage("Торговая точка номер 2", StorageType.TradePoint);

            storageList1 = new List<Storage>();
            storageList1.Add(storage1);
            storageList1.Add(storage2);
            storageList1.Add(storage3);

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var physicalLegalForm = new LegalForm("ИП", EconomicAgentType.PhysicalPerson);

            juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            physicalPerson = new PhysicalPerson(physicalLegalForm) { Id = 2 };

            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 1 };
            providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", physicalPerson) { Id = 2 };

            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrganization);

            providerContract = new ProviderContract(accountOrganization, providerOrganization, "ABC", "123", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(providerContract);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            var customDeclarationNumber = new String('0', 25);

            receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today, storage1, accountOrganization, provider, 100.05M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);

            priceRule = new AccountingPriceCalcRule(
               new AccountingPriceCalcByPurchaseCost(PurchaseCostDeterminationRuleType.ByMinimalPurchaseCost, new MarkupPercentDeterminationRule(10)));
            digitRule = new LastDigitCalcRule(LastDigitCalcRuleType.SetCustom);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
        }

        #endregion

        #region Тесты конструкторов

        [TestMethod]
        public void AccountingPriceList_Revaluation_Initial_Parameters_Must_Be_Set()
        {
            DateTime date1 = DateTime.Today.AddYears(1), date2 = DateTime.Today.AddYears(2).SetHoursMinutesAndSeconds(23, 59, 59);
            var accountingPriceList = new AccountingPriceList("123", date1, date2, storageList1, user, priceRule, digitRule);

            Assert.AreEqual(Guid.Empty, accountingPriceList.Id);

            Assert.AreEqual("123", accountingPriceList.Number);
            Assert.AreEqual(date1, accountingPriceList.StartDate);
            Assert.AreEqual(date2, accountingPriceList.EndDate);

            Assert.AreEqual(AccountingPriceListState.New, accountingPriceList.State);

            Assert.AreEqual(AccountingPriceListReason.Revaluation, accountingPriceList.Reason);
            Assert.AreEqual("Переоценка", accountingPriceList.ReasonDescription);
                        
            Assert.AreEqual(3, accountingPriceList.Storages.Count());

            Assert.AreEqual(0, accountingPriceList.ArticlePrices.Count());
        }

        [TestMethod]
        public void AccountingPriceList_Revaluation_Must_Throw_Exception_On_NonUnique_Storages()
        {
            var storage = new Storage("Склад", StorageType.DistributionCenter) { Id = 1 };

            try
            {
                var list = new AccountingPriceList("123", DateTime.Now, null, new List<Storage>() { storage, storage }, user, priceRule, digitRule);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список мест хранения содержит повторяющиеся элементы.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceList_Autogeneration_Initial_Parameters_Must_Be_Set()
        {
            DateTime date1 = DateTime.Today.AddYears(1), date2 = DateTime.Today.AddYears(2).SetHoursMinutesAndSeconds(23, 59, 59);
            var accountingPriceList = new AccountingPriceList("234", date1, date2, storage1, articleAccountingPriceCorrectList1, user, priceRule, digitRule);

            Assert.AreEqual(Guid.Empty, accountingPriceList.Id);

            Assert.AreEqual("234", accountingPriceList.Number);
            Assert.AreEqual(date1.SetHoursMinutesAndSeconds(0,0,0), accountingPriceList.StartDate);
            Assert.AreEqual(date2.SetHoursMinutesAndSeconds(23, 59, 59), accountingPriceList.EndDate);

            Assert.AreEqual(AccountingPriceListState.New, accountingPriceList.State);

            Assert.AreEqual(AccountingPriceListReason.Storage, accountingPriceList.Reason);
            Assert.AreEqual("По месту хранения", accountingPriceList.ReasonDescription);
                        
            Assert.AreEqual(1, accountingPriceList.Storages.Count());

            Assert.AreEqual(articleAccountingPriceWrongListOnlyA.Count(), accountingPriceList.ArticlePrices.Count());
        }

        [TestMethod]
        public void AccountingPriceList_Autogeneration_Must_Throw_Exception_On_NonUnique_Articles1()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage2, articleAccountingPriceWrongListOnlyA, user, priceRule, digitRule);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список товаров содержит повторяющиеся элементы.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceList_Autogeneration_Must_Throw_Exception_On_NonUnique_Articles2()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage2, new List<ArticleAccountingPrice> { articleAccountingPriceA1, articleAccountingPriceA1 }, user, priceRule, digitRule);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список товаров содержит повторяющиеся элементы.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceList_ReceiptWaybill_Initial_Parameters_Must_Be_Set()
        {
            DateTime date1 = DateTime.Today.AddYears(1), date2 = DateTime.Today.AddYears(2).SetHoursMinutesAndSeconds(23, 59, 59);
            var accountingPriceList = new AccountingPriceList("234", date1, date2, receiptWaybill, storageList1, articleAccountingPriceCorrectList1, user, priceRule, digitRule);

            Assert.AreEqual(Guid.Empty, accountingPriceList.Id);

            Assert.AreEqual("234", accountingPriceList.Number);
            Assert.AreEqual(date1, accountingPriceList.StartDate);
            Assert.AreEqual(date2, accountingPriceList.EndDate);

            Assert.AreEqual(AccountingPriceListState.New, accountingPriceList.State);

            Assert.AreEqual(AccountingPriceListReason.ReceiptWaybill, accountingPriceList.Reason);
            Assert.AreEqual("Приход №123АБВ от " + DateTime.Today.ToShortDateString(), accountingPriceList.ReasonDescription);
                        
            Assert.AreEqual(3, accountingPriceList.Storages.Count());

            Assert.AreEqual(articleAccountingPriceWrongListOnlyA.Count(), accountingPriceList.ArticlePrices.Count());
        }

        [TestMethod]
        public void AccountingPriceList_ReceiptWaybill_Must_Throw_Exception_On_NonUnique_Articles1()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), receiptWaybill, storageList1, articleAccountingPriceWrongListOnlyA, user, priceRule, digitRule);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список товаров содержит повторяющиеся элементы.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceList_ReceiptWaybill_Must_Throw_Exception_On_NonUnique_Articles2()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), receiptWaybill, storageList1, new List<ArticleAccountingPrice> { articleAccountingPriceA1, articleAccountingPriceA1 }, user, priceRule, digitRule);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Список товаров содержит повторяющиеся элементы.", ex.Message);
            }
        }

        #endregion

        #region Проверка даты удаления реестров и товаров

        [TestMethod]
        public void AccountingPriceList_ReDeletion_Must_Not_Work()
        {
            var accountingPriceList = new AccountingPriceList("123", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), new List<Storage>() {storage1}, user);

            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(accountingPriceList.DeletionDate);

            accountingPriceList.DeletionDate = curDate;

            Assert.AreEqual(curDate, accountingPriceList.DeletionDate);

            accountingPriceList.DeletionDate = nextDate;

            Assert.AreEqual(curDate, accountingPriceList.DeletionDate);
            Assert.AreNotEqual(nextDate, accountingPriceList.DeletionDate);
        }

        [TestMethod]
        public void AccountingPriceList_Deletion_Must_SetDateAtChildren()
        {
            var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage1, articleAccountingPriceCorrectList1, user);

            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(accountingPriceList.DeletionDate);

            accountingPriceList.DeletionDate = curDate;

            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[0].DeletionDate);
            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[1].DeletionDate);
            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[2].DeletionDate);

            accountingPriceList.DeletionDate = nextDate;

            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[0].DeletionDate);
            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[1].DeletionDate);
            Assert.AreEqual(curDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[2].DeletionDate);

            Assert.AreNotEqual(nextDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[0].DeletionDate);
            Assert.AreNotEqual(nextDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[1].DeletionDate);
            Assert.AreNotEqual(nextDate, accountingPriceList.ArticlePrices.ToArray<ArticleAccountingPrice>()[2].DeletionDate);
        }

        #endregion

        #region Проверка статических методов для установки и проверки дат

        [TestMethod]
        public void AccountingPriceList_StartDateTimeMustBeSet()
        {
            bool changed;
            DateTime startDate;

            // т.к. дата начала действяи РЦ меньше текущей даты, то дата начало должна стать равной текущей
            startDate = DateTime.Now.AddDays(-1);
            changed = AccountingPriceList.SetTimeForStartDateAndCheck(ref startDate, DateTime.Now);
            Assert.AreEqual(0, (startDate - DateTime.Now).Minutes); // разница должна быть меньше минуты 
            Assert.IsTrue(changed);

            // т.к. дата начала действия больше текущей, то она не должна измениться
            startDate = DateTime.Now.AddDays(1);
            var oldStartDate = startDate;
            changed = AccountingPriceList.SetTimeForStartDateAndCheck(ref startDate, DateTime.Now);
            Assert.AreEqual(oldStartDate, startDate);
            Assert.IsFalse(changed);
        }
        #endregion

        #region Работа с товарами

        #endregion

        #region Работа с местами хранения

        [TestMethod]
        public void AccountingPriceList_CantDeleteStoragesFromAutocreation()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage1, articleAccountingPriceCorrectList1, user);

                accountingPriceList.RemoveStorage(storage1);

                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно удалить место хранения из распространения реестра с основанием «По месту хранения».", ex.Message);
            }
        }

        #endregion

        #region Приемка и отмена

        [TestMethod]
        public void AccountingPriceList_AcceptAndCancellationMustWork()
        {
            var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage1, articleAccountingPriceCorrectList1, user);

            var dateBeforeAccept= DateTime.Now;
            accountingPriceList.Accept(DateTime.Now);
            var dateAfterAccept= DateTime.Now;
            Assert.AreEqual(AccountingPriceListState.Accepted, accountingPriceList.State);
            Assert.IsNotNull(accountingPriceList.AcceptanceDate);
            Assert.IsTrue(dateBeforeAccept<= accountingPriceList.AcceptanceDate && accountingPriceList.AcceptanceDate <= dateAfterAccept);

            accountingPriceList.CancelAcceptance();
            Assert.AreEqual(AccountingPriceListState.New, accountingPriceList.State);
            Assert.IsNull(accountingPriceList.AcceptanceDate);
        }

        [TestMethod]
        public void AccountingPriceList_CantCreateWithZeroStorages()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), new List<Storage>(), user);
                
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не выбрано ни одного места хранения.", ex.Message);
            }
        }

        [TestMethod]
        public void AccountingPriceList_CantAcceptWithZeroArticles()
        {
            try
            {
                var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), new List<Storage>() {storage1}, user);

                accountingPriceList.Accept(DateTime.Now);
                Assert.Fail("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно провести реестр цен без товаров.", ex.Message);
            }
        }      

        #endregion

        #region Перенести в тест сервиса индикаторов AccountingPriceListMainIndicatorServiceTest

        //[TestMethod]
        //public void AccountingPriceList_SumsMustRecalculateOk()
        //{
        //    var accountingPriceList = new AccountingPriceList("234", DateTime.Today.AddYears(1), DateTime.Today.AddYears(2), storage1, articleAccountingPriceCorrectList1, user);

        //    Assert.IsNull(accountingPriceList.PurchaseCostSum);
        //    Assert.IsNull(accountingPriceList.OldAccountingPriceSum);
        //    Assert.IsNull(accountingPriceList.NewAccountingPriceSum);
        //    Assert.AreEqual(AccountingPriceListState.New, accountingPriceList.State);
        //    Assert.AreEqual(false, accountingPriceList.AreIndicatorsCalculated);

        //    accountingPriceList.SetIndicators(100M, 200M, 260M);
        //    Assert.AreEqual(100M, accountingPriceList.PurchaseCostSum);
        //    Assert.AreEqual(200M, accountingPriceList.OldAccountingPriceSum);
        //    Assert.AreEqual(260M, accountingPriceList.NewAccountingPriceSum);
        //    Assert.IsTrue(accountingPriceList.AreIndicatorsCalculated);

        //    Assert.AreEqual(160M, accountingPriceList.PurchaseMarkupSum);
        //    Assert.AreEqual(160M, accountingPriceList.PurchaseMarkupPercent);
        //    Assert.AreEqual(60M, accountingPriceList.AccountingPriceDifSum);
        //    Assert.AreEqual(30M, accountingPriceList.AccountingPriceDifPercent);
        //}

        // TODO: Тестировать AccountingPriceDifSum, PurchaseMarkupPercent, ..., а именно деление на 0 и исключения
        // при неинициализированных суммах. В тесте сервиса

        #endregion

        // TODO: Тестировать запись (enum)0 в поля для трех enum-ов. Должны быть исключения.

        // TODO: AccountingPriceList_ReasonDescription_Must_Work_On_Receipt() - тест реестра с причиной Waybill
        // с несколькими объектами с разными номерами Waybill-ов и разными датами Waybill-ов, проверяем ReasonDescription

    }
}
