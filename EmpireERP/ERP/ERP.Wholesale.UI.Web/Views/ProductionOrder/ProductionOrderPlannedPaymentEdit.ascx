<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPlannedPaymentEditViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<div style="width:408px;">

<% using (Ajax.BeginForm("SaveProductionOrderPlannedPayment", "ProductionOrder", new AjaxOptions()
   {
       OnFailure = "ProductionOrder_Details.OnFailProductionOrderPlannedPaymentEdit",
       OnSuccess = "ProductionOrder_Details.OnSuccessProductionOrderPlannedPaymentEdit",
       OnBegin = "ProductionOrder_Details.OnBeginProductionOrderPlannedPaymentEdit"
   }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.ProductionOrderPlannedPaymentId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.PlannedPaymentCurrencyRateValue)%>
    <%:Html.HiddenFor(model => model.PlannedPaymentCurrencyRateId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderPlannedPayment") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageProductionOrderPlannedPaymentEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.SumInCurrency)%>:</td>
                <td>
                    <%: Html.NumericTextBoxFor(model => model.SumInCurrency, new { maxlength = 19, size = 14 }, !Model.AllowToEditSum)%>
                    <%: Html.ValidationMessageFor(model => model.SumInCurrency)%>
                </td>
            </tr>
            <tr>
                <td class="row_title" style="width: 105px"><%:Html.LabelFor(model => model.PlannedPaymentStartDate)%>:</td>
                <td>
                        <%= Html.DateRangePicker_Begin("plannedPaymentStartDate", model => model.PlannedPaymentStartDate, isDisabled : !Model.AllowToEdit)%>
                        <%= Html.DateRangePicker_End(model => model.PlannedPaymentEndDate, label: "-", isDisabled: !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.PlannedPaymentStartDate)%>
                        <%: Html.ValidationMessageFor(model => model.PlannedPaymentEndDate)%>                  
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PlannedPaymentPurpose)%>:</td>
                <td>
                    <%: Html.TextBoxFor(model => model.PlannedPaymentPurpose, new { maxlength = 50, size = 45 }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.PlannedPaymentPurpose)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.ProductionOrderPaymentTypeId)%>:</td>
                <td>
                    <%: Html.DropDownListFor(model => model.ProductionOrderPaymentTypeId, Model.ProductionOrderPaymentTypeList, !Model.AllowToEditSum)%>
                    <%: Html.ValidationMessageFor(model => model.ProductionOrderPaymentTypeId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PlannedPaymentCurrencyId)%>:</td>
                <td>
                    <%: Html.DropDownListFor(model => model.PlannedPaymentCurrencyId, Model.PlannedPaymentCurrencyList, !Model.AllowToEditSum)%>
                    <%: Html.ValidationMessageFor(model => model.PlannedPaymentCurrencyId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PlannedPaymentCurrencyRateName)%>:</td>
                <td>
                    <span id="PlannedPaymentCurrencyRateName"><%: Model.PlannedPaymentCurrencyRateName%></span>&nbsp;=&nbsp;<span id="PlannedPaymentCurrencyRateString"><%: Model.PlannedPaymentCurrencyRateString%></span>&nbsp;р.

                    <% string allowToChangePlannedPaymentCurrencyRateDisplay = (Model.AllowToChangeCurrencyRate ? "inline" : "none"); %>

                    <span id='linkChangePlannedPaymentCurrencyRate' class="edit_action" style="display:<%= allowToChangePlannedPaymentCurrencyRateDisplay %>">[ Изменить ]</span>
                </td>
            </tr>
        </table>

        <div class="h_delim"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="width:50%">
                    <%:Html.LabelFor(model => model.CurrentPaymentSumInCurrency)%>:
                </td>
                <td style="width:50%">
                    <%: Model.CurrentPaymentSumInCurrency%>&nbsp;<span id="PlannedPaymentCurrencyLiteralCode"><%: Model.PlannedPaymentCurrencyLiteralCode%></span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%:Html.LabelFor(model => model.CurrentPaymentSumInBaseCurrency)%>:
                </td>
                <td>
                    <%: Model.CurrentPaymentSumInBaseCurrency%>&nbsp;р.
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
            <%= Html.SubmitButton("btnSaveProductionOrderPlannedPayment", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>

</div>
