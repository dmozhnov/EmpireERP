<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
    ReturnFromClientWaybill_Details_MainDetails.Init();
</script>

<%: Html.HiddenFor(x => x.AllowToViewAcceptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewClientDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewDealDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewTeamDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewCreatedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewReceiptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewRecipientStorageDetails) %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 130px'>
            <%:Html.HelpLabelFor(model => model.StateName, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_State")%>:
        </td>
        <td style='width: 60%'>
            <strong><span id="StateName"><%: Model.StateName %></span></strong>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.PurchaseCostSum, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_PurchaseCostSum")%>:
        </td>
        <td style='width: 40%'>
            <span id="PurchaseCostSum"><%: Model.PurchaseCostSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.RecipientStorageName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.RecipientStorageId)%>            
            <a id="RecipientStorageName"><%: Model.RecipientStorageName%></a>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.SalePriceSum, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_SalePriceSum")%>:
        </td>
        <td>
            <span id="SalePriceSum"><%: Model.SalePriceSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.RecipientName, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_RecipientName")%>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.RecipientId)%>
            <a id="RecipientName"><%: Model.RecipientName%></a>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.AccountingPriceSum, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_AccountingPriceSum") %>:
        </td>
        <td>
            <span id="AccountingPriceSum"><%: Model.AccountingPriceSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.ClientName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.ClientId)%>
            <a id="ClientName"><%: Model.ClientName%></a>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.RowCount, "/Help/GetHelp_ReturnFromClientWaybill_Details_MainDetails_RowCount")%>:
        </td>
        <td>
            <span id="RowCount"><%: Model.RowCount %></span> &nbsp;||&nbsp; <span id="ShippingPercent"><%: Model.ShippingPercent %></span>&nbsp;%
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.DealName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.DealId)%>
            <a id="DealName"><%: Model.DealName%></a>
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
            <%:Html.LabelFor(model => model.ReasonName) %>:
        </td>
        <td>
            <%: Model.ReasonName%>
        </td>
        <td class='row_title'>
            
        </td>
        <td>
            
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td colspan="3">            
            <%: Html.HiddenFor(model => model.CuratorId)%>
            <a id="CuratorName"><%: Model.CuratorName%></a>
            <span class="main_details_action" style= <%if (Model.AllowToChangeCurator){ %>"display:inline;"<%} else {%> "display:none;"<%} %> id="linkChangeCurator">[ Изменить ]</span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.TeamName) %>:
        </td>
        <td colspan="3">
            <%: Html.HiddenFor(model => model.TeamId)%>
            <a id="TeamName"><%: Model.TeamName%></a>
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
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>
