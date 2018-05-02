<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ClientContract.ClientContractEditViewModel>" %>

<%--<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>--%>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    Deal_ClientContractEdit.Init();

    function OnSuccessOrganizationEdit(result) {
        Deal_ClientContractEdit.OnSuccessOrganizationEdit(result);
    }

    function OnSuccessEconomicAgentTypeSelect(ajaxContext) {
        Deal_ClientContractEdit.OnSuccessEconomicAgentTypeSelect(ajaxContext);
    }

    function OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName) {
        Deal_ClientContractEdit.OnAccountOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName);
    }

    function OnContractorOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName) {
        Deal_ClientContractEdit.OnContractorOrganizationSelectLinkClick(accountOrganizationId, accountOrganizationShortName);
    }

    function OnBeginContractEdit() {
        StartButtonProgress($("#btnSaveClientContract"));
    }
</script>

<div style="width:510px; padding-left:10px; padding-right:10px;">

<% using (Ajax.BeginForm(Model.PostActionName, Model.PostControllerName, new AjaxOptions() { OnBegin = "OnBeginContractEdit",
       OnSuccess = "OnSuccessContractEdit", OnFailure = "Deal_ClientContractEdit.OnFailContractEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.DealId) %>
    <%: Html.HiddenFor(model => model.ClientId) %>
    <%: Html.HiddenFor(model => model.AccountOrganizationId) %>
    <%: Html.HiddenFor(model => model.ClientOrganizationId) %>
    <%: Html.HiddenFor(model => model.AllowToEditOrganization) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ClientContract_Edit") %></div>
    <br />

    <div id="messageContractEdit"></div>

    <div class="group_title">Информация о договоре</div>
    <div class="h_delim"></div>
    <br />

    <table class="editor_table" style="width: 500px;">
        <tr>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ClientContract_Edit_Number")%>:
            </td>
            <td>
                <%: Html.TextBoxFor(model => model.Number, new { style = "width:220px;", maxlength = "50" }) %>
                <%: Html.ValidationMessageFor(model => model.Number) %>
            </td>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ClientContract_Edit_Date")%>:
            </td>
            <td style="text-align: left;">
                <%= Html.DatePickerFor(model => model.Date, new { id = "ContractDate" })%>
                <%: Html.ValidationMessageFor(model => model.Date) %>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_ClientContract_Edit_Name")%>:
            </td>
            <td colspan="3" style="text-align: left;">
                <%: Html.TextBoxFor(model => model.Name, new { style = "width:398px;", maxlength = "200" }) %>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </td>
        </tr>
    </table>

    <br />
    <div class="group_title">Между кем и кем</div>
    <div class="h_delim"></div>
    <br />

    <table class="editor_table" style="width:480px;">
        <tr>
            <td class="row_title" style="width:160px;">
                <%: Html.LabelFor(model => model.AccountOrganizationName) %>:
            </td>
            <td style="width:330px; padding-left:15px;">
                <% if (Model.AllowToEditOrganization) { %><span id="linkAccountOrganizationSelector" class="select_link"><% } %>
                    <span id="AccountOrganizationName"><%: Model.AccountOrganizationName%></span>
                <% if (Model.AllowToEditOrganization)
                   { %></span><% } %>
                <%: Html.ValidationMessageFor(x => x.AccountOrganizationId) %>
            </td>
        </tr>
        <tr>
            <td class="row_title" style="width:160px;">
                <%: Html.LabelFor(model => model.ClientOrganizationName) %>:
            </td>
            <td style="width:330px; padding-left:15px;">
                <% if (Model.AllowToEditOrganization && Model.AllowToEditClientOrganization)
                   { %><span id="linkClientOrganizationSelector" class="select_link"><% } %>
                    <span id="ClientOrganizationName"><%: Model.ClientOrganizationName%></span>
                <% if (Model.AllowToEditOrganization && Model.AllowToEditClientOrganization)
                   { %></span><% } %>
                <%: Html.ValidationMessageFor(x => x.ClientOrganizationId) %>
            </td>
        </tr>
    </table>

    <br />

    <div class="button_set">
        <input id="btnSaveClientContract" type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="accountOrganizationSelector"></div>
<div id="contractorOrganizationSelector"></div>

</div>
