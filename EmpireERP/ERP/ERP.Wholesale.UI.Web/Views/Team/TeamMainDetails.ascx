<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Team.TeamMainDetailsViewModel>" %>

<script type="text/javascript">
    Team_MainDetails.Init();    
</script>

<%:Html.HiddenFor(x => x.AllowToViewCreatorDetails) %>
<%: Html.HiddenFor(x => x.CreatorId) %>

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
            <%:Html.LabelFor(model => model.CreationDate) %>:
        </td>
        <td>
            <span id="CreationDate"><%: Model.CreationDate%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.StorageCount) %>:
        </td>
        <td>
            <span id="StorageCount"><%: Model.StorageCount%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.CreatedBy) %>:
        </td>
        <td>
            <a id="CreatedBy"><%: Model.CreatedBy%></a>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.DealCount) %>:
        </td>
        <td>
            <span id="DealCount"><%: Model.DealCount%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Comment) %>:
        </td>
        <td colspan="3">
            <%: Html.CommentFor(model => model.Comment, true) %>
        </td>        
    </tr>
</table>