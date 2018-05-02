<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchGridViewModel>" %>

<script type="text/javascript">
    ProductionOrder_Details_ProductionOrderBatchGrid.Init();
</script>

<div id="messageAddProductionOrderBatch"></div>

<div class="production_order_batch"><%:Model.TitleGrid%><%: Html.Help("/Help/GetHelp_ProductionOrder_Details_ProductionOrderBatchGrid") %>
    <% if (Model.AllowToAddBatch) 
       { %>
            <div style="margin-left: 20px; display: inline-block;"> <%= Html.Button("btnCreateNewOrderBatch", "Новая партия")%></div> 
    <% } %>
</div>

<table class="production_order_batch" style="width:99%">
    <tr>
        <% if (Model.Rows.Any(x => x.AllowToViewDetails))
           { %> 
        <th style="min-width:70px">Действие</th><%} %>
        <th><%:Html.LabelFor(model => Model.Rows.First().Name)%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().Date)%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().Volume)%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().ProductionCostSumInCurrency)%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().AccountingPriceSum)%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().PlannedProducingEndDate)%></th>
        <th><%:Model.Rows.First().StageHeader%></th>
        <th><%:Html.LabelFor(model => Model.Rows.First().ReceiptWaybillName)%></th>
    </tr>

    <% foreach (var batch in Model.Rows) { %>

    <tr style="height: 25px;">
        <% if(batch.AllowToViewDetails) {%>
            <td>
                <b><a class="linkDetails">Детали</a></b>
                <input type="hidden" class="Id" value="<%:batch.Id%>" />
            </td>
        <%} %>
        <td><%:batch.Name%></td>
        <td><%:batch.Date%></td>
        <td><%:batch.Volume%>&nbsp;м<sup>3</sup></td>
        <td><%:batch.ProductionCostSumInCurrency%>&nbsp;<%:batch.CurrencyLiteralCode%></td>
        <td><%:batch.AccountingPriceSum%>&nbsp;р.</td>
        <td><%:batch.PlannedProducingEndDate%></td>
        <td><%:batch.StageName%></td>
        <td>
            <% if (batch.ReceiptWaybillId != "" && batch.ReceiptWaybillId != Guid.Empty.ToString()) { %><a class="ReceiptWaybillName"><% } %>
            <%:batch.ReceiptWaybillName%>
            <% if (batch.ReceiptWaybillId != "" && batch.ReceiptWaybillId != Guid.Empty.ToString()) { %></a><% } %>
            <input type="hidden" class="ReceiptWaybillId" value="<%:batch.ReceiptWaybillId%>" />
        </td>
    </tr>

    <% } %>

</table>
