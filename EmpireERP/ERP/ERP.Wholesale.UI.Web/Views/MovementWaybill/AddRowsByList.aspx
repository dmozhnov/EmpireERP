<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.OutgoingWaybill.OutgoingWaybillAddRowsByListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Добавление позиций в накладную перемещения товаров
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Content/Style/Treeview.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        MovementWaybill_AddRowsByList.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.StorageId) %>
    <%: Html.HiddenFor(model => model.AccountOrganizationId) %>
    
    <%= Html.PageTitle("ExpenditureWaybill", "Добавление позиций в накладную перемещения товаров", Model.Name)%>

    <div class="button_set">        
        <input id="btnBack" type="button" value="Назад" />
    </div>

    <%= Html.GridFilterHelper("filterArticlesForWaybillRowsAdditionByList", Model.Filter, new List<string>() { "gridArticlesForWaybillRowsAdditionByList" }, true)%>

    <div id="messageArticlesForWaybillRowsAdditionByList"></div>
    <% Html.RenderPartial("~/Views/OutgoingWaybill/ArticlesForWaybillRowsAdditionByListGrid.ascx", Model.ArticleGrid); %>

    <div id="messageMovementWaybillRowList"></div>
    <% Html.RenderPartial("MovementWaybillRowGrid", Model.RowGrid); %>

    <div id="movementWaybillRowEdit"></div>
    <div id="movementWaybillSourceLink"></div>
    <div id="articleGroupFilterSelector"></div>
</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
