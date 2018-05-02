<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ContractorOrganization.ContractorOrganizationSelectViewModel>" %>

<script type="text/javascript">
    ContractorOrganization_Selector.Init();
</script>

<div style="width: 800px; padding: 0 10px 0;">
    <%: Html.HiddenFor(model => model.ContractorId) %>
    <input id="controllerName" type="hidden" value="<%= Model.ControllerName %>" />
    <input id="actionName" type="hidden" value="<%= Model.ActionName %>" />

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ContractorOrganization_Edit") %></div>
    <div class="h_delim"></div>

    <%= Html.GridFilterHelper("filterContractorOrganization", Model.Filter, new List<string>() { "gridOrganizationSelect" }, true)%>

    <% if(Model.AllowToCreateNewOrganization) { %>        
        <span class="selector_link" id="linkAddOrganization"><%: Model.NewOrganizationLinkName %></span>
        <br /><br />
    <%} %>

    <div id="messageOrganizationSelectList"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("~/Views/ContractorOrganization/ContractorOrganizationSelectGrid.ascx", Model.GridData); %>
    </div>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
