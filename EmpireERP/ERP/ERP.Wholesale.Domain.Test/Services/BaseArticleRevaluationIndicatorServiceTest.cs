using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Test.Infrastructure;
using ERP.Utils;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test.Services
{
    [TestClass]
    public class BaseArticleRevaluationIndicatorServiceTest
    {
        #region Инициализация и конструкторы

        private BaseArticleRevaluationIndicatorService<ExactArticleRevaluationIndicator> baseArticleRevaluationIndicatorService;
        private Mock<IExactArticleRevaluationIndicatorRepository> exactArticleRevaluationIndicatorRepository;

        private readonly short storageId = 1;
        private readonly int accountOrganizationId = 2;

        public BaseArticleRevaluationIndicatorServiceTest()
        {
            // инициализация IoC
            IoCInitializer.Init();
        }

        [TestInitialize]
        public void Init()
        {
            exactArticleRevaluationIndicatorRepository = Mock.Get(IoCContainer.Resolve<IExactArticleRevaluationIndicatorRepository>());

            baseArticleRevaluationIndicatorService = new ExactArticleRevaluationIndicatorService(exactArticleRevaluationIndicatorRepository.Object);
        }

        #endregion

        /// <summary>
        /// В БД не ни одного показателя по данному МХ и организации
        /// 
        /// При передаче в метод словаря "Дата/значение прироста показателя":
        /// 01.01 = +100.15 руб.
        /// 02.01 = +150.25 руб.
        /// 03.01 = +500.45 руб.
        /// 
        /// Должны быть созданы 3 показателя переоценки:
        /// 01.01 - 02.01 = 100.15 руб.
        /// 02.01 - 03.01 = 250.40 руб.
        /// 03.01 - ... = 750.85 руб.
        /// 
        /// </summary>
        [TestMethod]
        public void BaseArticleRevaluationServiceTest_UpdateByDeltasDictionary_And_EmptyExistingIndicatorList_Must_Add_All_From_DeltasDictionary()
        {
            // Assign
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0201 = new DateTime(DateTime.Now.Year, 1, 2);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);

            var deltasInfo = new DynamicDictionary<DateTime, decimal>();
            deltasInfo.Add(d0101, 100.15M);
            deltasInfo.Add(d0201, 150.25M);
            deltasInfo.Add(d0301, 500.45M);

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0101, storageId, accountOrganizationId))
                .Returns(new List<ExactArticleRevaluationIndicator>());

            // для проверки созданных показателей
            var createdIndicators = new List<ExactArticleRevaluationIndicator>();
            // получение созданных показателей
            GetCreatedIndicators(createdIndicators);

            // Act
            baseArticleRevaluationIndicatorService.Update(deltasInfo, storageId, accountOrganizationId);

            // Assert
            Assert.AreEqual(3, createdIndicators.Count);

            var firstIndicator = createdIndicators.FirstOrDefault(x => x.StartDate == d0101 && x.EndDate == d0201 && x.RevaluationSum == 100.15M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == null);
            Assert.IsNotNull(firstIndicator);

            var secondIndicator = createdIndicators.FirstOrDefault(x => x.StartDate == d0201 && x.EndDate == d0301 && x.RevaluationSum == 250.40M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == firstIndicator.Id);
            Assert.IsNotNull(secondIndicator);

            var thirdIndicator = createdIndicators.FirstOrDefault(x => x.StartDate == d0301 && x.EndDate == null && x.RevaluationSum == 750.85M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == secondIndicator.Id);
            Assert.IsNotNull(thirdIndicator);
        }

        /// <summary>
        /// В БД два показателя:
        /// 03.01 - 05.01 = 12.34 руб.
        /// 05.01 - ... = 56.78 руб.
        /// 
        /// При передаче в метод словаря "Дата/значение прироста показателя":
        /// 08.01 = +100.15 руб.
        /// 09.01 = +150.25 руб.
        /// 
        /// Должны получиться 4 показателя переоценки:
        /// 03.01 - 05.01 = 12.34 руб.
        /// 05.01 - 08.01 = 56.78 руб.
        /// 08.01 - 09.01 = 156.93 руб.
        /// 09.01 - ... = 307.18 руб.
        /// 
        /// </summary>
        [TestMethod]
        public void BaseArticleRevaluationServiceTest_UpdateByDeltasDictionary_Must_Be_Right1()
        {
            // Assign
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0801 = new DateTime(DateTime.Now.Year, 1, 8);
            var d0901 = new DateTime(DateTime.Now.Year, 1, 9);
            
            var firstExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0301, storageId, accountOrganizationId, 12.34M) 
                { EndDate = d0501, Id = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            var secondExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0501, storageId, accountOrganizationId, 56.78M) 
                { EndDate = null, Id = new Guid("22222222-2222-2222-2222-222222222222"), PreviousId = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0801, storageId, accountOrganizationId))
                .Returns(new List<ExactArticleRevaluationIndicator>() { secondExistingIndicator });

            // для проверки созданных показателей
            var allIndicators = new List<ExactArticleRevaluationIndicator>();
            // получение созданных показателей
            GetCreatedIndicators(allIndicators);

            var deltasInfo = new DynamicDictionary<DateTime, decimal>();
            deltasInfo.Add(d0801, 100.15M);
            deltasInfo.Add(d0901, 150.25M);

            // Act
            baseArticleRevaluationIndicatorService.Update(deltasInfo, storageId, accountOrganizationId);

            // получаем полную коллекцию всех имеющихся показателей
            allIndicators.AddRange(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // Assert
            Assert.AreEqual(4, allIndicators.Count);

            var firstIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0301 && x.EndDate == d0501 && x.RevaluationSum == 12.34M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == null);
            Assert.IsNotNull(firstIndicator);

            var secondIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0501 && x.EndDate == d0801 && x.RevaluationSum == 56.78M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == firstIndicator.Id);
            Assert.IsNotNull(secondIndicator);

            var thirdIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0801 && x.EndDate == d0901 && x.RevaluationSum == 156.93M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == secondIndicator.Id);
            Assert.IsNotNull(thirdIndicator);

            var fourthIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0901 && x.EndDate == null && x.RevaluationSum == 307.18M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == thirdIndicator.Id);
            Assert.IsNotNull(fourthIndicator);
        }

        /// <summary>
        /// В БД два показателя:
        /// 03.01 - 05.01 = 12.34 руб.
        /// 05.01 - ... = 56.78 руб.
        /// 
        /// При передаче в метод словаря "Дата/значение прироста показателя":
        /// 04.01 = +100.15 руб.
        /// 05.01 = +150.25 руб.
        /// 07.01 = +500.45 руб.
        /// 
        /// Должны получиться 4 показателя переоценки:
        /// 03.01 - 04.01 = 12.34 руб.
        /// 04.01 - 05.01 = 112.49 руб.
        /// 05.01 - 07.01 = 307.18 руб.
        /// 09.01 - ... = 807.63 руб.
        /// 
        /// </summary>
        [TestMethod]
        public void BaseArticleRevaluationServiceTest_UpdateByDeltasDictionary_Must_Be_Right2()
        {
            // Assign
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0701 = new DateTime(DateTime.Now.Year, 1, 7);
            
            var firstExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0301, storageId, accountOrganizationId, 12.34M) 
                { EndDate = d0501, Id = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            var secondExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0501, storageId, accountOrganizationId, 56.78M) 
                { EndDate = null, Id = new Guid("22222222-2222-2222-2222-222222222222"), PreviousId = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0401, storageId, accountOrganizationId))
                .Returns(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // для проверки созданных показателей
            var allIndicators = new List<ExactArticleRevaluationIndicator>();
            // получение созданных показателей
            GetCreatedIndicators(allIndicators);

            var deltasInfo = new DynamicDictionary<DateTime, decimal>();
            deltasInfo.Add(d0401, 100.15M);
            deltasInfo.Add(d0501, 150.25M);
            deltasInfo.Add(d0701, 500.45M);

            // Act
            baseArticleRevaluationIndicatorService.Update(deltasInfo, storageId, accountOrganizationId);

            // получаем полную коллекцию всех имеющихся показателей
            allIndicators.AddRange(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // Assert
            Assert.AreEqual(4, allIndicators.Count);
            
            var firstIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0301 && x.EndDate == d0401 && x.RevaluationSum == 12.34M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == null);
            Assert.IsNotNull(firstIndicator);

            var secondIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0401 && x.EndDate == d0501 && x.RevaluationSum == 112.49M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == firstIndicator.Id);
            Assert.IsNotNull(secondIndicator);

            var thirdIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0501 && x.EndDate == d0701 && x.RevaluationSum == 307.18M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == secondIndicator.Id);
            Assert.IsNotNull(thirdIndicator);

            var fourthIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0701 && x.EndDate == null && x.RevaluationSum == 807.63M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == thirdIndicator.Id);
            Assert.IsNotNull(fourthIndicator);
        }

        /// <summary>
        /// В БД два показателя:
        /// 03.01 - 05.01 = 12.34 руб.
        /// 05.01 - ... = 56.78 руб.
        /// 
        /// При передаче в метод словаря "Дата/значение прироста показателя":
        /// 03.01 = +100.15 руб.
        /// 06.01 = +150.25 руб.
        /// 
        /// Должны получиться 3 показателя переоценки:
        /// 03.01 - 05.01 = 112.49 руб.
        /// 05.01 - 06.01 = 156.93 руб.
        /// 06.01 - ... = 307.18 руб.
        /// 
        /// </summary>
        [TestMethod]
        public void BaseArticleRevaluationServiceTest_UpdateByDeltasDictionary_Must_Be_Right3()
        {
            // Assign
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);            
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);
            var d0601 = new DateTime(DateTime.Now.Year, 1, 6);

            var firstExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0301, storageId, accountOrganizationId, 12.34M) 
                { EndDate = d0501, Id = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            var secondExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0501, storageId, accountOrganizationId, 56.78M) 
                { EndDate = null, Id = new Guid("22222222-2222-2222-2222-222222222222"), PreviousId = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0301, storageId, accountOrganizationId))
                .Returns(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // для проверки созданных показателей
            var allIndicators = new List<ExactArticleRevaluationIndicator>();
            // получение созданных показателей
            GetCreatedIndicators(allIndicators);

            var deltasInfo = new DynamicDictionary<DateTime, decimal>();
            deltasInfo.Add(d0301, 100.15M);
            deltasInfo.Add(d0601, 150.25M);
            
            // Act
            baseArticleRevaluationIndicatorService.Update(deltasInfo, storageId, accountOrganizationId);

            // получаем полную коллекцию всех имеющихся показателей
            allIndicators.AddRange(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // Assert
            Assert.AreEqual(3, allIndicators.Count);

            var firstIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0301 && x.EndDate == d0501 && x.RevaluationSum == 112.49M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == null);
            Assert.IsNotNull(firstIndicator);

            var secondIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0501 && x.EndDate == d0601 && x.RevaluationSum == 156.93M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == firstIndicator.Id);
            Assert.IsNotNull(secondIndicator);

            var thirdIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0601 && x.EndDate == null && x.RevaluationSum == 307.18M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == secondIndicator.Id);
            Assert.IsNotNull(thirdIndicator);
        }

        /// <summary>
        /// В БД два показателя:
        /// 03.01 - 05.01 = 12.34 руб.
        /// 05.01 - ... = 56.78 руб.
        /// 
        /// При передаче в метод словаря "Дата/значение прироста показателя":
        /// 01.01 = +100.15 руб.
        /// 02.01 = +150.25 руб.
        /// 04.01 = +500.45 руб.
        /// 
        /// Должны получиться 5 показателей переоценки:
        /// 01.01 - 02.01 = 100.15 руб.
        /// 02.01 - 03.01 = 250.4 руб.
        /// 03.01 - 04.01 = 262.74 руб.
        /// 04.01 - 05.01 = 763.19 руб.
        /// 05.01 - ... = 807.63 руб.
        /// 
        /// </summary>
        [TestMethod]
        public void BaseArticleRevaluationServiceTest_UpdateByDeltasDictionary_Must_Be_Right4()
        {
            // Assign
            var d0101 = new DateTime(DateTime.Now.Year, 1, 1);
            var d0201 = new DateTime(DateTime.Now.Year, 1, 2);
            var d0301 = new DateTime(DateTime.Now.Year, 1, 3);
            var d0401 = new DateTime(DateTime.Now.Year, 1, 4);
            var d0501 = new DateTime(DateTime.Now.Year, 1, 5);

            var firstExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0301, storageId, accountOrganizationId, 12.34M) { EndDate = d0501, Id = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            var secondExistingIndicator = (ExactArticleRevaluationIndicator)new ExactArticleRevaluationIndicator_Accessor(d0501, storageId, accountOrganizationId, 56.78M) { EndDate = null, Id = new Guid("22222222-2222-2222-2222-222222222222"), PreviousId = new Guid("11111111-1111-1111-1111-111111111111") }.Target;

            exactArticleRevaluationIndicatorRepository.Setup(x => x.GetFrom(d0101, storageId, accountOrganizationId))
                .Returns(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // для проверки созданных показателей
            var allIndicators = new List<ExactArticleRevaluationIndicator>();
            // получение созданных показателей
            GetCreatedIndicators(allIndicators);

            var deltasInfo = new DynamicDictionary<DateTime, decimal>();
            deltasInfo.Add(d0101, 100.15M);
            deltasInfo.Add(d0201, 150.25M);
            deltasInfo.Add(d0401, 500.45M);

            // Act
            baseArticleRevaluationIndicatorService.Update(deltasInfo, storageId, accountOrganizationId);

            // получаем полную коллекцию всех имеющихся показателей
            allIndicators.AddRange(new List<ExactArticleRevaluationIndicator>() { firstExistingIndicator, secondExistingIndicator });

            // Assert
            Assert.AreEqual(5, allIndicators.Count);

            var firstIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0101 && x.EndDate == d0201 && x.RevaluationSum == 100.15M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == null);
            Assert.IsNotNull(firstIndicator);

            var secondIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0201 && x.EndDate == d0301 && x.RevaluationSum == 250.4M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == firstIndicator.Id);
            Assert.IsNotNull(secondIndicator);

            var thirdIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0301 && x.EndDate == d0401 && x.RevaluationSum == 262.74M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == secondIndicator.Id);
            Assert.IsNotNull(thirdIndicator);

            var fourthIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0401 && x.EndDate == d0501 && x.RevaluationSum == 763.19M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == thirdIndicator.Id);
            Assert.IsNotNull(fourthIndicator);

            var fifthIndicator = allIndicators.FirstOrDefault(x => x.StartDate == d0501 && x.EndDate == null && x.RevaluationSum == 807.63M &&
                x.StorageId == storageId && x.AccountOrganizationId == accountOrganizationId && x.PreviousId == fourthIndicator.Id);
            Assert.IsNotNull(fifthIndicator);
        }

        /// <summary>
        /// Получение созданных показателей
        /// </summary>        
        private void GetCreatedIndicators(List<ExactArticleRevaluationIndicator> createdIndicators)
        {
            exactArticleRevaluationIndicatorRepository.Setup(x => x.Save(It.IsAny<ExactArticleRevaluationIndicator>()))
                .Callback<ExactArticleRevaluationIndicator>(x =>
                {
                    var accessor = new ExactArticleRevaluationIndicator_Accessor(new PrivateObject(x));
                    accessor.Id = Guid.NewGuid();

                    x = (ExactArticleRevaluationIndicator)accessor.Target;

                    createdIndicators.Add(x);
                });
        }
    }
}
