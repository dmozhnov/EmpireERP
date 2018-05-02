using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Базовый класс для сервисов накладных
    /// </summary>
    /// <typeparam name="T">Тип накладной</typeparam>
    public abstract class BaseWaybillService<T> : BaseApplicationService<T>, IBaseWaybillService<T> where T : class
    {
        /// <summary>
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id">Код наколадной</param>
        /// <returns></returns>
        public abstract T CheckWaybillExistence(Guid id, User user);

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="writeoffWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        public abstract void CheckPossibilityToViewDetailsByUser(T waybill, User curator);

        #region Смена куратора

        public bool IsPossibilityToChangeCurator(T waybill, User user)
        {
            return IsPossibilityToPerformOperation(CheckPossibilityToChangeCurator, waybill, user);
        }

        public abstract void CheckPossibilityToChangeCurator(T waybill, User user);

        #endregion
    }
}
