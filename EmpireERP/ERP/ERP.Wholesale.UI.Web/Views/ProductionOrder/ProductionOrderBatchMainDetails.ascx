<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchMainDetailsViewModel>" %>

<style type="text/css">
span.green_option {
    color: #359935;
	font-weight: bold;
}
span.grey_option {
    color: #BBB;
	font-weight: bold;
}
</style>

<%: Html.HiddenFor(model => model.CuratorId) %>
<%: Html.HiddenFor(model => model.ProducerId) %>
<%: Html.HiddenFor(model => model.ProductionOrderId) %>
<%: Html.HiddenFor(model => model.ReceiptWaybillId) %>
<%: Html.HiddenFor(model => model.AllowToViewCuratorDetails) %>
<%: Html.HiddenFor(model => model.AllowToViewReceiptWaybillDetails)%>
<%: Html.HiddenFor(model => model.AllowToViewStageList)%>

<table class="main_details_table">
    <tr>
        <td class="row_title" style='min-width: 120px'>
            <%:Html.LabelFor(model => model.StateName) %>:
        </td>
        <td style='width: 75%'>
            <% string approvementStateDisplay = (Model.IsApprovementState ? "inline" : "none"); %>
            <% string isApprovedByLineManagerStyle = (Model.IsApprovedByLineManager ? "green_option" : "grey_option"); %>
            <% string isApprovedByFinancialDepartmentStyle = (Model.IsApprovedByFinancialDepartment ? "green_option" : "grey_option"); %>
            <% string isApprovedBySalesDepartmentStyle = (Model.IsApprovedBySalesDepartment ? "green_option" : "grey_option"); %>
            <% string isApprovedByAnalyticalDepartmentStyle = (Model.IsApprovedByAnalyticalDepartment ? "green_option" : "grey_option"); %>
            <% string isApprovedByProjectManagerStyle = (Model.IsApprovedByProjectManager ? "green_option" : "grey_option"); %>

            <span id="StateName" class="bold"><%:Model.StateName %></span><span id="ApprovementState" class="bold" style="display:<%= approvementStateDisplay %>">:
                [
                <span id="IsApprovedByLineManager" class="<%= isApprovedByLineManagerStyle %>"><%:Html.LabelFor(model => model.IsApprovedByLineManager) %></span>
                |
                <span id="IsApprovedByFinancialDepartment" class="<%= isApprovedByFinancialDepartmentStyle %>"><%:Html.LabelFor(model => model.IsApprovedByFinancialDepartment)%></span>
                |
                <span id="IsApprovedBySalesDepartment" class="<%= isApprovedBySalesDepartmentStyle %>"><%:Html.LabelFor(model => model.IsApprovedBySalesDepartment)%></span>
                |
                <span id="IsApprovedByAnalyticalDepartment" class="<%= isApprovedByAnalyticalDepartmentStyle %>"><%:Html.LabelFor(model => model.IsApprovedByAnalyticalDepartment)%></span>
                |
                <span id="IsApprovedByProjectManager" class="<%= isApprovedByProjectManagerStyle %>"><%:Html.LabelFor(model => model.IsApprovedByProjectManager)%></span>
                ]
            </span>
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
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td>
            <a id="CuratorName"><%:Model.CuratorName %></a>
        </td>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.ProducingPendingDate) %>:
        </td>
        <td valign="bottom">
            <span id="ProducingPendingDate"><%:Model.ProducingPendingDate%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.ProductionOrderName) %>:
        </td>
        <td valign="bottom">
            <a id="ProductionOrderName"><%:Model.ProductionOrderName%></a>
        </td>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.DeliveryPendingDate) %>:
        </td>
        <td valign="bottom">
            <span id="DeliveryPendingDate"><%:Model.DeliveryPendingDate%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.ProducerName) %>:
        </td>
        <td>
            <a id="ProducerName"><%:Model.ProducerName%></a>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.DivergenceFromPlan) %>:
        </td>
        <td>
            <span id="DivergenceFromPlan"><%:Model.DivergenceFromPlan%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.CurrentStageName)%>:
        </td>
        <td valign="bottom">
            <span id="CurrentStageName"><%:Model.CurrentStageName%></span>

            <% string allowToChangeStageDisplay = (Model.AllowToChangeStage ? "inline" : "none"); %>

            <span id="linkChangeStage" class="main_details_action" style="display:<%= allowToChangeStageDisplay %>">[ Изменить ]</span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Weight)%>:
        </td>
        <td>
            <span id="Weight"><%:Model.Weight%></span>&nbsp;кг
        </td>
    </tr>
    <tr>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.CurrentStageActualStartDate)%>:
        </td>
        <td valign="bottom">
            <span id="CurrentStageActualStartDate"><%:Model.CurrentStageActualStartDate%></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="CurrentStageDaysPassed"><%:Model.CurrentStageDaysPassed%></span>
        </td>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.Volume) %>:
        </td>
        <td valign="bottom">
            <span id="Volume"><%:Model.Volume%></span>&nbsp;м<sup>3</sup>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CurrentStageExpectedEndDate)%>:
        </td>
        <td>
            <span id="CurrentStageExpectedEndDate"><%:Model.CurrentStageExpectedEndDate%></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="CurrentStageDaysLeft"><%:Model.CurrentStageDaysLeft%></span>
        </td>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.ProductionCostSumInCurrency)%>:
        </td>
        <td valign="bottom">
            <span id="ProductionCostSumInCurrency"><%:Model.ProductionCostSumInCurrency%></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="ProductionCostSumInBaseCurrency"><%:Model.ProductionCostSumInBaseCurrency%></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.ContainerPlacement)%>:
        </td>
        <td valign="bottom">
            <span id="ContainerPlacement"><%:Model.ContainerPlacement%></span> (своб.: <%:Model.ContainerPlacementFreeVolume%>&nbsp;м<sup>3</sup>)

            <% string allowToRecalculatePlacementDisplay = (Model.AllowToRecalculatePlacement ? "inline" : "none"); %>

            <%--<span id="linkRecalculatePlacement" class="main_details_action" style="display:<%= allowToRecalculatePlacementDisplay %>">[ Изменить ]</span>--%>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.AccountingPriceSum) %>:
        </td>
        <td>
            <span id="AccountingPriceSum"><%:Model.AccountingPriceSum%></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CurrencyLiteralCode)%>:
        </td>
        <td>
            <span id="CurrencyLiteralCode"><%:Model.CurrencyLiteralCode %></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span class="greytext"><%:Html.LabelFor(model => model.CurrencyRate)%>:</span>&nbsp;<span id="CurrencyRateName"><%:Model.CurrencyRateName%></span>&nbsp;=&nbsp;<span id="CurrencyRate"><%:Model.CurrencyRate%></span>&nbsp;р.
        </td>
        <td class="row_title" valign="bottom">
            <%:Html.LabelFor(model => model.ReceiptWaybillName)%>:
        </td>
        <td valign="bottom">
            <span id="receiptWaybillLink">
                <% if (Model.ReceiptWaybillId != "" && Model.ReceiptWaybillId != Guid.Empty.ToString()) { %><a id="ReceiptWaybillName"><% } %>
                <%:Model.ReceiptWaybillName%>
                <% if (Model.ReceiptWaybillId != "" && Model.ReceiptWaybillId != Guid.Empty.ToString()) { %></a><% } %>
            </span>

            <% string allowToCreateReceiptWaybillDisplay = (Model.AllowToCreateReceiptWaybill ? "inline" : "none"); %>
            <% string allowToDeleteReceiptWaybillDisplay = (Model.AllowToDeleteReceiptWaybill ? "inline" : "none"); %>

            <span id="linkCreateReceiptWaybill" class="main_details_action" style="display:<%= allowToCreateReceiptWaybillDisplay %>">[ Создать ]</span>
            <span id="linkDeleteReceiptWaybill" class="main_details_action no_auto_progress" style="display:<%= allowToDeleteReceiptWaybillDisplay %>">[ Удалить ]</span>
        </td>
    </tr>
</table>

