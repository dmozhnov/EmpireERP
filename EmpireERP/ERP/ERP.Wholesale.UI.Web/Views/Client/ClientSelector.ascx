<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Client.ClientSelectViewModel>" %>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Client_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterClient", Model.FilterData,
        new List<string>() { "gridSelectClient" }, true) %>
    
    <div id="messageClientSelectList"></div>    
    <div id="grid_client_select" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("~/Views/Client/ClientSelectGrid.ascx", Model.ClientsGrid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>