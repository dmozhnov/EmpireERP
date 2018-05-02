<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderMainDetailsViewModel>" %>

<%: Html.HiddenFor(model => model.AccountOrganizationId) %>
<%: Html.HiddenFor(model => model.CuratorId) %>
<%: Html.HiddenFor(model => model.ProducerId) %>
<%: Html.HiddenFor(model => model.CurrencyId) %>
<%: Html.HiddenFor(model => model.CurrencyRateId) %>
<%: Html.HiddenFor(model => model.StorageId) %>
<%: Html.HiddenFor(model => model.AllowToViewStorageDetails) %>
<%: Html.HiddenFor(model => model.AllowToViewProducerDetails) %>
<%: Html.HiddenFor(model => model.AllowToViewCuratorDetails) %>
<%: Html.HiddenFor(model => model.AllowToViewStageList) %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style='min-width: 120px'>
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td style='width: 75%'>
             <a id="CuratorName"><%: Model.CuratorName%></a>

            <% string allowToChangeCuratorDisplay = (Model.AllowToChangeCurator ? "inline" : "none"); %>

            <span id='linkChangeCurator' class="main_details_action" style="display:<%= allowToChangeCuratorDisplay %>">[&nbsp;Изменить&nbsp;]</span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Date) %>:
        </td>
        <td style='width: 25%'>
            <span id="Date"><%:Model.Date %></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.ProducerName) %>:
        </td>
        <td>
            <% if (Model.AllowToViewProducerDetails)
               { %>
                <a id="ProducerName"><%:Model.ProducerName%></a>
            <%}
               else
               { %>
                <span id="ProducerName"><%:Model.ProducerName%></span>
            <%} %>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.DeliveryPendingDate, "/Help/GetHelp_ProductionOrder_Details_MainDetails_DeliveryPendingDate")%>:
        </td>
        <td>
            <span id="DeliveryPendingDate"><%:Model.DeliveryPendingDate%></span>
        </td>
    </tr>
    <tr>
        <% if (Model.IsSingleBatch) { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.CurrentStageName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_CurrentStageName")%>:
        </td>
        <td>
            <span id="CurrentStageName"><%:Model.CurrentStageName%></span>

            <% string allowToChangeBatchStageDisplay = (Model.AllowToChangeBatchStage ? "inline" : "none"); %>

            <span id='linkChangeStage' class="main_details_action" style="display:<%= allowToChangeBatchStageDisplay %>">[&nbsp;Изменить&nbsp;]</span>

            <%: Html.HiddenFor(model => model.SingleProductionOrderBatchId) %>
        </td>
        <% } else { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.State, "/Help/GetHelp_ProductionOrder_Details_MainDetails_State")%>:
        </td>
        <td>
            <span id="State"><%:Model.State%></span>
        </td>
        <% } %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.DivergenceFromPlan, "/Help/GetHelp_ProductionOrder_Details_MainDetails_DivergenceFromPlan")%>:
        </td>
        <td>
            <span id="DivergenceFromPlan"><%:Model.DivergenceFromPlan%></span>
        </td>
    </tr>
    <tr>
        <% if (Model.IsSingleBatch) { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.CurrentStageActualStartDate, "/Help/GetHelp_ProductionOrder_Details_MainDetails_CurrentStageActualStartDate")%>:
        </td>
        <td>
            <span id="CurrentStageActualStartDate" style="white-space: nowrap;"><%:Model.CurrentStageActualStartDate%></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="CurrentStageDaysPassed" style="white-space: nowrap;"><%:Model.CurrentStageDaysPassed%></span>
        </td>
        <% } else { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.MinOrderBatchStageName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_MinOrderBatchStageName")%>:
        </td>
        <td>
            <span id="MinOrderBatchStageName"><%:Model.MinOrderBatchStageName%></span>
        </td>
        <% } %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.PlannedExpensesSumInCurrency, "/Help/GetHelp_ProductionOrder_Details_MainDetails_PlannedExpensesSumInCurrency")%>:
        </td>
        <td>
            <b><span id="PlannedExpensesSumInCurrency" style="white-space: nowrap;"><%:Model.PlannedExpensesSumInCurrency%></span></b>&nbsp;&nbsp;||&nbsp;&nbsp;<b><span id="PlannedExpensesSumInBaseCurrency" style="white-space: nowrap;"><%:Model.PlannedExpensesSumInBaseCurrency%></span>&nbsp;р.</b>            

            <%:Html.HiddenFor(model => model.PlannedExpensesSumInCurrencyValue) %>
            <%:Html.HiddenFor(model => model.PlannedExpensesSumInBaseCurrencyValue) %>
        </td>
    </tr>
    <tr>
        <% if (Model.IsSingleBatch) { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.CurrentStageExpectedEndDate, "/Help/GetHelp_ProductionOrder_Details_MainDetails_CurrentStageExpectedEndDate")%>:
        </td>
        <td>
            <span id="CurrentStageExpectedEndDate"><%:Model.CurrentStageExpectedEndDate%></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="CurrentStageDaysLeft"><%:Model.CurrentStageDaysLeft%></span>
        </td>
        <% } else { %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.MaxOrderBatchStageName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_MaxOrderBatchStageName")%>:
        </td>
        <td>
            <span id="MaxOrderBatchStageName"><%:Model.MaxOrderBatchStageName%></span>
        </td>
        <% } %>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.ActualCostSumInCurrency, "/Help/GetHelp_ProductionOrder_Details_MainDetails_ActualCostSumInCurrency")%>:
        </td>
        <td>
            <b><span id="ActualCostSumInCurrency" style="white-space: nowrap;"><%:Model.ActualCostSumInCurrency%></span></b>&nbsp;&nbsp;||&nbsp;&nbsp;<b><span id="ActualCostSumInBaseCurrency" style="white-space: nowrap;"><%:Model.ActualCostSumInBaseCurrency%></span>&nbsp;р.</b>
            <%:Html.HiddenFor(model => model.ActualCostSumInCurrencyValue)%>
            <%:Html.HiddenFor(model => model.ActualCostSumInBaseCurrencyValue)%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.ContractName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_ContractName")%>:
        </td>
        <td>
            <span id="ContractName"><%:Model.ContractName%></span>

            <%  string allowToCreateContractDisplay = (Model.AllowToCreateContract ? "inline" : "none");
                string allowToEditContractDisplay = (Model.AllowToEditContract ? "inline" : "none");
            %>

            <span id='linkCreateContract' class="main_details_action" style="display:<%= allowToCreateContractDisplay %>">[&nbsp;Создать&nbsp;]</span>
            <span id='linkEditContract' class="main_details_action" style="display:<%= allowToEditContractDisplay %>">[&nbsp;Изменить&nbsp;]</span>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.PaymentSumInCurrency, "/Help/GetHelp_ProductionOrder_Details_MainDetails_PaymentSumInCurrency")%>:
        </td>
        <td>
            <b><span id="PaymentSumInCurrency" style="white-space: nowrap;"><%:Model.PaymentSumInCurrency%></span></b>&nbsp;&nbsp;||&nbsp;&nbsp;<b><span id="PaymentSumInBaseCurrency" style="white-space: nowrap;"><%:Model.PaymentSumInBaseCurrency%></span>&nbsp;р.</b>
            <%:Html.HiddenFor(model => model.PaymentSumInCurrencyValue)%>
            <%:Html.HiddenFor(model => model.PaymentSumInBaseCurrencyValue)%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.AccountOrganizationName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_AccountOrganizationName")%>:
        </td>
        <td>
            <span id="AccountOrganizationName">
                <%if (Model.AccountOrganizationId != "") { %><a id="AccountOrganizationLink"><% } %>
                    <%:Model.AccountOrganizationName%>
                <%if (Model.AccountOrganizationId != "") { %></a><% } %>
            </span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PaymentPercent) %>:
        </td>
        <td>
            <span id="PaymentPercent"><%:Model.PaymentPercent%></span> %
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.CurrencyLiteralCode, "/Help/GetHelp_ProductionOrder_Details_MainDetails_CurrencyLiteralCode")%>:
        </td>
        <td>
            <span id="CurrencyLiteralCode"><%:Model.CurrencyLiteralCode %></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span class="greytext"><%:Html.LabelFor(model => model.CurrencyRate)%>:</span>&nbsp;<span id="CurrencyRateName"><%:Model.CurrencyRateName%></span>&nbsp;=&nbsp;<span id="CurrencyRate"><%:Model.CurrencyRate%></span>&nbsp;р.

            <% string allowToChangeCurrencyRateDisplay = (Model.AllowToChangeCurrencyRate ? "inline" : "none"); %>

            <span id='linkChangeCurrencyRate' class="main_details_action" style="display:<%= allowToChangeCurrencyRateDisplay %>">[&nbsp;Изменить&nbsp;]</span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.WorkDaysPlanString)%>:
        </td>
        <td>
            <%:Model.WorkDaysPlanString%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.StorageName, "/Help/GetHelp_ProductionOrder_Details_MainDetails_StorageName")%>:
        </td>
        <td>
            <span id="StorageName">
                <%if (Model.StorageId != "" && Model.AllowToViewStorageDetails) { %><a id="StorageLink"><% } %>
                    <%:Model.StorageName%>
                <%if (Model.StorageId != "") { %></a><% } %>
            </span>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.AccountingPriceSum, "/Help/GetHelp_ProductionOrder_Details_MainDetails_AccountingPriceSum")%>:
        </td>
        <td>
            <span id="AccountingPriceSum"><%:Model.AccountingPriceSum%></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.ArticleTransportingPrimeCostCalculationType, "/Help/GetHelp_ProductionOrder_Details_MainDetails_ArticleTransportingPrimeCostCalculationType")%>:
        </td>
        <td>
            <span id="ArticleTransportingPrimeCostCalculationType"><%:Model.ArticleTransportingPrimeCostCalculationType%></span>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.MarkupPendingSum, "/Help/GetHelp_ProductionOrder_Details_MainDetails_MarkupPendingSum")%>:
        </td>
        <td>
            <span id="MarkupPendingSum"><%:Model.MarkupPendingSum%></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Comment)%>:
        </td>
        <td colspan='3'>
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>

<div id="ProductionOrderPlannedExpensesEdit"></div>
