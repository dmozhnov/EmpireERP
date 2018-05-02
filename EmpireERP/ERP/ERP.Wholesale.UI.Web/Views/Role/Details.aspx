<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Role.RoleDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали роли
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Role_Details.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("Role", "Детали роли", Model.Name, "/Help/GetHelp_Role_Details")%>

    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageRoleEdit"></div>
    <% Html.RenderPartial("RoleMainDetails", Model.MainDetails); %>
    <br />

    <%if(Model.AllowToViewUserList){ %>
    <div id="messageUserList"></div>
    <% Html.RenderPartial("UsersGrid", Model.UsersGrid); %>
    <%} %>

    <div id="messageRolePermissionEdit"></div>

    <%= Html.PageBoxTop("Настройка прав доступа роли")%>

        <div id="tabPanel_menu_container">
            <div id="commonTab" class="tabPanel_menu_item selected">Общие права</div>
            <div id="productionTab" class="tabPanel_menu_item">Производство</div>
            <div id="articleDistributionTab" class="tabPanel_menu_item">Товародвижение</div>
            <div id="salesTab" class="tabPanel_menu_item">Продажи</div>
            <div id="taskTab" class="tabPanel_menu_item">Задачи</div>
            <div id="reportsTab" class="tabPanel_menu_item">Отчеты</div>
            <div id="directoriesTab" class="tabPanel_menu_item">Справочники</div>
            <div id="usersTab" class="tabPanel_menu_item">Пользователи</div>
        </div>
                
        <div style="background: #fff; padding: 5px 0; border: 1px solid #ddd;">
            <div id="permissionGroupContainer">
                <% Html.RenderPartial("CommonPermissions", Model.CommonPermissions); %>
            </div>
        </div>
    
    <%= Html.PageBoxBottom() %>

    <div id="userSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
