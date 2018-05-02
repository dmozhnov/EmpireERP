<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountOrganization.AccountOrganizationMainDetailsViewModel>" %>

<%:Html.HiddenFor(x => x.OrganizationName) %>

<% if (Model.isJuridicalPerson) { %>

    <table class='main_details_table'>
        <tr>
            <td class='row_title' style="min-width: 150px;">
                <%: Html.LabelFor(x => x.ShortName) %>:
            </td>
            <td style="width: 60%;">
                <%: Model.ShortName %>
            </td>
             <td class='row_title'>
                 <%: Html.LabelFor(x => x.INN)%>&nbsp;|&nbsp;<%: Html.LabelFor(x => x.KPP)%>:
             </td>
             <td style="width: 40%;">
                 <%: Model.INN %> &nbsp;||&nbsp; <%: Model.KPP %>
             </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.FullName) %>:
            </td>
            <td>
                <%: Model.FullName %>
            </td>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.OGRN) %>:
            </td>
            <td>
                <%: Model.OGRN %>
            </td>
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
                <%: Model.Phone %> &nbsp;||&nbsp; <%: Model.Fax %>
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
                <%: Html.LabelFor(x => x.MainBookkeeper) %>:
            </td>
            <td>
                <%: Model.MainBookkeeper %>
            </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.DirectorName) %>:
            </td>
            <td>
                <%if (!String.IsNullOrEmpty(Model.DirectorPost)) {%>
                    <%: Model.DirectorPost%>:
                <%} %>
                <%: Model.DirectorName %>
            </td>  
             <td class='row_title'>
                <%: Html.LabelFor(x => x.CashierName) %>:
            </td>
            <td>
                <%: Model.CashierName %>
            </td>      
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.Comment) %>:
            </td>
            <td colspan='3'>
                <%: Html.CommentFor(x => x.Comment, true) %>
            </td>
        </tr>
    </table>
<%}
else
{ %>
    <table class='main_details_table'>
        <tr>
            <td class='row_title' style="min-width: 150px;">
                <%: Html.LabelFor(x => x.ShortName) %>:
            </td>
            <td style="width: 60%;">
                <%: Model.ShortName%>
            </td>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.INN) %>:
            </td>
            <td style="width: 40%;">
                <%: Model.INN %>
            </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.FullName) %>:
            </td>
            <td>
                <%: Model.FullName%>
            </td>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.OGRNIP) %>:
            </td>
            <td>
                <%: Model.OGRNIP%>
            </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.LegalForm) %>:
            </td>
            <td>
                <%: Model.LegalForm%>
            </td>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.Phone) %>:
            </td>
            <td>
                <%: Model.Phone %> &nbsp;||&nbsp; <%: Model.Fax %>
            </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.FIO) %>:
            </td>
            <td>
                <%: Model.FIO %>
            </td>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.Address) %>:
            </td>
            <td>
                <%: Model.Address %>
            </td>
        </tr>
        <tr>
            <td class='row_title'>
                <%: Html.LabelFor(x => x.Comment) %>:
            </td>
            <td colspan='3'>
                <%: Html.CommentFor(x => x.Comment, true) %>
            </td>
        </tr>
    </table>
<%} %>
