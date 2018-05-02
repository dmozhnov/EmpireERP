using System;
using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Элемент журнала событий
    /// </summary> 
    public class LogItem : Entity<long>
    {
        #region Свойства

        /// <summary>
        /// Время наступления события
        /// </summary>
        public virtual DateTime Time { get; protected set; }

        /// <summary>
        /// Код пользователя, с которым связано событие
        /// </summary>
        public virtual int? UserId { get; protected set; }

        /// <summary>
        /// Адрес, при обращении по которому произошло событие
        /// </summary>
        public virtual string Url { get; protected set; }

        /// <summary>
        /// Сообщение для пользователя
        /// </summary>
        public virtual string UserMessage { get; protected set; }

        /// <summary>
        /// Системное сообщение
        /// </summary>
        public virtual string SystemMessage { get; protected set; }

        #endregion

        #region Конструкторы

        protected LogItem() { }

        public LogItem(DateTime time, int? userId, string url, string userMessage, string systemMessage)
        {
            Time = time;
            UserId = userId;
            Url = url;
            UserMessage = userMessage;
            SystemMessage = systemMessage;
        }

        #endregion

    }
}
