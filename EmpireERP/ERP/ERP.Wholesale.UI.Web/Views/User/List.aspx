<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.User.UserListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Пользователи
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("User", "Пользователи", "", "/Help/GetHelp_User_List")%>

    <div id="messageUserList"></div>

    <%= Html.GridFilterHelper("filterUser", Model.FilterData,
        new List<string>() { "gridActiveUsers", "gridBlockedUsers"})%>

    <% Html.RenderPartial("ActiveUsersGrid", Model.ActiveUsersGrid); %>

    <% Html.RenderPartial("BlockedUsersGrid", Model.BlockedUsersGrid); %>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
