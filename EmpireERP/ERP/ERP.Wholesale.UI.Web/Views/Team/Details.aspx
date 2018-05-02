<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Team.TeamDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали команды
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Team_Details.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("Team", "Детали команды", Model.Name, "/Help/GetHelp_Team_Details")%>

    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageTeamEdit"></div>

    <div id="team_main_details">
        <% Html.RenderPartial("TeamMainDetails", Model.MainDetails); %>
    </div>
    <br />

    <% if(Model.AllowToViewUserList) {%>
    <div id="messageUserList"></div>
    <% Html.RenderPartial("UsersGrid", Model.UsersGrid); %>
    <%} %>

    <% if (Model.AllowToViewDealList)
       { %>
    <div id="messageDealList"></div>
    <% Html.RenderPartial("DealsGrid", Model.DealsGrid); %>
    <%} %>

    <%if(Model.AllowToViewStorageList){ %>
    <div id="messageStorageList"></div>
    <% Html.RenderPartial("StoragesGrid", Model.StoragesGrid); %>
    <%} %>

    <%if(Model.AllowToViewProductionOrderList){ %>
    <div id="messageProductionOrderList"></div>
    <% Html.RenderPartial("ProductionOrdersGrid", Model.ProductionOrdersGrid); %>
    <%} %>

    <div id="userSelector"></div>
    <div id="dealSelector"></div>
    <div id="productionOrderSelector"></div>
    <div id="storageSelectList"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
