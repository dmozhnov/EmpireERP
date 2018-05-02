<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Внутренние перемещения
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("MovementWaybill", "Внутренние перемещения", "")%>
    
    <%= Html.GridFilterHelper("filterMovementWaybill", Model.FilterData, 
        new List<string>() { "gridShippingPendingMovementWaybill", "gridShippedMovementWaybill", "gridReceiptedMovementWaybill" }) %>
            
    <% Html.RenderPartial("MovementWaybillShippingPendingGrid", Model.ShippingPending); %>

    <% Html.RenderPartial("MovementWaybillShippedGrid", Model.Shipped); %>

    <% Html.RenderPartial("MovementWaybillReceiptedGrid", Model.Receipted); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
