<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountOrganization.AccountOrganizationSelectViewModel>" %>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_AccountOrganization_Select") %></div>
    <div class="h_delim"></div>

    <div id="messageAccountOrganizationSelectList"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("AccountOrganizationSelectGrid", Model.GridData); %>
    </div>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
