<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountingPriceList.AccountingPriceListMainDetailsViewModel>" %>

<script type="text/javascript">
    AccountingPriceList_MainDetails.Init();
</script>

<%: Html.HiddenFor(model => model.ReasonReceiptWaybillId) %>
<%: Html.HiddenFor(model => model.AllowToViewReceiptWaybillDetails) %>

<%: Html.HiddenFor(x => x.CuratorId) %>
<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style="min-width: 150px;">
            <%: Html.LabelFor(model => model.StateDescription) %>:
        </td>
        <td style="width: 65%;">
            <span id="priceListDetailsState" class="bold"><%: Model.StateDescription%></span>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.PurchaseCostSum) %>:
        </td>
        <td style="width: 35%;">
            <span id="priceListDetailsPurchaseCostSum"><%: Model.PurchaseCostSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ReasonDescription) %>:
        </td>
        <td>
            <span id="priceListDetailsReason">
                <a id="ReasonDescription"><%: Model.ReasonDescription %></a>
               
            </span>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.OldAccountingPriceSum) %>:
        </td>
        <td>
            <span id="priceListDetailsOldAccountingPriceSum"><%: Model.OldAccountingPriceSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.StartDate) %>:
        </td>
        <td>
            <span id="StartDate"><%: Model.StartDate%></span>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.NewAccountingPriceSum) %>:
        </td>
        <td>
            <span id="priceListDetailsNewAccountingPriceSum"><%: Model.NewAccountingPriceSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.EndDate) %>:
        </td>
        <td>
            <span id="EndDate"><%: Model.EndDate%></span>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.AccountingPriceDifSum) %>:
        </td>
        <td>
            <span id="priceListDetailsAccountingPriceDifPercent"><%: Model.AccountingPriceDifPercent%></span> % &nbsp;||&nbsp; <span id="priceListDetailsAccountingPriceDifSum"><%: Model.AccountingPriceDifSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.CuratorName)%>:
        </td>
        <td>
            <a id="CuratorName"><%: Model.CuratorName %></a>
        </td>        
        <td class="row_title">
            <%: Html.LabelFor(model => model.PurchaseMarkupSum) %>:
        </td>
        <td>
            <span id="priceListDetailsPurchaseMarkupPercent"><%: Model.PurchaseMarkupPercent%></span> % &nbsp;||&nbsp; <span id="priceListDetailsPurchaseMarkupSum"><%: Model.PurchaseMarkupSum%></span> р.
        </td>
    </tr>
    <tr>     
        <td></td>
        <td></td>   
        <td class="row_title">
            <%: Html.LabelFor(model => model.RowCount) %>:
        </td>
        <td>
            <span id="priceListDetailsRowCount"><%: Model.RowCount %></span>
        </td>
    </tr>
</table>
