using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage
{
    public class ProductionOrderMaterialsPackageDocumentEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Имя фала
        /// </summary>
        [DisplayName("Файл")]
        public string FileName { get; set; }
 
        /// <summary>
        /// Описание файла
        /// </summary>
        [DisplayName("Описание")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор материала
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// Идентификатор пакета материалов
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// Загруженный файл
        /// </summary>
        public object FileUpload { get; set; }

        /// <summary>
        /// Признак разрешения выбора файла
        /// </summary>
        public bool AllowSelectFile{ get; set; }
    }
}
