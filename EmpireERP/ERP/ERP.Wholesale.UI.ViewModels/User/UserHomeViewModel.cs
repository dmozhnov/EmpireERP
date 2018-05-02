using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserHomeViewModel
    {
        /// <summary>
        /// Режим отображения страницы (welcome - первый вход администратора системы)
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Грид задач, где пользователь исполнитель
        /// </summary>
        public GridData UserAsExecutorGrid { get; set; }

        /// <summary>
        /// Грид задач, где пользователь автор
        /// </summary>
        public GridData UserAsCreatorGrid { get; set; }

        public UserHomeViewModel()
        {
            UserAsExecutorGrid = new GridData();
            UserAsCreatorGrid = new GridData();
        }
    }
}
