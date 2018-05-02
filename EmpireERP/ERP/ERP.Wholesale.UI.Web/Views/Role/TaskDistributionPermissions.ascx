<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.TaskDistributionPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessTaskDistributionPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messagTaskDistributionPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageTaskDistributionPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailTaskDistributionPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messagTaskDistributionPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageTaskDistributionPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveTaskDistributionPermissions", "Role", new AjaxOptions() {
       OnSuccess = "OnSuccessTaskDistributionPermissionsSave", OnFailure = "OnFailTaskDistributionPermissionsSave" })) %> 
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit) { %>
        <div class="button_set">
            <input type="submit" id="btnTaskDistributionPermissionsSaveTop" value="Сохранить" />
        </div>
    <%} %>

    <div id="messagTaskDistributionPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Задачи</div>
        <table>
            <%= Html.Permission(Model.Task_CreatedBy_List_Details_ViewModel) %>
            <%= Html.Permission(Model.Task_ExecutedBy_List_Details_ViewModel)%>
            <%= Html.Permission(Model.Task_Create_ViewModel) %>
            <%= Html.Permission(Model.Task_Edit_ViewModel) %>
            <%= Html.Permission(Model.Task_TaskExecutionItem_Edit_Delete_ViewModel) %>
            <%= Html.Permission(Model.Task_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Исполнение задачи</div>
        <table>
            <%= Html.Permission(Model.TaskExecutionItem_Create_ViewModel)%>
            <%= Html.Permission(Model.TaskExecutionItem_Edit_ViewModel) %>
            <%= Html.Permission(Model.TaskExecutionItem_EditExecutionDate_ViewModel) %>
            <%= Html.Permission(Model.TaskExecutionItem_Delete_ViewModel) %>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnTaskDistributionPermissionsSave" value="Сохранить" />
    </div>

    <%} %>
<%} %>