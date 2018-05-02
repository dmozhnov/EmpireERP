<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPlannedExpensesEditViewModel>" %>

<script type="text/javascript">
    $(document).ready(function () {
        ReCalcProductionExpensesInCurrency($("#ProductionExpensesInCurrency").val());
        ReCalcTransportationExpensesInCurrency($("#TransportationExpensesInCurrency").val());
        ReCalcExtraExpensesInCurrency($("#ExtraExpensesInCurrency").val());
        ReCalcCustomsExpensesInCurrency($("#CustomsExpensesInCurrency").val());

        $("#ProductionExpensesInCurrency").blur(function () {
            var val = parseFloat($(this).val());
            ReCalcProductionExpensesInCurrency(val);
        });

        $("#TransportationExpensesInCurrency").blur(function () {
            var val = parseFloat($(this).val());
            ReCalcTransportationExpensesInCurrency(val);
        });

        $("#ExtraExpensesInCurrency").blur(function () {
            var val = parseFloat($(this).val());
            ReCalcExtraExpensesInCurrency(val);
        });

        $("#CustomsExpensesInCurrency").blur(function () {
            var val = parseFloat($(this).val());
            ReCalcCustomsExpensesInCurrency(val);
        });
    });

    function OnBeginPlannedExpensesEdit() {
        StartButtonProgress($("#btnSave"));
    }

    function ReCalcProductionExpensesInCurrency(val) {
        var tmp = $("#ProductionOrderPlannedExpensesEdit #CurrencyRate").val().replaceAll(",", ".");
        var rate = parseFloat(tmp);

        if (isNaN(rate) || isNaN(val)) {
            $("#ProductionExpensesInBaseCurrency").text("---");
        }
        else {
            $("#ProductionExpensesInBaseCurrency").text(ValueForDisplay(val * rate,2));
        }
    }

    function ReCalcTransportationExpensesInCurrency(val) {
        var tmp = $("#ProductionOrderPlannedExpensesEdit #CurrencyRate").val().replaceAll(",", ".");
        var rate = parseFloat(tmp);

        if (isNaN(rate) || isNaN(val)) {
            $("#TransportationExpensesInBaseCurrency").text("---");
        }
        else {
            $("#TransportationExpensesInBaseCurrency").text(ValueForDisplay(val * rate,2));
        }
    }

    function ReCalcExtraExpensesInCurrency(val) {
        var tmp = $("#ProductionOrderPlannedExpensesEdit #CurrencyRate").val().replaceAll(",", ".");
        var rate = parseFloat(tmp);

        if (isNaN(rate) || isNaN(val)) {
            $("#ExtraExpensesInBaseCurrency").text("---");
        }
        else {
            $("#ExtraExpensesInBaseCurrency").text(ValueForDisplay(val * rate,2));
        }
    }

    function ReCalcCustomsExpensesInCurrency(val) {
        var tmp = $("#ProductionOrderPlannedExpensesEdit #CurrencyRate").val().replaceAll(",", ".");
        var rate = parseFloat(tmp);

        if (isNaN(rate) || isNaN(val)) {
            $("#CustomsExpensesInBaseCurrency").text("---");
        }
        else {
            $("#CustomsExpensesInBaseCurrency").text(ValueForDisplay(val * rate,2));
        }
    }

    function OnFailPlannedExpensesEdit(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderPlannedExpensesEdit");
    }
</script>

<% using (Ajax.BeginForm("SavePlannedExpenses", "ProductionOrder", new AjaxOptions() { OnBegin = "OnBeginPlannedExpensesEdit", OnSuccess = "ProductionOrder_Details.OnSuccessPlannedExpensesEdit", OnFailure = "OnFailPlannedExpensesEdit" }))%>
<%{ %>        
        <%:Html.HiddenFor(model => model.ProductionOrderId)%>
        <%:Html.HiddenFor(model => model.CurrencyRate)%>
        
        <div class="modal_title"><%: Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderPlannedExpenses") %></div>
        <div class="h_delim"></div>
        <div style='padding: 10px 10px 5px;'>
            <div id="messageProductionOrderPlannedExpensesEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.ProductionExpensesInCurrency)%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.ProductionExpensesInCurrency, new { maxlength = 15, style="width: 100px" }, !Model.AllowToEdit)%> <%: Model.Currency %>
                        <%: Html.ValidationMessageFor(model=>model.ProductionExpensesInCurrency) %>
                    </td>
                    <td class="row_title" style="width:70px">
                        В рублях:
                    </td>
                    <td style="min-width:80px">
                        <span id="ProductionExpensesInBaseCurrency" style="whitespace:no-wrap">---</span> р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.TransportationExpensesInCurrency)%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.TransportationExpensesInCurrency, new { maxlength = 15, style = "width: 100px" }, !Model.AllowToEdit)%> <%: Model.Currency %>
                        <%: Html.ValidationMessageFor(model => model.TransportationExpensesInCurrency)%>
                    </td>
                    <td class="row_title">
                        В рублях:
                    </td>
                    <td>
                        <span id="TransportationExpensesInBaseCurrency" style="whitespace:no-wrap">---</span> р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.ExtraExpensesInCurrency)%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.ExtraExpensesInCurrency, new { maxlength = 15, style = "width: 100px" }, !Model.AllowToEdit)%> <%: Model.Currency %>
                        <%: Html.ValidationMessageFor(model => model.ExtraExpensesInCurrency)%>
                    </td>
                    <td class="row_title">
                        В рублях:
                    </td>
                    <td>
                        <span id="ExtraExpensesInBaseCurrency" style="whitespace:no-wrap">---</span> р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.CustomsExpensesInCurrency)%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.CustomsExpensesInCurrency, new { maxlength = 15, style = "width: 100px" }, !Model.AllowToEdit)%> <%: Model.Currency %>
                        <%: Html.ValidationMessageFor(model => model.CustomsExpensesInCurrency)%>
                    </td>
                    <td class="row_title">
                        В рублях:
                    </td>
                    <td>
                        <span id="CustomsExpensesInBaseCurrency" style="whitespace:no-wrap">---</span> р.
                    </td>
                </tr>
            </table>

            <br />
            <div class="group_title">По плану платежей</div>
            <div class="h_delim"></div>
            <br />

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="width: 170px;">
                        <%: Html.LabelFor(model => model.PlannedProductionPaymentsInBaseCurrency)%>:
                    </td>
                    <td>
                        <%: Model.PlannedProductionPaymentsInBaseCurrency%>&nbsp;р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.PlannedTransportationPaymentsInBaseCurrency)%>:
                    </td>
                    <td>
                        <%: Model.PlannedTransportationPaymentsInBaseCurrency%>&nbsp;р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.PlannedExtraExpensesPaymentsInBaseCurrency)%>:
                    </td>
                    <td>
                        <%: Model.PlannedExtraExpensesPaymentsInBaseCurrency%>&nbsp;р.
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.LabelFor(model => model.PlannedCustomsPaymentsInBaseCurrency)%>:
                    </td>
                    <td>
                        <%: Model.PlannedCustomsPaymentsInBaseCurrency%>&nbsp;р.
                    </td>
                </tr>
            </table>

            <div class="button_set">
                <%= Html.SubmitButton("btnSave", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>
                <input type="button" value="Закрыть" onclick="HideModal()" />
            </div>

    </div>
<% }%>