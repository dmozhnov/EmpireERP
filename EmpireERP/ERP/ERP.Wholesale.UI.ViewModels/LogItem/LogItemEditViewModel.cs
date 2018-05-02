using System;

namespace ERP.Wholesale.UI.ViewModels.LogItem
{
    public class LogItemEditViewModel
    {
        /// <summary>
        /// Время наступления события
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Код пользователя, с которым связано событие
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Адрес, при обращении по которому произошло событие
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Сообщение для пользователя
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Системное сообщение
        /// </summary>
        public string SystemMessage { get; set; }
    }
}
