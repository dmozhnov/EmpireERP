using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage
{
    public class ProductionOrderMaterialsPackageEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [DisplayName("Название пакета")]
        [Required(ErrorMessage = "Укажите название пакета")]
        [StringLength(250,ErrorMessage="Не более {1} символов")]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [DisplayName("Описание")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull=false)]
        public string Description { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        [DisplayName("Дата обновления")]
        public string LastChangeDate { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [StringLength(400, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Заказ
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrder { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        [Required(ErrorMessage="Укажите заказ")]
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Разрешение на смену заказа
        /// </summary>
        public bool AllowToChangeProductionOrder {get;set;}
    }
}
