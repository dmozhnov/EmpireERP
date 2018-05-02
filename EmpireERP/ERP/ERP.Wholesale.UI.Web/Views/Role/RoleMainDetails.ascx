<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.RoleMainDetailsViewModel>" %>

<script type="text/javascript">
    // обновление основной информации
    function RefreshMainDetails(details) {
        $("#Name").text(details.Name);
        $("#UserCount").text(details.UserCount);        
        $("#Comment").text(details.Comment);
    }
</script>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 130px'>
            <%:Html.LabelFor(model => model.Name) %>:
        </td>
        <td style='width: 60%'>
            <span id="Name"><%: Model.Name%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.UserCount) %>:
        </td>
        <td style='width: 40%'>
            <span id="UserCount"><%: Model.UserCount%></span>
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