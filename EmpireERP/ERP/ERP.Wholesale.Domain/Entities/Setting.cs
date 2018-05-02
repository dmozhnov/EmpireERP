
namespace ERP.Wholesale.Domain.Entities
{
    public class Setting
    {
        /// <summary>
        /// Идентификатор. Таблица будет иметь только одну строку, поэтому это поле всегда будет равно 1. Наличие этого поля обосновано тем, что NHibernate требует, чтобы у таблицы было поле-идентификатор
        /// </summary>
        public virtual byte Id { get; set; }

        #region Ограничения на кол-во сущностей в БД

        /// <summary>
        /// Максимальное количество активных пользователей на аккунт
        /// </summary>
        public virtual short ActiveUserCountLimit { get; protected set; }

        /// <summary>
        /// Максимальное количество команд на аккаунт
        /// </summary>
        public virtual short TeamCountLimit { get; protected set; }

        /// <summary>
        /// Максимальное количество мест хранения на аккаунт
        /// </summary>
        public virtual short StorageCountLimit { get; protected set; }

        /// <summary>
        /// Максимальное количество собственных организаций на аккаунт
        /// </summary>
        public virtual short AccountOrganizationCountLimit { get; protected set; }

        /// <summary>
        /// Максимальное количество Гб данных (файлы + БД) на аккаунт
        /// </summary>
        public virtual short GigabyteCountLimit { get; protected set; }

        #endregion
        
        #region Использование статуса "Готово к проводке" в накладных

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных внутреннего перемещения
        /// </summary>
        public virtual bool UseReadyToAcceptStateForMovementWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных смены собственника
        /// </summary>
        public virtual bool UseReadyToAcceptStateForChangeOwnerWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных реализации
        /// </summary>
        public virtual bool UseReadyToAcceptStateForExpenditureWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных возврата
        /// </summary>
        public virtual bool UseReadyToAcceptStateForReturnFromClientWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных списания
        /// </summary>
        public virtual bool UseReadyToAcceptStateForWriteOffWaybill { get; set; }

        #endregion
    }
}
