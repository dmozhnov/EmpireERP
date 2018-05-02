using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage
{
    public class ProductionOrderMaterialsPackageMainDetailsViewModel
    {
        /// <summary>
        /// Заказ
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrder { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название пакета
        /// </summary>
        [DisplayName("Название пакета")]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [DisplayName("Описание")]
        public string Description { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Дата обновления
        /// </summary>
        [DisplayName("Дата обновления")]
        public string LastChangeDate { get; set; }

        /// <summary>
        /// Количество материалов в пакете
        /// </summary>
        [DisplayName("Кол-во документов")]
        public string DocumentCount { get; set; }

        /// <summary>
        /// Размер материалов пакета в мегабайтах
        /// </summary>
        [DisplayName("Общий размер пакета")]
        public string PakageSize { get; set; }

        public bool AllowToViewProductionOrder { get; set; }
    }
}
