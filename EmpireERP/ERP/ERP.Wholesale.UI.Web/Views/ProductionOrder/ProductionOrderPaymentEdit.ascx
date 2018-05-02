<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPaymentEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

 <script type="text/javascript">
     ProductionOrder_ProductionOrderPaymentEdit.Init();
</script>

<div style="width:460px;">

<% using (Ajax.BeginForm("SaveProductionOrderPayment", "ProductionOrder", new AjaxOptions()
   {
       OnBegin = "OnBeginProductionOrderPaymentEdit",
       OnFailure = "OnFailProductionOrderPaymentEdit", OnSuccess = "OnSuccessProductionOrderPaymentEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.ProductionOrderPaymentId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderPaymentTypeId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderPaymentDocumentId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderPlannedPaymentId)%>
    <%:Html.HiddenFor(model => model.PaymentCurrencyId)%>
    <%:Html.HiddenFor(model => model.PaymentCurrencyRateValue)%>
    <%:Html.HiddenFor(model => model.PaymentCurrencyRateId)%>
    <%:Html.HiddenFor(model => model.DebtRemainderInCurrencyValue)%>
    
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderPayment") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageProductionOrderPaymentEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="min-width: 160px"><%:Html.LabelFor(model => model.ProductionOrderName)%>:</td>
                <td style="width: 100%">
                    <%: Model.ProductionOrderName %>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ProductionOrderPaymentPurpose)%>:</td>
                <td>
                    <%: Model.ProductionOrderPaymentPurpose%>
                </td>
            </tr>
            <% if (Model.AllowToEdit) { %>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.DebtRemainderInCurrencyString)%>:</td>
                <td>
                    <%: Model.DebtRemainderInCurrencyString%>
                </td>
            </tr>
            <% } %>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentDocumentNumber)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.PaymentDocumentNumber, new { maxlength = 50, size = 45 }, !Model.AllowToEdit)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentDate)%>:</td>
                <td>
                    <%= Html.DatePickerFor(model => model.PaymentDate, isDisabled: !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.PaymentDate)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentCurrencyLiteralCode)%>:</td>
                <td>
                    <%: Model.PaymentCurrencyLiteralCode%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PaymentCurrencyRateName)%>:</td>
                <td>
                    <span id="PaymentCurrencyRateName"><%: Model.PaymentCurrencyRateName%></span>&nbsp;=&nbsp;<span id="PaymentCurrencyRateString"><%: Model.PaymentCurrencyRateString%></span>&nbsp;р.

                    <% string allowToChangePaymentCurrencyRateDisplay = (Model.AllowToChangeCurrencyRate ? "inline" : "none"); %>

                    <span id='linkChangePaymentCurrencyRate' class="edit_action" style="display:<%= allowToChangePaymentCurrencyRateDisplay %>">[ Изменить ]</span>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.SumInCurrency)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.SumInCurrency, new { maxlength = 19, size = 14 }, !Model.AllowToEdit)%>&nbsp;&nbsp;&nbsp;&nbsp;<span class="greytext">в р.:</span>&nbsp;&nbsp;<span id="SumInBaseCurrency"><%: Model.SumInBaseCurrency%></span>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.SumInCurrency)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ProductionOrderPaymentForm)%>:</td>
                <td>
                    <%: Html.DropDownListFor(model => model.ProductionOrderPaymentForm, Model.ProductionOrderPaymentFormList, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ProductionOrderPaymentForm)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ProductionOrderPlannedPaymentSumInCurrency)%>:</td>
                <td>
                    <span id="ProductionOrderPlannedPaymentSumInCurrency"><%: Model.ProductionOrderPlannedPaymentSumInCurrency%></span>&nbsp;<span class="ProductionOrderPlannedPaymentCurrencyLiteralCode"><%: Model.ProductionOrderPlannedPaymentCurrencyLiteralCode%></span>,
                    
                    <span class="greytext"><%:Html.LabelFor(model => model.ProductionOrderPlannedPaymentPaidSumInBaseCurrency)%>:</span>&nbsp;<span id="ProductionOrderPlannedPaymentPaidSumInBaseCurrency"><%: Model.ProductionOrderPlannedPaymentPaidSumInBaseCurrency%></span>&nbsp;р.

                    <span id="linkChangePlannedPayment" class="edit_action" style="display:<%= Model.AllowToEditPlannedPayment ? "inline" : "none" %>">[ Изменить ]</span>
                    <%: Html.ValidationMessageFor(model => model.ProductionOrderPlannedPaymentId)%>
                </td>
            </tr>
            <tr>
                <td class="greytext" colspan="2"><%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:</td>
            </tr>
            <tr>
                <td colspan="2">                    
                    <%:Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 3)%>      
                    <%:Html.ValidationMessageFor(model => model.Comment)%>                
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnSaveProductionOrderPayment", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

</div>
<div id="productionOrderPlannedPaymentSelector"></div>