<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealCreditInitialBalanceCorrectionDetailsViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DealCreditInitialBalanceCorrectionDetails.Init();
</script>

<div style="width: 800px; padding: 0 10px 0;">
    <%:Html.HiddenFor(model => model.DealCreditInitialBalanceCorrectionId)%>
    <%: Html.HiddenFor(x => x.DealId) %>
    <%: Html.HiddenFor(x => x.AllowToViewDealDetails) %>
    <%: Html.HiddenFor(x => x.TeamId) %>
    <%: Html.HiddenFor(x => x.AllowToViewTeamDetails) %>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealCreditInitialBalanceCorrection_Details") %></div>
    <br />

    <table class="display_table">
    <tr>
        <td class="row_title" style="width: 160px">
            <%:Html.LabelFor(model => model.CorrectionReason)%>:
        </td>
        <td>
            <%:Model.CorrectionReason%>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.Sum, "/Help/GetHelp_DealCreditInitialBalanceCorrection_Details_CorrectionSum")%>:
        </td>
        <td style="min-width: 120px">
            <%:Model.Sum%>&nbsp;р.
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
            <%: Html.HelpLabelFor(model => model.PaymentToClientSum, "/Help/GetHelp_DealCreditInitialBalanceCorrection_Details_PaymentToClientSum")%>:
        </td>
        <td>
            <%: Model.PaymentToClientSum %>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.DealName)%>:
        </td>
        <td>
            <a id="DealName"><%:Model.DealName%></a>
        </td>        
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.DistributedToSaleWaybillSum, "/Help/GetHelp_DealCreditInitialBalanceCorrection_Details_DistributedSum")%>:
        </td>
        <td>
            <%: Model.DistributedToSaleWaybillSum %> р. &nbsp;||&nbsp; <%: Model.DistributedToDealDebitInitialBalanceCorrectionSum %> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model =>model.TeamName) %>:
        </td>
        <td>
            <a id="TeamName"><%:Model.TeamName%></a>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.UndistributedSum, "/Help/GetHelp_DealCreditInitialBalanceCorrection_Details_UndistributedSum")%>:
        </td>
        <td>
            <%:Model.UndistributedSum%>&nbsp;р.
        </td>
    </tr>
    </table>

    <br />
    <div id="messageDealCreditInitialBalanceCorrectionDetails"></div>

    <div style="max-height: 250px; overflow: auto;">
        <%Html.RenderPartial("~/Views/DealPaymentDocument/SaleWaybillGrid.ascx", Model.SaleWaybillGrid);%>
    </div>

    <div style="max-height: 200px; overflow: auto;">
        <%Html.RenderPartial("~/Views/DealPaymentDocument/DealDebitInitialBalanceCorrectionGrid.ascx", Model.DealDebitInitialBalanceCorrectionGrid);%>
    </div>

    <div class="button_set">
        <%: Html.Button("btnDeleteDealCreditInitialBalanceCorrection", "Удалить корректировку", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
