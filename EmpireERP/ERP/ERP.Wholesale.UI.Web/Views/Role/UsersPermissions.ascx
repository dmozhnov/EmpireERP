<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.UsersPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessUsersPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageUsersPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageUsersPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailUsersPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageUsersPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageUsersPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveUsersPermissions", "Role", new AjaxOptions(){ OnSuccess = "OnSuccessUsersPermissionsSave", OnFailure = "OnFailUsersPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnUsersPermissionsSaveTop" value="Сохранить" />
    </div>

    <%} %>

    <div id="messageUsersPermissionsEdit"></div>

    <div class="permission_group">
        <div class="title">Пользователи</div>
        <table>
            <%= Html.Permission(Model.User_List_Details_ViewModel, true)%>
            
            <% if (Model.IsSystemAdmin)
               { %>
            <%= Html.Permission(Model.User_Create_ViewModel)%>
            <%= Html.Permission(Model.User_Edit_ViewModel)%>
            <%= Html.Permission(Model.User_Role_Add_ViewModel)%>
            <%= Html.Permission(Model.User_Role_Remove_ViewModel)%>
            <%= Html.Permission(Model.User_Delete_ViewModel)%>
            <%} %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Команды</div>
        <table>
            <%= Html.Permission(Model.Team_List_Details_ViewModel, true)%>

            <% if (Model.IsSystemAdmin)
               { %>
            <%= Html.Permission(Model.Team_Create_ViewModel) %>
            <%= Html.Permission(Model.Team_Edit_ViewModel) %>
            <%} %>

            <%= Html.Permission(Model.Team_Storage_Add_ViewModel) %>
            <%= Html.Permission(Model.Team_Storage_Remove_ViewModel) %>
            <%= Html.Permission(Model.Team_ProductionOrder_Add_ViewModel) %>
            <%= Html.Permission(Model.Team_ProductionOrder_Remove_ViewModel)%>
            <%= Html.Permission(Model.Team_Deal_Add_ViewModel) %>
            <%= Html.Permission(Model.Team_Deal_Remove_ViewModel) %>

            <% if (Model.IsSystemAdmin)
               { %>
            <%= Html.Permission(Model.Team_User_Add_ViewModel) %>
            <%= Html.Permission(Model.Team_User_Remove_ViewModel) %>
            <%= Html.Permission(Model.Team_Delete_ViewModel) %>
            <%} %>
        </table>
    </div>

        <div class="permission_group">
        <div class="title">Роли</div>
        <table>
            <%= Html.Permission(Model.Role_List_Details_ViewModel, true)%>

            <% if (Model.IsSystemAdmin)
               { %>
            <%= Html.Permission(Model.Role_Create_ViewModel) %>
            <%= Html.Permission(Model.Role_Edit_ViewModel) %>
            <%= Html.Permission(Model.Role_Delete_ViewModel) %>
            <%} %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Должности пользователей</div>
        <table>
            <%= Html.Permission(Model.EmployeePost_Create_ViewModel)%>
            <%= Html.Permission(Model.EmployeePost_Edit_ViewModel)%>
            <%= Html.Permission(Model.EmployeePost_Delete_ViewModel)%>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnUsersPermissionsSaveBottom" value="Сохранить" />
    </div>

    <%} %>

<% } %>