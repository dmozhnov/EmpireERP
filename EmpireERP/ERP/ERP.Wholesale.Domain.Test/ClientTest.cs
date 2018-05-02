using System;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ClientTest
    {
        private ClientType clientType;
        private ClientServiceProgram serviceProgram;
        private ClientRegion region;
        private JuridicalPerson juridicalPerson;
        private LegalForm legal;
        private Client client;
        private ClientOrganization clientOrganization;
        private User user;

        [TestInitialize]
        public void Init()
        {
            clientType = new ClientType("Тестовый тип клиента") { Id = 1 };
            serviceProgram = new ClientServiceProgram("Тестовая программа") { Id = 2 };
            region = new ClientRegion("Тестовый регион") { Id = 3 };
            legal = new LegalForm("ООО", EconomicAgentType.JuridicalPerson) { Id = 5 };
            juridicalPerson = new JuridicalPerson(legal) { Id = 6 };
            client = new Client("Тестовый клиент", clientType, ClientLoyalty.Customer, serviceProgram, region, 5) { Id = 4 };
            clientOrganization = new ClientOrganization("Тестовая организация клиента", "Тестовая организация клиента", juridicalPerson) { Id = 7 };
            var employee = new Employee("Иван", "Рюрикович", "Васильевич", new EmployeePost("Царь"), null);
            user = new User(employee, "И.В. Грозный", "ivanvas", "ivanvas", new Team("Тестовая команда", null), null);
        }
        
        [TestMethod]
        public void Client_Initial_Parameters_Must_Be_Set()
        {
            Assert.AreEqual(String.Empty, client.Comment);
            Assert.AreEqual(0, client.ContractCount);
            Assert.AreEqual(ContractorType.Client , client.ContractorType);
            Assert.IsNotNull(client.Contracts);            
            Assert.AreEqual(DateTime.Today.Date, client.CreationDate.Date);
            Assert.IsNull(client.DeletionDate);
            Assert.AreEqual(4, client.Id);
            Assert.AreEqual(ClientLoyalty.Customer, client.Loyalty);
            Assert.AreEqual("Тестовый клиент", client.Name);
            Assert.AreEqual(5, client.Rating);
            Assert.AreEqual(3, client.Region.Id);
            Assert.AreEqual(2, client.ServiceProgram.Id);
            Assert.AreEqual(1, client.Type.Id);
        }

        [TestMethod]
        public void Client_UserParameters_Must_Be_Set()
        {
            client.Comment = "Комментарий";
            client.DeletionDate = DateTime.Today;
            client.Id = 100;
            client.Loyalty = ClientLoyalty.Follower;
            client.Name = "Название";
            client.Rating = 10;

            Assert.AreEqual("Комментарий", client.Comment);
            Assert.AreEqual(DateTime.Today.Date, client.DeletionDate.Value.Date);
            Assert.AreEqual(100, client.Id);
            Assert.AreEqual(ClientLoyalty.Follower, client.Loyalty);
            Assert.AreEqual("Название", client.Name);
            Assert.AreEqual(10, client.Rating);
        }

        [TestMethod]
        public void Client_Deal_Addition_Must_Be_Ok()
        {
            var deal = new Deal("Тестовая сделка", user);

            client.AddDeal(deal);

            Assert.AreEqual(1, client.DealCount);
            Assert.AreEqual("Тестовая сделка", client.Deals.ElementAt(0).Name);
            Assert.AreEqual(4, deal.Client.Id);
        }

        [TestMethod]
        public void Client_ContactorOrganization_Addition_Must_Be_Ok()
        {
            client.AddContractorOrganization(clientOrganization);

            Assert.AreEqual(1, client.OrganizationCount);
            Assert.AreEqual(4, clientOrganization.Contractors.ElementAt(0).Id);
        }

        [TestMethod]
        public void Client_Attempt_ToAdd_Duplicate_ContactorOrganization_Must_Throw_Exception()
        {
            try
            {
                client.AddContractorOrganization(clientOrganization);
                client.AddContractorOrganization(clientOrganization);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная организация уже содержится в списке организаций контрагента.", ex.Message);
            }
        }

        [TestMethod]
        public void Client_Removing_ContactorOrganization_Must_Be_Ok()
        {
            client.AddContractorOrganization(clientOrganization);
            client.RemoveContractorOrganization(clientOrganization);

            Assert.AreEqual(0, clientOrganization.ContractorCount);
            Assert.AreEqual(0, client.OrganizationCount);
        }

        [TestMethod]
        public void Client_Attempt_To_Delete_Not_Added_ClientOrganization_Must_Throw_Exception()
        {
            try
            {
                client.RemoveContractorOrganization(clientOrganization);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная организация не содержится в списке организаций контрагента. Возможно, она была удалена.", ex.Message);
            }
        }

        [TestMethod]
        public void Client_Block_Must_Work_Correctly()
        {
            var before = DateTime.Now;
            client.Block(user);
            var after = DateTime.Now;

            Assert.IsTrue(client.IsBlockedManually);
            Assert.IsNotNull(client.ManualBlockingDate);
            Assert.IsTrue(client.ManualBlockingDate.Value.IsInRange(before, after));
            Assert.AreEqual(user, client.ManualBlocker);
            Assert.AreEqual("Царь", client.ManualBlocker.Employee.Post.Name);
        }

        [TestMethod]
        public void Client_Block_Must_Throw_Exception_On_Null_UserBlocker()
        {
            try
            {
                client.Block(null);
                throw new Exception("Исключения не было.");
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Пользователь, заблокировавший клиента, не указан.", ex.Message);
            }
        }

        [TestMethod]
        public void Client_Block_Must_Throw_Exception_On_Second_Block()
        {
            try
            {
                client.Block(user);
                client.Block(user);
                throw new Exception("Исключения не было.");
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Клиент уже заблокирован.", ex.Message);
            }
        }

        [TestMethod]
        public void Client_Unblock_Must_Work_Correctly()
        {
            client.Block(user);

            Assert.IsTrue(client.IsBlockedManually);

            client.Unblock();

            Assert.IsFalse(client.IsBlockedManually);
            Assert.IsNull(client.ManualBlockingDate);
            Assert.IsNull(client.ManualBlocker);
        }

        [TestMethod]
        public void Client_Unblock_Must_Throw_Exception_On_Second_Unblock()
        {
            try
            {
                client.Block(user);

                Assert.IsTrue(client.IsBlockedManually);

                client.Unblock();
                client.Unblock();

                throw new Exception("Исключения не было.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Клиент не заблокирован.", ex.Message);
            }
        }

    }
}
