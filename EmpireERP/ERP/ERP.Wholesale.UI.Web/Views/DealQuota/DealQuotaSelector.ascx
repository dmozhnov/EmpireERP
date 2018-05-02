<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealQuota.DealQuotaSelectViewModel>" %>

<div style="width: 750px;padding: 0 10px;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_DealQuota_Select") %></div>   
    <br />

    <%= Html.GridFilterHelper("filterDealQuota", Model.FilterData, new List<string>() { "gridDealQuotaSelect" }, true)%>

    <div id="messageDealQuotaSelectorListGrid"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("DealQuotaSelectGrid", Model.DealQuotasGrid); %>
    </div>

    <div class="button_set">            
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>    
</div>

