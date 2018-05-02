<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Currency.CurrencyEditViewModel>" %>

<script type="text/javascript">
    Currency_Edit.Init();

    function OnSuccessCurrencyEdit(ajaxContext) {
        Currency_Edit.OnSuccessCurrencyEdit(ajaxContext);
    }

    function OnFailCurrencyEdit(ajaxContext) {
        Currency_Edit.OnFailCurrencyEdit(ajaxContext);
    }
</script>

<% using (Ajax.BeginForm("Save", "Currency", new AjaxOptions() { OnSuccess = "OnSuccessCurrencyEdit", OnFailure = "OnFailCurrencyEdit" }))%>
<%{ %>
    
    <%: Html.HiddenFor(model => model.IsNew) %>
    <%: Html.HiddenFor(model => model.CurrencyId) %>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Currency_Edit_Currency") %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 0px 5px 10px; width: 776px;">
        <div id="messageCurrencyEdit"></div>

        <table class='editor_table' style="border: 1px !important">
            <tr>
                <td class='row_title' style="width: 50%"><%: Html.LabelFor(model => model.Name) %>:</td>
                <td colspan='3' style="min-width: 300px">
                    <%: Html.TextBoxFor(model => model.Name, new { maxlength = 20, style="width: 100%" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name) %>
                </td>
                <td style="width: 50%"></td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.LiteralCode) %>:</td>
                <td style="width: 120px;">
                    <%: Html.TextBoxFor(model => model.LiteralCode, new { maxlength = 3, style = "width: 100%" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.LiteralCode)%>
                </td>

                <td class='row_title' style="width: 60px;"><%: Html.LabelFor(model => model.NumericCode) %>:</td>
                <td style="width: 120px;">
                    <%: Html.TextBoxFor(model => model.NumericCode, new { maxlength = 3, style = "width: 100%" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.NumericCode)%>
                </td>
                <td></td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnSaveReceiptWaybillRow", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>          
        </div>
    </div>
<%} %>

<div style="padding: 10px 10px 5px 10px;" id="currentRateGrid" >
    <div id="messageCurrentRateGrid"></div>
    <div style="max-height: 320px; width: 770px; overflow: auto;">
        <% Html.RenderPartial("CurrencyRateGrid", Model.CurrencyRateGrid); %>
    </div>
</div>

<div class="button_set">
    <input type="button" value="Закрыть" onclick="HideModal()" />
</div>

<div id="currencyRateEdit"></div>