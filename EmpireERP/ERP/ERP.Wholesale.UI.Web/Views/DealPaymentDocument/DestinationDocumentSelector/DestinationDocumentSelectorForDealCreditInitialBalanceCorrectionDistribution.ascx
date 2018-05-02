<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DestinationDocumentSelectForDealCreditInitialBalanceCorrectionDistributionViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution.Init();
</script>

<div style="width: 850px; padding: 0 10px 0;">
<% using (Ajax.BeginForm(Model.DestinationDocumentSelectorActionName, Model.DestinationDocumentSelectorControllerName, new AjaxOptions()
    {
        OnBegin = "DealPaymentDocument_DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution.OnBeginDealCreditInitialBalanceCorrectionSave",
        OnFailure = "DealPaymentDocument_DestinationDocumentSelectorForDealCreditInitialBalanceCorrectionDistribution.OnFailDealCreditInitialBalanceCorrectionSave",
        OnSuccess = "OnSuccessDealCreditInitialBalanceCorrectionSave"
    }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.DealPaymentDocumentId)%>
    <%:Html.HiddenFor(model => model.SumValue)%>
    <%:Html.HiddenFor(model => model.UndistributedSumValue)%>
    <%:Html.HiddenFor(model => model.CurrentOrdinalNumber)%>
    <%:Html.HiddenFor(model => model.DistributionInfo)%>

    <%:Html.HiddenFor(model => model.DealId)%>
    <%:Html.HiddenFor(model => model.CorrectionReason)%>
    <%:Html.HiddenFor(model => model.Date)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments") %></div>
    <br />

    <table class="display_table">
        <tr>
            <td class="row_title" style="width: 160px">
                <%:Html.LabelFor(model => model.DealName)%>:
            </td>
            <td style="min-width: 200px">
                <%:Model.DealName%>
            </td>
            <td class="row_title">
                <%:Html.HelpLabelFor(model => model.SumString, "/Help/GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments_CorrectionSum")%>:
            </td>
            <td style="min-width: 70px">
                <%:Model.SumString%> р.
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.TeamId)%>:
            </td>
            <td>
                <%: Html.DropDownListFor(x=>x.TeamId, Model.TeamList, new { style = "min-width:160px;" }, !Model.IsNew)%>
            </td>
            <td class="row_title">
                <%:Html.HelpLabelFor(model => model.UndistributedSumString, "/Help/GetHelp_DealCreditInitialBalanceCorrection_SelectDestinationDocuments_UndistributedCorrectionSum")%>:
            </td>
            <td>
                <span id="UndistributedSumString"><%:Model.UndistributedSumString%></span> р.
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.Date)%>:
            </td>
            <td>
                <%:Model.Date%>
            </td>
            <td class="row_title">
                
            </td>
            <td>
                
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.CorrectionReason)%>:
            </td>
            <td colspan="3">
                <%:Model.CorrectionReason%>
            </td>                            
        </tr>
    </table>

    <br />

    <div id="messageDestinationDocumentForDealCreditInitialBalanceCorrectionDistributionSelectList"></div>

    <div style="max-height: 250px; overflow: auto;" id="SaleWaybillSelectGridContainer">
        <% Html.RenderPartial("~/Views/DealPaymentDocument/DestinationDocumentSelector/SaleWaybillSelectGrid.ascx", Model.SaleWaybillGridData); %>
    </div>

    <div style="max-height: 200px; overflow: auto;" id="DealDebitInitialBalanceCorrectionSelectGridContainer">
        <% Html.RenderPartial("~/Views/DealPaymentDocument/DestinationDocumentSelector/DealDebitInitialBalanceCorrectionSelectGrid.ascx", Model.DealDebitInitialBalanceCorrectionGridData); %>
    </div>

    <div class="button_set">
        <%: Html.SubmitButton("btnDistribute", "Разнести")%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>

<%} %>

</div>
