<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.WriteoffWaybill.WriteoffWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
    WriteoffWaybill_MainDetails.Init();    
</script>

<%: Html.HiddenFor(x => x.CuratorId) %>
<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>

<%: Html.HiddenFor(x => x.SenderStorageId) %>
<%: Html.HiddenFor(x => x.AllowToViewSenderStorageDetails) %>

<%: Html.HiddenFor(x => x.SenderId) %>

<%: Html.HiddenFor(x => x.AllowToViewCreatedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewAcceptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewWrittenoffByDetails) %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 120px'>
            <%:Html.LabelFor(model=>model.StateName) %>:
        </td>
        <td style='width: 75%'>
            <strong><span id="StateName"><%: Model.StateName %></span></strong>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.PurchaseCostSum) %>:
        </td>
        <td style='width: 25%'>
            <span id="PurchaseCostSum"><%: Model.PurchaseCostSum %></span> р.
        </td>
    </tr>
    <tr>   
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.SenderStorageName) %>:
        </td>
        <td>
            <a id="SenderStorageName"><%:Model.SenderStorageName%></a>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.SenderAccountingPriceSum) %>:
        </td>
        <td>
            <span id="SenderAccountingPriceSum"><%: Model.SenderAccountingPriceSum %></span><% if (Model.SenderAccountingPriceSum != "-") { %> р. <% }%>
        </td>
    </tr>
    <tr>   
        <td class='row_title'>
            <%:Html.LabelFor(model => model.SenderName)%>:
        </td>
        <td>
            <a id="SenderName"><%:Model.SenderName%></a>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.ReceivelessProfitPercent) %>:
        </td>
        <td>
            <span id="ReceivelessProfitPercent"><%: Model.ReceivelessProfitPercent %></span> % &nbsp;||&nbsp; <span id="ReceivelessProfitSum"><%: Model.ReceivelessProfitSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.WriteoffReasonName) %>:
        </td>
        <td>
            <span id="WriteoffReasonName"><%: Model.WriteoffReasonName %></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model=>model.RowCount) %>:
        </td>
        <td>
            <span id="RowCount"><%: Model.RowCount %></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.CuratorName)%>:
        </td>
        <td>
            <a id="CuratorName"><%: Model.CuratorName %></a>
            <span class="main_details_action" style= <%if (Model.AllowToChangeCurator){ %>"display:inline;"<%} else {%> "display:none;"<%} %> id="linkChangeCurator">[ Изменить ]</span>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.TotalWeight) %> <%: Html.LabelFor(model => model.TotalVolume) %>:
        </td>
        <td>
            <span id="TotalWeight"><%: Model.TotalWeight %></span> &nbsp;||&nbsp; <span id="TotalVolume"><%: Model.TotalVolume %></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            Движение накладной:
        </td>
        <td colspan="3">            
            <span class='greytext'><%: Html.LabelFor(model => model.CreatedByName)%>:</span>
            <%: Html.HiddenFor(model => model.CreatedById)%>
            <a id="CreatedByName"><%: Model.CreatedByName%></a>&nbsp;
            <span id="CreationDate" style="margin-right: -2px;"><%: Model.CreationDate %></span>
            
            <% var acceptedByStyle = (Model.AcceptedById == "" ? "none" : "inline"); %>
            <span id="AcceptedByContainer" style="margin-right: -2px; display: <%= acceptedByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.AcceptedByName)%>:</span>
                <%: Html.HiddenFor(model => model.AcceptedById) %>
                <a id="AcceptedByName"><%: Model.AcceptedByName %></a>&nbsp;
                <span id="AcceptanceDate"><%: Model.AcceptanceDate %></span>
            </span>
            
            <% var writtenoffByStyle = (Model.WrittenoffById == "" ? "none" : "inline"); %>
            <span id="WrittenoffByContainer" style="display: <%= writtenoffByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.WrittenoffByName)%>:</span>
                <%: Html.HiddenFor(model => model.WrittenoffById)%>
                <a id="WrittenoffByName"><%: Model.WrittenoffByName%></a>
                <span id="WriteoffDate"><%: Model.WriteoffDate%></span>
            </span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Comment)%>:
        </td>
        <td colspan='3'>
            <%: Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>
