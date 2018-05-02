<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Deal.DealSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Deal_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterSelectDeal", Model.FilterData,
        new List<string>() { "gridSelectDeal" }, true) %>
    
    <div id="messageDealSelectList"></div>    
    <div id="grid_deal_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("DealSelectGrid", Model.DealsGrid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>