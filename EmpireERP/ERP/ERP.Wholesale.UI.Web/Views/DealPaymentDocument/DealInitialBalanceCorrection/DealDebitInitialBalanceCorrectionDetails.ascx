<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealDebitInitialBalanceCorrectionDetailsViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DealDebitInitialBalanceCorrectionDetails.Init();
</script>

<div style="width:370px; padding-left:10px; padding-right:10px;">
    <%:Html.HiddenFor(model => model.DealDebitInitialBalanceCorrectionId)%>
    <%:Html.HiddenFor(model => model.TeamId)%>
    <%:Html.HiddenFor(model => model.AllowToViewTeamDetails)%>
    <%:Html.HiddenFor(model => model.DealId)%>
    <%:Html.HiddenFor(model => model.AllowToViewDealDetails)%>


    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_DealDebitInitialBalanceCorrection_Details") %></div>
    <br />

    <div id="messageDealDebitInitialBalanceCorrectionDetails"></div>

    <table class="display_table">
    <tr>
        <td class="row_title" style="width: 150px">
            <%:Html.LabelFor(model => model.CorrectionReason)%>:
        </td>        
        <td>
            <%:Model.CorrectionReason%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.DealName)%>:
        </td>
        <td>
            <a id="DealName"><%:Model.DealName%></a>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.TeamName)%>:
        </td>
        <td>
            <a id="TeamName"><%:Model.TeamName%></a>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Date)%>:
        </td>
        <td>
            <%:Model.Date%>
        </td>       
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Sum)%>:
        </td>
        <td>
            <%:Model.Sum%>&nbsp;р.
        </td>
    </tr>
    </table>

    <div class="button_set">
        <%: Html.Button("btnDeleteDealDebitInitialBalanceCorrection", "Удалить корректировку", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
