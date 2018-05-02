using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.User
{
    public class UserDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес возврата
        /// </summary>
        public string BackURL { get; set; }

        public string DisplayName { get; set; }

        public UserMainDetailsViewModel MainDetails { get; set; }

        public GridData UserRolesGrid { get; set; }

        public GridData UserTeamsGrid { get; set; }

        public GridData NewTaskGrid { get; set; }
        public GridData ExecutingTaskGrid { get; set; }
        public GridData CompletedTaskGrid { get; set; }

        public bool AllowToEdit { get; set; }

        public bool AllowToViewRoleList { get; set; }
        public bool AllowToViewTeamList { get; set; }

        public UserDetailsViewModel()
        {
            UserRolesGrid = new GridData();
            UserTeamsGrid = new GridData();
        }
    }
}
