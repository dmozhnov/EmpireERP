<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Team.TeamListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Команды
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Team", "Команды", "", "/Help/GetHelp_Team_List")%>

    <div id="messageTeamList"></div>

    <%= Html.GridFilterHelper("filterTeam", Model.FilterData,
        new List<string>() { "gridTeams"})%>

    <% Html.RenderPartial("TeamsGrid", Model.TeamsGrid); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
