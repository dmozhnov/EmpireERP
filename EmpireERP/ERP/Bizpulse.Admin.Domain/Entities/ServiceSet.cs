using System;
using System.Collections.Generic;
using System.Linq;
using Bizpulse.Admin.Domain.ValueObjects;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using Iesi.Collections.Generic;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Набор услуг клиента
    /// </summary>
    public class ServiceSet : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Клиент системы
        /// </summary>
        public virtual Client Client { get; protected internal set; }
        
        /// <summary>
        /// Конфигурация
        /// </summary>
        public virtual ServiceSetConfiguration Configuration { get; protected set; }

        /// <summary>
        /// Стоимость набора (с учетом скидки)
        /// </summary>
        public virtual decimal Cost { get; protected set; }

        /// <summary>
        /// Срок услуг в наборе (кол-во месяцев)
        /// </summary>
        public virtual byte MonthCount { get; protected set; }

        /// <summary>
        /// Базовая стоимость одной услуги в наборе
        /// </summary>
        public virtual decimal BaseServiceCost { get; protected set; }

        /// <summary>
        /// Реальная стоимость одной услуги в наборе
        /// </summary>
        public virtual decimal FactualServiceCost { get; protected set; }

        /// <summary>
        /// Базовая стоимость одного дня предоставления услуг из набора
        /// </summary>
        public virtual decimal? BaseCostPerDay { get; protected set; }

        /// <summary>
        /// Cтоимость одного дня предоставления услуг в наборе с учетом скидки
        /// </summary>
        public virtual decimal? CostPerDayWithDiscount { get; protected set; }

        /// <summary>
        /// Дата активации
        /// </summary>
        public virtual DateTime? ActivationDate { get; protected internal set; }

        /// <summary>
        /// Активирован ли набор
        /// </summary>
        public virtual bool IsActivated
        {
            get { return ActivationDate != null; }
        }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime? StartDate { get; protected internal set; }

        /// <summary>
        /// Дата окончания действия (закрытия)
        /// </summary>
        public virtual DateTime? EndDate { get; protected internal set; }

        /// <summary>
        /// Изначальное кол-во дней в наборе
        /// </summary>
        public virtual int? InitialDayCount { get; protected internal set; }
        
        /// <summary>
        /// Является ли набор набор текущим
        /// </summary>
        public virtual bool IsCurrent
        {
            get 
            {
                return IsActivated && StartDate <= DateTime.Now && EndDate > DateTime.Now;
            }
        }

        /// <summary>
        /// Услуги набора
        /// </summary>
        public virtual IEnumerable<Service> Services
        {
            get { return new ImmutableSet<Service>(services); }
        }
        private Iesi.Collections.Generic.ISet<Service> services = new HashedSet<Service>();

        /// <summary>
        /// Текущее количество услуг в наборе
        /// </summary>
        public virtual int ServiceCount
        {
            get { return Services.Count(); }
        }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate { get; protected internal set; }

        #endregion

        #region Конструкторы

        protected ServiceSet() {}

        public ServiceSet(ServiceSetConfiguration configuration, byte monthCount, decimal baseServiceCost, decimal factualServiceCost, DateTime currentDateTime)
        {
            ValidationUtils.NotNull(configuration, "Не указана конфигурация набора услуг.");
            ValidationUtils.Assert(monthCount > 0, "Количество месяцев должно быть больше 0.");
            
            CreationDate = currentDateTime;
            
            Configuration = configuration;
            MonthCount = monthCount;
            BaseServiceCost = baseServiceCost;
            FactualServiceCost = factualServiceCost;
            Cost = Math.Round(factualServiceCost * MonthCount, 2);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Активация набора
        /// </summary>
        /// <param name="currentDate">Текущая дата</param>
        public virtual void Activate(DateTime currentDate)
        {
            currentDate = currentDate.SetHoursMinutesAndSeconds(0, 0, 0);
            
            ValidationUtils.Assert(Client.PrepaymentSum >= Cost, "Недостаточно средств для активации набора услуг.");
            ValidationUtils.Assert(!IsActivated, "Набор уже активирован.");

            // находим текущий набор услуг
            var currentServiceSet = Client.ServiceSets.Where(x => x.IsCurrent).FirstOrDefault();

            // если есть текущий активированный набор
            if (currentServiceSet != null)
            {
                // выставляет дату, следующую после даты завершения текущего активированного набора, у которой время выставлено 23:59:59
                StartDate = currentServiceSet.EndDate.Value.AddSeconds(1);
            }
            else
            {
                // выставляем текущую дату
                StartDate = currentDate;
            }

            // прибавляем необходимое кол-во месяцев
            EndDate = StartDate.Value.AddMonths(MonthCount).AddDays(-1).SetHoursMinutesAndSeconds(23, 59, 59);

            // вычисляем изначальное кол-во дней в наборе (при этом добавляем 1 секунду, чтобы получилась уже следующая дата)
            InitialDayCount = EndDate.Value.AddSeconds(1).Subtract(StartDate.Value).Days;

            // вычисляем базовую стоимость одного дня предоставления услуг из набора
            BaseCostPerDay = Math.Round(BaseServiceCost * MonthCount / InitialDayCount.Value);

            // вычисляем стоимость одного дня предоставления услуг в наборе с учетом скидки
            CostPerDayWithDiscount = Math.Round(FactualServiceCost * MonthCount / InitialDayCount.Value);

            // списываем деньги со счета клиента
            Client.PrepaymentSum -= Cost;
            
            // выставляем дату активации набора
            ActivationDate = currentDate;
        }

        /// <summary>
        /// Добавление новой услуги в набор
        /// </summary>
        /// <param name="service"></param>
        public virtual void AddNewService(DateTime currentDateTime)
        {
            ValidationUtils.Assert(IsCurrent, "Невозможно добавить услугу не в текущий набор.");
            ValidationUtils.Assert(!Services.Any(x => x.IsCurrent), "Невозможно добавить услугу в набор, в котором уже есть активная услуга.");

            var service = new Service(currentDateTime);

            services.Add(service);
            service.ServiceSet = this;
        }

        /// <summary>
        /// Закрытие активной услуги набора
        /// </summary>
        public virtual void CloseActiveService()
        {

        }


        #endregion
    }
}
