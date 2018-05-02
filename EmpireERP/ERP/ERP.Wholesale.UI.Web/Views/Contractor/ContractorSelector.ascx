<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Contractor.ContractorSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %></div>
    <br />

    <%= Html.GridFilterHelper("contractorFilter", Model.Filter, new List<string>() { "gridContractor" }, true)%>

    <div id="messageContractorSelectList"></div>   
    <div id="grid_client_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ContractorSelectGrid", Model.Grid); %>
    </div>
    
    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>