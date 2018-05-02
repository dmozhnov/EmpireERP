using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class MovementWaybillTest
    {
        #region Конструкторы и инициализация

        private string numberA;
        private JuridicalPerson juridicalPersonA;
        private JuridicalPerson juridicalPersonB;
        private JuridicalPerson juridicalPersonC;
        private JuridicalPerson juridicalPersonD;
        private AccountOrganization senderOrganizationA;
        private AccountOrganization senderOrganizationB;
        private AccountOrganization receiverOrganizationC;
        private AccountOrganization receiverOrganizationD;
        private Storage storageA;
        private Storage storageB;
        private ArticleGroup articleGroup;
        private MeasureUnit measureUnit;
        private Article articleA;
        private Article articleB;
        private Article articleC;
        private ReceiptWaybillRow receiptWaybillRowA1;
        private ReceiptWaybillRow receiptWaybillRowA2;
        private ReceiptWaybillRow receiptWaybillRowB;
        private ReceiptWaybillRow receiptWaybillRowC;
        private MovementWaybillRow rowA1_1;
        private MovementWaybillRow rowA1_2;
        private MovementWaybillRow rowA2_1;
        private MovementWaybillRow rowA2_2;
        private MovementWaybillRow rowB;
        private MovementWaybillRow rowC;
        private List<ArticleAccountingPrice> priceLists;
        private User user;
        private ValueAddedTax valueAddedTax;

        [TestInitialize]
        public void Init()
        {
            numberA = "98";

            var legalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);

            juridicalPersonA = new JuridicalPerson(legalForm);
            juridicalPersonB = new JuridicalPerson(legalForm);
            juridicalPersonC = new JuridicalPerson(legalForm);
            juridicalPersonD = new JuridicalPerson(legalForm);

            senderOrganizationA = new AccountOrganization("Тестовое юридическое лицо A", "Тестовое юридическое лицо A", juridicalPersonA);
            senderOrganizationB = new AccountOrganization("Тестовое юридическое лицо B", "Тестовое юридическое лицо B", juridicalPersonB);
            receiverOrganizationC = new AccountOrganization("Тестовое юридическое лицо C", "Тестовое юридическое лицо C", juridicalPersonC);
            receiverOrganizationD = new AccountOrganization("Тестовое юридическое лицо D", "Тестовое юридическое лицо D", juridicalPersonD);

            storageA = new Storage("Тестовое хранилище A", StorageType.DistributionCenter) { Id = 1 };
            storageB = new Storage("Тестовое хранилище B", StorageType.TradePoint) { Id = 2 };

            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, true);
            articleB = new Article("Тестовый товар B", articleGroup, measureUnit, true);
            articleC = new Article("Тестовый товар C", articleGroup, measureUnit, true);
            valueAddedTax = new ValueAddedTax("18%", 18);

            receiptWaybillRowA1 = new ReceiptWaybillRow(articleA, 300, 3000, new ValueAddedTax("18%", 18));
            receiptWaybillRowA2 = new ReceiptWaybillRow(articleA, 400, 4000, new ValueAddedTax("18%", 18));
            receiptWaybillRowB = new ReceiptWaybillRow(articleB, 20, 250, new ValueAddedTax("18%", 18));
            receiptWaybillRowC = new ReceiptWaybillRow(articleC, 20, 250, new ValueAddedTax("18%", 18));

            rowA1_1 = new MovementWaybillRow(receiptWaybillRowA1, 60, valueAddedTax);
            rowA1_2 = new MovementWaybillRow(receiptWaybillRowA1, 22, valueAddedTax);
            rowA2_1 = new MovementWaybillRow(receiptWaybillRowA2, 40, valueAddedTax);
            rowA2_2 = new MovementWaybillRow(receiptWaybillRowA2, 55, valueAddedTax);
            rowB = new MovementWaybillRow(receiptWaybillRowB, 15, valueAddedTax);
            rowC = new MovementWaybillRow(receiptWaybillRowC, 18, valueAddedTax);

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100), new ArticleAccountingPrice(articleB, 200),
                new ArticleAccountingPrice(articleC, 300)};

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
        }

        #endregion

        /// <summary>
        /// Тест конструктора MovementWaybillRow - начальные параметры должны быть установлены
        ///</summary>
        [TestMethod]
        public void MovementWaybillRow_InitialParameters_MustBeSet()
        {
            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 300, 3000, valueAddedTax);
            var row = new MovementWaybillRow(receiptWaybillRow, 10, valueAddedTax);

            Assert.AreEqual(articleA.FullName, row.Article.FullName);
            Assert.AreEqual(10, row.MovingCount);
            Assert.AreEqual(0, row.FinallyMovedCount);
            Assert.AreEqual(0, row.AcceptedCount);
            Assert.IsNotNull(row.ReceiptWaybillRow);
            Assert.IsNull(row.DeletionDate);
            Assert.IsNotNull(row.CreationDate);
            Assert.AreEqual(OutgoingWaybillRowState.Undefined, row.OutgoingWaybillRowState);
        }

        /// <summary>
        /// Повторное удаление не должно работать
        /// </summary>
        [TestMethod]
        public void MovementWaybillRow_ReDeletion_Must_Not_Work()
        {
            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 300, 3000, valueAddedTax);
            var row = new MovementWaybillRow(receiptWaybillRow, 10, valueAddedTax);

            var curDate = DateTime.Now;
            var nextDate = curDate + new TimeSpan(1, 0, 0, 0);

            Assert.IsNull(row.DeletionDate);

            row.DeletionDate = curDate;

            Assert.AreEqual(curDate, row.DeletionDate);

            row.DeletionDate = nextDate;

            Assert.AreEqual(curDate, row.DeletionDate);
            Assert.AreNotEqual(nextDate, row.DeletionDate);
        }

        /// <summary>
        /// Тест конструктора MovementWaybill - начальные параметры должны быть установлены
        ///</summary>
        [TestMethod]
        public void MovementWaybill_InitialParameters_MustBeSet()
        {
            var date = DateTime.Now;

            MovementWaybill waybill = new MovementWaybill(numberA, date, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, user, date);

            Assert.AreEqual(0, waybill.Date.Hour + waybill.Date.Minute + waybill.Date.Second);
            Assert.AreEqual(numberA, waybill.Number);
            Assert.AreEqual(storageA.Name, waybill.SenderStorage.Name);
            Assert.AreEqual(senderOrganizationA.ShortName, waybill.Sender.ShortName);
            Assert.AreEqual(storageB.Name, waybill.RecipientStorage.Name);
            Assert.AreEqual(receiverOrganizationC.ShortName, waybill.Recipient.ShortName);
            Assert.AreEqual(user, waybill.Curator);

            Assert.IsNull(waybill.DeletionDate);
            Assert.IsNotNull(waybill.CreationDate);
            Assert.IsNotNull(waybill.Rows);
            Assert.AreEqual(0, waybill.RowCount);
            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
            Assert.AreEqual(String.Empty, waybill.Comment);
        }

        [TestMethod]
        public void MovementWaybill_IndicatorsMustRecalculateOk()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Today, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, user, DateTime.Now);

            //waybill.SetIndicators(1100M, 2234M, 2678.85M);

            //Assert.AreEqual(1100M, waybill.PurchaseCostSum);
            //Assert.AreEqual(2234M, waybill.SenderAccountingPriceSum);
            //Assert.AreEqual(2678.85M, waybill.RecipientAccountingPriceSum);
            //Assert.AreEqual(2678.85M - 2234M, waybill.MovementMarkupSum);
            //Assert.AreEqual(Math.Round((2678.85M - 2234M) / 2234M, 2), waybill.MovementMarkupPercent);

            //waybill.SetIndicators(1106M, 2289M, 3678.85M);

            //Assert.AreEqual(1106M, waybill.PurchaseCostSum);
            //Assert.AreEqual(2289M, waybill.SenderAccountingPriceSum);
            //Assert.AreEqual(3678.85M, waybill.RecipientAccountingPriceSum);
            //Assert.AreEqual(3678.85M - 2289M, waybill.MovementMarkupSum);
            //Assert.AreEqual(Math.Round((3678.85M - 2289M) / 2289M, 2), waybill.MovementMarkupPercent);
        }


        [TestMethod]
        public void MovementWaybill_Creation_Must_Throw_Exception_If_SenderStorage_Equals_RecipientStorage()
        {
            try
            {
                var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageA, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Места хранения отправителя и получателя не могут совпадать.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_If_Date_Is_Today_Hours_Minutes_Seconds_Must_Not_Be_0()
        {
            var movementWaybill = new MovementWaybill("123", DateTime.Today, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now) { Id = Guid.NewGuid() };
            Assert.AreEqual(0.ToString(), (movementWaybill.Date.Hour + movementWaybill.Date.Minute + movementWaybill.Date.Hour).ToString());
        }

        [TestMethod]
        public void MovementWaybill_If_Date_Is_Later_Than_Today_Hours_Minutes_Seconds_Must_0()
        {
            var movementWaybill = new MovementWaybill("123", DateTime.Today.AddDays(5), storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
            Assert.AreEqual(0, movementWaybill.Date.Hour + movementWaybill.Date.Minute + movementWaybill.Date.Hour);
        }

        [TestMethod]
        public void MovementWaybill_If_Date_Is_Earlier_Than_Today_Hours_Minutes_Seconds_Must_0()
        {
            var movementWaybill = new MovementWaybill("123", DateTime.Today.AddDays(-5), storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
            Assert.AreEqual(0, movementWaybill.Date.Hour + movementWaybill.Date.Minute + movementWaybill.Date.Hour);
        }

        [TestMethod]
        public void MovementWaybill_Must_Throw_Exception_On_Storage_Change_If_SenderStorage_Equals_RecipientStorage()
        {
            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
            try
            {
                waybill.RecipientStorage = storageA;
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Места хранения отправителя и получателя не могут совпадать.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_Must_Throw_Exception_On_Change_SenderStorage()
        {
            var waybill = new MovementWaybill_Accessor("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
            try
            {
                waybill.SenderStorage = new Storage("МХ", StorageType.DistributionCenter);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Для накладной перемещения невозможно сменить место хранения-отправителя.", ex.Message);
            }
        }

        [TestMethod]
        public void MovementWaybill_Must_Throw_Exception_On_Setting_RecipientStorage_To_Null()
        {
            var waybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB, valueAddedTax, user, user, DateTime.Now);
            try
            {
                waybill.RecipientStorage = null;
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Место хранения-получатель не указано.", ex.Message);
            }
        }

        //[TestMethod]
        //public void MovementWaybill_If_OutcomingWaybill_Exist_Row_Cannot_Be_Deleted()
        //{
        //    var accPriceA_sender = new ArticleAccountingPrice(articleA, 50);
        //    var accPriceB_sender = new ArticleAccountingPrice(articleB, 60);
        //    var accPriceB_recipient = new ArticleAccountingPrice(articleB, 70);
        //    var accPriceC_recipient = new ArticleAccountingPrice(articleC, 80);

        //    var senderPriceLists = new List<ArticleAccountingPrice> { accPriceA_sender, accPriceB_sender };
        //    var recipientPriceLists = new List<ArticleAccountingPrice> { accPriceB_recipient, accPriceC_recipient };

        //    var movementWaybill = new MovementWaybill("123", DateTime.Now, storageA, senderOrganizationA, storageB, senderOrganizationB);
        //    movementWaybill.AddRow(rowA1_1);

        //    Provider provider;
        //    provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium);
        //    provider.AddOrganization(senderOrganizationA.Organization);

        //    var receiptWaybill = new ReceiptWaybill("321", DateTime.Now, storageA, senderOrganizationA, provider, 5000, new ValueAddedTax("10%", 10));
        //    receiptWaybill.AddRow(rowA1_1.ReceiptWaybillRow);
        //    rowA1_1.ReceiptWaybillRow.PendingSum = 5000;
        //    receiptWaybill.Receipt(5000, 500, new List<ArticleAccountingPrice>());

        //    movementWaybill.DeleteRow(rowA1_1);

        //    movementWaybill.AddRow(rowB);
        //    movementWaybill.AddRow(rowC);

        //    movementWaybill.SetAsReadyToShip();
        //    movementWaybill.Ship(100M, 100M, 100M, senderPriceLists, recipientPriceLists);
        //    movementWaybill.CancelShipping();

        //    Assert.IsNull(rowA1_1.SenderArticleAccountingPrice);
        //    Assert.IsNull(rowB.SenderArticleAccountingPrice);
        //    Assert.IsNull(rowC.SenderArticleAccountingPrice);

        //    Assert.IsNull(rowA1_1.RecipientArticleAccountingPrice);
        //    Assert.IsNull(rowB.RecipientArticleAccountingPrice);
        //    Assert.IsNull(rowC.RecipientArticleAccountingPrice);

        //}

        private MovementWaybill InitMovementWaybill()
        {
            MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Now, storageA, senderOrganizationA,
                storageB, receiverOrganizationC, valueAddedTax, user, user, DateTime.Now);
            var receiptWaybillRow = new ReceiptWaybillRow(articleA, 300, 3000, valueAddedTax);
            var row = new MovementWaybillRow(receiptWaybillRow, 10, valueAddedTax);
            waybill.AddRow(row);

            return waybill;
        }

        /// <summary>
        /// Подготовка к проводке должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void MovementWaybill_PrepareToAccept_Must_Set_ReadyToAcceptState()
        {
            var waybill = InitMovementWaybill();

            waybill.PrepareToAccept();

            Assert.AreEqual(MovementWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Подготовка к проводке накладной, не имеющей позиций, должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void MovementWaybill_PrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                MovementWaybill waybill = new MovementWaybill(numberA, DateTime.Now, storageA, senderOrganizationA,
                    storageB, receiverOrganizationC, valueAddedTax, user, user, DateTime.Now);

                waybill.PrepareToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно подготовить к проводке накладную, не содержащую ни одной позиции.", ex.Message);
            }
        }

        /// <summary>
        /// Повторная подготовка к проводке накладной должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void MovementWaybill_RePrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitMovementWaybill();

                waybill.PrepareToAccept();

                waybill.PrepareToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно подготовить к проводке накладную со статусом «Готово к проводке».", ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке накладной должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void MovementWaybill_CancelReadinessToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitMovementWaybill();
                
                waybill.CancelReadinessToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Накладная еще не подготовлена к проводке.", ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке накладной должна пройти успешно
        ///</summary>
        [TestMethod]
        public void MovementWaybill_CancelReadinessToAccept_Must_Be_Ok()
        {
            var waybill = InitMovementWaybill();

            waybill.PrepareToAccept();

            waybill.CancelReadinessToAccept();

            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при запрещенной опции использования статуса «Готово к проводке» должна пройти успешно
        ///</summary>
        [TestMethod]
        public void MovementWaybill_Must_Be_Accepted_Ok()
        {
            var waybill = InitMovementWaybill();

            waybill.Accept(priceLists, priceLists, false, user, DateTime.Now);

            Assert.AreEqual(MovementWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при разрешенной опции использования статуса «Готово к проводке» должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void MovementWaybill_Accept_From_Draft_If_Use_ReadyToacceptState_Denied_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitMovementWaybill();

                waybill.Accept(priceLists, priceLists, true, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно провести накладную из состояния «Черновик».", ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки накладной при запрещенной опции использования статуса «Готово к проводке» должна выставить статус «Черновик»
        ///</summary>
        [TestMethod]
        public void MovementWaybill_CancelAcceptance_Must_Set_DraftState()
        {
            var waybill = InitMovementWaybill();

            waybill.Accept(priceLists, priceLists, false, user, DateTime.Now);

            waybill.CancelAcceptance(false);

            Assert.AreEqual(MovementWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки накладной при разрешенной опции использования статуса «Готово к проводке» должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void MovementWaybill_CancelAcceptance_Must_Set_ReadyToAcceptState()
        {
            var waybill = InitMovementWaybill();

            waybill.Accept(priceLists, priceLists, false, user, DateTime.Now);

            waybill.CancelAcceptance(true);

            Assert.AreEqual(MovementWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void MovementWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                var date = DateTime.Now;

                MovementWaybill waybill = new MovementWaybill(numberA, date, storageA, senderOrganizationA,
                    storageB, receiverOrganizationC, valueAddedTax, null, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }
    }  
}
