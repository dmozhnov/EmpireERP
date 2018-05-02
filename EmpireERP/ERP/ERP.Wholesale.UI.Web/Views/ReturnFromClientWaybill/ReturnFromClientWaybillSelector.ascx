<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillSelectViewModel>" %>

<div style="width: 900px; padding: 10px 10px 0;">
    <%= Html.GridFilterHelper("filterReturnFromClientWaybill", Model.FilterData,
                            new List<string>() { "gridSelectReturnFromClientWaybill" }, true)%>
    
    <div id="messageReturnFromClientWaybillSelectList"></div>    
    <div id="grid_return_from_client_waybill_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ReturnFromClientWaybillSelectGrid", Model.Data); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>