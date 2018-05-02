<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
     ReceiptWaybill_MainDetails.Init();
</script>

<%: Html.HiddenFor(x => x.IsCreatedFromProductionOrderBatch) %>

<%: Html.HiddenFor(x => x.StorageId) %>
<%: Html.HiddenFor(x => x.AllowToViewStorageDetails) %>

<%: Html.HiddenFor(x => x.AccountOrganizationId) %>

<%: Html.HiddenFor(x => x.ProviderId) %>
<%: Html.HiddenFor(x => x.AllowToViewProviderDetails) %>

<%: Html.HiddenFor(x => x.ProducerId) %>
<%: Html.HiddenFor(x => x.AllowToViewProducerDetails) %>

<%: Html.HiddenFor(x => x.ProductionOrderId) %>
<%: Html.HiddenFor(x => x.AllowToViewProductionOrderDetails) %>

<%: Html.HiddenFor(x => x.CuratorId) %>
<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>

<%: Html.HiddenFor(x => x.AllowToViewCreatedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewAcceptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewReceiptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewApprovedByDetails) %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style='min-width: 120px'>
            <%: Html.LabelFor(model => model.StateName) %>:
        </td>
        <td style="width: 65%;">
            <span id="StateName" class="bold"><%: Model.StateName %></span>
        </td>

        <td class="row_title" >
            <%if (!Model.ReceiptedWithDivergences) {%>
                <%: Html.LabelFor(model => model.Sum) %>:
            <%} else {%>
                <%: Html.LabelFor(model => model.ReceiptedSum) %>:
            <%} %>
        </td>
        <td style="width: 35%;">
            <%if (Model.ReceiptedWithDivergences) {%>
                <span id="PendingSumByRows" <% if (Model.AreSumDivergences) {%> class="attention" <%} %>><%: Model.PendingSum %>&nbsp;р.</span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="Sum" <% if (Model.AreSumDivergences) {%> class="attention" <%} %>><%: Model.Sum %>&nbsp;р.</span>
            <%} else {%>
                <span id="Sum" <% if (Model.AreSumDivergences) { %>class="attention"<%}%>><%: Model.Sum%>&nbsp;р.</span>
            <%} %>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.StorageName)%>:
        </td>
        <td>            
            <a id="StorageName"><%:Model.StorageName%></a>
        </td>
        <td class="row_title">
            <%if (!Model.ReceiptedWithDivergences) {%>
                <%: Html.LabelFor(model => model.RowCount)%>:
            <%} else {%>
                <%: Html.LabelFor(model => model.PendingRowCount) %>:
            <%} %>
        </td>
        <td>
            <%if (Model.ReceiptedWithDivergences) { %>
                <%: Model.PendingRowCount%>&nbsp;||&nbsp;<%: Model.ReceiptedRowCount%>
            <%} else {%>
                <span id="RowCount"><%: Model.RowCount %></span>
            <%} %>&nbsp;||&nbsp;<span id="ShippingPercent"><%: Model.ShippingPercent %></span>&nbsp;%
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.AccountOrganizationName)%>:
        </td>
        <td>            
            <a id="AccountOrganizationName"><%:Model.AccountOrganizationName%></a>
        </td>
        <td class="row_title">
            <%if (!Model.ReceiptedWithDivergences) {%>
                <%: Html.LabelFor(model => model.PendingSum)%>:
            <%} else {%>
                <%: Html.LabelFor(model => model.ApprovedSum)%>:
            <%} %>
        </td>
        <td>
            <%if (Model.ApprovedAfterDivergences) { %>
                <span id="ApprovedSum"><%: Model.ApprovedSum%></span>&nbsp;р.
            <%} else { %>
                <span id="PendingSum" <% if (Model.AreSumDivergences) {%> class="attention" <%} %>><%: Model.PendingSum%>&nbsp;р.</span>
            <%} %>
            <%if (Model.ReceiptedWithDivergences) { %>&nbsp;||&nbsp;<span id="ReceiptedSum" <% if (Model.AreSumDivergences) {%> class="attention" <%} %>><%:Model.ReceiptedSum%>&nbsp;р.</span><%} %>
        </td>
    </tr>
    <tr>
        <% if (Model.IsCreatedFromProductionOrderBatch) { %>
            <td class="row_title">
                <%: Html.LabelFor(model => model.ProducerName)%>
            </td>
            <td>
                <a id="ProducerName"><%: Model.ProducerName%></a>
            </td>
        <% } else { %>
            <td class="row_title">
                <%: Html.LabelFor(model => model.ProviderName)%>
            </td>
            <td>
                <a id="ProviderName"><%: Model.ProviderName%></a>
            </td>
        <% } %>
        <td class="row_title">
            <%if (!Model.ReceiptedWithDivergences) {%>
                <%: Html.LabelFor(model => model.ValueAddedTaxString)%>:
            <%} else {%>
                <%: Html.LabelFor(model => model.ShippingPercent)%>:
            <%} %>
        </td>
        <td>
            <span id="ValueAddedTaxString"><%: Model.ValueAddedTaxString %></span>
        </td>
    </tr>
    <tr>
        <% if (Model.IsCreatedFromProductionOrderBatch) { %>
            <td class="row_title">
                <%: Html.LabelFor(model => model.ProductionOrderName)%>
            </td>
            <td>
                <a id="ProductionOrderName"><%: Model.ProductionOrderName%></a>
            </td>
            <td class="row_title">
                <%: Html.LabelFor(model => model.TotalWeight) %> <%: Html.LabelFor(model => model.TotalVolume) %>:
            </td>
            <td>
                <span id="TotalWeight"><%: Model.TotalWeight %></span> &nbsp;||&nbsp; <span id="TotalVolume"><%: Model.TotalVolume %></span>
            </td>
        <% } else { %>
            <td class="row_title">
                <%: Html.LabelFor(model => model.ContractInfo)%>:
            </td>
            <td>
                <span id="ContractInfo"><%: Model.ContractInfo %></span>
            </td>
            <td class="row_title">
                <%: Html.LabelFor(model => model.DiscountPercent)%>:
            </td>
            <td>
                <%: Model.DiscountPercent %>&nbsp;%&nbsp;||&nbsp; <%: Model.DiscountSum %>&nbsp;р.
            </td>
        <% } %>
    </tr>
    <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ProviderNumber)%>:
        </td>
        <td>
            <%: Model.ProviderNumber %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ProviderDate)%>:
        </td>
        <td>
            <%: Model.ProviderDate %>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ProviderInvoiceNumber)%>:
        </td>
        <td>
            <%: Model.ProviderInvoiceNumber %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ProviderInvoiceDate)%>:
        </td>
        <td>
            <%: Model.ProviderInvoiceDate %>
        </td>
    </tr>
    <% } %>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.CuratorName)%>:
        </td>
        <td>
            <a id="CuratorName"><%: Model.CuratorName %></a>
            <span class="main_details_action" style= <%if (Model.AllowToChangeCurator){ %>"display:inline;"<%} else {%> "display:none;"<%} %> id="linkChangeCurator">[ Изменить ]</span>
        </td>
        <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
            <td class="row_title">
                <%: Html.LabelFor(model => model.TotalWeight) %> <%: Html.LabelFor(model => model.TotalVolume) %>:
            </td>
            <td>
                <span id="TotalWeight"><%: Model.TotalWeight %></span> &nbsp;||&nbsp; <span id="TotalVolume"><%: Model.TotalVolume %></span>
            </td>
        <% } else { %>
            <td class="row_title"></td>
            <td></td>
        <% } %>
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
            
            <% var receiptedByStyle = (Model.ReceiptedById == "" ? "none" : "inline"); %>
            <span id="ReceiptedByContainer" style="display: <%= receiptedByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ReceiptedByName)%>:</span>
                <%: Html.HiddenFor(model => model.ReceiptedById)%>
                <a id="ReceiptedByName"><%: Model.ReceiptedByName %></a>
                <span id="ReceiptDate"><%: Model.ReceiptDate %></span>
            </span>

            <% if (Model.ApprovedFinallyAfterDivergences) { %>
                <% var approvedByStyle = (Model.ApprovedById == "" ? "none" : "inline"); %>
                <span id="ApprovedByContainer" style="display: <%= approvedByStyle %>">
                    ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ApprovedByName)%>:</span>
                    <%: Html.HiddenFor(model => model.ApprovedById)%>
                    <a id="ApprovedByName"><%: Model.ApprovedByName%></a>
                    <span id="ApprovementDate"><%: Model.ApprovementDate %></span>
                </span>
            <% } %>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Comment)%>:
        </td>
        <td colspan='3'>            
            <%: Html.CommentFor(model => model.Comment, true)%>            
        </td>
    </tr>
</table>
