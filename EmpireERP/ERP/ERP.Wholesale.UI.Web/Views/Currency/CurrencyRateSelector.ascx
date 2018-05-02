<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Currency.SelectCurrencyRateViewModel>" %>

<script src="../../Scripts/DatePicker.min.js" type="text/javascript"></script>
<script src="../../Scripts/DatePicker.js" type="text/javascript"></script>

<script type="text/javascript">
    CurrencyRate_Selector.Init();
</script>

<%:Html.HiddenFor(model => model.SelectFunctionName)%>

<div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Currency_Select") %></div>
    
<div style="padding: 10px 10px 5px; width: 776px;">
    <div id="messageCurrencyEdit"></div>

    <%= Html.GridFilterHelper("currencyRateFilter", Model.Filter, new List<string>() { "gridCurrencyRateSelector" }, true) %>
    
    <div style="padding-left: 40px">
        <span class="select_link selectCurrentRate">Выбрать текущий курс</span>
    </div>
    <br />

    <div id="messageCurrencyRateSelectList"></div>

    <% Html.RenderPartial("CurrencyRateSelectGrid", Model.CurrencyRateGrid); %>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal();" />
    </div>
</div>