using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class DealPaymentDocumentTest
    {
        #region Инициализация и конструкторы

        private Deal deal;

        private Mock<User> user;
        private Mock<Team> team;

        [TestInitialize]
        public void Init()
        {
            user = new Mock<User>();
            user.Object.Id = 1;

            team = new Mock<Team>();
            team.Object.Id = 3;

            deal = new Deal("Тестовая сделка", user.Object) { Id = 2 };
        }

        #endregion

        #region DealPaymentDocumentCreation

        /// <summary>
        /// Создается оплата от клиента по сделке с параметрами:
        /// Сумма - 500 руб., номер платежного документа - "001", дата - 18.02.2012, форма оплаты - наличными денежными средствами
        /// 
        /// Все параметры должны быть установлены
        /// Коллекция разнесений сразу после создания должна быть пустой
        /// Ссылка на сделку должна быть пустой
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_DealPaymentFromClient_InitialParameters_Must_Be_Set()
        {
            // Act (создаем оплату от клиента)
            var currentDate = DateTime.Now;
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user.Object, "001", new DateTime(2012, 2, 18), 500M, DealPaymentForm.Cash, currentDate);

            Assert.AreEqual("001", dealPaymentFromClient.PaymentDocumentNumber);
            Assert.AreEqual(currentDate, dealPaymentFromClient.CreationDate);
            Assert.AreEqual(new DateTime(2012, 2, 18), dealPaymentFromClient.Date);
            Assert.IsNull(dealPaymentFromClient.Deal);
            Assert.IsNull(dealPaymentFromClient.DeletionDate);
            Assert.IsNotNull(dealPaymentFromClient.Distributions);
            Assert.AreEqual(0, dealPaymentFromClient.Distributions.Count());
            Assert.AreEqual(DealPaymentForm.Cash, dealPaymentFromClient.DealPaymentForm);
            Assert.AreEqual(500M, dealPaymentFromClient.Sum);
            Assert.AreEqual(user.Object, dealPaymentFromClient.User);
            Assert.AreEqual(team.Object, dealPaymentFromClient.Team);
        }

        /// <summary>
        /// Создается возврат оплаты клиенту по сделке с параметрами:
        /// Сумма - 510 руб., номер платежного документа - "0003", дата - 16.02.2012, форма оплаты - по безналичному расчету
        /// 
        /// Все параметры должны быть установлены
        /// Коллекция разнесений сразу после создания должна быть пустой
        /// Ссылка на сделку должна быть пустой
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_DealPaymentToClient_InitialParameters_Must_Be_Set()
        {
            // Act (создаем возврат оплаты клиенту)
            var currentDate = DateTime.Now;
            var dealPaymentToClient = new DealPaymentToClient(team.Object, user.Object, "0003", new DateTime(2012, 2, 16), 510M, DealPaymentForm.Cashless, currentDate);

            Assert.AreEqual("0003", dealPaymentToClient.PaymentDocumentNumber);
            Assert.AreEqual(currentDate, dealPaymentToClient.CreationDate);
            Assert.AreEqual(new DateTime(2012, 2, 16), dealPaymentToClient.Date);
            Assert.IsNull(dealPaymentToClient.Deal);
            Assert.IsNull(dealPaymentToClient.DeletionDate);
            Assert.IsNotNull(dealPaymentToClient.Distributions);
            Assert.AreEqual(0, dealPaymentToClient.Distributions.Count());
            Assert.AreEqual(DealPaymentForm.Cashless, dealPaymentToClient.DealPaymentForm);
            Assert.AreEqual(510M, dealPaymentToClient.Sum);
            Assert.AreEqual(user.Object, dealPaymentToClient.User);
            Assert.AreEqual(team.Object, dealPaymentToClient.Team);
        }

        /// <summary>
        /// Создается кредитовая корректировка сальдо по сделке с параметрами:
        /// Сумма - 520 руб., причина корректировки - "ВОЗВРАТ", дата - 16.02.2012, 
        /// 
        /// Все параметры должны быть установлены
        /// Коллекция разнесений сразу после создания должна быть пустой
        /// Ссылка на сделку должна быть пустой
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_DealCreditInitialBalanceCorrection_InitialParameters_Must_Be_Set()
        {
            // Act (создаем кредитовую корректировку сальдо)
            var currentDate = DateTime.Now;
            var dealCreditInitialBalanceCorrection = new DealCreditInitialBalanceCorrection(team.Object, user.Object, "ВОЗВРАТ", new DateTime(2012, 2, 16), 520M, currentDate);

            Assert.AreEqual("ВОЗВРАТ", dealCreditInitialBalanceCorrection.CorrectionReason);
            Assert.AreEqual(currentDate, dealCreditInitialBalanceCorrection.CreationDate);
            Assert.AreEqual(new DateTime(2012, 2, 16), dealCreditInitialBalanceCorrection.Date);
            Assert.IsNull(dealCreditInitialBalanceCorrection.Deal);
            Assert.IsNull(dealCreditInitialBalanceCorrection.DeletionDate);
            Assert.IsNotNull(dealCreditInitialBalanceCorrection.Distributions);
            Assert.AreEqual(0, dealCreditInitialBalanceCorrection.Distributions.Count());
            Assert.AreEqual(520M, dealCreditInitialBalanceCorrection.Sum);
            Assert.AreEqual(user.Object, dealCreditInitialBalanceCorrection.User);
            Assert.AreEqual(team.Object, dealCreditInitialBalanceCorrection.Team);
        }

        /// <summary>
        /// Создается дебетовая корректировка сальдо по сделке с параметрами:
        /// Сумма - 520 руб., причина корректировки - "Первоначальный долг", дата - 16.02.2012, 
        /// 
        /// Все параметры должны быть установлены
        /// Коллекция разнесений сразу после создания должна быть пустой
        /// Ссылка на сделку должна быть пустой
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_DealDebitInitialBalanceCorrection_InitialParameters_Must_Be_Set()
        {
            // Act (создаем дебетовую корректировку сальдо)
            var currentDate = DateTime.Now;
            var dealDebitInitialBalanceCorrection = new DealDebitInitialBalanceCorrection(team.Object, user.Object, "Первоначальный долг", new DateTime(2012, 2, 16), 520M, currentDate);

            Assert.AreEqual("Первоначальный долг", dealDebitInitialBalanceCorrection.CorrectionReason);
            Assert.AreEqual(currentDate, dealDebitInitialBalanceCorrection.CreationDate);
            Assert.AreEqual(new DateTime(2012, 2, 16), dealDebitInitialBalanceCorrection.Date);
            Assert.IsNull(dealDebitInitialBalanceCorrection.Deal);
            Assert.IsNull(dealDebitInitialBalanceCorrection.DeletionDate);
            Assert.IsNotNull(dealDebitInitialBalanceCorrection.Distributions);
            Assert.AreEqual(0, dealDebitInitialBalanceCorrection.Distributions.Count());
            Assert.AreEqual(520M, dealDebitInitialBalanceCorrection.Sum);
            Assert.AreEqual(user.Object, dealDebitInitialBalanceCorrection.User);
            Assert.AreEqual(team.Object, dealDebitInitialBalanceCorrection.Team);
        }

        #endregion

        #region AddingDealPaymentDocumentToDeal

        /// <summary>
        /// Создается оплата от клиента по сделке, форма оплаты - наличными денежными средствами
        /// Добавляется в сделку
        /// 
        /// Ссылка на сделку сразу после создания должна быть пустой, а сумма оплат по сделке - нулевой
        /// После добавления:
        /// Ссылка на сделку должна быть не пустой
        /// Сумма оплат по данной сделке должна быть равна сумме оплаты
        /// Сумма оплат наличными по данной сделке должна быть равна сумме оплаты
        /// Сумма неразнесенных оплат по данной сделке должна быть равна сумме оплаты
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_Adding_DealPaymentFromClient_With_Cash_To_Deal_Must_Set_DealPaymentSum_CashDealPaymentSum_UndistributedDealPaymentFromClientSum()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user.Object, "002", new DateTime(2012, 2, 19), 600M, DealPaymentForm.Cash, currentDate);

            Assert.IsNull(dealPaymentFromClient.Deal);
            Assert.AreEqual(0M, deal.DealPaymentSum);
            Assert.AreEqual(0M, deal.CashDealPaymentSum);
            Assert.AreEqual(0M, deal.UndistributedDealPaymentFromClientSum);

            // Act
            deal.AddDealPaymentDocument(dealPaymentFromClient);

            Assert.AreEqual(deal, dealPaymentFromClient.Deal);
            Assert.AreEqual(600M, deal.DealPaymentSum);
            Assert.AreEqual(600M, deal.CashDealPaymentSum);
            Assert.AreEqual(600M, deal.UndistributedDealPaymentFromClientSum);
        }

        /// <summary>
        /// Создается оплата от клиента по сделке, форма оплаты - по безналичному расчету
        /// Добавляется в сделку
        /// 
        /// Ссылка на сделку сразу после создания должна быть пустой, а сумма оплат по сделке - нулевой
        /// После добавления:
        /// Ссылка на сделку должна быть не пустой
        /// Сумма оплат по данной сделке должна быть равна сумме оплаты
        /// Сумма оплат наличными по данной сделке должна быть нулевой
        /// Сумма неразнесенных оплат по данной сделке должна быть равна сумме оплаты
        /// </summary>
        [TestMethod]
        public void DealPaymentDocumentTest_Adding_DealPaymentFromClient_With_Cashless_To_Deal_Must_Set_DealPaymentSum_UndistributedDealPaymentFromClientSum_Not_CashDealPaymentSum()
        {
            // Создаем оплату от клиента
            var currentDate = DateTime.Now;
            var dealPaymentFromClient = new DealPaymentFromClient(team.Object, user.Object, "004", new DateTime(2012, 2, 19), 600M, DealPaymentForm.Cashless, currentDate);

            Assert.IsNull(dealPaymentFromClient.Deal);
            Assert.AreEqual(0M, deal.DealPaymentSum);
            Assert.AreEqual(0M, deal.CashDealPaymentSum);
            Assert.AreEqual(0M, deal.UndistributedDealPaymentFromClientSum);

            // Act
            deal.AddDealPaymentDocument(dealPaymentFromClient);

            Assert.AreEqual(deal, dealPaymentFromClient.Deal);
            Assert.AreEqual(600M, deal.DealPaymentSum);
            Assert.AreEqual(0M, deal.CashDealPaymentSum);
            Assert.AreEqual(600M, deal.UndistributedDealPaymentFromClientSum);
        }

        #endregion

        //[TestMethod]
        //public void Payment_Empty_Number_MustBeAccepted()
        //{
        //    payment = new Payment(PaymentType.PaymentToClient, "", DateTime.Today, 100, DealPaymentForm.Cash) { Id = new Guid("22222222-0000-0000-0000-000000000000") };

        //    Assert.AreEqual("", payment.PaymentDocumentNumber);
        //    Assert.AreEqual(DateTime.Today, payment.CreationDate.Date);
        //    Assert.AreEqual(DateTime.Today, payment.Date);
        //    Assert.IsNull(payment.Deal);
        //    Assert.IsNull(payment.DeletionDate);
        //    Assert.IsNotNull(payment.Distributions);
        //    Assert.AreEqual(DealPaymentForm.Cash, payment.Form);
        //    Assert.AreNotEqual(Guid.Empty, payment.Id);
        //    Assert.AreEqual(100, payment.Sum);
        //}

        //[TestMethod]
        //public void Payment_NullPaymentDocumentNumber_Must_Throw_Exception()
        //{
        //    try
        //    {
        //        var payment = new Payment(null, DateTime.Now, 100M, DealPaymentForm.Cash);
        //        throw new Exception("Исключения не было.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Номер документа не может быть пустой строкой.", ex.Message);
        //    }
        //}

        //[TestMethod]
        //public void Payment_Constructor_Must_Throw_Exception_On_TooBigDate()
        //{
        //    try
        //    {
        //        var payment = new Payment(PaymentType.PaymentToClient, "1", DateTime.Now.AddDays(2), 100M, DealPaymentForm.Cash);
        //        throw new Exception("Исключения не было.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Дата оплаты не может быть больше текущей даты.", ex.Message);
        //    }
        //}

        //[TestMethod]
        //public void Payment_Constructor_Must_Throw_Exception_On_ZeroSum()
        //{
        //    try
        //    {
        //        var payment = new Payment(PaymentType.PaymentToClient, "1", DateTime.Now, 0, DealPaymentForm.Cash);
        //        throw new Exception("Исключения не было.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Сумма оплаты не может быть отрицательной или равной нулю.", ex.Message);
        //    }
        //}

        //[TestMethod]
        //public void Payment_Constructor_Must_Throw_Exception_On_NegativeSum()
        //{
        //    try
        //    {
        //        var payment = new Payment(PaymentType.PaymentToClient, "1", DateTime.Now, -100M, DealPaymentForm.Cash);
        //        throw new Exception("Исключения не было.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual("Сумма оплаты не может быть отрицательной или равной нулю.", ex.Message);
        //    }
        //}

        //// сообщение ожидать другое: со словами "текущей даты"
        //[TestMethod]
        //public void Payment_Assignment_Must_Throw_Exception_On_FutureDate()
        //{
        //    try
        //    {
        //        var payment = new Payment(PaymentType.PaymentToClient, "1", DateTime.Now, 100M, DealPaymentForm.Cashless);
        //        payment.Date = DateTime.Now.AddDays(1);
        //        throw new Exception("Исключения не было.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.AreEqual(String.Format("Дата оплаты не может быть больше даты создания оплаты ({0}).", DateTime.Now.ToShortDateString()), ex.Message);
        //    }
        //}
    }
}
