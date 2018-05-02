<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillSelectViewModel>" %>

<div style="width: 800px; padding: 10px 10px 0;">
    <%= Html.GridFilterHelper("filterChangeOwnerWaybill", Model.FilterData,
                    new List<string>() { "gridSelectChangeOwnerWaybill" }, true)%>
    
    <div id="messageChangeOwnerWaybillSelectList"></div>    
    <div id="grid_change_owner_waybill_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ChangeOwnerWaybillSelectGrid", Model.Data); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>