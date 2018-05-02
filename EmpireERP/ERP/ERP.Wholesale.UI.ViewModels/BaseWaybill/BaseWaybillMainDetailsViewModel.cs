using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.BaseWaybill
{
    /// <summary>
    /// Базовая модель главных деталей накладных
    /// </summary>
    public class BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Статус накладной
        /// </summary>
        [DisplayName("Статус накладной")]
        public string StateName { get; set; }

        /// <summary>
        /// Общий вес
        /// </summary>
        [DisplayName("Общий вес (кг)")]
        public string TotalWeight { get; set; }

        /// <summary>
        /// Общий объем
        /// </summary>
        [DisplayName("| объем (м3)")]
        public string TotalVolume { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }
        public string CuratorId { get; set; }
        public bool AllowToViewCuratorDetails { get; set; }
        public bool AllowToChangeCurator { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Пользователь, создавший накладную
        /// </summary>
        [DisplayName("Создание")]
        public string CreatedByName { get; set; }
        public string CreatedById { get; set; }
        public bool AllowToViewCreatedByDetails { get; set; }
        public string CreationDate { get; set; }

        /// <summary>
        /// Пользователь, осуществивший проводку
        /// </summary>
        [DisplayName("Проводка")]
        public string AcceptedByName { get; set; }
        public string AcceptedById { get; set; }
        public bool AllowToViewAcceptedByDetails { get; set; }
        public string AcceptanceDate { get; set; }
    }
}
