using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Базовый класс для служб уровня приложения
    /// </summary>
    public abstract class BaseApplicationService<T> where T: class
    {
        /// <summary>
        /// Проверка возможности выполнения операции
        /// </summary>        
        protected bool IsPossibilityToPerformOperation(Action<T, User> action, T entity, User user)
        {
            try
            {
                action(entity, user);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
