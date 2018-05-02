using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class ProviderTest
    {
        private ProviderType providerType;
        private Provider provider;
        private EconomicAgent juridicalPerson;
        private EconomicAgent physicalPerson;
        private ProviderOrganization providerOrganization;
        private ProviderContract contract;
        private AccountOrganization accountOrganization;
        
        [TestInitialize]
        public void Init()
        {
            providerType = new ProviderType("Тестовый тип поставщика") { Id = 1 };
            provider = new Provider("Поставщик", providerType, ProviderReliability.Medium, 5) { Id = 2 };
            juridicalPerson = new JuridicalPerson(new LegalForm("ООО", EconomicAgentType.JuridicalPerson)) { Id = 3 };
            providerOrganization = new ProviderOrganization("Юрик", "Юрик", juridicalPerson) { Id = 4 };
            physicalPerson = new PhysicalPerson(new LegalForm("ИП", EconomicAgentType.PhysicalPerson)) { Id = 5 };
            accountOrganization = new AccountOrganization("Физик", "Физик", physicalPerson) { Id = 6 };
            contract = new ProviderContract(accountOrganization, providerOrganization, "Договор", "111", DateTime.Today, DateTime.Today) { Id = 7 };
        }
        
        [TestMethod]
        public void Provider_Initial_Parameters_Must_Be_Set()
        {
            Assert.AreEqual(2, provider.Id);
            Assert.AreEqual(String.Empty, provider.Comment);
            Assert.AreEqual(0, provider.ContractCount);
            Assert.AreEqual(0, provider.OrganizationCount);
            Assert.IsNotNull(provider.Organizations);
            Assert.AreEqual(ContractorType.Provider, provider.ContractorType);
            Assert.IsNotNull(provider.Contracts);
            Assert.IsNotNull(provider.CreationDate);
            Assert.IsNull(provider.DeletionDate);            
            Assert.AreEqual(ProviderReliability.Medium, provider.Reliability);
            Assert.AreEqual("Поставщик", provider.Name);
            Assert.AreEqual(providerType.Id, provider.Type.Id);
            Assert.AreEqual(5, provider.Rating);
        }

        [TestMethod]
        public void Provider_User_Parameters_Must_Be_Set()
        {
            provider.Comment = "Комментарий";
            provider.Rating = 8;
            provider.Reliability = ProviderReliability.High;
            provider.Name = "Новое название";

            Assert.AreEqual("Комментарий", provider.Comment);
            Assert.AreEqual(8, provider.Rating);
            Assert.AreEqual(ProviderReliability.High, provider.Reliability);
            Assert.AreEqual("Новое название", provider.Name);
        }

        [TestMethod]
        public void Provider_Addition_ProviderOrganization_Must_Be_Ok()
        {
            provider.AddContractorOrganization(providerOrganization);

            Assert.AreEqual(1, provider.OrganizationCount);
            Assert.AreEqual(4, (new List<ContractorOrganization>(provider.Organizations))[0].Id);
            Assert.AreEqual(1, providerOrganization.ContractorCount);
            Assert.AreEqual(2, (new List<Contractor>(providerOrganization.Contractors))[0].Id);
        }
                
        [TestMethod]
        public void Provider_Attempt_To_Add_Duplicate_ProviderOrganization_Must_Throw_Exception()
        {
            try
            {
                provider.AddContractorOrganization(providerOrganization);
                provider.AddContractorOrganization(providerOrganization);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная организация уже содержится в списке организаций контрагента.", ex.Message);
            }            
        }
        
        [TestMethod]
        public void Provider_Removing_ProviderOrganization_Must_Be_Ok()
        {
            provider.AddContractorOrganization(providerOrganization);
            provider.RemoveContractorOrganization(providerOrganization);

            Assert.AreEqual(0, providerOrganization.ContractorCount);
            Assert.AreEqual(0, provider.OrganizationCount);
        }

        [TestMethod]
        public void Provider_Attempt_To_Delete_Not_Added_ProviderOrganization_Must_Throw_Exception()
        {
            try
            {
                provider.RemoveContractorOrganization(providerOrganization);
                
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данная организация не содержится в списке организаций контрагента. Возможно, она была удалена.", ex.Message);
            }
        }

        [TestMethod]
        public void Provider_Addition_Contract_Must_Be_Ok()
        {
            provider.AddProviderContract(contract);

            Assert.AreEqual(1, provider.ContractCount);
            Assert.AreEqual(7, (new List<Contract>(provider.Contracts))[0].Id);
            Assert.AreEqual(1, contract.ContractorCount);
            Assert.AreEqual(2, (new List<Contractor>(contract.Contractors))[0].Id);
        }

        [TestMethod]
        public void Provider_Attempt_To_Add_Duplicate_ProviderContract_Must_Throw_Exception()
        {
            try
            {
                provider.AddProviderContract(contract);
                provider.AddProviderContract(contract);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данный договор уже содержится в списке договоров поставщика.", ex.Message);
            }
        }

        [TestMethod]
        public void Provider_Removing_ProviderContract_Must_Be_Ok()
        {
            provider.AddProviderContract(contract);
            provider.RemoveProviderContract(contract);

            Assert.AreEqual(0, provider.ContractCount);
            Assert.AreEqual(0, contract.ContractorCount);
        }

        [TestMethod]
        public void Provider_Attempt_To_Delete_Not_Added_ProviderContract_Must_Throw_Exception()
        {
            try
            {
                provider.RemoveProviderContract(contract);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Данный договор не содержится в списке договоров поставщика. Возможно, он был удален.", ex.Message);
            }
        }

        public void Provider_Attempt_To_Set_Incorrect_Rating_Must_Throw_Exception()
        {
            try
            {
                provider.Rating = 11;

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Значение рейтинга должно попадать в интервал [0-10].", ex.Message);
            }
        }

    }
}
