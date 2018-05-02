using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test.Waybill
{
    [TestClass]
    public class ExpenditureWaybillTest
    {
        #region Поля

        private DateTime currentDate;
        private Storage storage;
        private Mock<Deal> deal;
        private DealQuota quota;
        private Mock<User> user;
        private Mock<User> createdBy;
        private DateTime creationDate;
        private Mock<Team> team;
        private ValueAddedTax valueAddedTax;
        private Mock<ClientContract> contract;
        private Mock<AccountOrganization> accountOrganization;
        private Article art1, art2, art3;
        private ArticleGroup ag;
        private IList<ArticleAccountingPrice> prices;
        private ReceiptWaybillRow receiptWaybillRow;

        private ExpenditureWaybill waybill;
        private ExpenditureWaybillRow waybillRow;

        #endregion

        #region Инициализация
        
        /// <summary>
        /// Параметры конструктора должны быть установлены
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            creationDate = DateTime.Now;
            currentDate = DateTime.Now;
            storage = new Storage("qwe", StorageType.ExtraStorage);
            deal = new Mock<Deal>();
            quota = new DealQuota("asd", 10, 45, 15000);
            user = new Mock<User>();
            createdBy = new Mock<User>();
            team = new Mock<Team>();
            contract = new Mock<ClientContract>();
            accountOrganization = new Mock<AccountOrganization>();
            valueAddedTax = new ValueAddedTax("18%", 18);
            ag = new ArticleGroup("Группа товаров", "Группа товаров");
            art1 = new Article("Товар 1", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            art2 = new Article("Товар 2", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            art3 = new Article("Товар 3", ag, new MeasureUnit("шт.", "штуки", "123", 1), false);
            prices = new List<ArticleAccountingPrice>();
            prices.Add(new ArticleAccountingPrice(art1, 10M));
            prices.Add(new ArticleAccountingPrice(art2, 13M));
            prices.Add(new ArticleAccountingPrice(art3, 15M));
            receiptWaybillRow = new ReceiptWaybillRow(art1, 150, valueAddedTax, 75);

            user.Setup(x => x.Id).Returns(43);
            createdBy.Setup(x => x.Id).Returns(1);
            team.Setup(x => x.Id).Returns(1);
            deal.Setup(x => x.IsActive).Returns(true);
            deal.Setup(x => x.IsClosed).Returns(false);
            deal.Setup(x => x.Quotas).Returns(new List<DealQuota> { quota });
            deal.Setup(x => x.Contract).Returns(contract.Object);
            deal.Setup(x => x.Id).Returns(2);
            accountOrganization.Setup(x => x.Storages).Returns(new List<Storage> { storage });
            contract.Setup(x => x.AccountOrganization).Returns(accountOrganization.Object);

            waybill = new ExpenditureWaybill("123", currentDate, storage, deal.Object, team.Object, quota, false, user.Object, DeliveryAddressType.CustomAddress, "qwerty", creationDate, createdBy.Object);
            waybillRow = new ExpenditureWaybillRow(receiptWaybillRow, 10, valueAddedTax);
            waybill.AddRow(waybillRow);
        }

        #endregion

        #region Методы

        [TestMethod]
        public void ExpenditureWaybill_Initial_Params_Must_Be_Set()
        {
            waybill = new ExpenditureWaybill("123", currentDate, storage, deal.Object, team.Object, quota, false, user.Object, DeliveryAddressType.CustomAddress, "qwerty", creationDate, createdBy.Object);

            Assert.AreEqual("123", waybill.Number);
            Assert.AreEqual(currentDate.SetHoursMinutesAndSeconds(0,0,0), waybill.Date);
            Assert.AreEqual(storage, waybill.SenderStorage);
            Assert.AreEqual(deal.Object.Id, waybill.Deal.Id);
            Assert.AreEqual(team.Object.Id, waybill.Team.Id);
            Assert.AreEqual(quota, waybill.Quota);
            Assert.AreEqual(false, waybill.IsPrepayment);
            Assert.AreEqual(user.Object.Id, waybill.Curator.Id);
            Assert.AreEqual(DeliveryAddressType.CustomAddress, waybill.DeliveryAddressType);
            Assert.AreEqual("qwerty", waybill.DeliveryAddress);
            Assert.AreEqual(createdBy.Object.Id, waybill.CreatedBy.Id);
            Assert.AreEqual(creationDate, waybill.CreationDate);
        }

        /// <summary>
        /// Конструктор накладной должен сгенерировать исключение, когда куратор равен null
        /// </summary>
        [TestMethod]
        public void ExpenditureWaybill_Must_Throw_Exception_If_Curator_Is_Null()
        {
            try
            {
                waybill = new ExpenditureWaybill("123", currentDate, storage, deal.Object, team.Object, quota, false, null, DeliveryAddressType.CustomAddress, "qwerty", creationDate, createdBy.Object);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указан куратор.", ex.Message);
            }
        }

        /// <summary>
        /// Подготовка к проводке должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_PrepareToAccept_Must_Set_ReadyToAcceptState()
        {
            waybill.PrepareToAccept();

            Assert.AreEqual(ExpenditureWaybillState.ReadyToAccept, waybill.State);
        }

        /// <summary>
        /// Подготовка к проводке накладной, не имеющей позиций, должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_PrepareToAccept_Must_Throw_Exception()
        {
            try
            {
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
        public void ExpenditureWaybill_RePrepareToAccept_Must_Throw_Exception()
        {
            try
            {
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
        public void ExpenditureWaybill_CancelReadinessToAccept_Must_Throw_Exception()
        {
            try
            {
                waybill.CancelReadinessToAccept();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Невозможно отменить готовность к проводке для накладной со статусом «{0}».", waybill.State.GetDisplayName()), ex.Message);
            }
        }

        /// <summary>
        /// Отменить готовность к проводке накладной должна пройти успешно
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_CancelReadinessToAccept_Must_Be_Ok()
        {
            waybill.PrepareToAccept();

            waybill.CancelReadinessToAccept();

            Assert.AreEqual(ExpenditureWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при запрещенной опции использования статуса «Готово к проводке» должна пройти успешно
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_Must_Be_Accepted_Ok()
        {
            waybill.Accept(prices, false, user.Object, DateTime.Now);

            Assert.AreEqual(ExpenditureWaybillState.ArticlePending, waybill.State);
        }

        /// <summary>
        /// Проводка накладной из статуса «Черновик» при разрешенной опции использования статуса «Готово к проводке» должна сгенерировать исключение
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_Accept_From_Draft_If_Use_ReadyToacceptState_Denied_Must_Throw_Exception()
        {
            try
            {
                waybill.Accept(prices, true, user.Object, DateTime.Now);

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
        public void ExpenditureWaybill_CancelAcceptance_Must_Set_DraftState()
        {
            waybill.Accept(prices, false, user.Object, DateTime.Now);

            waybill.CancelAcceptance(false);

            Assert.AreEqual(ExpenditureWaybillState.Draft, waybill.State);
        }

        /// <summary>
        /// Отмена проводки накладной при разрешенной опции использования статуса «Готово к проводке» должна выставить статус «Готово к проводке»
        ///</summary>
        [TestMethod]
        public void ExpenditureWaybill_CancelAcceptance_Must_Set_ReadyToAcceptState()
        {
            waybill.Accept(prices, false, user.Object, DateTime.Now);

            waybill.CancelAcceptance(true);

            Assert.AreEqual(ExpenditureWaybillState.ReadyToAccept, waybill.State);
        }

        #endregion
    }
}
