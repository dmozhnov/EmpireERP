using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Indicators;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.Domain.Test.Services
{
    [TestClass]
    public class ArticleAvailabilityIndicatorServiceTest
    {
        private ExactArticleAvailabilityIndicatorService_Accessor service;
        private Mock<IExactArticleAvailabilityIndicatorRepository> exactArticleAvailabilityIndicatorRepository;

        private readonly short storageId = 1;
        private readonly int accountOrganizationId = 2;
        private readonly int articleId = 3;

        /// <summary>
        /// Список показателей, якобы возвращенных из БД
        /// </summary>
        private List<ExactArticleAvailabilityIndicator> selectedFromDBIndicators;

        [TestInitialize]
        public void Init()
        {
            exactArticleAvailabilityIndicatorRepository = new Mock<IExactArticleAvailabilityIndicatorRepository>();
            service = new ExactArticleAvailabilityIndicatorService_Accessor(exactArticleAvailabilityIndicatorRepository.Object);

            selectedFromDBIndicators = new List<ExactArticleAvailabilityIndicator>();

            exactArticleAvailabilityIndicatorRepository.Setup(x => x.Query<ExactArticleAvailabilityIndicator>(true, "")
                .PropertyIn(z => z.BatchId, (ISubQuery)null)
                .Where(y => true)
                .ToList<ExactArticleAvailabilityIndicator>()).Returns(selectedFromDBIndicators);

            // мочим сохранение показателя
            exactArticleAvailabilityIndicatorRepository.Setup(x => x.Save(It.IsAny<ExactArticleAvailabilityIndicator>()))
                .Callback<ExactArticleAvailabilityIndicator>(x =>
                {                                        
                    // ищем показатель по Id
                    var existingIndicator = selectedFromDBIndicators.Where(z => z.Id == x.Id).FirstOrDefault();

                    // если показателя нет - добавляем его
                    if (existingIndicator == null)
                    {
                        // accessor создаем для установки защищенного свойства Id                        
                        var accessor = new ExactArticleAvailabilityIndicator_Accessor(new PrivateObject(x));
                        accessor.Id = Guid.NewGuid();                        
                        
                        x = (ExactArticleAvailabilityIndicator)accessor.Target;

                        selectedFromDBIndicators.Add(x);
                    }                    
                });
        }
        
        #region Update
        
        /// <summary>
        /// Показателей по указанным параметрам нет, поэтому должен добавиться один показатель
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_First_Indicator_Must_Be_Added_Correctly()
        {
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(DateTime.Now, storageId, accountOrganizationId, articleId, Guid.NewGuid(), 100, 10));

            service.Update(storageId, accountOrganizationId, null, indicators);

            Assert.AreEqual(1, selectedFromDBIndicators.Count);
            Assert.AreEqual(storageId, selectedFromDBIndicators[0].StorageId);
            Assert.AreEqual(accountOrganizationId, selectedFromDBIndicators[0].AccountOrganizationId);
            Assert.AreEqual(articleId, selectedFromDBIndicators[0].ArticleId);
            Assert.AreEqual(100, selectedFromDBIndicators[0].PurchaseCost);
            Assert.AreEqual(10, selectedFromDBIndicators[0].Count);
        }

        /// <summary>
        /// В БД один показатель с кол-вом 5шт. Добавляем еще один на текущий момент с кол-вом 10шт. Должно стать два показателя с кол-вом 5шт и 15шт соответственно
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_Second_Indicator_Must_Be_Added_Correctly()
        {
            var today = DateTime.Today;
            var yasterday = today.AddDays(-1);
            var batchId = new Guid("11111111-1111-1111-1111-111111111111");
            var id = new Guid("33333333-3333-3333-3333-333333333333");

            // заполняем коллекцию, якобы возвращаемую из БД
            var accessor = new ExactArticleAvailabilityIndicator_Accessor(yasterday, storageId, accountOrganizationId, articleId, batchId, 100M, 5) { Id = id };
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor.Target);

            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(today, storageId, accountOrganizationId, articleId, batchId, 100M, 10));

            service.Update(storageId, accountOrganizationId, null, indicators);

            var first = selectedFromDBIndicators.Where(x => x.Id == id).FirstOrDefault();
            var second = selectedFromDBIndicators.Where(x => x.Id != id).FirstOrDefault();

            Assert.AreEqual(2, selectedFromDBIndicators.Count());
            Assert.AreEqual(today, first.EndDate);

            Assert.AreEqual(accountOrganizationId, second.AccountOrganizationId);
            Assert.AreEqual(articleId, second.ArticleId);
            Assert.AreEqual(batchId, second.BatchId);
            Assert.AreEqual(15, second.Count);
            Assert.IsNull(second.EndDate);
            Assert.AreNotEqual(Guid.Empty, second.Id);
            Assert.AreEqual(first.Id, second.PreviousId);
            Assert.AreEqual(100M, second.PurchaseCost);
            Assert.AreEqual(today, second.StartDate);
            Assert.AreEqual(storageId, second.StorageId);
        }

        /// <summary>
        /// В БД один показатель. Добавляем еще один с той же датой начала. К первому показателю должно прибавиться значение из второго
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_Second_Indicator_With_Same_StartDate_Must_Be_Added_Correctly()
        {
            var today = DateTime.Today;            
            var batchId = new Guid("11111111-1111-1111-1111-111111111111");
            var id = new Guid("33333333-3333-3333-3333-333333333333");

            // заполняем коллекцию, якобы возвращаемую из БД
            var accessor = new ExactArticleAvailabilityIndicator_Accessor(today, storageId, accountOrganizationId, articleId, batchId, 100M, 5) { Id = id };
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor.Target);

            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(today, storageId, accountOrganizationId, articleId, batchId, 100M, 10));

            service.Update(storageId, accountOrganizationId, null, indicators);
            
            var first = selectedFromDBIndicators.Where(x => x.Id == id).FirstOrDefault();

            Assert.AreEqual(1, selectedFromDBIndicators.Count());
            Assert.AreEqual(15, first.Count);
        }
        
        /// <summary>
        /// В БД один показатель с кол-вом 5 шт. Добавляем еще один с датой начала, меньшей даты начала текущего, и с кол-вом 10шт. 
        /// В итоге должно получиться два показателя с кол-вом 10 и 15
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_Second_Indicator_With_Earlier_StartDate_Must_Increase_First_Indicator()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var batchId = Guid.NewGuid();
            var id = new Guid("11111111-1111-1111-1111-111111111111");

            // заполняем коллекцию, якобы возвращаемую из БД
            var accessor = new ExactArticleAvailabilityIndicator_Accessor(today, storageId, accountOrganizationId, articleId, batchId, 100M, 5) { Id = id };
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor.Target);

            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(yesterday, storageId, accountOrganizationId, articleId, batchId, 100M, 10));

            service.Update(storageId, accountOrganizationId, null, indicators);

            var first = selectedFromDBIndicators.Where(x => x.Id == id).FirstOrDefault();
            var _new = selectedFromDBIndicators.Where(x => x.Id != id).FirstOrDefault();

            Assert.AreEqual(2, selectedFromDBIndicators.Count());

            Assert.AreEqual(10, _new.Count);
            Assert.IsNull(_new.PreviousId);
            Assert.AreEqual(yesterday, _new.StartDate);
            Assert.AreEqual(today, _new.EndDate);

            Assert.AreEqual(15, first.Count);
            Assert.AreEqual(_new.Id, first.PreviousId);
        }

        /// <summary>
        /// Дата начала нового показателя совпадает с датой начала одного из существующих показателей
        /// 
        /// В БД 4 показателя:
        /// 1.1 - 2.1   10шт
        /// 2.1 - 3.1   15шт
        /// 3.1 - 4.1   20шт
        /// 4.1 - ---   30шт
        /// 
        /// После умешьшения показателя с 2.1 на 3шт должно получиться
        /// 1.1 - 2.1   10шт
        /// 2.1 - 3.1   12шт
        /// 3.1 - 4.1   17шт
        /// 4.1 - ---   27шт
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_Second_Indicator_Must_Decrease_Following_Indicators_On_3()
        {
            var batchId = Guid.NewGuid();
            
            // заполняем коллекцию, якобы возвращаемую из БД
            var id1 = new Guid("11111111-1111-1111-1111-111111111111");
            var id2 = new Guid("22222222-2222-2222-2222-222222222222");
            var id3 = new Guid("33333333-3333-3333-3333-333333333333");
            var id4 = new Guid("44444444-4444-4444-4444-444444444444");

            var accessor1 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 1), storageId, accountOrganizationId, articleId, batchId, 100M, 10) { Id = id1, EndDate = new DateTime(2011, 1, 2) };
            var accessor2 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 2), storageId, accountOrganizationId, articleId, batchId, 100M, 15) { Id = id2, EndDate = new DateTime(2011, 1, 3), PreviousId = id1 };
            var accessor3 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 3), storageId, accountOrganizationId, articleId, batchId, 100M, 20) { Id = id3, EndDate = new DateTime(2011, 1, 4), PreviousId = id2 };
            var accessor4 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 4), storageId, accountOrganizationId, articleId, batchId, 100M, 30) { Id = id4, PreviousId = id3 };
                        
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor2.Target);
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor3.Target);
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor4.Target);

            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(new DateTime(2011, 1, 2), storageId, accountOrganizationId, articleId, batchId, 100M, -3));

            service.Update(storageId, accountOrganizationId, null, indicators);

            Assert.AreEqual(3, selectedFromDBIndicators.Count);
            Assert.AreEqual(10, accessor1.Count);
            Assert.AreEqual(12, accessor2.Count);
            Assert.AreEqual(17, accessor3.Count);
            Assert.AreEqual(27, accessor4.Count);
        }

        /// <summary>        
        /// Дата начала нового показателя меньше даты начала самого раннего показателя с этими параметрами.
        /// 
        /// В БД 2 показателя:
        /// 10.1 - 11.1   10шт
        /// 11.1 - ----   20шт
        /// 
        /// После увеличения показателя с 5.1 на 3шт должно получиться
        /// 5.1  - 10.1   3шт  
        /// 10.1 - 11.1   13шт
        /// 11.1 - 12.1   23шт
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_New_Indicator_Must_Be_Added_Before_Current_First_Indicator_Without_Previous_And_Increase_On_3()
        {
            var batchId = Guid.NewGuid();

            // заполняем коллекцию, якобы возвращаемую из БД
            var id1 = new Guid("11111111-1111-1111-1111-111111111111");
            var id2 = new Guid("22222222-2222-2222-2222-222222222222");

            var accessor1 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 10), storageId, accountOrganizationId, articleId, batchId, 100M, 10) { Id = id1, EndDate = new DateTime(2011, 1, 11) };
            var accessor2 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 11), storageId, accountOrganizationId, articleId, batchId, 100M, 20) { Id = id2, PreviousId = id1 };

            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor1.Target);
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor2.Target);
            
            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(new DateTime(2011, 1, 5), storageId, accountOrganizationId, articleId, batchId, 100M, 3));

            service.Update(storageId, articleId, null, indicators);

            var first = selectedFromDBIndicators.Where(x => x.Id == id1).FirstOrDefault();
            var second = selectedFromDBIndicators.Where(x => x.Id == id2).FirstOrDefault();
            var _new = selectedFromDBIndicators.Where(x => x.Id != id1 && x.Id != id2).FirstOrDefault();

            Assert.AreEqual(3, selectedFromDBIndicators.Count);

            Assert.AreEqual(3, _new.Count);
            Assert.AreEqual(new DateTime(2011, 1, 5), _new.StartDate);
            Assert.AreEqual(new DateTime(2011, 1, 10), _new.EndDate);

            Assert.AreEqual(13, first.Count);
            Assert.AreEqual(_new.Id, first.PreviousId);
            Assert.AreEqual(new DateTime(2011, 1, 10), first.StartDate);
            Assert.AreEqual(new DateTime(2011, 1, 11), first.EndDate);

            Assert.AreEqual(23, second.Count);            
        }

        /// <summary>        
        /// Дата начала нового показателя больше даты начала самого раннего показателя с этими параметрами. 
        /// 
        /// В БД 3 показателя:
        /// 1.1  - 5.1    10шт
        /// 5.1  - 10.1   20шт
        /// 10.1 - ----   30шт
        /// 
        /// После уменьшения показателя с 3.1 на 5шт должно получиться
        /// 1.1  - 3.1    10шт
        /// 3.1  - 5.1    5шт
        /// 5.1  - 10.1   15шт
        /// 10.1 - ----   25шт
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_New_Indicator_Must_Devide_First_Indicator_And_Decrease_SecondPart_On_5()
        {
            var batchId = Guid.NewGuid();
 
            // заполняем коллекцию, якобы возвращаемую из БД
            var id1 = new Guid("11111111-1111-1111-1111-111111111111");
            var id2 = new Guid("22222222-2222-2222-2222-222222222222");
            var id3 = new Guid("33333333-3333-3333-3333-333333333333");

            var accessor1 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 1), storageId, accountOrganizationId, articleId, batchId, 100M, 10) { Id = id1, EndDate = new DateTime(2011, 1, 5) };
            var accessor2 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 5), storageId, accountOrganizationId, articleId, batchId, 100M, 20) { Id = id2, PreviousId = id1, EndDate = new DateTime(2011, 1, 10) };
            var accessor3 = new ExactArticleAvailabilityIndicator_Accessor(new DateTime(2011, 1, 10), storageId, accountOrganizationId, articleId, batchId, 100M, 30) { Id = id3, PreviousId = id2 };

            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor1.Target);
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor2.Target);
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor3.Target);

            // создаем новый показатель
            var indicators = new List<ExactArticleAvailabilityIndicator>();
            indicators.Add(new ExactArticleAvailabilityIndicator(new DateTime(2011, 1, 3), storageId, accountOrganizationId, articleId, batchId, 100M, -5));

            service.Update(storageId, accountOrganizationId, null, indicators);

            var first = selectedFromDBIndicators.Where(x => x.Id == id1).FirstOrDefault();
            var second = selectedFromDBIndicators.Where(x => x.Id == id2).FirstOrDefault();
            var third = selectedFromDBIndicators.Where(x => x.Id == id3).FirstOrDefault();
            var _new = selectedFromDBIndicators.Where(x => x.PreviousId == id1).FirstOrDefault();

            Assert.AreEqual(4, selectedFromDBIndicators.Count);

            Assert.AreEqual(10, first.Count);
            Assert.IsNull(first.PreviousId);
            Assert.AreEqual(new DateTime(2011, 1, 1), first.StartDate);
            Assert.AreEqual(new DateTime(2011, 1, 3), first.EndDate);

            Assert.AreEqual(5, _new.Count);
            Assert.AreEqual(id1, _new.PreviousId);
            Assert.AreEqual(new DateTime(2011, 1, 3), _new.StartDate);
            Assert.AreEqual(new DateTime(2011, 1, 5), _new.EndDate);

            Assert.AreEqual(15, second.Count);
            Assert.AreEqual(_new.Id, second.PreviousId);
            Assert.AreEqual(new DateTime(2011, 1, 5), second.StartDate);
            Assert.AreEqual(new DateTime(2011, 1, 10), second.EndDate);                       

            Assert.AreEqual(25, third.Count);
            Assert.AreEqual(id2, third.PreviousId);
            Assert.AreEqual(new DateTime(2011, 1, 10), third.StartDate);
            Assert.IsNull(third.EndDate);
        }

        #endregion

        #region GetFrom
        
        /// <summary>
        /// Должен возвратить пустой список показателей
        /// </summary>
        [TestMethod]
        public void ArticleAvailabilityIndicatorServiceTest_Must_Return_Empty_List()
        {
            var result = service.GetFrom(storageId, accountOrganizationId, null, DateTime.Now, new List<ExactArticleAvailabilityIndicator>());

            Assert.AreEqual(0, result.Count());
        }

        /// <summary>
        /// Должен возвратить только один имеющийся показатель
        /// </summary>
        [TestMethod]        
        public void ArticleAvailabilityIndicatorServiceTest_Must_Return_One_Single_Row()
        {
            var batchId = new Guid("11111111-1111-1111-1111-111111111111");

            var accessor = new ExactArticleAvailabilityIndicator_Accessor(DateTime.Today.AddDays(-1), storageId, accountOrganizationId, articleId, batchId, 100M, 5) { Id = Guid.NewGuid() };
            selectedFromDBIndicators.Add((ExactArticleAvailabilityIndicator)accessor.Target);

            var result = service.GetFrom(storageId, accountOrganizationId, null, DateTime.Now, new List<ExactArticleAvailabilityIndicator>());

            Assert.AreEqual(1, result.Count());
        }
        
        #endregion
    }
}
