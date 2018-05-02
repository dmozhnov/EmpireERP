using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ERP.Wholesale.ApplicationServices.Test
{
    /// <summary>
    /// Тест сервиса  курсов валюты
    /// </summary>
    [TestClass]
    public class CurrencyRateServiceTest
    {
        #region Поля

        private CurrencyRateService currencyRateService;
        private Mock<ICurrencyRateRepository> currencyRateRepository;
        private Currency currency;
        private List<CurrencyRate> rateList;
        private List<CurrencyRate> createdRateList;
        private User user;
        
        #endregion

        #region Инициализация тестов

        [TestInitialize]
        public void Init()
        {
            rateList = new List<CurrencyRate>();    // Список курсов для валюты
            createdRateList = new List<CurrencyRate>(); //Список созданных курсов валюты

            // инициализация валюты
            var mockCurrency = new Mock<Currency>();
            mockCurrency.Setup(x => x.Id).Returns(1);
            mockCurrency.Setup(x => x.AddRate(It.IsAny<CurrencyRate>()))
                .Callback<CurrencyRate>(rate => rateList.Add(rate));
            mockCurrency.Setup(x => x.RemoveRate(It.IsAny<CurrencyRate>()))
                .Callback<CurrencyRate>(rate => rateList.Remove(rate));
            currency = mockCurrency.Object;

            // Инициализаия репозитория курсов валюты
            currencyRateRepository = new Mock<ICurrencyRateRepository>();

            currencyRateRepository.Setup(x => x.GetRatesOnDate(It.IsAny<short>(), It.IsAny<DateTime>()))
                .Returns<short, DateTime>((currencyId, date) =>
                {
                    if (currencyId != 1) { return new List<CurrencyRate>(); }

                    return rateList.Where(x => x.StartDate <= date && (x.EndDate >= date || x.EndDate == null));
                });

            currencyRateRepository.Setup(x => x.GetNextRateByDate(It.IsAny<short>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns<short, DateTime, DateTime>((currencyId, date, creationDate) =>
                {
                    return rateList.Where(x => x.Currency.Id == currencyId && (x.StartDate > date || (x.StartDate == date && x.CreationDate > creationDate)))
                        .OrderBy(x => x.StartDate)
                        .ThenBy(x => x.CreationDate)
                        .FirstOrDefault();
                });

            currencyRateRepository.Setup(x => x.CheckCurrencyRateUsing(It.IsAny<int>())).Returns(false);

            currencyRateRepository.Setup(x => x.GetPreviouseRate(It.IsAny<int>()))
                .Returns<int>(currencyRateId =>
                {
                    return rateList.Where(x => x.Id == currencyRateId)
                        .Select(x => x.PreviousCurrencyRate)
                        .FirstOrDefault();
                });

            currencyRateRepository.Setup(x => x.GetNextRate(It.IsAny<int>()))
               .Returns<int>(currencyRateId =>
               {
                   return rateList.Where(x => x.PreviousCurrencyRate != null && x.PreviousCurrencyRate.Id == currencyRateId)
                        .FirstOrDefault();
               });

            currencyRateService = new CurrencyRateService(currencyRateRepository.Object);

            var mockUser = new Mock<User>();
            mockUser.Setup(x => x.HasPermission(It.IsAny<Permission>())).Returns(true);
            
            user = mockUser.Object;
        }

        /// <summary>
        /// Создание курса валюты
        /// </summary>
        /// <param name="rateValue">Значение курса</param>
        /// <param name="startDate">Дата начала действия</param>
        /// <param name="creationDate">Дата создания курса</param>
        /// <returns>Курс валюты</returns>
        private CurrencyRate CreateRate(decimal rateValue, DateTime startDate, DateTime creationDate)
        {
            var rate = new CurrencyRate(currency, currency, rateValue, startDate, creationDate) 
                { 
                    Id = createdRateList.Count() + rateList.Count() 
                };
            createdRateList.Add(rate);

            return rate;
        }

        /// <summary>
        /// Добавление курса в валюту
        /// </summary>
        /// <param name="currency">Валюта</param>
        /// <param name="creationDate">Дата создания курса</param>
        /// <param name="startDate">Дата начала действия курса</param>
        /// <param name="endDate">Дата завершения действия курса</param>
        /// <param name="rate">Значение курса валюты</param>
        private void AddRateToCurrency(Currency currency, DateTime creationDate, DateTime startDate, DateTime? endDate, decimal rate)
        {
            var last = rateList.LastOrDefault();
            if (last != null && startDate < last.StartDate)
                throw new Exception("Error!");

            rateList.Add(new CurrencyRate(currency, currency, rate, startDate, creationDate)
            {
                EndDate = endDate,
                Id = createdRateList.Count() + rateList.Count(),
                PreviousCurrencyRate = rateList.LastOrDefault()
            });
        }

        #endregion

        #region Методы

        #region Добавление курса валюты

        /// <summary>
        /// Добавление первого курса для валюты должно пройти успешно
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_First_Rate_Must_Be_Success()
        {
            var date = DateTime.Now;
            var rate = CreateRate(42, date, date);

            currencyRateService.AddRate(currency, rate, user);

            Assert.AreEqual(1, rateList.Count());
            Assert.AreEqual(date, rateList[0].StartDate);
            Assert.AreEqual(null, rateList[0].EndDate);
        }

        /// <summary>
        /// Добавление курса с датой начала, совпадающей с датой начала имеющегося у валюты курса.
        /// При этом имеющийся должен закрыться от даты начала добавляемого, а новый добавиться действующим.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_Rate_With_The_Same_StartDate_Then_Existing_Rate_Must_Be_Success()
        {
            #region Assing

            var date = DateTime.Now;
            var rate = CreateRate(42, date, date);

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date, null, 42);

            #endregion

            // Act
            currencyRateService.AddRate(currency, rate, user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            // Проверяем изменения в имеющемся курсе
            Assert.AreEqual(date, rateList[0].StartDate);
            Assert.AreEqual(date, rateList[0].EndDate);
            // Проверяем добавленный курс
            Assert.AreEqual(date, rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);

            #endregion
        }

        /// <summary>
        /// Добавление курса с датой начала, совпадающей с датой начала имеющегося у валюты курса. При этом:
        /// 1) Первый курс действует 2 дня
        /// 2) Второй курс действует 0 дней (дата начала и окончания совпадает)
        /// 3) Третий курс открыт
        /// При этом третий курс должен закрыться от даты начала добавляемого, а новый добавиться действующим.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_Rate_With_The_Same_StartDate_Then_Existing_Rate_Must_Be_Success_2()
        {
            #region Assing

            var date = DateTime.Now;
            var rate = CreateRate(42, date, date.AddHours(2));

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date.AddDays(-2), date.AddDays(-2), date, 4);
            AddRateToCurrency(currency, date, date, date, 33);
            AddRateToCurrency(currency, date.AddHours(1), date, null, 2);

            #endregion

            // Act
            currencyRateService.AddRate(currency, rate, user);

            #region Assert

            Assert.AreEqual(4, rateList.Count());
            // Проверяем изменения в имеющемся курсе
            Assert.AreEqual(date.AddDays(-2), rateList[0].StartDate);
            Assert.AreEqual(date, rateList[0].EndDate);

            Assert.AreEqual(date, rateList[1].StartDate);
            Assert.AreEqual(date, rateList[1].EndDate);

            Assert.AreEqual(date, rateList[2].StartDate);
            Assert.AreEqual(date, rateList[2].EndDate);
            // Проверяем добавленный курс
            Assert.AreEqual(date, rateList[3].StartDate);
            Assert.AreEqual(null, rateList[3].EndDate);

            Assert.AreEqual(rateList[2], rate.PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Добавление курса с датой начала, находящейся до начала действия единственного курса валюты.
        /// При этом имеющийся должен остаться без изменений, а новый добавиться с датой закрытия равной дате начала действия имеющегося.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_Rate_Before_Existing_Rate_Must_Be_Success()
        {
            #region Assing

            var date = DateTime.Now;
            var rate = CreateRate(42, date, date);

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            currencyRateService.AddRate(currency, rate, user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            // Проверяем изменения в имеющемся курсе
            Assert.AreEqual(date.AddDays(1), rateList[0].StartDate);
            Assert.AreEqual(null, rateList[0].EndDate);
            // Проверяем добавленный курс
            Assert.AreEqual(date, rateList[1].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[1].EndDate);
            
            Assert.AreEqual(null, rate.PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Добавление курса с датой начала, находящейся по середине действия единственного курса валюты.
        /// При этом имеющийся должен закрыться от даты начала добавляемого, а новый добавиться действующим.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_Rate_In_Existing_Rate_Must_Be_Success()
        {
            #region Assing

            var date = DateTime.Now;
            var rate = CreateRate(42, date, date);

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), null, 42);

            #endregion

            // Act
            currencyRateService.AddRate(currency, rate, user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            // Проверяем изменения в имеющемся курсе
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date, rateList[0].EndDate);
            // Проверяем добавленный курс
            Assert.AreEqual(date, rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);
            Assert.AreEqual(rateList[0], rate.PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Добавление курса с датой начала, находящейся по середине действия первого курса валюты.
        /// При этом первый курс должен закрыться от даты начала добавляемого, второй остаться без изменений. 
        /// Новый должен добавиться с датой завершения, равной дате завершения первого курса.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_AddRate_Add_Rate_In_Existing_Rates_Must_Be_Success()
        {
            #region Assing

            var date = DateTime.Now;
            var rate = CreateRate(42, date, date);

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            currencyRateService.AddRate(currency, rate, user);

            #region Assert

            Assert.AreEqual(3, rateList.Count());
            // Проверяем изменения в имеющемся курсе
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date, rateList[0].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);
            // Проверяем добавленный курс
            Assert.AreEqual(date, rateList[2].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[2].EndDate);
            Assert.AreEqual(rateList[0], rate.PreviousCurrencyRate);

            #endregion
        }

        #endregion

        #region Удаление курса валюты

        /// <summary>
        /// Удаление единственного курса валюты.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_DeleteRate_Delete_First_Rate()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date, null, 42);

            #endregion

            // Act
            currencyRateService.DeleteRate(rateList[0], user);

            #region Assert

            Assert.AreEqual(0, rateList.Count());

            #endregion
        }

        /// <summary>
        /// Удаление первого курса валюты. Второй курс должен остаться без изменений.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_DeleteRate_Delete_First_Rate_Second_Not_Change()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            currencyRateService.DeleteRate(rateList[0], user);

            #region Assert

            Assert.AreEqual(1, rateList.Count());
            Assert.AreEqual(date.AddDays(1), rateList[0].StartDate);
            Assert.AreEqual(null, rateList[0].EndDate);
            Assert.AreEqual(null, rateList[0].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Удаление второго курса валюты. Первый курс должен стать действующим.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_DeleteRate_Delete_Second_Rate_First_Must_Be_Set_Active()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            currencyRateService.DeleteRate(rateList[1], user);

            #region Assert

            Assert.AreEqual(1, rateList.Count());
            Assert.AreEqual(date, rateList[0].StartDate);
            Assert.AreEqual(null, rateList[0].EndDate);

            #endregion
        }

        /// <summary>
        /// Удаление среднего из трех имеющихся курсов при этом первый должен начать действовать до начала третьего курса.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_DeleteRate_Delete_Middle_Rate_From_Three_Existing_Rates()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date, 42);
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            currencyRateService.DeleteRate(rateList[1], user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[0].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Три курса валюты имеют одинаковые даты начала и конца. При этом дата начала совпадает с датой конца.
        /// Удаляется средний из курсов. В результате оставшиеся курсы не должны измениться.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_DeleteRate_Delete_Middle_Rate_From_Three_Equals_Rates()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date.AddHours(-1), date, date, 42);
            AddRateToCurrency(currency, date, date, date, 42);
            AddRateToCurrency(currency, date.AddHours(1), date, date, 42);

            #endregion

            // Act
            currencyRateService.DeleteRate(rateList[1], user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            Assert.AreEqual(date, rateList[0].StartDate);
            Assert.AreEqual(date, rateList[0].EndDate);
            Assert.AreEqual(date, rateList[1].StartDate);
            Assert.AreEqual(date, rateList[1].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        #endregion

        #region Редактирование курса

        /// <summary>
        /// Начало курса переносится на один час раньше. При этом он не достигает даты начала предыдущего курса.
        /// Т. о. должна измениться только начальная дата редактируемого курса и конечная дата предыдущего курса.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_StartDate_Must_Be_Decrease()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date, 42);
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), date.AddDays(2), 42);

            #endregion

            // Act
            var rate = rateList[1];
            currencyRateService.EditRate(rate, date.AddHours(-1), 42, user);

            #region Assert

            Assert.AreEqual(3, rateList.Count());
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddHours(-1), rateList[0].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[2].StartDate);
            Assert.AreEqual(date.AddDays(2), rateList[2].EndDate);
            Assert.AreEqual(date.AddHours(-1), rateList[1].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[1].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Начало курса переносится на один час позже. При этом он не достигает даты окончания курса.
        /// Т. о. должна измениться только начальная дата редактируемого курса и конечная дата предыдущего курса.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_StartDate_Must_Be_Increase()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date, 42);
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), date.AddDays(2), 42);

            #endregion

            // Act
            var rate = rateList[1];
            currencyRateService.EditRate(rate, date.AddHours(1), 42, user);

            #region Assert

            Assert.AreEqual(3, rateList.Count());
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddHours(1), rateList[0].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[2].StartDate);
            Assert.AreEqual(date.AddDays(2), rateList[2].EndDate);
            Assert.AreEqual(date.AddHours(1), rateList[1].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[1].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Начало второго курса переносится на два дня раньше. При этом он  достигает даты начала действия первого курса.
        /// Т. о. редактируемый курс должен перейти на место перед первым курсом. А первый курс должен продлить свое действие до третьего.
        /// Третий курс не изменяется.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_StartDate_Must_Be_Decrease_And_Rate_Must_Be_Moved_Before_Previouse_Rate()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date, 42);
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), date.AddDays(2), 42);

            #endregion

            // Act
            var rate = rateList[1];
            currencyRateService.EditRate(rate, date.AddDays(-2), 42, user);

            #region Assert

            Assert.AreEqual(3, rateList.Count());
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[0].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[2].StartDate);
            Assert.AreEqual(date.AddDays(2), rateList[2].EndDate);
            Assert.AreEqual(date.AddDays(-2), rateList[1].StartDate);
            Assert.AreEqual(date.AddDays(-1), rateList[1].EndDate);
            Assert.AreEqual(rateList[1], rateList[0].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Начало второго курса переносится на два дня позже. При этом он достигает даты начала действия следующего курса.
        /// Т. о. редактируемый курс должен перейти на место после третьего курса. Первый курс должен продлить свое действие до третьего.
        /// Третий курс завершит свое действие в момент начала второго.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_StartDate_Must_Be_Increase_And_Rate_Must_Be_Moved_After_Next_Rate()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date, date.AddDays(-1), date, 42);
            AddRateToCurrency(currency, date, date, date.AddDays(1), 42);
            AddRateToCurrency(currency, date, date.AddDays(1), null, 42);

            #endregion

            // Act
            var rate = rateList[1];
            currencyRateService.EditRate(rate, date.AddDays(2), 42, user);

            #region Assert

            Assert.AreEqual(3, rateList.Count());
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddDays(1), rateList[0].EndDate);
            
            Assert.AreEqual(date.AddDays(1), rateList[2].StartDate);
            Assert.AreEqual(date.AddDays(2), rateList[2].EndDate);
            
            Assert.AreEqual(date.AddDays(2), rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);

            Assert.AreEqual(rateList[2], rateList[1].PreviousCurrencyRate);
            #endregion
        }

        /// <summary>
        /// Начальная дата для первого курса увеличивается на один день. При этом она не достигает начала второго курса.
        /// Поэтому второй курс остается без изменений.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_For_First_Rate_StartDate_Must_Be_Increase()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date.AddHours(-1), date, date.AddDays(10), 42);
            AddRateToCurrency(currency, date, date.AddDays(10), null, 42);

            #endregion

            // Act
            var rate = rateList[0];
            currencyRateService.EditRate(rate, date.AddDays(1), 42, user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            Assert.AreEqual(date.AddDays(10), rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);
            Assert.AreEqual(date.AddDays(1), rateList[0].StartDate);
            Assert.AreEqual(date.AddDays(10), rateList[0].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        /// <summary>
        /// Начальная дата для первого курса уменьшается на один день. Второй курс остается без изменений.
        /// </summary>
        [TestMethod]
        public void CurrencyRateService_EditRate_For_First_Rate_StartDate_Must_Be_Decrease()
        {
            #region Assing

            var date = DateTime.Now;

            // Добавляем курсы в валюту
            AddRateToCurrency(currency, date.AddHours(1), date, date.AddDays(10), 42);
            AddRateToCurrency(currency, date, date.AddDays(10), null, 42);

            #endregion

            // Act
            var rate = rateList[0];
            currencyRateService.EditRate(rate, date.AddDays(-1), 42, user);

            #region Assert

            Assert.AreEqual(2, rateList.Count());
            Assert.AreEqual(date.AddDays(10), rateList[1].StartDate);
            Assert.AreEqual(null, rateList[1].EndDate);
            Assert.AreEqual(date.AddDays(-1), rateList[0].StartDate);
            Assert.AreEqual(date.AddDays(10), rateList[0].EndDate);
            Assert.AreEqual(rateList[0], rateList[1].PreviousCurrencyRate);

            #endregion
        }

        #endregion

        #endregion
    }
}
