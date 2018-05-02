<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderPaymentTypeSelectorViewModel>"%>

<div style="width:310px;">

    <div class="modal_title"><%: Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Select_ProductionOrderPaymentType") %></div>
    <div class="h_delim"></div>
    <br />

    <div style='padding: 0px 10px 5px 35px;'>
        <div id="messageProductionOrderPaymentTypeSelect"></div>

        <span id="linkProduction" class="select_link">Оплата за производство товаров</span>
        <br /><br />
        <span id="linkTransportation" class="select_link">Оплата транспортных листов</span>
        <br /><br />
        <span id="linkExtraExpenses" class="select_link">Оплата листов дополнительных расходов</span>
        <br /><br />
        <span id="linkCustoms" class="select_link">Оплата таможенных пошлин и налогов</span>
        <br />
    </div>

    <div class="button_set">
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>

</div>
