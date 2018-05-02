using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class WriteoffWaybillTest
    {
        WriteoffWaybill writeoffWaybill;
        Storage storage;
        WriteoffReason writeoffReason;
        ReceiptWaybill receiptWaybill;
        ReceiptWaybillRow receiptWaybillRow;
        AccountOrganization accountOrganization;
        List<ArticleAccountingPrice> priceLists;
        Article article;
        ProviderContract providerContract;
        User user;

        [TestInitialize]
        public void Init()
        {
            storage = new Storage("Тестовое место хранения", StorageType.DistributionCenter) { Id = 1 };
            writeoffReason = new WriteoffReason("Тестовая причина списания") { Id = 2 };

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson) { Id = 3 };
            var juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 4 };
            var juridicalPerson2 = new JuridicalPerson(juridicalLegalForm) {Id = 15 };
            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 5 };

            var provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5) { Id = 6 };
            var providerOrganization = new ProviderOrganization("Организация поставщика", "Организация поставщика", juridicalPerson2);
            var articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа");
            var measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            var customDeclarationNumber = new String('0',25);
            
            article = new Article("Тестовый товар А", articleGroup, measureUnit, true);
            providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "4645", DateTime.Now, DateTime.Now);
            provider.AddProviderContract(providerContract);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);
            receiptWaybill = new ReceiptWaybill("123АБВ", DateTime.Today.AddDays(1), storage, accountOrganization, provider, 1234.5M, 0M, new ValueAddedTax("18%", 18), providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybillRow = new ReceiptWaybillRow(article, 100, 1234.5M, receiptWaybill.PendingValueAddedTax);
            receiptWaybill.AddRow(receiptWaybillRow);

            writeoffWaybill = new WriteoffWaybill("123", DateTime.Today, storage, accountOrganization, writeoffReason, user, user, DateTime.Now);

            priceLists = new List<ArticleAccountingPrice>() { new ArticleAccountingPrice(article, 10M) };
        }

        [TestMethod]
        public void WriteoffWaybill_Initial_Parameters_Must_Be_Set()
        {
            Assert.AreEqual(DateTime.Today.Date, writeoffWaybill.CreationDate.Date);
            Assert.AreEqual(DateTime.Today.Date, writeoffWaybill.Date.Date);
            Assert.AreEqual(String.Empty, writeoffWaybill.Comment);
            Assert.AreEqual(Guid.Empty, writeoffWaybill.Id);
            Assert.AreEqual("123", writeoffWaybill.Number);
            Assert.AreEqual(0, writeoffWaybill.PurchaseCostSum);
            //Assert.AreEqual(0, writeoffWaybill.ReceivelessProfitPercent);
            Assert.AreEqual(0, writeoffWaybill.ReceivelessProfitSum);
            Assert.AreEqual(0, writeoffWaybill.RowCount);
            Assert.IsNotNull(writeoffWaybill.Rows);
            Assert.AreEqual(0, writeoffWaybill.SenderAccountingPriceSum);
            Assert.AreEqual(storage.Id, writeoffWaybill.SenderStorage.Id);
            Assert.AreEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
            Assert.AreEqual(writeoffReason.Id, writeoffWaybill.WriteoffReason.Id);
            Assert.AreEqual(user, writeoffWaybill.Curator);
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                writeoffWaybill = new WriteoffWaybill("123", DateTime.Today, storage, accountOrganization, writeoffReason, null, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Must_Throw_Exception_If_SenderStorage_Is_Null()
        {
            try
            {
                var wb = new WriteoffWaybill("123", DateTime.Today, null, accountOrganization, writeoffReason, user, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указано место хранения отправителя.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Must_Throw_Exception_If_WriteoffReason_Is_Null()
        {
            try
            {
                var wb = new WriteoffWaybill("123", DateTime.Today, storage, accountOrganization, null, user, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указано основание для списания.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Addition_Row_From_Pending_Must_Be_Ok()
        {
            //var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            //writeoffWaybill.AddRow(writeoffWaybillRow);

            //Assert.AreEqual(1, writeoffWaybill.RowCount);
            //Assert.AreEqual(writeoffWaybill.Id, writeoffWaybillRow.WriteoffWaybill.Id);
            //Assert.AreEqual(Math.Round(receiptWaybillRow.PurchaseCost * writeoffWaybillRow.WritingoffCount, 2), writeoffWaybill.PurchaseCostSum);
            //Assert.AreEqual(WriteoffWaybillState.ArticlePending, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_Addition_Row_From_Accepted_IncomingWaybill_Must_Be_Ok()
        {
            //receiptWaybill.Accept(priceLists);
            //receiptWaybillRow.ReceiptedCount = receiptWaybillRow.PendingCount;
            //receiptWaybillRow.ProviderCount = receiptWaybillRow.PendingCount;
            //receiptWaybillRow.ProviderSum = receiptWaybillRow.PendingSum;
            //receiptWaybill.Receipt(1234.5M);
            //var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

            //writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;
            //writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);

            //Assert.AreEqual(WriteoffWaybillState.ReadyToWriteoff, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_Repeat_By_Batch_And_Article_Row_Addition_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

                writeoffWaybill.AddRow(writeoffWaybillRow);
                writeoffWaybill.AddRow(writeoffWaybillRow);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной по данной партии и товару уже добавлена.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Last_Row_Deletion_Must_Be_Ok()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            writeoffWaybill.AddRow(writeoffWaybillRow);
            writeoffWaybill.DeleteRow(writeoffWaybillRow);

            Assert.AreEqual(0, writeoffWaybill.RowCount);
            Assert.AreEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_NotExisting_Row_Deletion_Must_Throw_Exception()
        {
            try
            {
                writeoffWaybill.DeleteRow(new WriteoffWaybillRow(receiptWaybillRow, 50));

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной не найдена. Возможно, она была удалена.", ex.Message);
            }
        }

        /// <summary>
        /// Подготовка накладной к проводке должна выставить статус «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_PrepareToAccept_Must_Set_ReadyToAcceptState()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.PrepareToAccept();

            Assert.AreEqual(WriteoffWaybillState.ReadyToAccept, writeoffWaybill.State);
        }

        /// <summary>
        /// Повторная подготовка к проводке накладной должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void WriteoffWaybill_RePrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);
                writeoffWaybill.AddRow(writeoffWaybillRow);

                writeoffWaybill.PrepareToAccept();

                writeoffWaybill.PrepareToAccept();
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
        public void WriteoffWaybill_CancelReadinessToAccept_Must_Throw_Exception()
        {
            try
            {
                writeoffWaybill.CancelReadinessToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно отменить готовность к проводке для накладной со статусом «Черновик».", ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке накладной должна пройти успешно
        ///</summary>
        [TestMethod]
        public void WriteoffWaybill_CancelReadinessToAccept_Must_Be_Ok()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);
            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.PrepareToAccept();

            writeoffWaybill.CancelReadinessToAccept();

            Assert.AreEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
        }

        /// <summary>
        /// Отмена готовонсти накладной к проводке должна выставить статус «Черновик»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_CancelReadinessToAccept_Must_Set_DraftState()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);
            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.PrepareToAccept();  

            writeoffWaybill.CancelReadinessToAccept();

            Assert.AreEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
        }

        /// <summary>
        /// Попытка провести накладную из статуса «Черновик» при запрещенной опции использования статуса «Готово к проводке» должно сгенерировать исключение
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Accept_From_Draft_If_Use_ReadyToAcceptState_Is_Denied_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

                writeoffWaybill.AddRow(writeoffWaybillRow);

                writeoffWaybill.Accept(priceLists, true, user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно провести накладную из состояния «{0}».", writeoffWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Попытка провести накладную из статуса «Черновик» при разрешенной опции использования статуса «Готово к проводке» должна быть успешной
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Accept_From_Draft_If_Use_ReadyToAcceptState_Is_Access_Must_Be_Ok()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);

            Assert.AreNotEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
        }

        /// <summary>
        /// Попытка отменить проводку накладной при запрещенной опции использования статуса «Готово к проводке» должно установить статус «Черновик»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_CancelAcceptance_If_Use_ReadyToAcceptState_Is_Denied_Must_Set_DraftState()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);

            writeoffWaybill.CancelAcceptance(false);

            Assert.AreEqual(WriteoffWaybillState.Draft, writeoffWaybill.State);
        }

        /// <summary>
        /// Попытка отменить проводку накладной при разрешенной опции использования статуса «Готово к проводке» должно установить статус «Готово к проводке»
        /// </summary>
        [TestMethod]
        public void WriteoffWaybill_Attempt_To_CancelAcceptance_If_Use_ReadyToAcceptState_Is_Denied_Must_Set_ReadyToAcceptState()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

            writeoffWaybill.AddRow(writeoffWaybillRow);

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);

            writeoffWaybill.CancelAcceptance(true);

            Assert.AreEqual(WriteoffWaybillState.ReadyToAccept, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Writeoff_If_Not_ReadyToWriteoff_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, 50);

                writeoffWaybill.AddRow(writeoffWaybillRow);

                writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
                writeoffWaybill.Writeoff(user, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно списать товары по накладной со статусом «{0}».", writeoffWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Cancel_Writeoff_If_Not_Writtenoff_Must_Throw_Exception()
        {
            try
            {
                writeoffWaybill.CancelWriteoff();

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить списание товаров по накладной со статусом «{0}».", writeoffWaybill.State.GetDisplayName()), ex.Message);
            }
        }


        [TestMethod]
        public void WriteoffWaybillRow_If_ReceiptWaybillRow_Is_Null_On_Creation_Must_Throw()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(null, 50);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана партия товара.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybillRow_If_WritingoffCount_Is_Null_On_Creation_Must_Throw()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow(receiptWaybillRow, -50);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество списываемого товара должно быть положительным числом.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Writeoff_From_ReadyToWriteoff_Must_Be_Ok()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

            // переводим накладную в состояние "Готово к списанию"
            writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
            writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
            writeoffWaybill.Writeoff(user, DateTime.Now);

            Assert.AreEqual(WriteoffWaybillState.Writtenoff, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_Cancelling_Writeoff_From_Writtenoff_Must_Be_Ok()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

            // переводим накладную в состояние "Готово к списанию"
            writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
            writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
            writeoffWaybill.Writeoff(user, DateTime.Now);
            writeoffWaybill.CancelWriteoff();

            Assert.AreEqual(WriteoffWaybillState.ReadyToWriteoff, writeoffWaybill.State);
        }

        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Add_Row_To_Writtenoff_Waybill_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

                // переводим накладную в состояние "Готово к списанию"
                writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
                writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

                writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
                writeoffWaybill.Writeoff(user, DateTime.Now);

                writeoffWaybill.AddRow(new WriteoffWaybillRow(receiptWaybillRow, 45));

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно добавить позицию в накладную со статусом «{0}».", writeoffWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Attempt_To_Delete_Row_From_Writtenoff_Waybill_Must_Throw_Exception()
        {
            try
            {
                var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

                // переводим накладную в состояние "Готово к списанию"
                writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
                writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

                writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
                writeoffWaybill.Writeoff(user, DateTime.Now);

                writeoffWaybill.DeleteRow((WriteoffWaybillRow)writeoffWaybillRow.Target);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно удалить позицию из накладной со статусом «{0}».", writeoffWaybill.State.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod]
        public void WriteoffWaybill_Writeoff_Must_Set_Fixed_AccountingPriceList()
        {
            var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

            // переводим накладную в состояние "Готово к списанию"
            writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
            writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            priceLists = new List<ArticleAccountingPrice> {
                    new ArticleAccountingPrice(writeoffWaybillRow.Article, 50), new ArticleAccountingPrice(article, 25) };

            writeoffWaybill.Accept(priceLists, false, user, DateTime.Now);
            writeoffWaybill.Writeoff(user, DateTime.Now);

            Assert.AreEqual(priceLists.ElementAt(0), writeoffWaybillRow.SenderArticleAccountingPrice);
        }

        [TestMethod]
        public void WriteoffWaybill_WriteoffCancellation_Must_Set_Fixed_AccountingPriceList_To_Null()
        {
            //var writeoffWaybillRow = new WriteoffWaybillRow_Accessor(receiptWaybillRow, 50);

            //// переводим накладную в состояние "Готово к списанию"
            //writeoffWaybill.AddRow((WriteoffWaybillRow)writeoffWaybillRow.Target);
            //writeoffWaybillRow.OutgoingWaybillRowState = OutgoingWaybillRowState.ReadyToArticleMovement;

            //priceLists = new List<ArticleAccountingPrice> {
            //        new ArticleAccountingPrice(writeoffWaybillRow.Article, 50), new ArticleAccountingPrice(article, 25) };

            //writeoffWaybill.Accept(priceLists);
            //writeoffWaybill.Writeoff();

            //writeoffWaybill.CancelWriteoff();

            //Assert.IsNull(writeoffWaybillRow.SenderArticleAccountingPrice);
        }

    }
}
