<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ClientContract.ClientContractSelectViewModel>" %>

<script type="text/javascript">
    ClientContract_Selector.Init();
</script>

<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title"><%: Model.Title %></div>
    <br />

    <%= Html.GridFilterHelper("filterClientContract", Model.FilterData,
        new List<string>() { "gridSelectClientContract" }, true) %>

    <% if(Model.AllowToCreateContract) { %>        
        <span class="selector_link" id="linkCreateClientContract">Создать новый договор и связать его со сделкой</span>
        <br /><br />
    <%} %>
    
    <div id="messageClientContractSelectList"></div>    
    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("~/Views/ClientContract/ClientContractSelectGrid.ascx", Model.ClientContractGrid); %>
    </div>

    <div id="clientContractEdit"></div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>