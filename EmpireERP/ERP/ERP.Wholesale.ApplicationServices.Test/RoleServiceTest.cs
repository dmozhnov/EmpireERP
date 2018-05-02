using System;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.ApplicationServices.Test
{
    [TestClass()]
    public class RoleServiceTest
    {
        private Mock<IRoleRepository> roleRepository;

        private RoleService_Accessor service;
        private Role role;

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            roleRepository = Mock.Get(IoCContainer.Resolve<IRoleRepository>());
            
            service = new RoleService_Accessor(roleRepository.Object);
            role = new Role("Тестовая роль");
        }

        [TestMethod()]        
        public void PermissionDistributions_With_Direct_Relation_And_Less_Child_Value_Must_Be_Ok()
        {
            role.AddPermissionDistribution(new PermissionDistribution(Permission.Storage_List_Details, PermissionDistributionType.All));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_List_Details, PermissionDistributionType.Teams));
            role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_Create_Edit, PermissionDistributionType.Personal));

            service.CheckPermissionDistributionValues(role);

            Assert.AreEqual(3, role.PermissionDistributions.Count());
        }

        [TestMethod()]
        public void PermissionDistributions_With_Direct_Relation_And_Larger_Child_Value_Must_Throw_Exception()
        {
            try
            {
                role.AddPermissionDistribution(new PermissionDistribution(Permission.Storage_List_Details, PermissionDistributionType.All));
                role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_List_Details, PermissionDistributionType.Personal));
                role.AddPermissionDistribution(new PermissionDistribution(Permission.ReceiptWaybill_Create_Edit, PermissionDistributionType.Teams));

                service.CheckPermissionDistributionValues(role);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(String.Format("Право «{0}» не может иметь распространение, большее, чем право «{1}».",
                            Permission.ReceiptWaybill_Create_Edit.GetDisplayName(), Permission.ReceiptWaybill_List_Details.GetDisplayName()), ex.Message);
            }
        }

        [TestMethod()]
        public void Permission_Without_PersonalPermissionDistribution_Must_Be_Ok()
        {
            try
            {
                role.AddPermissionDistribution(new PermissionDistribution(Permission.Storage_List_Details, PermissionDistributionType.All));
                role.AddPermissionDistribution(new PermissionDistribution(Permission.WriteoffWaybill_List_Details, PermissionDistributionType.Personal));
                role.AddPermissionDistribution(new PermissionDistribution(Permission.WriteoffReason_Create, PermissionDistributionType.All));

                service.CheckPermissionDistributionValues(role);                
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        
    }
}
