using Bizpulse.Admin.Domain.Entities;
using ERP.Utils;

namespace Bizpulse.Admin.Domain.ValueObjects
{
    /// <summary>
    /// Конфигурация набора услуг клиента 
    /// </summary>
    public class ServiceSetConfiguration
    {
        #region Свойства

        /// <summary>
        /// Тарифный план на услугу Bizpulse
        /// </summary>
        public virtual Rate Rate { get; protected set; }

        /// <summary>
        /// Дополнительное кол-во пользователей
        /// </summary>
        public virtual short ExtraActiveUserCount { get; protected set; }

        /// <summary>
        /// Дополнительное кол-во команд
        /// </summary>
        public virtual short ExtraTeamCount { get; protected set; }

        /// <summary>
        /// Дополнительное кол-во мест хранения
        /// </summary>
        public virtual short ExtraStorageCount { get; protected set; }

        /// <summary>
        /// Дополнительное кол-во организаций аккаунта
        /// </summary>
        public virtual short ExtraAccountOrganizationCount { get; protected set; }

        /// <summary>
        /// Дополнительное кол-во гигабайт данных
        /// </summary>
        public virtual short ExtraGigabyteCount { get; protected set; }

        /// <summary>
        /// Название конфигурации
        /// </summary>
        public virtual string Name 
        {
            get 
            {
                return string.Format("Тарифный план «{0}»", Rate.Name) + 
                    (ExtraActiveUserCount > 0 ? string.Format(", +{0} {1}", ExtraActiveUserCount, StringUtils.UserCount(ExtraActiveUserCount)) : "") + 
                    (ExtraStorageCount > 0 ? string.Format(", +{0} {1}", ExtraStorageCount, StringUtils.StorageCount(ExtraStorageCount)) : "") + 
                    (ExtraAccountOrganizationCount > 0 ? string.Format(", +{0} {1}", ExtraAccountOrganizationCount, StringUtils.JuridicalPersonCount(ExtraAccountOrganizationCount)) : "") +
                    (ExtraTeamCount > 0 ? string.Format(", +{0} {1}", ExtraTeamCount, StringUtils.TeamCount(ExtraTeamCount)) : "") + 
                    (ExtraGigabyteCount > 0 ? string.Format(", +{0} Гб данных", ExtraGigabyteCount) : "");
            }
        }

        /// <summary>
        /// Итоговое максимальное кол-во пользователей
        /// </summary>
        public virtual short TotalActiveUserCountLimit 
        { 
            get 
            {
                return (short)(Rate.ActiveUserCountLimit + ExtraActiveUserCount);
            }
        }

        /// <summary>
        /// Итоговое максимальное кол-во команд
        /// </summary>
        public virtual short TotalTeamCountLimit 
        {
            get
            {
                return (short)(Rate.TeamCountLimit + ExtraTeamCount);
            }
        }

        /// <summary>
        /// Итоговое максимальное кол-во мест хранения
        /// </summary>
        public virtual short TotalStorageCountLimit
        {
            get
            {
                return (short)(Rate.StorageCountLimit + ExtraStorageCount);
            }
        }

        /// <summary>
        /// Итоговое максимальное кол-во организаций аккаунта
        /// </summary>
        public virtual short TotalAccountOrganizationCountLimit
        {
            get
            {
                return (short)(Rate.AccountOrganizationCountLimit + ExtraAccountOrganizationCount);
            }
        }

        /// <summary>
        /// Итоговое максимальное кол-во гигабайт данных
        /// </summary>
        public virtual short TotalGigabyteCountLimit
        {
            get
            {
                return (short)(Rate.GigabyteCountLimit + ExtraGigabyteCount);
            }
        }
        
        #endregion

        #region Конструкторы

        protected ServiceSetConfiguration() {}

        public ServiceSetConfiguration(Rate rate, short extraActiveUserCount, short extraTeamCount, short extraStorageCount, 
            short extraAccountOrganizationCount, short extraGigabyteCount)
        {
            Rate = rate;
            ExtraActiveUserCount = extraActiveUserCount;
            ExtraTeamCount = extraTeamCount;
            ExtraStorageCount = extraStorageCount;
            ExtraAccountOrganizationCount = extraAccountOrganizationCount;
            ExtraGigabyteCount = extraGigabyteCount;
        }

        #endregion

    }
}
