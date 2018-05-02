<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderExtraExpensesSheetEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">

    function OnBeginProductionOrderExtraExpensesSheetEdit() {
        StartButtonProgress($("#btnProductionOrderExtraExpensesSheetEdit"));
    }
</script>

<div style="width:600px;">

<% using (Ajax.BeginForm("SaveProductionOrderExtraExpensesSheet", "ProductionOrder", new AjaxOptions() { OnBegin = "OnBeginProductionOrderExtraExpensesSheetEdit", 
       OnFailure = "OnFailProductionOrderExtraExpensesSheetEdit", OnSuccess = "OnSuccessProductionOrderExtraExpensesSheetEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.ExtraExpensesSheetId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.ExtraExpensesSheetCurrencyDeterminationTypeId)%>
    <%:Html.HiddenFor(model => model.ExtraExpensesSheetCurrencyRateForEdit)%>
    <%:Html.HiddenFor(model => model.ExtraExpensesSheetCurrencyRateId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderExtraExpensesSheet") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 0">
        <div id="messageProductionOrderExtraExpensesSheetEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="max-width: 85px"><%:Html.LabelFor(model => model.ProductionOrderName)%>:</td>
                <td style="min-width: 300px">
                    <%: Model.ProductionOrderName %>
                </td>
                <td class="row_title" style="min-width: 92px"><%:Html.LabelFor(model => model.ExtraExpensesSheetDate)%>:</td>
                <td style="max-width: 90px">
                    <%= Html.DatePickerFor(model => model.ExtraExpensesSheetDate, isDisabled: !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ExtraExpensesSheetDate)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ExtraExpensesContractorName)%>:</td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.ExtraExpensesContractorName, new { maxlength = 100, style = "width: 98%" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ExtraExpensesContractorName)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ExtraExpensesSheetCurrencyId)%>:</td>
                <td colspan="3">
                    <%: Html.DropDownListFor(model => model.ExtraExpensesSheetCurrencyId, Model.CurrencyList, !Model.AllowToChangeCurrency)%>
                    <%: Html.ValidationMessageFor(model => model.ExtraExpensesSheetCurrencyId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ExtraExpensesSheetCurrencyRateName)%>:</td>
                <td>
                    <span id="ExtraExpensesSheetCurrencyRateName"><%: Model.ExtraExpensesSheetCurrencyRateName%></span>&nbsp;=&nbsp;<span id="ExtraExpensesSheetCurrencyRate"><%: Model.ExtraExpensesSheetCurrencyRateForDisplay%></span>&nbsp;р.

                    <% string allowToChangeCurrencyRateDisplay = (Model.AllowToChangeCurrencyRate ? "inline" : "none"); %>

                    <span id='linkChangeCurrencyRate' class="edit_action" style="display:<%= allowToChangeCurrencyRateDisplay %>">[ Изменить ]</span>
                </td>
                <td class="row_title"></td>
                <td>
                </td>
            </tr>
        </table>

        <div class="h_delim"></div>
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width: 140px"><%:Html.LabelFor(model => model.CostInCurrency)%>:</td>
                <td style="width: 200px">
                    <%: Html.NumericTextBoxFor(model => model.CostInCurrency, new { maxlength = 19, size = 22 }, !Model.AllowToEditPaymentDependentFields)%>&nbsp;<span class="ExtraExpensesSheetCurrencyLiteralCode"><%: Model.ExtraExpensesSheetCurrencyLiteralCode%></span>
                    <%: Html.ValidationMessageFor(model => model.CostInCurrency)%>
                </td>
                <td class="row_title" style="width: 100px"><%:Html.LabelFor(model => model.CostInBaseCurrency)%>:</td>
                <td style="width: 120px">
                    <span id="CostInBaseCurrency"><%: Model.CostInBaseCurrency%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ExtraExpensesPurpose)%>:</td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.ExtraExpensesPurpose, new { maxlength = 100, style = "width: 98%" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ExtraExpensesPurpose)%>
                </td>
            </tr>
        </table>

        <div class="h_delim"></div>
        <br />

        <table class="editor_table">
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentSumInCurrency)%>:</td>
                <td>
                    <%: Model.PaymentSumInCurrency%>&nbsp;<span class="ExtraExpensesSheetCurrencyLiteralCode"><%: Model.ExtraExpensesSheetCurrencyLiteralCode%></span>
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
            <%= Html.SubmitButton("btnProductionOrderExtraExpensesSheetEdit", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

</div>
