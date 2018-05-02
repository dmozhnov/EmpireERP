using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.UnitOfWork;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Utils;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public abstract class BaseReportPresenter
    {
        #region Поля

        protected readonly IUnitOfWorkFactory unitOfWorkFactory;
        protected readonly IUserService userService;

        #endregion

        #region Конструкторы

        protected BaseReportPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получить параметры периода построения отчета
        /// </summary>
        /// <param name="settings">Модель настроек</param>
        /// <param name="currentDate">текущая дата</param>
        /// <param name="startDate">начальная дата периода</param>
        /// <param name="endDate">конечная дата периода</param>
        protected void ParseDatePeriod(string startDateString, string endDateString, DateTime currentDate, out DateTime startDate, out DateTime endDate)
        {
            startDate = ValidationUtils.TryGetDate(startDateString);
            endDate = ValidationUtils.TryGetDate(endDateString);

            ValidationUtils.Assert(startDate <= endDate, "Дата окончания периода не может быть меньше даты начала периода.");
            ValidationUtils.Assert(endDate <= currentDate.Date, "Дата окончания периода должна быть меньше или равна текущей дате.");

            // Устанавливаем последнюю секунду указанной даты
            endDate = endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        
        #endregion
    }
}
