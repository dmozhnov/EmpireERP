using System;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.Collections;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass]
    public class DealServiceTest
    {
        #region Поля

        private Mock<DealService> dealServiceMock;
        private DealService dealService;

        private Mock<IDealIndicatorService> dealIndicatorService;
        private Mock<IExpenditureWaybillIndicatorService> expenditureWaybillIndicatorService;
        private Mock<IStorageService> storageService;
        private Mock<IClientContractIndicatorService> clientContractIndicatorService;

        private Mock<IClientContractRepository> clientContractRepository;
        private Mock<IDealRepository> dealRepository;
        private Mock<ITaskRepository> taskRepository;

        private AccountOrganization accountOrganization;
        private ClientOrganization clientOrganization;
                
        private Mock<User> user;
        
        #endregion

        #region Инициализация

        public DealServiceTest()
        {
            
        }
        
        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();
            
            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var juridicalPerson1 = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            accountOrganization = new AccountOrganization("Тестовая собственная организация", "Тестовая собственная организация", juridicalPerson1) { Id = 1 };

            var juridicalPerson2 = new JuridicalPerson(juridicalLegalForm) { Id = 2 };
            clientOrganization = new ClientOrganization("Тестовая организация клиента", "Тестовая организация клиента", juridicalPerson2) { Id = 2 };

            clientContractRepository = Mock.Get(IoCContainer.Resolve<IClientContractRepository>());
            dealRepository = Mock.Get(IoCContainer.Resolve<IDealRepository>());

            dealIndicatorService = Mock.Get(IoCContainer.Resolve<IDealIndicatorService>());
            expenditureWaybillIndicatorService = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillIndicatorService>());
            storageService = Mock.Get(IoCContainer.Resolve<IStorageService>());
            clientContractIndicatorService = Mock.Get(IoCContainer.Resolve<IClientContractIndicatorService>());
            taskRepository = Mock.Get(IoCContainer.Resolve<ITaskRepository>());

            dealServiceMock = new Mock<DealService>(dealRepository.Object, clientContractRepository.Object, taskRepository.Object, dealIndicatorService.Object,
                expenditureWaybillIndicatorService.Object, storageService.Object, clientContractIndicatorService.Object);

            dealService = dealServiceMock.Object;

            user = new Mock<User>();
            user.Setup(x => x.GetPermissionDistributionType(It.IsAny<Permission>())).Returns(PermissionDistributionType.All);

            clientContractIndicatorService.Setup(x => x.CalculateCashPaymentLimitExcessByPaymentsFromClient(It.IsAny<ClientContract>())).Returns(0);
        }

        #endregion

        #region Тесты

        /// <summary>
        /// При вызове SetContract сделке без договора должен назначиться указанный договор.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_SetContract_Ok()
        {
            //Arrange
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var deal = new Deal_Accessor("Сделка1", It.IsAny<User>()) { Client = client.Object };
            var contract = new ClientContract(accountOrganization, clientOrganization, "Тестовый договор", "1", DateTime.Now, DateTime.Now);

            //Act
            dealService.SetContract((Deal)deal.Target, contract, user.Object);

            //Assert
            Assert.AreEqual(contract, deal.Contract);
        }

        /// <summary>
        /// При вызове SetContract сделке с договором должен назначиться указанный договор.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_ChangeContract_Ok()
        {
            //Arrange
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var oldContract = new ClientContract(accountOrganization, clientOrganization, "Старый договор", "1", DateTime.Now, DateTime.Now);
            var newContract = new ClientContract(accountOrganization, clientOrganization, "Новый договор", "2", DateTime.Now, DateTime.Now);

            var deal = new Deal_Accessor("Сделка1", It.IsAny<User>()) { Client = client.Object, Contract = oldContract };            

            //Act
            dealService.SetContract((Deal)deal.Target, newContract, user.Object);

            //Assert
            Assert.AreEqual(newContract, deal.Contract);
        }

        /// <summary>
        /// Если у сделки сменился договор и при этом старый договор больше не относится ни к какой сделке, то он должен быть удален из системы.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_ChangeContractAndOldContractIsNotUsedByAnyDeal_OldContractDeletes()
        {
            //Arrange
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var oldContract = new ClientContract(accountOrganization, clientOrganization, "Старый договор", "1", DateTime.Now, DateTime.Now);
            var newContract = new ClientContract(accountOrganization, clientOrganization, "Новый договор", "2", DateTime.Now, DateTime.Now);

            var deal = new Deal_Accessor("Сделка1", It.IsAny<User>()) { Client = client.Object, Contract = oldContract };

            clientContractRepository.Setup(x => x.IsUsedBySingleDeal(It.IsAny<ClientContract>(), It.IsAny<Deal>())).Returns(true);

            //Act
            dealService.SetContract((Deal)deal.Target, newContract, user.Object);

            //Assert
            clientContractRepository.Verify(x => x.Delete(oldContract), Times.Once());
        }

        /// <summary>
        /// Если у сделки сменился договор и при этом старый договор относится к каким-либо другим сделкам, то он должен оставатьсяв системе.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_ChangeContractAndOldContractIsUsedByAnyDeal_OldContractStaysUndeleted()
        {
            //Arrange
            var client = new Mock<Client>();
            client.Setup(x => x.Organizations).Returns(new List<ContractorOrganization> { clientOrganization });

            var oldContract = new ClientContract(accountOrganization, clientOrganization, "Старый договор", "1", DateTime.Now, DateTime.Now);
            var newContract = new ClientContract(accountOrganization, clientOrganization, "Новый договор", "2", DateTime.Now, DateTime.Now);

            var deal = new Deal_Accessor("Сделка1", It.IsAny<User>()) { Client = client.Object, Contract = oldContract };

            clientContractRepository.Setup(x => x.IsUsedBySingleDeal(It.IsAny<ClientContract>(), It.IsAny<Deal>())).Returns(false);

            //Act
            dealService.SetContract((Deal)deal.Target, newContract, user.Object);
            
            //Assert
            clientContractRepository.Verify(x => x.Delete(It.IsAny<ClientContract>()), Times.Never());            
        }

        /// <summary>
        /// Если назначаем сделке (не имевшей ранее никакого договора) договор, то должен вызываться метод проверки прав на установку нового договора.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_SetContractToDealThatHasNoContract_ChecksPossibilityToSetContract()
        {
            //Arrange
            var client = new Mock<Client>();

            var сontract = new ClientContract(accountOrganization, clientOrganization, "Тестовый договор", "1", DateTime.Now, DateTime.Now);

            var deal = new Mock<Deal>();

            //Act
            dealService.SetContract(deal.Object, сontract, user.Object);

            //Assert
            dealServiceMock.Verify(x => x.CheckPossibilityToAddContract(deal.Object, user.Object), Times.Once());
            dealServiceMock.Verify(x => x.CheckPossibilityToChangeContract(deal.Object, user.Object), Times.Never());
        }

        /// <summary>
        /// Если назначаем сделке (имевшей ранее старый договор) новый договор, то должен вызываться метод проверки прав на смену договора.
        /// </summary>
        [TestMethod]
        public void DealServiceTest_SetNewContractToDealThatHasOldContract_ChecksPossibilityToChangeContract()
        {
            //Arrange
            var client = new Mock<Client>();

            var oldContract = new Mock<ClientContract>();
            var newContract = new Mock<ClientContract>();

            var deal = new Mock<Deal>();
            deal.Setup(x => x.Contract).Returns(oldContract.Object);

            //Act
            dealService.SetContract(deal.Object, newContract.Object, user.Object);

            //Assert
            dealServiceMock.Verify(x => x.CheckPossibilityToChangeContract(deal.Object, user.Object), Times.Once());
            dealServiceMock.Verify(x => x.CheckPossibilityToAddContract(deal.Object, user.Object), Times.Never());           
        }
                
        #endregion
    }
}
