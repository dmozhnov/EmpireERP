<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
    MovementWaybill_Details_MainDetails.Init();
</script>

<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>

<%: Html.HiddenFor(x => x.SenderStorageId) %>
<%: Html.HiddenFor(x => x.AllowToViewSenderStorageDetails) %>

<%: Html.HiddenFor(x => x.RecipientStorageId) %>
<%: Html.HiddenFor(x => x.AllowToViewRecipientStorageDetails) %>

<%: Html.HiddenFor(x => x.AllowToViewCreatedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewAcceptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewShippedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewReceiptedByDetails) %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style='min-width: 160px'>
            <%: Html.LabelFor(model => model.StateName) %>:
        </td>
        <td style="width: 65%;">
            <strong><span id="StateName"><%: Model.StateName %></span></strong>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.PurchaseCostSum) %>:
        </td>
        <td style="width: 35%;">
            <span id="PurchaseCostSum"><%: Model.PurchaseCostSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.SenderStorageName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.SenderStorageId) %>
            <a id="SenderStorageName"><%: Model.SenderStorageName%></a>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.SenderAccountingPriceSum)%> <%: Html.LabelFor(model => model.RecipientAccountingPriceSum)%>:
        </td>
        <td>
            <span id="SenderAccountingPriceSum"><%: Model.SenderAccountingPriceSum%></span> &nbsp;||&nbsp; <span id="RecipientAccountingPriceSum"><%: Model.RecipientAccountingPriceSum%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.SenderName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.SenderId)%>
            <a id ="SenderName"><%: Model.SenderName%></a>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.MovementMarkupPercent) %>:
        </td>
        <td>
            <span id="MovementMarkupPercent"><%: Model.MovementMarkupPercent %></span> % &nbsp;||&nbsp; <span id="MovementMarkupSum"><%: Model.MovementMarkupSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RecipientStorageName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.RecipientStorageId)%>
            <a id="RecipientStorageName"><%: Model.RecipientStorageName%></a>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RowCount) %>:
        </td>
        <td>
            <span id="RowCount"><%: Model.RowCount %></span> &nbsp;||&nbsp; <span id="ShippingPercent"><%: Model.ShippingPercent %></span> %
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RecipientName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.RecipientId)%>
            <a id="RecipientName"><%: Model.RecipientName%></a>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.SenderValueAddedTaxString)%>:
        </td>
        <td>
            <span id="SenderValueAddedTaxString"><%: Model.SenderValueAddedTaxString%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td>
            <a id="CuratorName"><%: Model.CuratorName%></a>
            <span class="main_details_action" style= <%if (Model.AllowToChangeCurator){ %>"display:inline;"<%} else {%> "display:none;"<%} %> id="linkChangeCurator">[ Изменить ]</span>
            <%:Html.HiddenFor(model => model.CuratorId) %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RecipientValueAddedTaxString) %>:
        </td>
        <td>
            <span id="RecipientValueAddedTaxString"><%:Model.RecipientValueAddedTaxString%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.TotalWeight) %> <%: Html.LabelFor(model => model.TotalVolume) %>:
        </td>
        <td>
            <span id="TotalWeight"><%: Model.TotalWeight %></span> &nbsp;||&nbsp; <span id="TotalVolume"><%: Model.TotalVolume %></span>
        </td>
        <td class='row_title'>
            
        </td>
        <td>
            
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
            
            <% var shippedByStyle = (Model.ShippedById == "" ? "none" : "inline"); %>
            <span id="ShippedByContainer" style="display: <%= shippedByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ShippedByName)%>:</span>
                <%: Html.HiddenFor(model => model.ShippedById)%>
                <a id="ShippedByName"><%: Model.ShippedByName%></a>
                <span id="ShippingDate"><%: Model.ShippingDate%></span>
            </span>

            <% var receiptedByStyle = (Model.ReceiptedById == "" ? "none" : "inline"); %>
            <span id="ReceiptedByContainer" style="display: <%= receiptedByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ReceiptedByName)%>:</span>
                <%: Html.HiddenFor(model => model.ReceiptedById)%>
                <a id="ReceiptedByName"><%: Model.ReceiptedByName %></a>
                <span id="ReceiptDate"><%: Model.ReceiptDate %></span>
            </span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Comment) %>:
        </td>
        <td colspan="3">
            <%: Html.CommentFor(x => x.Comment, true) %>
        </td>
    </tr>
</table>

