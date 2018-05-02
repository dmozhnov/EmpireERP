using ERP.Infrastructure.Entities;
using Iesi.Collections.Generic;
using System.Collections.Generic;

namespace Bizpulse.Admin.Domain.Entities
{
    /// <summary>
    /// Тарифный план на услугу Bizpulse
    /// </summary>
    public class Rate : Entity<short>
    {
        #region Свойства

        /// <summary>
        /// Название
        /// </summary>
        /// <remarks>не более 100 символов</remarks>
        public virtual string Name { get; protected set; }

        #region Состав
        
        /// <summary>
        /// Количество активных пользователей, включенных по умолчанию в тарифный план
        /// </summary>
        public virtual short ActiveUserCountLimit { get; protected set; }

        /// <summary>
        /// Количество команд, включенных по умолчанию в тарифный план
        /// </summary>
        public virtual short TeamCountLimit { get; protected set; }

        /// <summary>
        /// Количество мест хранения, включенных по умолчанию в тарифный план
        /// </summary>
        public virtual short StorageCountLimit { get; protected set; }

        /// <summary>
        /// Количество организаций аккаунта, включенных по умолчанию в тарифный план
        /// </summary>
        public virtual short AccountOrganizationCountLimit { get; protected set; }

        /// <summary>
        /// Количество гигабайт данных, включенных по умолчанию в тарифный план
        /// </summary>
        public virtual short GigabyteCountLimit { get; protected set; } 

        #endregion

        #region Используемые опции

        /// <summary>
        /// Использовать ли опцию «Дополнительный пользователь»
        /// </summary>
        public virtual bool UseExtraActiveUserOption { get; protected set; }

        /// <summary>
        /// Использовать ли опцию «Дополнительная команда»
        /// </summary>
        public virtual bool UseExtraTeamOption { get; protected set; }

        /// <summary>
        /// Использовать ли опцию «Дополнительное мест хранения»
        /// </summary>
        public virtual bool UseExtraStorageOption { get; protected set; }

        /// <summary>
        /// Использовать ли опцию «Дополнительная организация аккаунта»
        /// </summary>
        public virtual bool UseExtraAccountOrganizationOption { get; protected set; }

        /// <summary>
        /// Использовать ли опцию «Дополнительный гигабайт данных»
        /// </summary>
        public virtual bool UseExtraGigabyteOption { get; protected set; }

        #endregion

        #region Стоимости опций

        /// <summary>
        /// Стоимость опции «Дополнительный пользователь»
        /// </summary>
        public virtual decimal ExtraActiveUserOptionCostPerMonth { get; protected set; }

        /// <summary>
        /// Стоимость опции «Дополнительная команда»
        /// </summary>
        public virtual decimal ExtraTeamOptionCostPerMonth { get; protected set; }

        /// <summary>
        /// Стоимость опции «Дополнительное мест хранения»
        /// </summary>
        public virtual decimal ExtraStorageOptionCostPerMonth { get; protected set; }

        /// <summary>
        /// Стоимость опции «Дополнительная организация аккаунта»
        /// </summary>
        public virtual decimal ExtraAccountOrganizationOptionCostPerMonth { get; protected set; }

        /// <summary>
        /// Стоимость опции «Дополнительный гигабайт данных»
        /// </summary>
        public virtual decimal ExtraGigabyteOptionCostPerMonth { get; protected set; }

        #endregion

        /// <summary>
        /// Базовая стоимость услуг по тарифному плану в месяц
        /// </summary>
        public virtual decimal BaseCostPerMonth { get; protected set; }
        
        #endregion

        #region Конструкторы

        protected Rate() {}

        #endregion

        #region Методы

        #endregion
    }
}
