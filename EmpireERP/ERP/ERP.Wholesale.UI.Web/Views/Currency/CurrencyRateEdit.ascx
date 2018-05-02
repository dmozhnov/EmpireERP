<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Currency.CurrencyRateEditViewModel>" %>

<script type="text/javascript" src="/Scripts/DatePicker.js"></script>
<script type="text/javascript">
    function OnBeginCurrencyRateEdit(ajaxContext) {
        StartButtonProgress($("#btnSaveCurrencyRate"));
    }

    function OnSuccessCurrencyRateEdit(ajaxContext) {
        CurrencyRate_Edit.OnSuccessCurrencyRateEdit(ajaxContext);
    }

    function OnFailCurrencyRateEdit(ajaxContext) {
        CurrencyRate_Edit.OnFailCurrencyRateEdit(ajaxContext);
    }
</script>
<% using (Ajax.BeginForm("SaveRate", "Currency", new AjaxOptions()
   {
       OnBegin = "OnBeginCurrencyRateEdit",
       OnSuccess = "OnSuccessCurrencyRateEdit",
       OnFailure = "OnFailCurrencyRateEdit"
   }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.CurrencyId)%>
    <%: Html.HiddenFor(model => model.CurrencyRateId)%>

    <div class="modal_title"><%: Model.Title%><%: Html.Help("/Help/GetHelp_Currency_Edit_CurrencyRate")%></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px; width: 370px;">
        <div id="messageCurrencyRateEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.Rate)%>:</td>
                <td style="width: 200px;">
                    <%: Html.TextBoxFor(model => model.Rate, new { maxlength = 19, style = "width:163px" }, !Model.AllowToEditCurrencyRate)%>&nbsp;р.
                    <%: Html.ValidationMessageFor(model => model.Rate)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.StartDate)%>:</td>
                <td>
                    <%: Html.DatePickerFor(model => model.StartDate)%>
                    <%: Html.ValidationMessageFor(model => model.StartDate)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnSaveCurrencyRate", "Сохранить")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />
        </div>
    </div>
<%} %>