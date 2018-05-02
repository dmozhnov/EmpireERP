<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.DealPaymentDocument.DealPaymentToClientDetailsViewModel>" %>

<script type="text/javascript">
    DealPaymentDocument_DealPaymentToClientDetails.Init();
</script>

<%: Html.HiddenFor(x => x.DealId) %>
<%: Html.HiddenFor(x => x.AllowToViewDealDetails) %>
<%: Html.HiddenFor(x => x.TeamId) %>
<%: Html.HiddenFor(x => x.AllowToViewTeamDetails) %>
<%: Html.HiddenFor(x => x.ReturnedById) %>
<%: Html.HiddenFor(x => x.AllowToViewReturnedByDetails) %>


<div style="width:470px; padding: 0 10px;">
    <%:Html.HiddenFor(model => model.PaymentId)%>
    
    <div class="modal_title"><%:Model.Title%></div>
    <br />
    
    <div id="messageDealPaymentToClientDetails"></div>

    <table class="display_table">
        <tr>
            <td class="row_title" style="width: 180px;"><%: Html.LabelFor(model => model.DealName)%>:</td>
            <td>
                <a id="DealName"><%:Model.DealName%></a>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%: Html.LabelFor(model => model.TeamName)%>:</td>
            <td>
                <a id="TeamName"><%:Model.TeamName%></a>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%: Html.LabelFor(model => model.ReturnedByName)%>:</td>
            <td>
                <a id="ReturnedByName"><%:Model.ReturnedByName%></a>
                <% if(Model.AllowToChangeReturnedBy) { %><span id='changeReturnedBy' class="main_details_action">[ Изменить ]</span> <% } %>
            </td>
        </tr>
        <tr>
            <td class="row_title"><%: Html.LabelFor(model => model.PaymentDocumentNumber)%>:</td>
            <td><%: Model.PaymentDocumentNumber%></td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.Date)%>:</td>
            <td><%: Model.Date%></td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.Sum)%>:</td>
            <td><%: Model.Sum%> р.</td>
        </tr>
        <tr>
            <td class="row_title"><%:Html.LabelFor(model => model.DealPaymentForm)%>:</td>
            <td><%: Model.DealPaymentForm%></td>
        </tr>
    </table>

    <div class="button_set">
        <%: Html.Button("btnDeleteDealPaymentToClient", "Удалить оплату", Model.AllowToDelete, Model.AllowToDelete)%>
        <input type="button" value="Закрыть" onclick="HideModal()" />     
    </div>
</div>

<div id="userSelector"></div>