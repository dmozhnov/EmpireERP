using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ReturnFromClientWaybillTest
    {
        ReturnFromClientWaybill returnFromClientWaybill;
        AccountOrganization accountOrganization, accountOrganizationAnother;
        ClientOrganization clientOrganization;
        EconomicAgent economicAgent;
        Client client;
        Deal deal;
        Storage storage, storage2;
        ReturnFromClientReason returnFromClientReason;
        User curator;
        Employee employee1;
        IList<ArticleAccountingPrice> Prices;
        ValueAddedTax valueAddedTax;
        Article art1, art2, art3;
        ArticleGroup ag;
        private Team team;

        ReceiptWaybillRow recRow1, recRow1_1, recRow2, recRow3;
        ReturnFromClientWaybillRow row1, row2;
        ExpenditureWaybillRow saleRow1, saleRow1_1, saleRow2, saleRow3, saleRowAnother1, saleRowAnother2, saleRowAnother3;
        ExpenditureWaybill sale, saleAnother;
        
        Deal deal1,deal2;
        DealQuota quota1, quota2;

        [TestInitialize]
        public void Init()
        {
            employee1 = new Employee("Работник 1 имя", "Работник 1 фамилия", "Работник 1 отчество", new EmployeePost("Работник 1 пост"), null);
            curator = new User(employee1, "Куратор", "log", "pas", new Team("Тестовая команда", null), null);
            team = new Team("123", curator);
            economicAgent = new PhysicalPerson(new LegalForm("Легал форм", EconomicAgentType.PhysicalPerson));
            accountOrganization = new AccountOrganization("Орг1 кор имя", "орг1 длин имя", economicAgent) { Id = 1 };
            accountOrganizationAnother = new AccountOrganization("Орг2 кор имя", "орг2 длин имя", economicAgent) { Id = 2 };
            clientOrganization = new ClientOrganization("client org", "cllll", economicAgent) { Id = 3 };
            client = new Client("клиент1", new ClientType("основной тип клиента"), ClientLoyalty.Follower, new ClientServiceProgram("программа 1"), new ClientRegion("Регион 1"), 10);
            deal = new Deal("Тестовая сделка", curator);
            client.AddDeal(deal);
            storage = new Storage("Склад 1", StorageType.ExtraStorage);
            storage.AddAccountOrganization(accountOrganization);
            storage.AddAccountOrganization(accountOrganizationAnother);
            storage2 = new Storage("Склад 2", StorageType.ExtraStorage);
            returnFromClientReason = new ReturnFromClientReason("Брак");
            returnFromClientWaybill = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, storage, returnFromClientReason, curator, curator, DateTime.Now);
            valueAddedTax = new ValueAddedTax("18%", 18);
            ag = new ArticleGroup("Группа товаров", "Группа товаров");
            art1 = new Article("Товар 1", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            art2 = new Article("Товар 2", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            art3 = new Article("Товар 3", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            Prices = new List<ArticleAccountingPrice>();
            Prices.Add(new ArticleAccountingPrice(art1, 10M));
            Prices.Add(new ArticleAccountingPrice(art2, 13M));
            Prices.Add(new ArticleAccountingPrice(art3, 15M));

            deal1 = new Deal("Deal1", curator);
            deal2 = new Deal("Deal2", curator);
            quota1 = new DealQuota("qq", 1);
            quota2 = new DealQuota("qq", 2);

            deal1.AddQuota(quota1);
            deal2.AddQuota(quota2);
            deal1.Contract = new ClientContract(accountOrganization, clientOrganization, "Договор", "1", DateTime.Now, DateTime.Now);
            deal2.Contract = new ClientContract(accountOrganizationAnother, clientOrganization, "kk", "22", DateTime.Today, DateTime.Now);

            recRow1 = new ReceiptWaybillRow(art1, 5, 50, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            recRow1_1 = new ReceiptWaybillRow(art1, 2, 30, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            recRow2 = new ReceiptWaybillRow(art2, 7, 35, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            recRow3 = new ReceiptWaybillRow(art3, 9, 90, new ValueAddedTax("18%", 18)) { Id = Guid.NewGuid() };
            recRow1.PurchaseCost = 50 / 5;
            recRow1.PurchaseCost = 35 / 7;
            recRow1.PurchaseCost = 90 / 9;

            saleRow1 = new ExpenditureWaybillRow(recRow1, 3, valueAddedTax) { Id = Guid.NewGuid() };
            saleRow1_1 = new ExpenditureWaybillRow(recRow1_1, 1, valueAddedTax) { Id = Guid.NewGuid() };
            saleRow2 = new ExpenditureWaybillRow(recRow2, 4, valueAddedTax) { Id = Guid.NewGuid() };
            saleRow3 = new ExpenditureWaybillRow(recRow3, 5, valueAddedTax) { Id = Guid.NewGuid() };

            saleRowAnother1 = new ExpenditureWaybillRow(recRow1, 3, valueAddedTax) { Id = Guid.NewGuid() };
            saleRowAnother2 = new ExpenditureWaybillRow(recRow2, 4, valueAddedTax) { Id = Guid.NewGuid() };
            saleRowAnother3 = new ExpenditureWaybillRow(recRow3, 5, valueAddedTax) { Id = Guid.NewGuid() };

            sale = new ExpenditureWaybill("1", DateTime.Today, storage, deal1, team, quota1, true, curator, DeliveryAddressType.ClientAddress, "", DateTime.Now, curator) { Id = Guid.NewGuid() };
            saleAnother = new ExpenditureWaybill("1", DateTime.Today, storage, deal2, team, quota2, true, curator, DeliveryAddressType.ClientAddress, "", DateTime.Now, curator) { Id = Guid.NewGuid() };

            sale.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRow1);
            sale.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRow2);
            sale.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRow3);
            saleAnother.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRowAnother1);
            saleAnother.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRowAnother2);
            saleAnother.As<ExpenditureWaybill>().AddRow((ExpenditureWaybillRow)saleRowAnother3);

            sale.As<ExpenditureWaybill>().Accept(Prices, false, curator, DateTime.Now);

            saleAnother.As<ExpenditureWaybill>().Accept(Prices, false, curator, DateTime.Now);
        }

        #region Инициализация

        [TestMethod]
        public void ReturnFromClientWaybill_Initial_Parameters_Must_Be_Set()
        {
            Assert.AreEqual(DateTime.Today.Date, returnFromClientWaybill.CreationDate.Date);
            Assert.AreEqual(DateTime.Today.Date, returnFromClientWaybill.Date.Date);
            Assert.IsNull(returnFromClientWaybill.AcceptanceDate);
            Assert.IsNull(returnFromClientWaybill.ReceiptDate);
            Assert.IsNull(returnFromClientWaybill.DeletionDate);

            Assert.AreEqual(String.Empty, returnFromClientWaybill.Comment);
            Assert.AreEqual(Guid.Empty, returnFromClientWaybill.Id);
            Assert.AreEqual("142", returnFromClientWaybill.Number);
            Assert.AreEqual(returnFromClientWaybill.Name, "№ 142 от " + DateTime.Today.Date.ToShortDateString());
            Assert.AreEqual(returnFromClientWaybill.State, ReturnFromClientWaybillState.Draft);

            Assert.AreEqual(returnFromClientWaybill.IsAccepted, false);
            Assert.AreEqual(returnFromClientWaybill.IsReceipted, false);

            Assert.AreEqual(0, returnFromClientWaybill.PurchaseCostSum);
            Assert.AreEqual(returnFromClientWaybill.SalePriceSum, 0);            

            Assert.AreEqual(0, returnFromClientWaybill.RowCount);
            Assert.IsNotNull(returnFromClientWaybill.Rows);

            Assert.AreEqual(returnFromClientWaybill.Deal, deal);
            Assert.AreEqual(returnFromClientWaybill.Deal.Id, deal.Id);

            Assert.AreEqual(returnFromClientWaybill.Curator, curator);
            Assert.AreEqual(returnFromClientWaybill.Curator.Id, curator.Id);

            Assert.AreEqual(returnFromClientWaybill.Recipient, accountOrganization);
            Assert.AreEqual(returnFromClientWaybill.Recipient.Id, accountOrganization.Id);

            Assert.AreEqual(returnFromClientWaybill.RecipientStorage, storage);
            Assert.AreEqual(returnFromClientWaybill.RecipientStorage.Id, storage.Id);

            Assert.AreEqual(returnFromClientWaybill.ReturnFromClientReason, returnFromClientReason);
            Assert.AreEqual(returnFromClientWaybill.ReturnFromClientReason.Id, returnFromClientReason.Id);

            Assert.AreEqual(curator, returnFromClientWaybill.Curator);
        }

        #endregion

        #region Свойства

        [TestMethod]
        public void ReturnFromClientWaybill_DeletionDate_Must_Be_Set_Correctly()
        {
            returnFromClientWaybill.DeletionDate = DateTime.Today.Date;
            Assert.IsTrue(returnFromClientWaybill.DeletionDate.HasValue);
            Assert.AreEqual(DateTime.Today.Date, returnFromClientWaybill.DeletionDate.Value);
        }

        [TestMethod]
        public void ReturnFromClientWaybill_DeletionDate_Must_Not_Be_Set_Again()
        {
            var date1 = DateTime.Today.Date;
            var date2 = DateTime.Today.AddDays(1).Date;
            returnFromClientWaybill.DeletionDate = date1;
            returnFromClientWaybill.DeletionDate = date2;
            Assert.AreEqual(returnFromClientWaybill.DeletionDate.Value, date1);
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_RecipientAccountingPriceSum_Try_Set_AfterAccept()
        {
            try
            {
                var waybill = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, storage, returnFromClientReason, curator, curator, DateTime.Now);
                var row = new ReturnFromClientWaybillRow(saleRow1, 1);
                waybill.AddRow(row);

                waybill.RecipientAccountingPriceSum = 100.5M;
                waybill.Accept(Prices, false, curator, DateTime.Now);
                waybill.RecipientAccountingPriceSum = 101.5M;

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно установить значение суммы в учетных ценах, т.к. накладная уже проведена.", ex.Message);
            }

        }

        [TestMethod]
        public void ReturnFromClientWaybill_Attempt_To_Read_RecipientAccountingPriceSum_Before_Acceptance_Must_Throw_Exception()
        {
            try
            {
                var rfcwb = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, storage, returnFromClientReason, curator, curator, DateTime.Now);
                rfcwb.RecipientAccountingPriceSum = 100.5M;

                // пытаемся получить сумму в учетных ценах для непроведенной накладной
                var sum = rfcwb.RecipientAccountingPriceSum;

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно получить значение суммы товаров в учетных ценах для непроведенной накладной.", ex.Message);
            }            
        }

        #endregion

        #region Конструкторы

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_AccountOrganization_Is_Null()
        {
            try
            {
                var rfcwb = new ReturnFromClientWaybill("142", DateTime.Today, null, deal, team, storage, returnFromClientReason, curator, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана организация-приемщик.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_Deal_Is_Null()
        {
            try
            {
                var rfcwb = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, null, team, storage, returnFromClientReason, curator, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана сделка.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_Storage_Is_Null()
        {
            try
            {
                var rfcwb = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, null, returnFromClientReason, curator, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указано место хранения-приемщик.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_ReturnFromClientReason_Is_Null()
        {
            try
            {
                var rfcwb = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, storage, null, curator, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана причина возврата.", ex.Message);
            }
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                returnFromClientWaybill = new ReturnFromClientWaybill("142", DateTime.Today, accountOrganization, deal, team, storage, returnFromClientReason, null, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }

        #endregion

        #region Методы

        #region Строки

        [TestMethod]
        public void ReturnFromClientWaybillRow_Must_Throw_Exception_If_ReturnCount_Large_Then_Accessible()
        {
            try
            {
                var row1 = new ReturnFromClientWaybillRow(saleRow1, 6);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно вернуть количество товара большее, чем реализовано.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybillRow_Must_Throw_Exception_If_ReturnCount_Less_Then_Zero()
        {
            try
            {
                var row1 = new ReturnFromClientWaybillRow(saleRow1, -1);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно вернуть количество товара, меньшее нуля.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybillRow_SaleWaubillRow_Must_Equal()
        {
            row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
            Assert.AreEqual(row1.SaleWaybillRow, saleRow1);
            Assert.AreEqual(row1.SaleWaybillRow.Id, saleRow1.Id);
        }

        [TestMethod]
        public void ReturnFromClientWaybillRow_Initial_Parameters_Must_Be_Set()
        {
            var row1 = new ReturnFromClientWaybillRow(saleRow1, 3);


            Assert.AreEqual(row1.ReturnCount, 3);
            Assert.IsNotNull(row1.SaleWaybillRow);
            Assert.AreEqual(row1.PurchaseCost, row1.SaleWaybillRow.PurchaseCost);
            Assert.AreEqual(row1.SalePrice, row1.SaleWaybillRow.SalePrice);
        }

        [TestMethod]
        public void ReturnFromClientWaybillRow_Must_Throw_Exception_If_SaleWaybillRow_Is_Null()
        {
            try
            {
                var row1 = new ReturnFromClientWaybillRow(null, 3);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана позиция накладной реализации.", ex.Message);
            }
        }

        #endregion

        #region Добавление / удаление позиций

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_State_Is_Accepted()
        {
            try
            {
                row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
                row2 = new ReturnFromClientWaybillRow(saleRow2, 1);

                returnFromClientWaybill.AddRow(row1);
                returnFromClientWaybill.Accept(Prices, false, curator, DateTime.Now);
                returnFromClientWaybill.AddRow(row2);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно добавить позицию в накладную со статусом «Проведено».", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_Client_And_Account_Organization_Are_Difference()
        {
            try
            {
                row1 = new ReturnFromClientWaybillRow(saleRowAnother1, 1);
                returnFromClientWaybill.AddRow(row1);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Организация-приемщик и организация-продавец товара должны совпадать.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_SaleWaybillNotInit()
        {
            try
            {
                row1 = new ReturnFromClientWaybillRow(saleRow1_1, 1);
                returnFromClientWaybill.AddRow(row1);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно добавить позицию со ссылкой на позицию накладной реализации без указанной реализации.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_Batch_Already_Added()
        {
            try
            {
                row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
                row2 = new ReturnFromClientWaybillRow(saleRow1, 1);
                returnFromClientWaybill.AddRow(row1);
                returnFromClientWaybill.AddRow(row2);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Позиция накладной по данной позиции накладной реализации уже добавлена.", ex.Message);
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Must_Throw_Exception_If_DeleteRow_From_Accepted_WaybillRow()
        {
            try
            {
                row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
                returnFromClientWaybill.AddRow(row1);
                returnFromClientWaybill.Accept(Prices, false, curator, DateTime.Now);
                returnFromClientWaybill.DeleteRow(row1);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Невозможно удалить позицию из накладной со статусом "))
                    Assert.Fail("Исключение не верно.");
            }
        }

        [TestMethod]
        public void ReturnFromClientWaybill_AddRow_Must_Init()
        {
            row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
            returnFromClientWaybill.AddRow(row1);
            Assert.AreEqual(row1.ReturnFromClientWaybill.Id, returnFromClientWaybill.Id);
            Assert.AreEqual(row1.ReturnFromClientWaybill, returnFromClientWaybill);
            Assert.AreEqual(returnFromClientWaybill.RowCount, 1);
        }

        [TestMethod]
        public void ReturnFromClientWaybill_Delete_Row_Must_Work()
        {
            row1 = new ReturnFromClientWaybillRow(saleRow1, 1);
            returnFromClientWaybill.AddRow(row1);
            Assert.AreEqual(returnFromClientWaybill.RowCount, 1);
            returnFromClientWaybill.DeleteRow(row1);
            Assert.AreEqual(returnFromClientWaybill.RowCount, 0);
        }

        #endregion

        private ReturnFromClientWaybill GetWaybill()
        {
            row1 = new ReturnFromClientWaybillRow(saleRow1, 1);

            returnFromClientWaybill.AddRow(row1);

            return returnFromClientWaybill;
        }

        /// <summary>
        /// Подготовка к проводке должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_PrepareToAccept_Must_Set_ReadyToAcceptState()
        {
            var waybill = GetWaybill();

            waybill.PrepareToAccept();

            Assert.AreEqual(ReturnFromClientWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Подготовка к проводке накладной, не имеющей позиций, должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_PrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = GetWaybill();

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
        public void ReturnFromClientWaybill_RePrepareToAccept_Must_Throw_Exception()
        {
            try
            {
                var waybill = GetWaybill();

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
        public void ReturnFromClientWaybill_CancelReadinessToAccept_Must_Throw_Exception()
        {
            var waybill = GetWaybill();

            try
            {
                waybill.CancelReadinessToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке накладной должна пройти успешно
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_CancelReadinessToAccept_Must_Be_Ok()
        {
            var waybill = GetWaybill();

            waybill.PrepareToAccept();

            waybill.CancelReadinessToAccept();

            Assert.AreEqual(ReturnFromClientWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при запрещенной опции использования статуса «Готово к проводке» должна пройти успешно
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_Must_Be_Accepted_Ok()
        {
            var waybill = GetWaybill();

            waybill.Accept(Prices, false, curator, DateTime.Now);

            Assert.AreEqual(ReturnFromClientWaybillState.Accepted, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при разрешенной опции использования статуса «Готово к проводке» должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_Accept_From_Draft_If_Use_ReadyToacceptState_Denied_Must_Throw_Exception()
        {
            try
            {
                var waybill = GetWaybill();

                waybill.Accept(Prices, true, curator, DateTime.Now);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно провести накладную со статусом «Черновик».", ex.Message);
            }
        }

        /// <summary>
        /// Отмена проводки накладной при запрещенной опции использования статуса «Готово к проводке» должна выставить статус «Черновик»
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_CancelAcceptance_Must_Set_DraftState()
        {
            var waybill = GetWaybill();

            waybill.Accept(Prices, false, curator, DateTime.Now);

            waybill.CancelAcceptance(false);

            Assert.AreEqual(ReturnFromClientWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки накладной при разрешенной опции использования статуса «Готово к проводке» должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ReturnFromClientWaybill_CancelAcceptance_Must_Set_ReadyToAcceptState()
        {
            var waybill = GetWaybill();

            waybill.Accept(Prices, false, curator, DateTime.Now);

            waybill.CancelAcceptance(true);

            Assert.AreEqual(ReturnFromClientWaybillState.ReadyToAccept, waybill.State);
        }

        #endregion
    }
}