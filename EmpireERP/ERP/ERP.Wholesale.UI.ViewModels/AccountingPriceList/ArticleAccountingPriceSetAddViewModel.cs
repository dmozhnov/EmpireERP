using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class ArticleAccountingPriceSetAddViewModel
    {
        /// <summary>
        /// Идентификатор реестра (родительского объекта)
        /// </summary>
        public Guid AccountingPriceListId { get; set; }

        public string AccountingPriceListName { get; set; }

        public string BackURL { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        /// <summary>
        /// Список групп товаров
        /// </summary>
        public Dictionary<string, string> ArticleGroups { get; set; }

        /// <summary>
        /// Строка кодов выбранных групп товаров
        /// </summary>
        public string ArticleGroupsIDs { get; set; }
        public string AllArticleGroups { get; set; }

        /// <summary>
        /// Признак: только наличие
        /// </summary>
        [DisplayName("По точному наличию на МХ")]
        public string OnlyAvailability { get; set; }

        /// <summary>
        /// Список мест хранения
        /// </summary>
        public Dictionary<string, string> Storages { get; set; }

        /// <summary>
        /// Строка кодов выбранных мест хранения
        /// </summary>
        public string StorageIDs { get; set; }
        public string AllStorages { get; set; }

        public ArticleAccountingPriceSetAddViewModel()
        {
            ArticleGroups = new Dictionary<string, string>();
            Storages = new Dictionary<string, string>();
            OnlyAvailability = "0";
            AllArticleGroups = "0";
            AllStorages = "0";
        }
    }
}
