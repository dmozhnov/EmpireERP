using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IBaseWaybillService<T> where T : class
    {
        /// <summary>
        /// Получение накладной по id с проверкой ее существования
        /// </summary>
        /// <param name="id">Код наколадной</param>
        /// <returns></returns>
        T CheckWaybillExistence(Guid id, User user);

        /// <summary>
        /// Проверка, видна ли накладная куратору
        /// </summary>
        /// <param name="writeoffWaybill">Накладная</param>
        /// <param name="curator">Куратор</param>
        void CheckPossibilityToViewDetailsByUser(T waybill, User curator);

        #region Смена куратора

        bool IsPossibilityToChangeCurator(T waybill, User user);
        void CheckPossibilityToChangeCurator(T waybill, User user);

        #endregion
    }
}
