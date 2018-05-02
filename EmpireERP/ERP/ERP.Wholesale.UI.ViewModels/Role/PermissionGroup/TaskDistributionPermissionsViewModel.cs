using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class TaskDistributionPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }

        #region Задачи

        public byte Task_CreatedBy_List_Details { get; set; }
        public PermissionViewModel Task_CreatedBy_List_Details_ViewModel { get; set; }

        public byte Task_ExecutedBy_List_Details { get; set; }
        public PermissionViewModel Task_ExecutedBy_List_Details_ViewModel { get; set; }

        public byte Task_Create { get; set; }
        public PermissionViewModel Task_Create_ViewModel { get; set; }

        public byte Task_Edit { get; set; }
        public PermissionViewModel Task_Edit_ViewModel { get; set; }

        public byte Task_TaskExecutionItem_Edit_Delete { get; set; }
        public PermissionViewModel Task_TaskExecutionItem_Edit_Delete_ViewModel { get; set; }

        public byte Task_Delete { get; set; }
        public PermissionViewModel Task_Delete_ViewModel { get; set; }

        #endregion

        #region Исполнения задачи

        public byte TaskExecutionItem_Create { get; set; }
        public PermissionViewModel TaskExecutionItem_Create_ViewModel { get; set; }

        public byte TaskExecutionItem_Edit { get; set; }
        public PermissionViewModel TaskExecutionItem_Edit_ViewModel { get; set; }

        public byte TaskExecutionItem_EditExecutionDate { get; set; }
        public PermissionViewModel TaskExecutionItem_EditExecutionDate_ViewModel { get; set; }

        public byte TaskExecutionItem_Delete { get; set; }
        public PermissionViewModel TaskExecutionItem_Delete_ViewModel { get; set; }

        #endregion
    }
}
