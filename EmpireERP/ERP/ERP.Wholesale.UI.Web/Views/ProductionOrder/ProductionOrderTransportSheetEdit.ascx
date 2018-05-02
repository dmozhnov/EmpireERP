<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderTransportSheetEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>
<script type="text/javascript">
    function OnBeginProductionOrderTransportSheetSave() {
            StartButtonProgress($("#btnSave"));
        }
</script>

<div style="width:600px; padding-left:10px; padding-right:10px;">

<% using (Ajax.BeginForm("SaveProductionOrderTransportSheet", "ProductionOrder", new AjaxOptions() { OnBegin = "OnBeginProductionOrderTransportSheetSave", OnFailure = "OnFailProductionOrderTransportSheetEdit", OnSuccess = "OnSuccessProductionOrderTransportSheetEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.TransportSheetId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.TransportSheetCurrencyDeterminationTypeId)%>
    <%:Html.HiddenFor(model => model.TransportSheetCurrencyRateForEdit)%>
    <%:Html.HiddenFor(model => model.TransportSheetCurrencyRateId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderTransportSheet") %></div>
    <div class="h_delim"></div>

    <br />
    <div id="messageProductionOrderTransportSheetEdit"></div>

    <table class="editor_table">
        <tr>
            <td class="row_title" style="max-width: 85px"><%:Html.LabelFor(model => model.ProductionOrderName)%>:</td>
            <td>
                <%: Model.ProductionOrderName %>
            </td>
            <td class="row_title" style="max-width: 100px"><%:Html.LabelFor(model => model.RequestDate)%>:</td>
            <td>
                <%= Html.DatePickerFor(model => model.RequestDate, isDisabled: !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.RequestDate)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.ForwarderName)%>:</td>
            <td>
                <%: Html.TextBoxFor(model => model.ForwarderName, new { maxlength = 200, size = 40 }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.ForwarderName)%>
            </td>
            <td class="row_title"><%:Html.LabelFor(model => model.ShippingDate)%>:</td>
            <td>
                <%= Html.DatePickerFor(model => model.ShippingDate, isDisabled: !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.ShippingDate)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.TransportSheetCurrencyId)%>:</td>
            <td>
                <%: Html.DropDownListFor(model => model.TransportSheetCurrencyId, Model.CurrencyList, !Model.AllowToChangeCurrency)%>
                <%: Html.ValidationMessageFor(model => model.TransportSheetCurrencyId)%>
            </td>
            <td class="row_title"><%:Html.LabelFor(model => model.PendingDeliveryDate)%>:</td>
            <td>
                <%= Html.DatePickerFor(model => model.PendingDeliveryDate, isDisabled: !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.PendingDeliveryDate)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.TransportSheetCurrencyRateName)%>:</td>
            <td>
                <span id="TransportSheetCurrencyRateName"><%: Model.TransportSheetCurrencyRateName%></span>&nbsp;=&nbsp;<span id="TransportSheetCurrencyRate"><%: Model.TransportSheetCurrencyRateForDisplay%></span>&nbsp;р.

                <% string allowToChangeCurrencyRateDisplay = (Model.AllowToChangeCurrencyRate ? "inline" : "none"); %>

                <span id='linkChangeCurrencyRate' class="edit_action" style="display:<%= allowToChangeCurrencyRateDisplay %>">[ Изменить ]</span>
            </td>
            <td class="row_title"><%:Html.LabelFor(model => model.ActualDeliveryDate)%>:</td>
            <td>
                <%= Html.DatePickerFor(model => model.ActualDeliveryDate, isDisabled: !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.ActualDeliveryDate)%>
            </td>
        </tr>
    </table>

    <div class="h_delim"></div>
    <br />

    <table class="editor_table">
        <tr>
            <td class="row_title" style="width: 146px"><%:Html.LabelFor(model => model.CostInCurrency)%>:</td>
            <td style="width: 201px">
                <%: Html.NumericTextBoxFor(model => model.CostInCurrency, new { maxlength = 19, size = 22 }, !Model.AllowToEditPaymentDependentFields)%>&nbsp;<span class="TransportSheetCurrencyLiteralCode"><%: Model.TransportSheetCurrencyLiteralCode%></span>
                <%if (Model.AllowToEditPaymentDependentFields) { %><%: Html.ValidationMessageFor(model => model.CostInCurrency)%><%} %>
            </td>
            <td class="row_title" style="width: 110px"><%:Html.LabelFor(model => model.CostInBaseCurrency)%>:</td>
            <td style="width: 103px">
                <span id="CostInBaseCurrency"><%: Model.CostInBaseCurrency%></span>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.BillOfLadingNumber)%>:</td>
            <td colspan="3">
                <%: Html.TextBoxFor(model => model.BillOfLadingNumber, new { maxlength = 100, style = "width: 98%" }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.BillOfLadingNumber)%>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.ShippingLine)%>:</td>
            <td colspan="3">
                <%: Html.TextBoxFor(model => model.ShippingLine, new { maxlength = 100, style = "width: 98%" }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.ShippingLine)%>
            </td>
        </tr>
    </table>
    <table class="editor_table">
        <tr>
            <td class="row_title" style="width: 146px"><%:Html.LabelFor(model => model.PortDocumentNumber)%>:</td>
            <td style="width: 156px">
                <%: Html.TextBoxFor(model => model.PortDocumentNumber, new { maxlength = 100, size = 22 }, !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.PortDocumentNumber)%>
            </td>
            <td class="row_title" style="width: 169px"><%:Html.LabelFor(model => model.PortDocumentDate)%>:</td>
            <td style="width: 89px">
                <%= Html.DatePickerFor(model => model.PortDocumentDate, isDisabled: !Model.AllowToEdit)%>
                <%: Html.ValidationMessageFor(model => model.PortDocumentDate)%>
            </td>
        </tr>
    </table>

    <div class="h_delim"></div>
    <br />

    <table class="editor_table">
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.PaymentSumInCurrency)%>:</td>
            <td>
                <%: Model.PaymentSumInCurrency%>&nbsp;<span class="TransportSheetCurrencyLiteralCode"><%: Model.TransportSheetCurrencyLiteralCode%></span>
            </td>
            <td class="row_title"><%:Html.LabelFor(model => model.PaymentSumInBaseCurrency)%>:</td>
            <td>
                <%: Model.PaymentSumInBaseCurrency%>&nbsp;р.
            </td>
            <td class="row_title"><%:Html.LabelFor(model => model.PaymentPercent)%>:</td>
            <td>
                <%: Model.PaymentPercent%>&nbsp;%
            </td>
        </tr>
        <tr>
            <td class="greytext" colspan="6"><%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:</td>
        </tr>
        <tr>
            <td colspan="6">                
                <%:Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 3)%>  
                <%:Html.ValidationMessageFor(model => model.Comment)%>              
            </td>
        </tr>
    </table>

    <div class="button_set">
        <%= Html.SubmitButton("btnSave", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

</div>
