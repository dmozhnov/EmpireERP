<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Storage.StorageMainDetailsViewModel>" %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style='min-width:100px'>
            <%: Html.LabelFor(model=>model.Id) %>:
        </td>
        <td width='60%'>
            <%: Model.Id.ToString() %>
        </td>
        <td class="row_title" style='min-width:100px'>
            <%: Html.LabelFor(model=>model.SectionCount) %>:
        </td>
        <td style='width:40%'>
            <span id="SectionCount"><%: Model.SectionCount.ToString() %></span>             
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model=>model.Name) %>:
        </td>
        <td>
            <span id="Name"><%: Model.Name %></span>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model=>model.AccountOrganizationCount) %>:
        </td>
        <td>
            <span id="AccountOrganizationCount"><%: Model.AccountOrganizationCount.ToString() %></span>            
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model=>model.TypeName) %>:
        </td>
        <td colspan="3">
            <span id="TypeName"><%: Model.TypeName %></span>
        </td>                
    </tr>        
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model=>model.Comment) %>:
        </td>
        <td colspan="3">
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>