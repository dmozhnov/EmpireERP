<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ExpenditureWaybill.ExpenditureWaybillMainDetailsViewModel>" %>

<script type="text/javascript">
    ExpenditureWaybill_Details_MainDetails.Init();
</script>

<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewClientDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewDealDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewSenderStorageDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewTeamDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewCreatedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewAcceptedByDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewShippedByDetails) %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 160px'>
            <%:Html.HelpLabelFor(model => model.StateName, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_State")%>:
        </td>
        <td style='width: 60%'>
            <strong><span id="StateName"><%: Model.StateName %></span></strong>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.PurchaseCostSum, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_PurchaseCostSum")%>:
        </td>
        <td style='width: 40%'>
            <span id="PurchaseCostSum"><%: Model.PurchaseCostSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.ClientName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.ClientId) %>
            <a id="ClientName"><%: Model.ClientName %></a>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.SenderAccountingAndSalePriceSum, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_SenderAccountingAndSalePriceSum")%>:
        </td>
        <td>
            <span id="SenderAccountingPriceSum"><%: Model.SenderAccountingPriceSum%></span> р. &nbsp;||&nbsp; <span id="SalePriceSum"><%: Model.SalePriceSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.DealName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.DealId)%>
            <a id="DealName"><%: Model.DealName %></a>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.ValueAddedTaxString, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_ValueAddedTax")%>:
        </td>
        <td>
            <span id="ValueAddedTaxString"><%:Model.ValueAddedTaxString%></span>
        </td>
    </tr>
    <tr>  
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.DealQuotaName, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_DealQuota")%>:
        </td>
        <td>
            <span id="DealQuotaName"><%: Model.DealQuotaName %></span>
            <% string allowToChangeDealQuota = (Model.AllowToChangeDealQuota ? "inline" : "none"); %>            
            <span id='linkChangeDealQuota' class="main_details_action" style="display:<%= allowToChangeDealQuota %>">[ Изменить ]</span> 
            
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.TotalDiscountPercent, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_TotalDiscount")%>:
        </td>
        <td>
            <span id="TotalDiscountPercent"><%: Model.TotalDiscountPercent%></span> % &nbsp;||&nbsp; <span id="TotalDiscountSum"><%: Model.TotalDiscountSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.SenderStorageName) %>:
        </td>
        <td>
            <a id="SenderStorageName"><%: Model.SenderStorageName%></a>
            <%: Html.HiddenFor(model => model.SenderStorageId) %>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.PaymentPercent, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_PaymentSum")%>:
        </td>
        <td>
            <span id="PaymentPercent"><%: Model.PaymentPercent%></span> % &nbsp;||&nbsp; <span id="PaymentSum"><%: Model.PaymentSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.AccountOrganizationName, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_AccountOrganization")%>:
        </td>
        <td>            
            <%: Html.HiddenFor(model => model.AccountOrganizationId) %>
            <a id="AccountOrganizationName"><%: Model.AccountOrganizationName%></a>
        </td>        
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.TotalReturnedSum, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_ReturnSum")%>:
        </td>
        <td> 
            <span id="TotalReturnedSum"><%: Model.TotalReturnedSum %></span> р.&nbsp;||&nbsp; 
            <span id="TotalReservedByReturnSum"><%: Model.TotalReservedByReturnSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.DeliveryAddress) %>:
        </td>
        <td><span id="DeliveryAddress">
            <%: Model.DeliveryAddress %></span>
        </td>        
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.MarkupPercent, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_MarkupSum")%>:
        </td>
        <td>
            <span id="MarkupPercent"><%: Model.MarkupPercent%></span> % &nbsp;||&nbsp; <span id="MarkupSum"><%: Model.MarkupSum %></span> р.
        </td>        
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.PaymentType) %>:
        </td>
        <td>
            <strong><span id="PaymentType"><%: Model.PaymentType%></span></strong>
        </td>        
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.ReturnLostProfitSum, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_ReturnLostProfitSum")%>:
        </td>
        <td>
            <span id="ReturnLostProfitSum">
                <%: Model.ReturnLostProfitSum %>
            </span>&nbsp;р.
            &nbsp;||&nbsp; 
             <span id="ReservedByReturnLostProfitSum">
                <%: Model.ReservedByReturnLostProfitSum %>
            </span>&nbsp;р.
        </td>        
    </tr>
     <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td>
            <a id="CuratorName"><%: Model.CuratorName%></a>
            <span class="main_details_action" style= <%if (Model.AllowToChangeCurator){ %>"display:inline;"<%} else {%> "display:none;"<%} %> id="linkChangeCurator">[ Изменить ]</span>
            <%: Html.HiddenFor(x => x.CuratorId) %>
        </td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.RowCount, "/Help/GetHelp_ExpenditureWaybill_Details_MainDetails_RowCount")%>:
        </td>
        <td>
            <span id="RowCount"><%: Model.RowCount%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.TeamName) %>:
        </td>
        <td>
            <a id="TeamName"><%: Model.TeamName%></a>
            <%: Html.HiddenFor(x => x.TeamId)%>
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
            
            <% var shippedByStyle = (Model.ShippedById == "" ? "none" : "inline"); %>
            <span id="ShippedByContainer" style="display: <%= shippedByStyle %>">
                ,&nbsp; <span class='greytext'><%: Html.LabelFor(model => model.ShippedByName)%>:</span>
                <%: Html.HiddenFor(model => model.ShippedById)%>
                <a id="ShippedByName"><%: Model.ShippedByName%></a>
                <span id="ShippingDate"><%: Model.ShippingDate%></span>
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