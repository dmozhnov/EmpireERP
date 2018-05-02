<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderContractEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    ProductionOrder_ProducerContractEdit.Init();
</script>

<div style="width:500px; padding-left:10px; padding-right:10px;">

<% using (Ajax.BeginForm("SaveContract", "ProductionOrder", new AjaxOptions() { OnBegin = "ProductionOrder_Details.OnBeginProductionOrderSave", OnSuccess = "ProductionOrder_Details.OnSuccessContractEdit", OnFailure = "ProductionOrder_ProducerContractEdit.OnFailContractEdit" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.ProductionOrderId) %>
    <%: Html.HiddenFor(model => model.AccountOrganizationId) %>
    <%: Html.HiddenFor(model => model.AllowToChangeAccountOrganization) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderContract") %></div>
    <br />

    <div id="messageProducerContractEdit"></div>

    <div class="group_title">Информация о контракте</div>
    <div class="h_delim"></div>
    <br />

    <table class="editor_table" style="width: 500px;">
        <tr>
            <td class="row_title">
                <%: Html.LabelFor(model => model.Number) %>:
            </td>
            <td>
                <%: Html.TextBoxFor(model => model.Number, new { style = "width:220px;", maxlength = "50" }) %>
                <%: Html.ValidationMessageFor(model => model.Number) %>
            </td>
            <td class="row_title">
                <%: Html.LabelFor(model => model.ContractDate) %>:
            </td>
            <td style="text-align: left;">
                <%= Html.DatePickerFor(model => model.ContractDate, null, !Model.AllowToChangeDate, !Model.AllowToChangeDate)%>
                <%: Html.ValidationMessageFor(model => model.ContractDate) %>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%: Html.LabelFor(model => model.Name) %>:
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
                <% if (Model.AllowToChangeAccountOrganization) { %><span id="linkAccountOrganizationSelector" class="select_link"><% } %>
                    <span id="AccountOrganizationName"><%: Model.AccountOrganizationName%></span>
                <% if (Model.AllowToChangeAccountOrganization) { %></span><% } %>
                <%: Html.ValidationMessageFor(x => x.AccountOrganizationId) %>
            </td>
        </tr>
        <tr>
            <td class="row_title" style="width:160px;">
                <%: Html.LabelFor(model => model.ProducerOrganizationName) %>:
            </td>
            <td style="width:330px; padding-left:15px;">
                <%: Model.ProducerOrganizationName%>
            </td>
        </tr>
    </table>

    <br />

    <div class="button_set">
        <%= Html.SubmitButton("btnSave", "Сохранить")%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

<div id="accountOrganizationSelector"></div>

</div>
