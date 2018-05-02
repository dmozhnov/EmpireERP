using System;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ContractTest
    {
        private JuridicalPerson juridicalPerson;
        private PhysicalPerson physicalPerson;
        private AccountOrganization accountOrganization;
        private Provider provider;
        private Client client;
        private ClientOrganization clientOrganization;
        private ProviderOrganization providerOrganization;

        [TestInitialize]
        public void Init()
        {
            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var physicalLegalForm = new LegalForm("ООО", EconomicAgentType.PhysicalPerson);

            juridicalPerson = new JuridicalPerson(juridicalLegalForm)
            {
                Id = 1,
                CashierName = "Кассиров В.В.",
                DirectorName = "Директоров В.В.",
                DirectorPost = "Главный директор",
                INN = "123456789",
                KPP = "2020202020",
                MainBookkeeperName = "Главбухова А.А.",
                OGRN = "12345",
                OKPO = "5431"
            };

            physicalPerson = new PhysicalPerson(physicalLegalForm)
            {
                Id = 2,
                INN = "234567890",
                OGRNIP = "23456",
                OwnerName = "Физиков Ф.Ф.",
                Passport = new ValueObjects.PassportInfo
                {
                    Series = "18 00",
                    Number = "689689",
                    IssuedBy = "Центральным РОВД г. Волгограда",
                    IssueDate = DateTime.Parse("04.04.2001"),
                    DepartmentCode = "342-001"
                }
            };

            accountOrganization = new AccountOrganization("short_Name", "full_Name", juridicalPerson) { Id = 21 };
            clientOrganization = new ClientOrganization("Короткое имя", "Длинное имя", physicalPerson) { Id = 22 };
            providerOrganization = new ProviderOrganization("Краткое имя", "Полное имя", juridicalPerson) { Id = 23 };

            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrganization);

            client = new Client("Тестовый клиент", new ClientType("Тестовый тип клиента"), ClientLoyalty.Follower,
                new ClientServiceProgram("Программа"), new ClientRegion("Волгоград"), (byte)3);
        }

        [TestMethod]
        public void ProviderContract_Fields_MustBeFilled()
        {
            var date = new DateTime(2010, 12, 31);
            var providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "1945", date, date);

            Assert.AreEqual(accountOrganization, providerContract.AccountOrganization);
            Assert.AreEqual(providerOrganization, providerContract.ContractorOrganization);
            Assert.IsNotNull(providerContract.Contractors);
            Assert.IsNull(providerContract.DeletionDate);
            Assert.IsNull(providerContract.EndDate);
            Assert.AreEqual(String.Empty, providerContract.Comment);
            Assert.AreEqual("1945", providerContract.Number);
            Assert.AreEqual("Договор", providerContract.Name);
            Assert.AreEqual(date, providerContract.StartDate);
            Assert.AreEqual(date, providerContract.Date);
            Assert.AreEqual(1, accountOrganization.ContractCount);
            Assert.AreEqual(1, providerOrganization.ContractCount);
            Assert.AreEqual("Договор № 1945 от 31.12.2010", providerContract.FullName);
        }

        [TestMethod]
        public void ProviderContract_With_No_Number_Must_Have_Different_FullName()
        {
            var date = new DateTime(2010, 12, 31);
            var providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "", date, date);

            Assert.AreEqual("", providerContract.Number);
            Assert.AreEqual("Договор от 31.12.2010", providerContract.FullName);
        }

        [TestMethod]
        public void ProviderContract_As_Must_Return_Entity_Of_Correct_Type()
        {
            var date = new DateTime(2010, 12, 31);
            var providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "1", date, date);

            var contractReturnedByAs = providerContract.As<Contract>();
            Assert.IsTrue(Object.ReferenceEquals(providerContract, contractReturnedByAs));
            Assert.IsTrue(contractReturnedByAs is ProviderContract);
            Assert.IsNotNull(contractReturnedByAs as ProviderContract);
            Assert.IsNotNull((ProviderContract)contractReturnedByAs);
            Assert.IsNull(contractReturnedByAs as ClientContract);
            Assert.AreEqual(typeof(ProviderContract), contractReturnedByAs.GetType());
        }

        [TestMethod]
        public void ProviderContract_Is_Must_Return_Correct_Values()
        {
            var date = new DateTime(2010, 12, 31);
            var providerContract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "1", date, date);

            Assert.IsTrue(providerContract.Is<ProviderContract>());
            Assert.IsTrue(providerContract.Is<Contract>());
            Assert.IsFalse(providerContract.Is<ClientContract>());
            Assert.IsFalse(providerContract.Is<AccountOrganization>());
            Assert.IsFalse(providerContract.Is<InvalidCastException>());
            Assert.IsTrue(providerContract.Is<Entity<short>>());
            Assert.IsTrue(providerContract.Is<object>());
        }
    }
}
