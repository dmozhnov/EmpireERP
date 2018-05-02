<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DestinationDocumentSelectForClientOrganizationPaymentFromClientDistributionViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DestinationDocumentSelectForClientOrganizationPaymentFromClientDistribution.Init();
</script>

<div style="width: 850px; padding: 0 10px 0;">
<% using (Ajax.BeginForm(Model.DestinationDocumentSelectorActionName, Model.DestinationDocumentSelectorControllerName, new AjaxOptions()
    {
        OnBegin = "DealPaymentDocument_DestinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution.OnBeginClientOrganizationPaymentFromClientSave",
        OnFailure = "DealPaymentDocument_DestinationDocumentSelectorForClientOrganizationPaymentFromClientDistribution.OnFailClientOrganizationPaymentFromClientSave",
        OnSuccess = "OnSuccessClientOrganizationPaymentFromClientSave"
    }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.SumValue)%>
    <%:Html.HiddenFor(model => model.UndistributedSumValue)%>
    <%:Html.HiddenFor(model => model.CurrentOrdinalNumber)%>
    <%:Html.HiddenFor(model => model.DistributionInfo)%>

    <%:Html.HiddenFor(model => model.ClientOrganizationId)%>
    <%:Html.HiddenFor(model => model.PaymentDocumentNumber)%>
    <%:Html.HiddenFor(model => model.Date)%>
    <%:Html.HiddenFor(model => model.DealPaymentForm)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealPaymentFromClient_SelectDestinationDocuments_DestinationDocumentSelectorForDealPaymentFromClientDistribution") %></div>
    <br />

    <table class="display_table">
    <tr>
        <td class="row_title" style="min-width: 130px">
            <%:Html.LabelFor(model => model.ClientOrganizationName)%>:
        </td>
        <td style="min-width: 280px">
            <%:Model.ClientOrganizationName%>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Date)%>:
        </td>
        <td>
            <%:Model.Date%>
        </td>                
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.TeamId)%>:
        </td>
        <td>
            <%: Html.DropDownListFor(x => x.TeamId, Model.TeamList, new { style = "min-width:160px;" })%>
            <%: Html.ValidationMessageFor(x => x.TeamId)%>
        </td>
        
        <td class="row_title" style="min-width: 200px">
            <%:Html.LabelFor(model => model.PaymentDocumentNumber)%>:
        </td>
        <td style="min-width: 150px">
            <%:Model.PaymentDocumentNumber%>
        </td>
    </tr>
    <tr>
        <td class="row_title"><%:Html.LabelFor(model => model.TakenById)%>:</td>
        <td>
            <%: Html.DropDownListFor(x => x.TakenById, Model.TakenByList, new { style = "min-width:160px;" }, !Model.AllowToChangeTakenBy)%>
            <%: Html.ValidationMessageFor(model => model.TakenById)%>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.SumString, "/Help/GetHelp_DealPaymentFromClient_SelectDestinationDocuments_PaymentSum")%>:            
        </td>
        <td>
            <%:Model.SumString%> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PaymentFormName)%>:
        </td>
        <td>
            <%:Model.PaymentFormName%>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.UndistributedSumString, "/Help/GetHelp_DealPaymentFromClient_SelectDestinationDocuments_UndistributedPaymentSum")%>:
        </td>
        <td>
            <span id="UndistributedSumString"><%:Model.UndistributedSumString%></span> р.
        </td>
    </tr>
    </table>
    
    <br />

    <div id="messageDestinationDocumentForClientOrganizationPaymentFromClientDistributionSelectList"></div>

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
