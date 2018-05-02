using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Settings
{
    public class SettingViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }

        #region Группа настроек по использованию статуса "Готово к проводке"

        /// <summary>
        /// Заголовок для группы настроек по использованию статуса "Готово к проводке"
        /// </summary>
        public string GroupTitleForReadyToAcceptState { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных внутреннего перемещения
        /// </summary>
        [DisplayName("В накладных внутреннего перемещения")]
        public string UseReadyToAcceptStateForMovementWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных смены собственника
        /// </summary>
        [DisplayName("В накладных смены собственника")]
        public string UseReadyToAcceptStateForChangeOwnerWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных реализации
        /// </summary>
        [DisplayName("В накладных реализации")]
        public string UseReadyToAcceptStateForExpenditureWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных возврата от клиента
        /// </summary>
        [DisplayName("В накладных возврата от клиента")]
        public string UseReadyToAcceptStateForReturnFromClientWaybill { get; set; }

        /// <summary>
        /// Признак использования статуса "Готово к проводке" в накладных списания
        /// </summary>
        [DisplayName("В накладных списания")]
        public string UseReadyToAcceptStateForWriteOffWaybill { get; set; }

        #endregion
    }
}
