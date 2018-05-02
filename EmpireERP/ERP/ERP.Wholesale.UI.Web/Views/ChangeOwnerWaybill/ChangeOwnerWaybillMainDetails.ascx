<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
    ChangeOwnerWaybill_MainDetails.Init();
</script>

<table class='main_details_table'>
    <%: Html.HiddenFor(model => model.CuratorId) %>
    <%: Html.HiddenFor(model => model.AllowToViewCuratorDetails) %>

    <%: Html.HiddenFor(model => model.AllowToViewCreatedByDetails)%>
    <%: Html.HiddenFor(model => model.AllowToViewAcceptedByDetails)%>
    <%: Html.HiddenFor(model => model.AllowToViewChangedOwnerByDetails)%>

    <tr>
        <td class="row_title" style='min-width: 160px'>
            <%: Html.LabelFor(model => model.StateName) %>:
        </td>
        <td style="width: 75%;">
            <strong><span id="StateName"><%: Model.StateName %></span></strong>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.PurchaseCostSum) %>:
        </td>
        <td style="width: 25%;">
            <span id="PurchaseCostSum"><%: Model.PurchaseCostSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.SenderName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.SenderId) %>
            <a id="SenderName"><%: Model.SenderName%></a>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.AccountingPriceSum)%>:
        </td>
        <td>
            <span id="AccountingPriceSum"><%: Model.AccountingPriceSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RecipientName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.RecipientId)%>
            <a id="mainDetailsRecipientLink"><%: Model.RecipientName%></a>
            
            <% if(Model.AllowToChangeRecipient){ %>            
            <span id="linkChangeRecipient" class="main_details_action">[ Изменить ]</span>
            <%} %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.RowCount) %>:
        </td>
        <td>
            <span id="RowCount"><%: Model.RowCount%></span>&nbsp;||&nbsp;<span id="ShippingPercent"><%: Model.ShippingPercent%></span>&nbsp;%
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.StorageName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.StorageId) %>
            <a id="RecipientStorageName"><%: Model.StorageName %></a>
        </td>
         <td class="row_title">
            <%: Html.LabelFor(model => model.ValueAddedTaxString)%>:
        </td>
        <td>
            <span id="ValueAddedTaxString"><%: Model.ValueAddedTaxString%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.CuratorName) %>:
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
            
            <% var changedOwnerByStyle = (Model.ChangedOwnerById == "" ? "none" : "inline"); %>
            <span id="ChangedOwnerByContainer" style="display: <%= changedOwnerByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ChangedOwnerByName)%>:</span>
                <%: Html.HiddenFor(model => model.ChangedOwnerById)%>
                <a id="ChangedOwnerByName"><%: Model.ChangedOwnerByName%></a>
                <span id="ChangeOwnerDate"><%: Model.ChangeOwnerDate%></span>
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
