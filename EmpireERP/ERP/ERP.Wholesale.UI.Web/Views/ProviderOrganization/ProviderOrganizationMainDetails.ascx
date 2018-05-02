<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProviderOrganization.ProviderOrganizationMainDetailsViewModel>" %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style="min-width: 160px;">
            <%: Html.LabelFor(x => x.ShortName) %>:
        </td>
        <td width='50%'>
            <span id="ShortName"><%: Model.ShortName%></span>
        </td>        
        <% if (Model.isJuridicalPerson)
           { %>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.KPP)%>:
            </td>
            <td width='50%'>
                <% if (!String.IsNullOrWhiteSpace(Model.INN) || !String.IsNullOrWhiteSpace(Model.KPP))
                   { %>
                <%: !String.IsNullOrEmpty(Model.INN) ? Model.INN : "-" %>&nbsp;||&nbsp;<%: !String.IsNullOrEmpty(Model.KPP) ? Model.KPP : "-" %>
                <% } %>
            </td>
        <%}
           else
           { %>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.INN)%>:
            </td>
            <td width='50%'>
                <%: Model.INN %>
            </td>
        <%} %>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.FullName)%>:
        </td>
        <td>
            <%: Model.FullName%>
        </td>
        <% if (Model.isJuridicalPerson)
           { %>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.OGRN) %>:
        </td>
        <td>
            <%: Model.OGRN %>
        </td>
        <%}
           else
           {%>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.OGRNIP) %>:
        </td>
        <td>
            <%: Model.OGRNIP %>
        </td>
        <%} %>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.LegalForm) %>:
        </td>
        <td>
            <%: Model.LegalForm %>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.Phone) %>:
        </td>
        <td>
            <%: Model.Phone %>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.Address) %>:
        </td>
        <td>
            <%: Model.Address %>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.Fax) %>:
        </td>
        <td>
            <%: Model.Fax %>
        </td>
    </tr>
    <% if (Model.isJuridicalPerson)
       { %>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.DirectorName) %>:
        </td>
        <td>
            <%: Model.DirectorPost %>
            <% if (!String.IsNullOrWhiteSpace(Model.DirectorPost) && !String.IsNullOrWhiteSpace(Model.DirectorName))
               { %>:&nbsp;<% } %>
            <%: Model.DirectorName %>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(x => x.PurchaseSum, "/Help/GetHelp_ProviderOrganization_Details_MainDetails_PurchaseSum")%>:
        </td>
        <td>
            <%: Model.PurchaseSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.MainBookkeeper) %>:
        </td>
        <td>
            <%: Model.MainBookkeeper %>
        </td>

        <td class='row_title'>
            <%: Html.HelpLabelFor(x => x.DeliveryPendingSum, "/Help/GetHelp_ProviderOrganization_Details_MainDetails_DeliveryPendingSum")%>:
        </td>
        <td>
            <%: Model.DeliveryPendingSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.CashierName) %>:
        </td>
        <td>
            <%: Model.CashierName%>
        </td>
        <td class='row_title'>
        </td>
        <td>
        </td>
    </tr>
    <%}
       else
       { %>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.FIO) %>:
        </td>
        <td>
            <%: Model.FIO %>
        </td>

        <td class='row_title'>
            <%: Html.LabelFor(x => x.PurchaseSum) %>:
        </td>
        <td>
            <%: Model.PurchaseSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            
        </td>
        <td>
            
        </td>

        <td class='row_title'>
            <%: Html.LabelFor(x => x.DeliveryPendingSum) %>:
        </td>
        <td>
            <%: Model.DeliveryPendingSum%>&nbsp;р.
        </td>
    </tr>
    <%} %>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.Comment) %>:
        </td>
        <td colspan='3'>
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>
