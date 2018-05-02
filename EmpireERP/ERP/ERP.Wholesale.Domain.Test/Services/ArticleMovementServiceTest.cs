using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Services.Test
{
    [TestClass]
    public class ArticleMovementServiceTest
    {
        #region Инициализация и конструкторы

        private ArticleMovementService_Accessor articleMovementService;
        
        private Mock<IWaybillRowArticleMovementRepository> waybillRowArticleMovementRepository;
        private Mock<IReceiptWaybillRepository> receiptWaybillRepository;
        private Mock<IMovementWaybillRepository> movementWaybillRepository;        
        private Mock<IExpenditureWaybillRepository> expenditureWaybillRepository;
        private Mock<IChangeOwnerWaybillRepository> changeOwnerWaybillRepository;
        private Mock<IReturnFromClientWaybillRepository> returnFromClientWaybillRepository;
        private Mock<IWriteoffWaybillRepository> writeoffWaybillRepository;
        private Mock<IIncomingWaybillRowService> incomingWaybillRowService;
        private Mock<IOutgoingWaybillRowService> outgoingWaybillRowService;

        private MeasureUnit measureUnit;
        private ArticleGroup articleGroup;
        private Article articleA;
        private Storage storageA;
        private JuridicalPerson juridicalPerson;
        private AccountOrganization accountOrganization;
        private Provider provider;
        private ValueAddedTax valueAddedTax;
        private ProviderOrganization providerOrganization;
        private PhysicalPerson physicalPerson;
        private ProviderContract providerContract;
        private User user;

        private ReceiptWaybill receiptWaybill1;
        private ReceiptWaybillRow receiptWaybillRow1;

        [TestInitialize]
        public void Init()
        {
            // инициализация IoC
            IoCInitializer.Init();

            receiptWaybillRepository = Mock.Get(IoCContainer.Resolve<IReceiptWaybillRepository>());
            waybillRowArticleMovementRepository = Mock.Get(IoCContainer.Resolve<IWaybillRowArticleMovementRepository>());
            movementWaybillRepository = Mock.Get(IoCContainer.Resolve<IMovementWaybillRepository>());
            expenditureWaybillRepository = Mock.Get(IoCContainer.Resolve<IExpenditureWaybillRepository>());
            changeOwnerWaybillRepository = Mock.Get(IoCContainer.Resolve<IChangeOwnerWaybillRepository>());
            returnFromClientWaybillRepository = Mock.Get(IoCContainer.Resolve<IReturnFromClientWaybillRepository>());
            writeoffWaybillRepository = Mock.Get(IoCContainer.Resolve<IWriteoffWaybillRepository>());            

            incomingWaybillRowService = Mock.Get(IoCContainer.Resolve<IIncomingWaybillRowService>());
            outgoingWaybillRowService = Mock.Get(IoCContainer.Resolve<IOutgoingWaybillRowService>());

            articleMovementService = new ArticleMovementService_Accessor(waybillRowArticleMovementRepository.Object,
                receiptWaybillRepository.Object, movementWaybillRepository.Object, changeOwnerWaybillRepository.Object,
                returnFromClientWaybillRepository.Object, writeoffWaybillRepository.Object, expenditureWaybillRepository.Object,
                incomingWaybillRowService.Object, outgoingWaybillRowService.Object);

            articleGroup = new ArticleGroup("Тестовая группа", "Тестовая группа") { Id = 1 };
            measureUnit = new MeasureUnit("шт.", "Штука", "123", 0) { Id = 1 };
            articleA = new Article("Тестовый товар", articleGroup, measureUnit, false) { Id = 1 };

            var juridicalLegalForm = new LegalForm("ООО", EconomicAgentType.JuridicalPerson);
            var physicalLegalForm = new LegalForm("ИП", EconomicAgentType.PhysicalPerson);

            juridicalPerson = new JuridicalPerson(juridicalLegalForm) { Id = 1 };
            physicalPerson = new PhysicalPerson(physicalLegalForm) { Id = 2 };

            accountOrganization = new AccountOrganization("Тестовое юридическое лицо", "Тестовое юридическое лицо", juridicalPerson) { Id = 1 };
            providerOrganization = new ProviderOrganization("Тестовое физическое лицо", "Тестовое физическое лицо", physicalPerson) { Id = 2 };
                        
            provider = new Provider("Тестовый поставщик", new ProviderType("Тестовый тип поставщика"), ProviderReliability.Medium, 5);
            provider.AddContractorOrganization(providerOrganization);

            providerContract = new ProviderContract(accountOrganization, providerOrganization, "ABC", "123", DateTime.Now, DateTime.Today);
            provider.AddProviderContract(providerContract);

            storageA = new Storage("A", StorageType.DistributionCenter) { Id = 1 };
            accountOrganization.AddStorage(storageA);

            valueAddedTax = new ValueAddedTax("18%", 18);

            user = new User(new Employee("Иван", "Иванов", "Иванович", new EmployeePost("Менеджер"), null), "Иванов Иван", "ivanov", "pa$$w0rd", new Team("Тестовая команда", null), null);

            var customDeclarationNumber = new String('0', 25);

            // приход 20 штук товара на склад A. ReservedCount пока не выставляется
            receiptWaybill1 = new ReceiptWaybill("123", DateTime.Today, storageA, accountOrganization, provider, 100, 0M, valueAddedTax, providerContract, customDeclarationNumber, user, user, DateTime.Now);
            receiptWaybillRow1 = new ReceiptWaybillRow(articleA, 20, 100, receiptWaybill1.PendingValueAddedTax) { Id = new Guid("e3eef62e-4764-487a-9d09-8032d642ce7f") };
            receiptWaybill1.AddRow(receiptWaybillRow1);

        }

        /// <summary>
        /// Настройка поведения метода GetIncomingWaybillRows
        /// </summary>
        /// <param name="incomingRow1">Позиция входящей накладной (обязательно приходной)</param>
        /// <param name="incomingRow2">Позиция входящей накладной (обязательно перемещение)</param>
        private void SetupGetIncomingWaybillRowsBehaviour(IncomingWaybillRow incomingRow1, IncomingWaybillRow incomingRow2 = null)
        {
            // для приходной накладной
            ValidationUtils.Assert(incomingRow1.Type == WaybillType.ReceiptWaybill, "Позиция должна относиться к приходной накладной.");
            receiptWaybillRepository.Setup(x => x.GetRows(It.IsAny<ISubQuery>())).Returns(new List<ReceiptWaybillRow>() { new Mock<ReceiptWaybillRow>().Object });
            incomingWaybillRowService.Setup(x => x.ConvertToIncomingWaybillRow(It.IsAny<ReceiptWaybillRow>())).Returns(incomingRow1);

            // для накладной перемещения
            if (incomingRow2 != null)
            {
                ValidationUtils.Assert(incomingRow2.Type == WaybillType.MovementWaybill, "Позиция должна относиться к накладной перемещение.");
                movementWaybillRepository.Setup(x => x.GetRows(It.IsAny<ISubQuery>())).Returns(new List<MovementWaybillRow>() { new Mock<MovementWaybillRow>().Object });
                incomingWaybillRowService.Setup(x => x.ConvertToIncomingWaybillRow(It.IsAny<MovementWaybillRow>())).Returns(incomingRow2);
            }
        }

        #endregion

        #region SetManualSourcesForOutgoingWaybillRow

        /// <summary>
        /// При указании неинициализированного списка источников должна возникать ошибка
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_SetManualSourcesForOutgoingWaybillRow_Nullable_DistributionInfo_Must_Throw_Error()
        {
            try
            {
                articleMovementService.SetManualSourcesForOutgoingWaybillRow(null, Guid.NewGuid(), WaybillType.MovementWaybill, 7);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Источники не заданы.", ex.Message);
            }            
        }

        /// <summary>
        /// При указании пустого списка источников должна возникать ошибка
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_SetManualSourcesForOutgoingWaybillRow_Empty_DistributionInfo_Must_Throw_Error()
        {
            try
            {
                articleMovementService.SetManualSourcesForOutgoingWaybillRow(new List<WaybillRowManualSource>(), Guid.NewGuid(), WaybillType.MovementWaybill, 7);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество источников должно быть больше 0.", ex.Message);
            }
        }

        /// <summary>
        /// Имеются два источника 5 и 10 шт. Кол-во по позиции = 7. Должно быть выброшено исключение о несоответствии количеств
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_SetManualSourcesForOutgoingWaybillRow_CountBySources_Not_Equals_CountByRow_Must_Throw_Error()
        {
            try
            {
                var distributionInfo = new List<WaybillRowManualSource>() {
                        new WaybillRowManualSource() { WaybillRowId = Guid.NewGuid(), WaybillType = WaybillType.ReceiptWaybill, Count = 5 },
                        new WaybillRowManualSource() { WaybillRowId = Guid.NewGuid(), WaybillType = WaybillType.MovementWaybill, Count = 10 }
                    };

                articleMovementService.SetManualSourcesForOutgoingWaybillRow(distributionInfo, Guid.NewGuid(), WaybillType.MovementWaybill, 7);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Количество товара по позиции не совпадает с суммой количеств по источникам.", ex.Message);
            }
        }

        /// <summary>
        /// Имеются два источника 5 и 10 шт. Кол-во по позиции = 15. Кол-во связей должно быть 2. Т.к. товара в точном наличии достаточно, то
        /// позиция получает статус ReadyToArticleMovement
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_SetManualSourcesForOutgoingWaybillRow_Two_Sources_Must_Be_Set_Successfuly_With_ReadyToArticleMovement()
        {
            var guid1 = new Guid("11111111-1111-1111-1111-111111111111");
            var guid2 = new Guid("22222222-2222-2222-2222-222222222222");

            var distributionInfo = new List<WaybillRowManualSource>() {
                new WaybillRowManualSource() { WaybillRowId = guid1, WaybillType = WaybillType.ReceiptWaybill, Count = 5 },
                new WaybillRowManualSource() { WaybillRowId = guid2, WaybillType = WaybillType.MovementWaybill, Count = 10 }
            };

            // для подсчета кол-ва связей
            int waybillRowArticleMovementCount = 0;
            waybillRowArticleMovementRepository.Setup(x => x.Save(It.IsAny<WaybillRowArticleMovement>()))
                .Callback<WaybillRowArticleMovement>(z => { waybillRowArticleMovementCount++; });

            var incomingWaybillRow1 = new IncomingWaybillRow() { Id = guid1, AvailableInStorageCount = 10, AvailableToReserveCount = 10 };
            var incomingWaybillRow2 = new IncomingWaybillRow() { Id = guid2, AvailableInStorageCount = 15, AvailableToReserveCount = 15 };

            incomingWaybillRowService.Setup(x => x.GetRows(It.IsAny<Dictionary<Guid, WaybillType>>())).Returns(
                new List<IncomingWaybillRow>() { incomingWaybillRow1, incomingWaybillRow2 });

            var state = articleMovementService.SetManualSourcesForOutgoingWaybillRow(distributionInfo, Guid.NewGuid(), WaybillType.MovementWaybill, 15);
            
            Assert.AreEqual(2, waybillRowArticleMovementCount);
            Assert.AreEqual(1, incomingWaybillRow1.UsageAsManualSourceCount);
            Assert.AreEqual(1, incomingWaybillRow2.UsageAsManualSourceCount);
            Assert.AreEqual(OutgoingWaybillRowState.ReadyToArticleMovement, state);
        }

        /// <summary>
        /// Имеются два источника 5 и 10 шт. Кол-во по позиции = 15. Кол-во связей должно быть 2. Т.к. товара в точном наличии недостаточно, то
        /// позиция получает статус ArticlePending
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_SetManualSourcesForOutgoingWaybillRow_Two_Sources_Must_Be_Added_Successfuly_With_ArticlePending()
        {
            var guid1 = new Guid("11111111-1111-1111-1111-111111111111");
            var guid2 = new Guid("22222222-2222-2222-2222-222222222222");
            
            var distributionInfo = new List<WaybillRowManualSource>() {
                    new WaybillRowManualSource() { WaybillRowId = guid1, WaybillType = WaybillType.ReceiptWaybill, Count = 5 },
                    new WaybillRowManualSource() { WaybillRowId = guid2, WaybillType = WaybillType.MovementWaybill, Count = 10 }
                };

            // для подсчета кол-ва связей
            int waybillRowArticleMovementCount = 0;
            waybillRowArticleMovementRepository.Setup(x => x.Save(It.IsAny<WaybillRowArticleMovement>()))
                .Callback<WaybillRowArticleMovement>(z => { waybillRowArticleMovementCount++; });

            var incomingWaybillRow1 = new IncomingWaybillRow() { Id = guid1, PendingCount = 5, AvailableInStorageCount = 0, AvailableToReserveCount = 5 };
            var incomingWaybillRow2 = new IncomingWaybillRow() { Id = guid2, PendingCount = 10, AvailableInStorageCount = 0, AvailableToReserveCount = 10 };

            incomingWaybillRowService.Setup(x => x.GetRows(It.IsAny<Dictionary<Guid, WaybillType>>())).Returns(
                new List<IncomingWaybillRow>() { incomingWaybillRow1, incomingWaybillRow2 });

            var state = articleMovementService.SetManualSourcesForOutgoingWaybillRow(distributionInfo, Guid.NewGuid(), WaybillType.MovementWaybill, 15);

            Assert.AreEqual(2, waybillRowArticleMovementCount);
            Assert.AreEqual(1, incomingWaybillRow1.UsageAsManualSourceCount);
            Assert.AreEqual(1, incomingWaybillRow2.UsageAsManualSourceCount);
            Assert.AreEqual(OutgoingWaybillRowState.ArticlePending, state);
        }

        #endregion

        #region ResetManualSourcesForOutgoingWaybillRow

        /// <summary>
        /// Имеется позиция накладной перемещения с установленными вручную двумя источниками с резервируемым из них кол-вом 10 и 15 шт.
        /// После сброса установленных вручную источников кол-во использований источников должно быть декрементровано, а сами связи с источниками - удалены.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_ResetManualSourcesForOutgoingWaybillRow_UsageAsManualSourceCount_Must_Be_Decreased()
        {
            // код позиции накладной перемещения
            var waybillRowId = new Guid("11111111-1111-1111-1111-111111111111");
            
            // коды позиций-источников
            var source1Id = new Guid("22222222-2222-2222-2222-222222222222");
            var source2Id = new Guid("33333333-3333-3333-3333-333333333333");

            // получаем все источники исходной позиции накладной перемещения
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, waybillRowId, WaybillType.MovementWaybill, 10),
                new WaybillRowArticleMovement(source2Id, WaybillType.MovementWaybill, waybillRowId, WaybillType.MovementWaybill, 15)            
            };

            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

            // источники, преобразованные к IncomingWaybillRow
            var incomingWaybillRows = new List<IncomingWaybillRow>() {
                new IncomingWaybillRow() { Id = source1Id, UsageAsManualSourceCount = 2 },
                new IncomingWaybillRow() { Id = source2Id, UsageAsManualSourceCount = 3 }
            };

            incomingWaybillRowService.Setup(x => x.GetRows(It.IsAny<Dictionary<Guid, WaybillType>>())).Returns(incomingWaybillRows);

            // при удалении связи увеличиваем счетчик deletedWaybillRowArticleMovementCount
            var deletedWaybillRowArticleMovementCount = 0;
            waybillRowArticleMovementRepository.Setup(x => x.Delete(It.IsAny<WaybillRowArticleMovement>())).Callback(
                () => { deletedWaybillRowArticleMovementCount++; });

            // выполняем сброс источников
            articleMovementService.ResetManualSourcesForOutgoingWaybillRow(It.IsAny<ISubQuery>());

            Assert.AreEqual(1, incomingWaybillRows.Where(x => x.Id == source1Id).FirstOrDefault().UsageAsManualSourceCount);
            Assert.AreEqual(2, incomingWaybillRows.Where(x => x.Id == source2Id).FirstOrDefault().UsageAsManualSourceCount);
            Assert.AreEqual(2, deletedWaybillRowArticleMovementCount); 
        }

        #endregion

        #region AcceptWaybillRowsWithManualSources

        /// <summary>
        /// Имеем позицию накладной перемещения с установленными вручную двумя источниками, из которых берется 10 и 15 шт. товара соответственно.
        /// При резервировании товара у источников должно увеличиться кол-во проведенного товара соответственно на 10 и 15.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_AcceptWaybillRowsWithManualSources_SourceAcceptedCount_Must_Be_Increased()
        {
            // коды позиций
            var destinationId = new Guid("11111111-1111-1111-1111-111111111111");
            var source1Id = new Guid("22222222-2222-2222-2222-222222222222");
            var source2Id = new Guid("33333333-3333-3333-3333-333333333333");

            // получаем все источники для исходной позиции
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destinationId, WaybillType.MovementWaybill, 10),
                new WaybillRowArticleMovement(source2Id, WaybillType.MovementWaybill, destinationId, WaybillType.MovementWaybill, 15)            
            };
            
            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

            // приводим все источники к incomingWaybillRow                    
            var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, AvailableToReserveCount = 30, AcceptedCount = 0, UsageAsManualSourceCount = 1, 
                Type = WaybillType.ReceiptWaybill, Batch = receiptWaybillRow1, WaybillName = receiptWaybillRow1.BatchName, AcceptanceDate = DateTime.Today.AddDays(-2) };
            var incomingRow2 = new IncomingWaybillRow() { Id = source2Id, AvailableToReserveCount = 20, AcceptedCount = 2, UsageAsManualSourceCount = 1, 
                Type = WaybillType.MovementWaybill, Batch = receiptWaybillRow1, WaybillName = receiptWaybillRow1.BatchName, AcceptanceDate = DateTime.Today.AddDays(-1) };

            // настраиваем метод GetIncomingWaybillRows
            SetupGetIncomingWaybillRowsBehaviour(incomingRow1, incomingRow2);
            
            // выполняем резервирование товара
            articleMovementService.AcceptWaybillRowsWithManualSources(null, new List<OutgoingWaybillRowSourceReservationInfo>(), DateTime.Today);

            Assert.AreEqual(10, incomingRow1.AcceptedCount);
            Assert.AreEqual(17, incomingRow2.AcceptedCount);
        }

        /// <summary>
        /// Имеем позицию накладной перемещения с установленным вручную источником, из которого берется 10 шт. товара.
        /// Однако по данному источнику в момент проводки накладной для резервирования доступно только 5, о чем должно 
        /// быть выброшено исключение.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_AcceptWaybillRowsWithManualSources_When_Not_Enough_AvailableToReserveCount_Exception_Must_Be_Thrown()
        {
            try
            {
                // коды позиций
                var destinationId = new Guid("11111111-1111-1111-1111-111111111111");
                var source1Id = new Guid("22222222-2222-2222-2222-222222222222");        

                // получаем все источники для исходной позиции
                var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                    new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destinationId, WaybillType.MovementWaybill, 10)
                };

                waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

                // приводим все источники к incomingWaybillRow                                    
                var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, AvailableToReserveCount = 5, AcceptedCount = 0, UsageAsManualSourceCount = 1,
                        Batch = receiptWaybillRow1, WaybillName = receiptWaybillRow1.BatchName, Type = WaybillType.ReceiptWaybill, AcceptanceDate = DateTime.Today.AddDays(-1) };

                // настраиваем метод GetIncomingWaybillRows
                SetupGetIncomingWaybillRowsBehaviour(incomingRow1);

                // выполняем резервирование товара
                articleMovementService.AcceptWaybillRowsWithManualSources(null, new List<OutgoingWaybillRowSourceReservationInfo>(), DateTime.Today);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно товара «Тестовый товар» по накладной № 123 от " + DateTime.Today.ToShortDateString() + ".", ex.Message);                
            }
        }

        #endregion

        #region AcceptWaybillRowsWithoutManualSources

        /// <summary>
        /// Имеется позиция накладной перемещения с кол-вом 20 шт. Однако по данной партии доступно к резервированию только 10 шт,
        /// о чем должно быть выброшено исключение.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_AcceptWaybillRowsWithoutManualSources_When_Not_Enough_AvailableToReserveCount_Exception_Must_Be_Thrown()
        {
            try
            {                                
                // исходящая позиция
                var outgoingWaybillRows = new List<OutgoingWaybillRow>() {
                    new OutgoingWaybillRow() { Batch = receiptWaybillRow1, Count = 20 }                
                };

                // доступные для резервирования позиции
                var availableToReserveIncomingRows = new List<IncomingWaybillRow>() {
                    new IncomingWaybillRow() { Batch = receiptWaybillRow1, AvailableToReserveCount = 10 }
                };

                incomingWaybillRowService.Setup(x => x.GetAvailableToReserveList(It.IsAny<ISubQuery>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>())).
                    Returns(availableToReserveIncomingRows);

                articleMovementService.AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, null, new List<OutgoingWaybillRowSourceReservationInfo>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>());
                
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно товара «Тестовый товар» по партии № 123 от " + DateTime.Today.ToShortDateString() + " на момент 01.01.0001 0:00:00.", ex.Message);
            }
        }

        /// <summary>
        /// По опреледенной партии имеется исходящая позиция с кол-вом 50 шт. и 3 входящие с доступным кол-вом 10, 20 и 30 шт. соответственно.
        /// Резервирование товара должно производится в порядке увеличения даты накладной. У первого и второго источника должно быть
        /// проведено все доступное кол-во, у третьего - только 20 шт. Статус исходящей накладной должен быть «Ожидание товара»
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_AcceptWaybillRowsWithoutManualSources_Accept_Must_Be_Ok_And_OutgoingRowState_Must_Be_ArticlePending()
        {
            var guid1 = new Guid("11111111-1111-1111-1111-111111111111");
            var guid2 = new Guid("22222222-2222-2222-2222-222222222222");
            var guid3 = new Guid("33333333-3333-3333-3333-333333333333");
            var guid4 = new Guid("44444444-4444-4444-4444-444444444444");
            
            // исходящая позиция
            var outgoingWaybillRows = new List<OutgoingWaybillRow>() {
                new OutgoingWaybillRow() { Id = guid1, Type = WaybillType.MovementWaybill, Batch = receiptWaybillRow1, Count = 50, State = OutgoingWaybillRowState.ArticlePending }                
            };

            // доступные для резервирования позиции
            var source1 = new IncomingWaybillRow() { Id = guid2, Type = WaybillType.ReceiptWaybill, Batch = receiptWaybillRow1, 
                AvailableToReserveCount = 10, WaybillDate = new DateTime(2011, 01, 01) };
            var source2 = new IncomingWaybillRow() { Id = guid3, Type = WaybillType.ReturnFromClientWaybill, Batch = receiptWaybillRow1, 
                AvailableToReserveCount = 20, WaybillDate = new DateTime(2011, 01, 02) };
            var source3 = new IncomingWaybillRow() { Id = guid4, Type = WaybillType.MovementWaybill, Batch = receiptWaybillRow1, 
                AvailableToReserveCount = 30, WaybillDate = new DateTime(2011, 01, 03) };
            
            var availableToReserveIncomingRows = new List<IncomingWaybillRow>() { source2, source3, source1 };

            incomingWaybillRowService.Setup(x => x.GetAvailableToReserveList(It.IsAny<ISubQuery>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>())).
                    Returns(availableToReserveIncomingRows);

            // инфраструктура для проверки сформированных связей источник-приемник
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>();            
            waybillRowArticleMovementRepository.Setup(x => x.Save(It.IsAny<WaybillRowArticleMovement>())).Callback<WaybillRowArticleMovement>(z => {
                waybillRowArticleMovements.Add(z);
            });

            // выполняем резервирование
            articleMovementService.AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, null, new List<OutgoingWaybillRowSourceReservationInfo>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>());

            Assert.AreEqual(10, source1.AcceptedCount);
            Assert.AreEqual(20, source2.AcceptedCount);
            Assert.AreEqual(20, source3.AcceptedCount);
            
            Assert.AreEqual(3, waybillRowArticleMovements.Count);

            Assert.AreEqual(source1.Id, waybillRowArticleMovements[0].SourceWaybillRowId);
            Assert.AreEqual(source1.Type, waybillRowArticleMovements[0].SourceWaybillType);
            Assert.AreEqual(guid1, waybillRowArticleMovements[0].DestinationWaybillRowId);
            Assert.AreEqual(WaybillType.MovementWaybill, waybillRowArticleMovements[0].DestinationWaybillType);
            Assert.AreEqual(10, waybillRowArticleMovements[0].MovingCount);
            Assert.IsFalse(waybillRowArticleMovements[0].IsManuallyCreated);

            Assert.AreEqual(source2.Id, waybillRowArticleMovements[1].SourceWaybillRowId);
            Assert.AreEqual(source2.Type, waybillRowArticleMovements[1].SourceWaybillType);
            Assert.AreEqual(20, waybillRowArticleMovements[1].MovingCount);

            Assert.AreEqual(source3.Id, waybillRowArticleMovements[2].SourceWaybillRowId);
            Assert.AreEqual(source3.Type, waybillRowArticleMovements[2].SourceWaybillType);
            Assert.AreEqual(20, waybillRowArticleMovements[2].MovingCount);

            Assert.AreEqual(OutgoingWaybillRowState.ArticlePending, outgoingWaybillRows.FirstOrDefault().State);
        }

        /// <summary>
        /// По опреледенной партии имеется исходящая позиция с кол-вом 10 шт. и 1 входящая с доступным кол-вом на складе 30.
        /// У источника должно быть проведено 10 шт. товара. Статус исходящей накладной должен быть «Готово ктовародвижению»
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_AcceptWaybillRowsWithoutManualSources_Accept_Must_Be_Ok_And_OutgoingRowState_Must_Be_ReadyToArticleMovement()
        {
            var guid1 = new Guid("11111111-1111-1111-1111-111111111111");
            var guid2 = new Guid("22222222-2222-2222-2222-222222222222");
            
            // исходящая позиция
            var outgoingWaybillRows = new List<OutgoingWaybillRow>() {
                new OutgoingWaybillRow() { Id = guid1, Type = WaybillType.MovementWaybill, Batch = receiptWaybillRow1, Count = 10, State = OutgoingWaybillRowState.ArticlePending }                
            };

            // доступные для резервирования позиции
            var source1 = new IncomingWaybillRow() { Id = guid2, Type = WaybillType.ReceiptWaybill, Batch = receiptWaybillRow1, 
                AvailableInStorageCount = 30, AvailableToReserveCount = 30, WaybillDate = new DateTime(2011, 01, 01) };
            
            var availableToReserveIncomingRows = new List<IncomingWaybillRow>() { source1 };

            incomingWaybillRowService.Setup(x => x.GetAvailableToReserveList(It.IsAny<ISubQuery>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>())).
                    Returns(availableToReserveIncomingRows);

            // инфраструктура для проверки сформированных связей источник-приемник
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>();            
            waybillRowArticleMovementRepository.Setup(x => x.Save(It.IsAny<WaybillRowArticleMovement>())).Callback<WaybillRowArticleMovement>(z => {
                waybillRowArticleMovements.Add(z);
            });

            // выполняем резервирование
            articleMovementService.AcceptWaybillRowsWithoutManualSources(outgoingWaybillRows, null, new List<OutgoingWaybillRowSourceReservationInfo>(), It.IsAny<Storage>(), It.IsAny<AccountOrganization>(), It.IsAny<DateTime>());

            Assert.AreEqual(10, source1.AcceptedCount);
            
            Assert.AreEqual(1, waybillRowArticleMovements.Count);

            Assert.AreEqual(source1.Id, waybillRowArticleMovements[0].SourceWaybillRowId);
            Assert.AreEqual(source1.Type, waybillRowArticleMovements[0].SourceWaybillType);
            Assert.AreEqual(guid1, waybillRowArticleMovements[0].DestinationWaybillRowId);
            Assert.AreEqual(WaybillType.MovementWaybill, waybillRowArticleMovements[0].DestinationWaybillType);
            Assert.AreEqual(10, waybillRowArticleMovements[0].MovingCount);
            Assert.IsFalse(waybillRowArticleMovements[0].IsManuallyCreated);

            Assert.AreEqual(OutgoingWaybillRowState.ReadyToArticleMovement, outgoingWaybillRows.FirstOrDefault().State);
        }

        #endregion

        #region CancelArticleAcceptance

        /// <summary>
        /// Имеются две позиции исходящей накладной с кол-вом товара 10 и 15 шт. соответственно. У первой источник задан вручную,
        /// у второй - автоматически при проводке.
        /// При отмене проводки поле AcceptedCount у источников должно быть обнулено. Информация о перемещении товара по второй позиции должна быть удалена.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_CancelArticleAcceptance_Must_Reset_AcceptedCount()
        {
            var source1Id = new Guid("11111111-1111-1111-1111-111111111111");
            var source2Id = new Guid("22222222-2222-2222-2222-222222222222");
            var destination1Id = new Guid("33333333-3333-3333-3333-333333333333");
            var destination2Id = new Guid("44444444-4444-4444-4444-444444444444");
                        
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destination1Id, WaybillType.MovementWaybill, 10) { IsManuallyCreated = true},
                new WaybillRowArticleMovement(source2Id, WaybillType.ReceiptWaybill, destination2Id, WaybillType.MovementWaybill, 15)            
            };

            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

            var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, AcceptedCount = 10, Type = WaybillType.ReceiptWaybill };
            var incomingRow2 = new IncomingWaybillRow() { Id = source2Id, AcceptedCount = 15, Type = WaybillType.MovementWaybill };

            // настраиваем метод GetIncomingWaybillRows
            SetupGetIncomingWaybillRowsBehaviour(incomingRow1, incomingRow2);
            
            // находим код источника удаленной записи
            var deletedWaybillRowArticleMovementSourceId = Guid.NewGuid();
            waybillRowArticleMovementRepository.Setup(x => x.Delete(It.IsAny<WaybillRowArticleMovement>()))
                .Callback<WaybillRowArticleMovement>(x => { deletedWaybillRowArticleMovementSourceId = x.SourceWaybillRowId; });

            // считаем кол-во сохраненных записей
            var savedIncomingWaybillRows = 0;
            incomingWaybillRowService.Setup(x => x.SaveRows(It.IsAny<List<IncomingWaybillRow>>()))
                .Callback<IEnumerable<IncomingWaybillRow>>(x => { savedIncomingWaybillRows = x.Count(); });

            articleMovementService.CancelArticleAcceptance((ISubQuery)null);    // вместо ISubQuery здесь можно передать null

            Assert.AreEqual(0, incomingRow1.AcceptedCount);
            Assert.AreEqual(0, incomingRow2.AcceptedCount);
            Assert.AreEqual(source2Id, deletedWaybillRowArticleMovementSourceId); // связь с позицией без ручного источника должна быть удалена
            Assert.AreEqual(2, savedIncomingWaybillRows);
        }

        #endregion                
                
        #region ShipAcceptedArticles

        [TestMethod]
        public void ArticleMovementService_ShipAcceptedArticles_Null_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.ShipAcceptedArticles((MovementWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная перемещения.", ex.Message);
            }
        }

        /// <summary>
        /// Имеется позиция входящей накладной с проведенным кол-вом = 10. При выполнении отгрузки это кол-во должно перейти в отгруженное.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_ShipAcceptedArticlesAction_Must_Decrease_AcceptedCount_And_Increase_ShippedCount()
        {
            var incomingRow = new IncomingWaybillRow() { AcceptedCount = 10, ShippedCount = 0 };
            var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 10);

            articleMovementService.shipAcceptedArticlesAction(incomingRow, waybillRowArticleMovement);

            Assert.AreEqual(0, incomingRow.AcceptedCount);
            Assert.AreEqual(10, incomingRow.ShippedCount);
        }

        /// <summary>
        /// Имеется позиция входящей накладной с проведенным кол-вом = 10 шт. При выполнении отгрузки 15 шт. должно быть выброшено исключение
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_ShipAcceptedArticlesAction_If_Not_Enought_To_Ship_Exception_Must_Be_Thrown()
        {
            try
            {
                var incomingRow = new IncomingWaybillRow() { AcceptedCount = 10, ShippedCount = 0 };
                var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 15);

                articleMovementService.shipAcceptedArticlesAction(incomingRow, waybillRowArticleMovement);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно проведенного товара для отгрузки.", ex.Message);
            }
        }

        #endregion

        #region CancelArticleShipping

        [TestMethod]
        public void ArticleMovementService_CancelArticleShipping_Null_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.CancelArticleShipping((MovementWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная перемещения.", ex.Message);
            }
        }

        /// <summary>
        /// Имеется позиция входящей накладной с отгруженным кол-вом = 10. При выполнении отмены отгрузки это кол-во должно перейти в проведенное.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_СancelArticleShippingAction_Must_Increase_AcceptedCount_And_Decrease_ShippedCount()
        {
            var incomingRow = new IncomingWaybillRow() { AcceptedCount = 0, ShippedCount = 10 };
            var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 10);

            articleMovementService.cancelArticleShippingAction(incomingRow, waybillRowArticleMovement);

            Assert.AreEqual(10, incomingRow.AcceptedCount);
            Assert.AreEqual(0, incomingRow.ShippedCount);
        }

        /// <summary>
        /// Имеется позиция входящей накладной с отгруженным кол-вом = 10 шт. При выполнении отмены отгрузки 15 шт. должно быть выброшено исключение
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_СancelArticleShippingAction_If_Not_Enought_To_Ship_Exception_Must_Be_Thrown()
        {
            try
            {
                var incomingRow = new IncomingWaybillRow() { AcceptedCount = 0, ShippedCount = 10 };
                var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 15);

                articleMovementService.cancelArticleShippingAction(incomingRow, waybillRowArticleMovement);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно отгруженного товара для отмены отгрузки.", ex.Message);
            }
        }

        #endregion
                
        #region FinallyMoveAcceptedArticles

        [TestMethod]
        public void ArticleMovementService_FinallyMoveAcceptedArticles_Null_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.FinallyMoveAcceptedArticles((ChangeOwnerWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная смены собственника.", ex.Message);
            }
            
            try
            {
                articleMovementService.FinallyMoveAcceptedArticles((ExpenditureWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная реализации товаров.", ex.Message);
            }

            try
            {
                articleMovementService.FinallyMoveAcceptedArticles((WriteoffWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная списания.", ex.Message);
            }
        }

        /// <summary>
        /// Имеется позиция входящей накладной с проведенным кол-вом = 10. При выполнении окончательного перемещения это кол-во должно перейти в окончательно перемещенное.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_FinallyMoveAcceptedArticles_Must_Decrease_AcceptedCount_And_Increase_FinallyMovedCount()
        {
            var incomingRow = new IncomingWaybillRow() { AcceptedCount = 10, FinallyMovedCount = 0 };
            var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 10);

            articleMovementService.finallyMoveAcceptedArticlesAction(incomingRow, waybillRowArticleMovement);

            Assert.AreEqual(0, incomingRow.AcceptedCount);
            Assert.AreEqual(10, incomingRow.FinallyMovedCount);
        }

        /// <summary>
        /// Имеется позиция входящей накладной с проведенным кол-вом = 10 шт. При выполнении окончательного перемещения 15 шт. должно быть выброшено исключение
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_FinallyMoveAcceptedArticles_If_Not_Enought_To_FinalMove_Exception_Must_Be_Thrown()
        {
            try
            {
                var incomingRow = new IncomingWaybillRow() { AcceptedCount = 10, FinallyMovedCount = 0 };
                var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 15);

                articleMovementService.finallyMoveAcceptedArticlesAction(incomingRow, waybillRowArticleMovement);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно проведенного товара для окончательного перемещения.", ex.Message);
            }
        }

        #endregion

        #region FinallyMoveShippedArticles

        [TestMethod]
        public void ArticleMovementService_FinallyMoveShippedArticles_Null_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.FinallyMoveShippedArticles((MovementWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная перемещения.", ex.Message);
            }
        }

        /// <summary>
        /// Имеется позиция входящей накладной с отгруженным кол-вом = 10. При выполнении окончательного перемещения это кол-во должно перейти в окончательно перемещенное.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_FinallyMoveShippedArticles_Must_Decrease_AcceptedCount_And_Increase_ShippedCount()
        {
            var incomingRow = new IncomingWaybillRow() { ShippedCount = 10, FinallyMovedCount = 0 };
            var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 10);

            articleMovementService.finallyMoveShippedArticlesAction(incomingRow, waybillRowArticleMovement);

            Assert.AreEqual(0, incomingRow.ShippedCount);
            Assert.AreEqual(10, incomingRow.FinallyMovedCount);
        }

        /// <summary>
        /// Имеется позиция входящей накладной с отгруженным кол-вом = 10 шт. При выполнении окончательного перемещения 15 шт. должно быть выброшено исключение
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_FinallyMoveShippedArticles_If_Not_Enought_To_FinalMove_Exception_Must_Be_Thrown()
        {
            try
            {
                var incomingRow = new IncomingWaybillRow() { ShippedCount = 10, FinallyMovedCount = 0 };
                var waybillRowArticleMovement = new WaybillRowArticleMovement(Guid.NewGuid(), WaybillType.ReceiptWaybill, Guid.NewGuid(), WaybillType.MovementWaybill, 15);

                articleMovementService.finallyMoveShippedArticlesAction(incomingRow, waybillRowArticleMovement);

                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Недостаточно отгруженного товара для окончательного перемещения.", ex.Message);
            }
        }

        #endregion

        #region CancelArticleFinalMoving

        [TestMethod]
        public void ArticleMovementService_CancelArticleFinalMoving_Null_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.CancelArticleFinalMoving((MovementWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная перемещения.", ex.Message);
            }

            try
            {
                articleMovementService.CancelArticleFinalMoving((ChangeOwnerWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная смены собственника.", ex.Message);
            }

            try
            {
                articleMovementService.CancelArticleFinalMoving((WriteoffWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная списания.", ex.Message);
            }

            try
            {
                articleMovementService.CancelArticleFinalMoving((ExpenditureWaybill)null);
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная реализации товаров.", ex.Message);
            }
        }

        /// <summary>
        /// Имеется исходящая позиция накладной перемещения, резервирующая товар из позиции приходной накладной в кол-ве 10 шт.
        /// При отмене окончательного перемещения исходящей накладной у входящей позиции ShippedCount должен стать равным 10, а FinallyMovedCount - 0;
        /// </summary>
        [TestMethod]        
        public void ArticleMovementService_CancelArticleFinalMoving_Must_Decrease_FinallyMovedCount_And_Increase_ShippedCount()
        {
            var source1Id = new Guid("11111111-1111-1111-1111-111111111111");
            var destination1Id = new Guid("22222222-2222-2222-2222-222222222222");

            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destination1Id, WaybillType.MovementWaybill, 10)
            };

            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

            var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, ShippedCount = 0, FinallyMovedCount = 10, Type = WaybillType.ReceiptWaybill };

            // настраиваем метод GetIncomingWaybillRows
            SetupGetIncomingWaybillRowsBehaviour(incomingRow1);

            articleMovementService.CancelArticleFinalMoving((ISubQuery)null, true);    // вместо ISubQuery здесь можно передать null

            Assert.AreEqual(10, incomingRow1.ShippedCount);
            Assert.AreEqual(0, incomingRow1.FinallyMovedCount);
        }

        /// <summary>
        /// Имеется исходящая позиция накладной реализации товаров, резервирующая товар из позиции приходной накладной в кол-ве 10 шт.
        /// При отмене окончательного перемещения исходящей накладной у входящей позиции AcceptedCount должен стать равным 10, а FinallyMovedCount - 0;
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_CancelArticleFinalMoving_Must_Decrease_FinallyMovedCount_And_Increase_AcceptedCount()
        {
            var source1Id = new Guid("11111111-1111-1111-1111-111111111111");
            var destination1Id = new Guid("22222222-2222-2222-2222-222222222222");

            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destination1Id, WaybillType.ExpenditureWaybill, 10)
            };

            waybillRowArticleMovementRepository.Setup(x => x.GetByDestinationSubQuery(It.IsAny<ISubQuery>())).Returns(waybillRowArticleMovements);

            var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, AcceptedCount = 0, FinallyMovedCount = 10, Type = WaybillType.ReceiptWaybill };

            // настраиваем метод GetIncomingWaybillRows
            SetupGetIncomingWaybillRowsBehaviour(incomingRow1);

            articleMovementService.CancelArticleFinalMoving((ISubQuery)null, false);    // вместо ISubQuery здесь можно передать null

            Assert.AreEqual(10, incomingRow1.AcceptedCount);
            Assert.AreEqual(0, incomingRow1.FinallyMovedCount);
        }



        #endregion

        #region GetIncomingWaybillRows

        /// <summary>
        /// Имеется позиция исходящей накладной, резервирующая 10 и 20 шт. товаров из двух источников соответственно.
        /// Метод должен вернуть источники с информацией о резервируемом из них кол-ве.
        /// </summary>
        [TestMethod]
        public void ArticleMovementService_GetIncomingWaybillRows_Must_Return_Two_Sources()
        {
            var source1Id = new Guid("11111111-1111-1111-1111-111111111111");
            var source2Id = new Guid("22222222-2222-2222-2222-222222222222");
            var destination1Id = new Guid("33333333-3333-3333-3333-333333333333");
                        
            var waybillRowArticleMovements = new List<WaybillRowArticleMovement>() {
                new WaybillRowArticleMovement(source1Id, WaybillType.ReceiptWaybill, destination1Id, WaybillType.MovementWaybill, 10),
                new WaybillRowArticleMovement(source2Id, WaybillType.MovementWaybill, destination1Id, WaybillType.MovementWaybill, 20)            
            };

            waybillRowArticleMovementRepository.Setup(x => x.GetByDestination(It.IsAny<Guid>())).Returns(waybillRowArticleMovements);
            
            var incomingRow1 = new IncomingWaybillRow() { Id = source1Id, AcceptedCount = 10, Type = WaybillType.ReceiptWaybill };
            var incomingRow2 = new IncomingWaybillRow() { Id = source2Id, AcceptedCount = 20, Type = WaybillType.MovementWaybill };
            var incomingRows = new List<IncomingWaybillRow>() { incomingRow1, incomingRow2 };

            incomingWaybillRowService.Setup(x => x.GetRows(It.IsAny<Dictionary<Guid, WaybillType>>())).Returns(incomingRows);

            var result = articleMovementService.GetIncomingWaybillRows(It.IsAny<Guid>());

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(10, result[incomingRow1]);
            Assert.AreEqual(20, result[incomingRow2]);
        }
        
        #endregion

        #region UpdateOutgoingWaybillsStates

        [TestMethod]
        public void ArticleMovementService_UpdateOutgoingWaybillsStates_Null_MovementWaybill_Model_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.UpdateOutgoingWaybillsStates((ReceiptWaybill)null, It.IsAny<DateTime?>());
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана приходная накладная.", ex.Message);
            }
        }

        [TestMethod]
        public void ArticleMovementService_UpdateOutgoingWaybillsStates_Null_ReceiptWaybill_Model_Must_Throw_Exception()
        {
            try
            {
                articleMovementService.UpdateOutgoingWaybillsStates((MovementWaybill)null, It.IsAny<DateTime?>());
                Assert.Fail("Исключение не вызвано.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Не указана накладная перемещения.", ex.Message);
            }
        }

        
        

        #endregion
    }
}
