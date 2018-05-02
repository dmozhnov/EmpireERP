<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillSelectViewModel>" %>

<div style="width: 800px; padding: 10px 10px 0;">
    <%= Html.GridFilterHelper("filterMovementWaybill", Model.FilterData,
            new List<string>() { "gridSelectMovementWaybill" }, true)%>
    
    <div id="messageMovementWaybillSelectList"></div>    
    <div id="grid_movement_waybill_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("MovementWaybillSelectGrid", Model.Data); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>