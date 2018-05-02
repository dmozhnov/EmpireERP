using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ChangeOwnerWaybillTest
    {
        #region Конструкторы и инициализация

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
        private ChangeOwnerWaybillRow rowA1_1;
        private ChangeOwnerWaybillRow rowA1_2;
        private ChangeOwnerWaybillRow rowA2_1;
        private ChangeOwnerWaybillRow rowA2_2;
        private ChangeOwnerWaybillRow rowB;
        private ChangeOwnerWaybillRow rowC;
        private List<ArticleAccountingPrice> priceLists;
        private ValueAddedTax valueAddedTax;
        private User user;

        [TestInitialize]
        public void Init()
        {
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
            articleA = new Article("Тестовый товар A", articleGroup, measureUnit, true) { Id = 1 };
            articleB = new Article("Тестовый товар B", articleGroup, measureUnit, true) { Id = 2 };
            articleC = new Article("Тестовый товар C", articleGroup, measureUnit, true) { Id = 3 };

            receiptWaybillRowA1 = new ReceiptWaybillRow(articleA, 300, 3000, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            receiptWaybillRowA1.PurchaseCost = receiptWaybillRowA1.PendingSum / receiptWaybillRowA1.PendingCount;

            receiptWaybillRowA2 = new ReceiptWaybillRow(articleA, 400, 4000, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            receiptWaybillRowA2.PurchaseCost = receiptWaybillRowA1.PendingSum / receiptWaybillRowA1.PendingCount;

            receiptWaybillRowB = new ReceiptWaybillRow(articleB, 20, 250, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            receiptWaybillRowB.PurchaseCost = receiptWaybillRowA1.PendingSum / receiptWaybillRowA1.PendingCount;

            receiptWaybillRowC = new ReceiptWaybillRow(articleC, 20, 250, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            receiptWaybillRowC.PurchaseCost = receiptWaybillRowA1.PendingSum / receiptWaybillRowA1.PendingCount;

            rowA1_1 = new ChangeOwnerWaybillRow(receiptWaybillRowA1, 60, valueAddedTax) { Id = Guid.NewGuid() };
            rowA1_2 = new ChangeOwnerWaybillRow(receiptWaybillRowA1, 22, valueAddedTax) { Id = Guid.NewGuid() };
            rowA2_1 = new ChangeOwnerWaybillRow(receiptWaybillRowA2, 40, valueAddedTax) { Id = Guid.NewGuid() };
            rowA2_2 = new ChangeOwnerWaybillRow(receiptWaybillRowA2, 55, valueAddedTax) { Id = Guid.NewGuid() };
            rowB = new ChangeOwnerWaybillRow(receiptWaybillRowB, 15, valueAddedTax) { Id = Guid.NewGuid() };
            rowC = new ChangeOwnerWaybillRow(receiptWaybillRowC, 18, valueAddedTax) { Id = Guid.NewGuid() };

            priceLists = new List<ArticleAccountingPrice>()
            {
                new ArticleAccountingPrice(articleA, 1200),
                new ArticleAccountingPrice(articleB, 400),
                new ArticleAccountingPrice(articleC, 984)
            };

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);

            valueAddedTax = new ValueAddedTax("НДС", 18);
        }

        #endregion

        #region Конструктор

        [TestMethod]
        public void ChangeOwnerWaybill_Initial_Params_Must_Be_Set()
        {
            string number = "42";
            var date1 = DateTime.Now;

            var waybill = new ChangeOwnerWaybill(number, date1, storageA, senderOrganizationA, receiverOrganizationC, valueAddedTax, user, user, DateTime.Now);

            var date2 = DateTime.Now;

            Assert.AreEqual(null, waybill.AcceptanceDate);
            Assert.AreEqual(0, waybill.AccountingPriceSum);
            Assert.AreEqual(String.Empty, waybill.Comment);
            Assert.AreEqual(true, date1 <= waybill.CreationDate && waybill.CreationDate <= date2);
            Assert.AreEqual(null, waybill.DeletionDate);
            Assert.AreEqual(false, waybill.IsAccepted);
            Assert.AreEqual(true, waybill.IsNew);
            Assert.AreEqual(number, waybill.Number);
            Assert.AreEqual(0, waybill.PurchaseCostSum);
            Assert.AreEqual(receiverOrganizationC, waybill.Recipient);
            Assert.AreEqual(0, waybill.RowCount);
            Assert.AreEqual(senderOrganizationA, waybill.Sender);
            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
            Assert.AreEqual(storageA, waybill.Storage);
            Assert.AreEqual(valueAddedTax, waybill.ValueAddedTax);
            Assert.AreEqual(user, waybill.Curator);
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void ChangeOwnerWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                string number = "42";
                var date = DateTime.Now;

                var waybill = new ChangeOwnerWaybill(number, date, storageA, senderOrganizationA, receiverOrganizationC, valueAddedTax, null, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }

        #endregion

        #region Добавление

        [TestMethod]
        public void ChangeOwnerWaybill_AddRow_Success()
        {
            var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
            waybill.AddRow(rowA1_1);
            waybill.AddRow(rowB);

            Assert.AreEqual(2, waybill.RowCount);
            Assert.AreEqual(rowA1_1, waybill.Rows.ElementAt(0));
            Assert.AreEqual(rowB, waybill.Rows.ElementAt(1));
        }

        [TestMethod]
        public void ChangeOwnerWaybill_AddRow_Some_Row_From_One_Article_And_Batch_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
                waybill.AddRow(rowA1_1);
                waybill.AddRow(rowA1_2);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной по данной партии товара уже добавлена.", ex.Message);
            }
        }

        [TestMethod]
        public void ChangeOwnerWaybill_AddRow_To_Acceptence_Waybill()
        {
            //try
            //{
            //    var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user);
            //    waybill.AddRow(rowA1_1);

            //    waybill.Accept(new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(articleA, 100) });
            //    waybill.AddRow(rowA1_1);

            //    Assert.Fail("Должно быть сгенерировано исключение, т.к. невозможно добавлять позиции в принятую накладную.");
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно добавить позицию в накладную со статусом «Перемещено».", ex.Message);
            //}
        }

        #endregion

        #region Приемка накладной

        [TestMethod]
        public void ChangeOwnerWaybill_Accept_Success()
        {
            //var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user);
            //waybill.AddRow(rowA1_1);
            //waybill.AddRow(rowB);

            //var date1 = DateTime.Now;
            //waybill.Accept(priceLists);
            //var date2 = DateTime.Now;

            //Assert.AreEqual(true, date1 <= waybill.AcceptanceDate && waybill.AcceptanceDate <= date2);

            //Assert.AreEqual(78000, waybill.AccountingPriceSum);

            //Assert.AreEqual(true, waybill.IsAccepted);
            //Assert.AreEqual(false, waybill.IsNew);
            //Assert.AreEqual(750, waybill.PurchaseCostSum);
            //Assert.AreEqual(ChangeOwnerWaybillState.OwnerChanged, waybill.State);
        }

        [TestMethod]
        public void ChangeOwnerWaybill_Double_Accept_Must_Be_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
                waybill.AddRow(rowA1_1);
                waybill.AddRow(rowB);

                waybill.Accept(priceLists, false, user, DateTime.Now);
                waybill.Accept(priceLists, false, user, DateTime.Now);

                Assert.Fail("Повторная проводка накладной не сгененрировала исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно провести накладную из состояния «Ожидание товара».", ex.Message);
            }
        }

        [TestMethod]
        public void ChangeOwnerWaybill_Accept_Empty_Waybill_Must_Be_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);

                waybill.Accept(priceLists, false, user, DateTime.Now);

                Assert.Fail("Проводка пустой накладной должна генерировать исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно провести накладную без позиций.", ex.Message);
            }
        }

        #endregion

        #region Отмена проводки накладной

        [TestMethod]
        public void ChangeOwnerWaybill_CancelAccept_Success()
        {
            var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
            waybill.AddRow(rowA1_1);
            waybill.AddRow(rowB);

            waybill.Accept(priceLists, false, user, DateTime.Now);

            waybill.CancelAcceptance(false);

            Assert.AreEqual(null, waybill.AcceptanceDate);
            Assert.AreEqual(false, waybill.IsAccepted);
            Assert.AreEqual(true, waybill.IsNew);
            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        [TestMethod]
        public void ChangeOwnerWaybill_CancelAccept_With_Reservation_Article_Fail()
        {
            //try
            //{
            //    var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user);
            //    waybill.AddRow(rowA1_1);
            //    waybill.AddRow(rowB);

            //    waybill.Accept(priceLists);

            //    rowB.ReservedCount = 1;

            //    waybill.CancelAcceptance();
            //}
            //catch (Exception ex)
            //{
            //    Assert.AreEqual("Невозможно отменить проводку накладной, т.к. ее позиции участвуют в дальнейшем товародвижении.", ex.Message);
            //}
        }

        [TestMethod]
        public void ChangeOwnerWaybill_CancelAccept_Not_Accepted_Waybill_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
                waybill.AddRow(rowA1_1);
                waybill.AddRow(rowB);

                waybill.CancelAcceptance(false);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить проводку накладной со статусом «Черновик».", ex.Message);
            }
        }

        #endregion

        #region Удаление строки

        [TestMethod]
        public void ChangeOwnerWaybill_DeleteRow_Delete_One_Row_Success()
        {
            var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
            waybill.AddRow(rowA1_1);
            waybill.AddRow(rowB);

            waybill.DeleteRow(rowA1_1);

            Assert.AreEqual(1, waybill.RowCount);
            Assert.AreEqual(rowB, waybill.Rows.ElementAt(0));
            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        [TestMethod]
        public void ChangeOwnerWaybill_DeleteRow_Delete_All_Row_Success()
        {
            var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
            waybill.AddRow(rowA1_1);
            waybill.AddRow(rowB);

            waybill.DeleteRow(rowA1_1);
            waybill.DeleteRow(rowB);

            Assert.AreEqual(0, waybill.RowCount);
            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        [TestMethod]
        public void ChangeOwnerWaybill_DeleteRow_From_Acceptence_Waybill_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
                waybill.AddRow(rowA1_1);
                waybill.AddRow(rowB);

                waybill.Accept(priceLists, false, user, DateTime.Now);

                waybill.DeleteRow(rowA1_1);

                Assert.Fail("При попытке удаления строки из проведенной накладной, должно генерироваться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно удалить позицию из накладной со статусом «Ожидание товара».", ex.Message);
            }
        }

        [TestMethod]
        public void ChangeOwnerWaybill_DeleteRow_Row_Not_Containce_In_Waybill_Fail()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
                waybill.AddRow(rowA1_1);
                waybill.AddRow(rowB);

                waybill.DeleteRow(rowC);

                Assert.Fail("При попытке удаления не существующей строки, должно генерироваться исключение.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной не найдена. Возможно, она была удалена.", ex.Message);
            }
        }

        private ChangeOwnerWaybill InitChangeOwnerWaybill()
        {
            var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);
            waybill.AddRow(rowA1_1);
            waybill.AddRow(rowB);

            return waybill;
        }

        /// <summary>
        /// Подготовка к проводке должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ChangeOwnerWaybill_PrepareToAccept_Must_Set_ReadyToAcceptState()
        {
            var waybill = InitChangeOwnerWaybill();

            waybill.PrepareToAccept();

            Assert.AreEqual(ChangeOwnerWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Подготовка к проводке накладной, не имеющей позиций, должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ChangeOwnerWaybill_PrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = new ChangeOwnerWaybill("42", DateTime.Now, storageA, senderOrganizationA, receiverOrganizationC, null, user, user, DateTime.Now);

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
        public void ChangeOwnerWaybill_RePrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitChangeOwnerWaybill();

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
        public void ChangeOwnerWaybill_CancelReadinessToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitChangeOwnerWaybill();

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
        public void ChangeOwnerWaybill_CancelReadinessToAccept_Must_Be_Ok()
        {
            var waybill = InitChangeOwnerWaybill();

            waybill.PrepareToAccept();

            waybill.CancelReadinessToAccept();

            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при запрещенной опции использования статуса «Готово к проводке» должна пройти успешно
        ///</summary>
        [TestMethod]
        public void ChangeOwnerWaybill_Must_Be_Accepted_Ok()
        {
            var waybill = InitChangeOwnerWaybill();

            waybill.Accept(priceLists, false, user, DateTime.Now);

            Assert.AreEqual(ChangeOwnerWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при разрешенной опции использования статуса «Готово к проводке» должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ChangeOwnerWaybill_Accept_From_Draft_If_Use_ReadyToacceptState_Denied_Must_Throw_Exception()
        {
            try
            {
                var waybill = InitChangeOwnerWaybill();

                waybill.Accept(priceLists, true, user, DateTime.Now);

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
        public void ChangeOwnerWaybill_CancelAcceptance_Must_Set_DraftState()
        {
            var waybill = InitChangeOwnerWaybill();

            waybill.Accept(priceLists, false, user, DateTime.Now);

            waybill.CancelAcceptance(false);

            Assert.AreEqual(ChangeOwnerWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки накладной при разрешенной опции использования статуса «Готово к проводке» должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ChangeOwnerWaybill_CancelAcceptance_Must_Set_ReadyToAcceptState()
        {
            var waybill = InitChangeOwnerWaybill();

            waybill.Accept(priceLists, false, user, DateTime.Now);

            waybill.CancelAcceptance(true);

            Assert.AreEqual(ChangeOwnerWaybillState.ReadyToAccept, waybill.State);
        }
        #endregion
    }
}
