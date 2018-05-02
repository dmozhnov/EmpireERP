<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ClientOrganization.ClientOrganizationMainDetailsViewModel>" %>

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
                    <% if (!String.IsNullOrWhiteSpace(Model.INN) || !String.IsNullOrWhiteSpace(Model.KPP)) { %>
                <%: !String.IsNullOrEmpty(Model.INN) ? Model.INN : "---" %>&nbsp;||&nbsp;<%: !String.IsNullOrEmpty(Model.KPP) ? Model.KPP : "---" %>
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
            <%: Html.LabelFor(x => x.LegalForm)%>:
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
            <%: Html.LabelFor(x => x.DirectorName)%>:
        </td>
        <td>
            <%: Model.DirectorPost %>
            <% if (!String.IsNullOrWhiteSpace(Model.DirectorPost) && !String.IsNullOrWhiteSpace(Model.DirectorName)) { %>:&nbsp;<% } %>
            <%: Model.DirectorName %>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(x => x.SaleSum, "/Help/GetHelp_ClientOrganization_Details_MainDetails_SaleSum")%>:
        </td>
        <td>
            <%: Model.SaleSum%>&nbsp;р.
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
            <%: Html.HelpLabelFor(x => x.ShippingPendingSaleSum, "/Help/GetHelp_ClientOrganization_Details_MainDetails_ShippingPendingSaleSum")%>:
        </td>
        <td>
            <%: Model.ShippingPendingSaleSum%> р.
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
            <%: Html.HelpLabelFor(x => x.PaymentSum, "/Help/GetHelp_ClientOrganization_Details_MainDetails_PaymentSum")%>:
        </td>
        <td>
            <%: Model.PaymentSum%>&nbsp;р.&nbsp;||&nbsp;&nbsp;<%: Model.Balance%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td></td>
        <td></td>
        <td class='row_title'>
            <%:Html.HelpLabelFor(model => model.TotalReservedByReturnSum, "/Help/GetHelp_ClientOrganization_Details_MainDetails_TotalReservedByReturnSum")%>:
        </td>
        <td>
            <%: Model.TotalReturnedSum%>&nbsp;р.&nbsp;&nbsp;||&nbsp;&nbsp;<%: Model.TotalReservedByReturnSum%>&nbsp;р.
        </td>
    </tr>
    <%}
       else
       { %>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.FIO)%>:
        </td>
        <td>
            <%: Model.FIO %>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.SaleSum)%>:
        </td>
        <td>
            <%: Model.SaleSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
         <td class='row_title'>
        </td>
        <td>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.ShippingPendingSaleSum)%>:
        </td>
        <td>
            <%: Model.ShippingPendingSaleSum%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
        </td>
        <td>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.PaymentSum)%>:
        </td>
        <td>
            <%: Model.PaymentSum%>&nbsp;р.&nbsp;&nbsp;||&nbsp;&nbsp;<%: Model.Balance%>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td></td>
        <td></td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.TotalReservedByReturnSum) %>:
        </td>
        <td>
            <%: Model.TotalReservedByReturnSum%>&nbsp;р.&nbsp;&nbsp;||&nbsp;&nbsp;<%: Model.TotalReturnedSum%>&nbsp;р.
        </td>
    </tr>
    <%} %>

    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(x => x.Comment)%>:
        </td>
        <td colspan='3'>
            <%: Html.CommentFor(x => x.Comment, true) %>
        </td>
    </tr>
</table>
