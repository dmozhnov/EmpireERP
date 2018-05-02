<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.User.UserDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали пользователя
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        User_Details.Init();

        $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByExecutedBy($("#Id").val());
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("User", "Детали пользователя", Model.DisplayName, "/Help/GetHelp_User_Details")%>

    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageUserEdit"></div>
    <% Html.RenderPartial("UserMainDetails", Model.MainDetails); %>
    <br />

    <%if(Model.AllowToViewRoleList) { %>
        <div id="messageUserRoleList"></div>
        <% Html.RenderPartial("UserRolesGrid", Model.UserRolesGrid); %>
    <%} %>

    <%if(Model.AllowToViewTeamList){ %>
        <div id="messageUserTeamList"></div>
        <% Html.RenderPartial("UserTeamsGrid", Model.UserTeamsGrid); %>
    <%} %>

    <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.NewTaskGrid); %>

    <% Html.RenderPartial("~/Views/Task/ExecutingTaskGrid.ascx", Model.ExecutingTaskGrid); %>

    <% Html.RenderPartial("~/Views/Task/CompletedTaskGrid.ascx", Model.CompletedTaskGrid); %>    
    
    <div id="roleSelector"></div>
    <div id="teamSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
