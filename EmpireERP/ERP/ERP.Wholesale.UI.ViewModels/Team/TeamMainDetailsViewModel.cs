using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Team
{
    public class TeamMainDetailsViewModel
    {       
        /// <summary>
        /// Название
        /// </summary>
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Кто создал
        /// </summary>
        [DisplayName("Кто создал")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Общее кол-во членов
        /// </summary>
        [DisplayName("Общее кол-во пользователей")]
        public string UserCount { get; set; }

        /// <summary>
        /// Кол-во мест хранения
        /// </summary>
        [DisplayName("Кол-во мест хранения")]
        public string StorageCount { get; set; }

        /// <summary>
        /// Кол-во сделок
        /// </summary>
        [DisplayName("Кол-во сделок")]
        public string DealCount { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        public bool AllowToViewCreatorDetails { get; set; }
        public string CreatorId { get; set; }
    }
}
