<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Provider.ProviderMainDetailsViewModel>" %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style="min-width: 110px">
            <%: Html.LabelFor(model => model.Name) %>:
        </td>
        <td style="width: 60%;">
            <%= Model.Name %>
        </td>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.PurchaseCostSum, "/Help/GetHelp_Provider_Details_MainDetails_PurchaseCostSum")%>:
        </td>
        <td style="width: 40%;">
            <%= Model.PurchaseCostSum %> р.
        </td>
    </tr>    
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.TypeName) %>:
        </td>
        <td>
            <%= Model.TypeName %>
        </td>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.PendingPurchaseCostSum, "/Help/GetHelp_Provider_Details_MainDetails_PendingPurchaseCostSum")%>:
        </td>
        <td>
            <%= Model.PendingPurchaseCostSum %> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ReliabilityName) %>:
        </td>
        <td>
            <%= Model.ReliabilityName %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ProviderOrganizationCount)%>:
        </td>
        <td>
            <span id="ProviderOrganizationCount"><%= Model.ProviderOrganizationCount %></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.Rating) %>:
        </td>
        <td>
            <%= Model.Rating %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.ContractCount) %>:
        </td>
        <td>
            <span id="ContractCount"><%= Model.ContractCount %></span>
        </td>     
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.CreationDate) %>:
        </td>
        <td colspan="3">
            <%= Model.CreationDate %>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.Comment) %>:
        </td>
        <td colspan="3">
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>

