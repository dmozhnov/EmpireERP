<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealPaymentFromClientDetailsViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DealPaymentFromClientDetails.Init();
</script>

<%: Html.HiddenFor(x => x.DealId) %>
<%: Html.HiddenFor(x => x.AllowToViewDealDetails) %>
<%: Html.HiddenFor(x => x.TeamId) %>
<%: Html.HiddenFor(x => x.AllowToViewTeamDetails) %>
<%: Html.HiddenFor(x => x.TakenById) %>
<%: Html.HiddenFor(x => x.AllowToViewTakenByDetails) %>

<div style="width: 800px; padding: 0 10px 0;">
    <%:Html.HiddenFor(model => model.PaymentId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealPaymentFromClient_Details") %></div>
    <br />

    <table class="display_table">
    <tr>
        <td class="row_title" style="width: 120px">
            <%:Html.LabelFor(model => model.PaymentDocumentNumber)%>:
        </td>
        <td>
            <%:Model.PaymentDocumentNumber%>
        </td>
        <td class="row_title">
            <%:Html.HelpLabelFor(model => model.Sum, "/Help/GetHelp_DealPaymentFromClient_Details_PaymentSum")%>:
        </td>
        <td style="width: 140px">
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
            <%: Html.HelpLabelFor(model => model.PaymentToClientSum, "/Help/GetHelp_DealPaymentFromClient_Details_PaymentToClientSum")%>:
        </td>
        <td>
            <%: Model.PaymentToClientSum %>&nbsp;р.
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
            <%: Html.HelpLabelFor(model => model.DistributedToSaleWaybillSum, "/Help/GetHelp_DealPaymentFromClient_Details_DistributedSum")%>:
        </td>
        <td>
            <%: Model.DistributedToSaleWaybillSum %> р. &nbsp;||&nbsp; <%: Model.DistributedToDealDebitInitialBalanceCorrectionSum %> р.
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
            <%:Html.HelpLabelFor(model => model.UndistributedSum, "/Help/GetHelp_DealPaymentFromClient_Details_UndistributedSum")%>:
        </td>
        <td>
            <%:Model.UndistributedSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(x=>x.TeamName) %>:
        </td>
        <td colspan="3">
            <a id="TeamName"><%: Model.TeamName%></a>
        </td>
    </tr>
    <tr>
        <td class="row_title"><%: Html.LabelFor(model => model.TakenByName)%>:</td>
        <td colspan="3">
            <a id="TakenByName"><%:Model.TakenByName%></a>
            <% if(Model.AllowToChangeTakenBy) { %><span id='changeTakenBy' class="main_details_action">[ Изменить ]</span> <% } %>
        </td>
    </tr>
    </table>

    <br />
    <div id="messageDealPaymentFromClientDetails"></div>

    <div style="max-height: 250px; overflow: auto;">
        <%Html.RenderPartial("~/Views/DealPaymentDocument/SaleWaybillGrid.ascx", Model.SaleWaybillGrid);%>
    </div>

    <div style="max-height: 200px; overflow: auto;">
        <%Html.RenderPartial("~/Views/DealPaymentDocument/DealDebitInitialBalanceCorrectionGrid.ascx", Model.DealDebitInitialBalanceCorrectionGrid);%>
    </div>

    <div class="button_set">
        <%: Html.Button("btnDeleteDealPaymentFromClient", "Удалить оплату", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>

<div id="userSelector"></div>