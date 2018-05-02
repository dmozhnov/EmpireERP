<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderSelectorViewModel>" %>

<link href="../../Content/Style/DatePicker.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/DatePicker.min.js" type="text/javascript"></script>
<script src="../../Scripts/DatePicker.js" type="text/javascript"></script>

<div style="width: 800px; padding: 0 10px 0">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrder_Select") %></div>
    <br />
    
    <%=Html.GridFilterHelper("filterProductionOrder", Model.Filter, new List<string>() { "gridProductionOrder" }, true)%>
    
    <div id="messageProductionOrderSelectGrid"></div>
    <div id="grid_productionOrder_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ProductionOrderSelectGrid", Model.Grid); %>
    </div>
    
    <div class="button_set">
       <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>