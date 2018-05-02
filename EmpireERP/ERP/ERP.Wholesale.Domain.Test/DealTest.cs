using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class DealTest
    {
        private ClientType clientType;
        private ClientServiceProgram serviceProgram;
        private ClientRegion region;
        private Client client;
        private User user;
        private JuridicalPerson juridicalPerson;
        private AccountOrganization accountOrganization;
        private ClientOrganization clientOrganization;
        private ClientContract contract;

        [TestInitialize]
        public void Init()
        {
            clientType = new ClientType("Тестовый тип клиента") { Id = 1 };
            serviceProgram = new ClientServiceProgram("Тестовая программа") { Id = 2 };
            region = new ClientRegion("Тестовый регион") { Id = 3 };
            client = new Client("Тестовый клиент", clientType, ClientLoyalty.Customer, serviceProgram, region, 5) { Id = 4 };
            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 1 };
            clientOrganization = new ClientOrganization("Тестовая организация клиента", "Тестовая организация клиента", new JuridicalPerson(juridicalLegalForm) { Id = 2 }) { Id = 3 };
            contract = new ClientContract(accountOrganization, clientOrganization, "12", "1", DateTime.Now, DateTime.Now);
        }

        [TestMethod]
        public void Deal_InitialParameters_Must_Be_Set()
        {
            var deal = new Deal("Тестовая сделка", user);

            Assert.IsNull(deal.Client);
            Assert.AreEqual(String.Empty, deal.Comment);
            Assert.IsNull(deal.Contract);
            Assert.IsNotNull(deal.Quotas);
            Assert.AreEqual(DateTime.Today, deal.CreationDate.Date);
            Assert.AreEqual(DateTime.Today, deal.StartDate.Date);
            Assert.AreEqual(DateTime.Today, deal.StageDate.Date);
            Assert.AreEqual(0, deal.Id);
            Assert.IsTrue(deal.IsActive);
            Assert.AreEqual("Тестовая сделка", deal.Name);
            Assert.AreEqual(DealStage.ClientInvestigation, deal.Stage);
            Assert.AreEqual(1, deal.StageHistory.Count());
            Assert.IsNotNull(deal.StageHistory.SingleOrDefault().StartDate);
            Assert.AreEqual(0, deal.StageHistory.SingleOrDefault().Deal.Id);
            Assert.AreEqual(DealStage.ClientInvestigation, deal.StageHistory.SingleOrDefault().DealStage);
            Assert.IsNull(deal.StageHistory.SingleOrDefault().EndDate);
            Assert.IsNull(deal.StageHistory.SingleOrDefault().StageDuration);
            Assert.AreEqual(DateTime.Today, deal.StageHistory.SingleOrDefault().StartDate.Date);
            Assert.IsTrue(deal.IsActive);
        }

        [TestMethod]
        public void Deal_UserParameters_Must_Be_Set()
        {
            var deal = new Deal("Тестовая сделка", user)
            {
                Id = 5,
                Comment = "Комментарий",
            };

            Assert.AreEqual(5, deal.Id);
            Assert.AreEqual("Комментарий", deal.Comment);
        }

        [TestMethod]
        public void Deal_Quota_Addition_Must_Be_Ok()
        {
            var deal = new Deal("Тестовая сделка", user) { Id = 1 };
            var quota = new DealQuota("Тестовая квота", 10) { Id = 2 };

            deal.AddQuota(quota);

            Assert.AreEqual(1, deal.QuotaCount);
            Assert.AreEqual(2, deal.Quotas.ElementAt(0).Id);
            Assert.AreEqual(1, deal.Id);
            Assert.AreEqual(DateTime.Today, quota.StartDate.Date);
        }

        [TestMethod]
        public void Deal_Quota_Deletion_Must_Be_Ok()
        {
            var deal = new Deal("Тестовая сделка", user) { Id = 1 };
            var quota1 = new DealQuota("Тестовая квота 1", 10) { Id = 2 };
            var quota2 = new DealQuota("Тестовая квота 2", 99) { Id = 3 };

            deal.AddQuota(quota1);
            deal.AddQuota(quota2);

            Assert.AreEqual(2, deal.QuotaCount);

            deal.RemoveQuota(quota2, true);

            Assert.AreEqual(1, deal.QuotaCount);
            Assert.AreEqual(null, quota2.DeletionDate);
        }

        //[TestMethod]
        //// Переделать под новые оплаты (DealPaymentDocument)
        //public void Deal_Payment_Addition_Must_Be_Ok()
        //{
        //    var deal = new Deal("Тестовая сделка", user) { Id = 1 };
        //    var payment = new Payment(PaymentType.PaymentFromClient, "10", DateTime.Today, 100, DealPaymentForm.Cash);
        //    deal.AddPayment(payment);

        //    Assert.AreEqual(1, deal.Payments.Count());
        //    Assert.AreEqual(deal.Id, payment.Deal.Id);
        //    Assert.AreEqual(100, deal.PaymentSum);
        //}

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage1()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);

            Assert.AreEqual(DealStage.ClientInvestigation, deal.Stage);
            Assert.AreEqual(1, deal.StageHistory.Count());
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.CommercialProposalPreparation, deal.Stage);
            Assert.AreEqual(2, deal.StageHistory.Count());
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage2()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.CommercialProposalPreparation;

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.Negotiations, deal.Stage);
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage3()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.Negotiations;

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.ContractSigning, deal.Stage);
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Not_Work_From_Stage4_Without_Contract()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.ContractSigning;
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
            Assert.IsNull(deal.Contract);

            try
            {
                deal.CheckPossibilityToMoveToNextStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Невозможно перевести сделку на этап «Исполнение договора», так как по ней отсутствует договор.", ex.Message);
            }
        }


        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage4_With_Contract()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.ContractSigning;

            deal.Contract = contract;
            Assert.IsNotNull(deal.Contract);

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.ContractExecution, deal.Stage);
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage5()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.ContractExecution;

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.ContractClosing, deal.Stage);
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Work_From_Stage6()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.ContractClosing;

            deal.CheckPossibilityToMoveToNextStage();
            deal.MoveToNextStage();

            Assert.AreEqual(DealStage.SuccessfullyClosed, deal.Stage);
            Assert.IsFalse(deal.IsActive);
            Assert.IsTrue(deal.IsClosed);
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Not_Work_From_Stage7_1()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.PerformStageChanging(DealStage.SuccessfullyClosed);
            Assert.IsFalse(deal.IsActive);
            Assert.IsTrue(deal.IsClosed);

            try
            {
                deal.CheckPossibilityToMoveToNextStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Этап сделки «Успешно закрыто» не имеет следующего этапа.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Not_Work_From_Stage7_2()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.PerformStageChanging(DealStage.ContractAbrogated);
            Assert.IsFalse(deal.IsActive);
            Assert.IsTrue(deal.IsClosed);

            try
            {
                deal.CheckPossibilityToMoveToNextStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Этап сделки «Договор расторгнут» не имеет следующего этапа.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Not_Work_From_Stage7_3()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.PerformStageChanging(DealStage.DealRejection);
            Assert.IsFalse(deal.IsActive);
            Assert.IsTrue(deal.IsClosed);

            try
            {
                deal.CheckPossibilityToMoveToNextStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Этап сделки «Отказ» не имеет следующего этапа.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_Payment_NextStage_Must_Not_Work_From_Stage0()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.DecisionMakerSearch;
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);

            try
            {
                deal.CheckPossibilityToMoveToNextStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Этап сделки «Поиск принимающего решения» не имеет следующего этапа.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_Payment_PreviousStage_Must_Not_Work_From_Stage1()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.Stage = DealStage.ClientInvestigation;
            Assert.IsTrue(deal.IsActive);
            Assert.IsFalse(deal.IsClosed);

            try
            {
                deal.CheckPossibilityToMoveToPreviousStage();
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Этап сделки «Исследование клиента» не имеет предыдущего этапа.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_Payment_PreviousStage_Must_Work_From_Stage2()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.stageHistory.Clear();
            deal.PerformStageChanging(DealStage.ClientInvestigation);
            deal.PerformStageChanging(DealStage.CommercialProposalPreparation);

            deal.CheckPossibilityToMoveToPreviousStage();
            deal.MoveToPreviousStage(); // Из-за отсутствия порядкового номера этапа время будет одинаковым, и метод перехода сработает неверно.
            // Из пройденных этапов будет выбран не последний, а неопределенный.
        }

        [TestMethod]
        public void Deal_Payment_PreviousStage_Must_Work_From_Stage0()
        {
            var deal = new Deal_Accessor("Тестовая сделка", user);
            deal.PerformStageChanging(DealStage.DecisionMakerSearch);
            Assert.AreEqual(DealStage.DecisionMakerSearch, deal.Stage);

            deal.CheckPossibilityToMoveToPreviousStage();
            deal.MoveToPreviousStage(); // Из-за отсутствия порядкового номера этапа время будет одинаковым, и метод перехода сработает неверно.
            // Из пройденных этапов будет выбран не последний, а неопределенный.
        }

        [TestMethod]
        public void Deal_TryToSetContractWithClientOrganizationThatIsNotLinkedToClient_ThrowsException()
        {
            try
            {
                var client = new Mock<Client>();
                client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

                var deal = new Deal_Accessor("Тестовая сделка", user) { Client = client.Object };

                var newClientOrganization = new Mock<ClientOrganization>();

                var newContract = new Mock<ClientContract>();
                newContract.Setup(x => x.ContractorOrganization).Returns(newClientOrganization.Object);

                deal.Contract = newContract.Object;

                Assert.Fail("Исключение не вызвано.");
            }            
            catch (Exception ex)
            {
                Assert.AreEqual("Выбранная организация клиента больше не принадлежит данному клиенту. Возможно, она была удалена.", ex.Message);
            }
        }

        [TestMethod]
        public void Deal_SetContract_Ok()
        {
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var deal = new Deal_Accessor("Тестовая сделка", user) { Client = client.Object };

            var newContract = new Mock<ClientContract>();
            newContract.Setup(x => x.ContractorOrganization).Returns(clientOrganization);

            deal.Contract = newContract.Object;

            Assert.AreEqual(newContract.Object, deal.Contract);
        }

        [TestMethod]
        public void Deal_ChangeContract_Ok()
        {
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var oldContract = new Mock<ClientContract>();

            var deal = new Deal_Accessor("Тестовая сделка", user) { Client = client.Object, contract = oldContract.Object };

            var newContract = new Mock<ClientContract>();
            newContract.Setup(x => x.ContractorOrganization).Returns(clientOrganization);

            deal.Contract = newContract.Object;

            Assert.AreEqual(newContract.Object, deal.Contract);
        }

        [TestMethod]
        public void Deal_SetContractToNull_Ok()
        {
            var deal = new Deal_Accessor();

            deal.Contract = null;

            Assert.IsNull(deal.Contract);
        }

        [TestMethod]
        public void Deal_ChangeContractToNull_Ok()
        {
            var clientContract = new Mock<ClientContract>();
            var deal = new Deal_Accessor() { contract = clientContract.Object };

            deal.Contract = null;

            Assert.IsNull(deal.Contract);
        }

    }
}
