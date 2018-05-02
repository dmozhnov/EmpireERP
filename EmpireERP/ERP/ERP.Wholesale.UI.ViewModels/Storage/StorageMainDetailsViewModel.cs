using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Storage
{
    public class StorageMainDetailsViewModel
    {
        [DisplayName("Код")]
        public short Id { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Тип")]
        public string TypeName { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        [DisplayName("Кол-во секций")]
        public string SectionCount { get; set; }

        [DisplayName("Кол-во организаций")]
        public string AccountOrganizationCount { get; set; }
    }
}