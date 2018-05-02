<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.User.UserMainDetailsViewModel>" %>

<script type="text/javascript">
    User_MainDetails.Init();
</script>

<%: Html.HiddenFor(model => model.IsAdmin) %>
<%: Html.HiddenFor(model => model.IsBlocked) %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 130px'>
            <%:Html.LabelFor(model => model.LastName) %>:
        </td>
        <td style='width: 60%'>
            <span id="LastName"><%: Model.LastName%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.CreationDate) %>:
        </td>
        <td style='width: 40%'>
            <span id="CreationDate"><%: Model.CreationDate%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.FirstName) %>:
        </td>
        <td>
            <span id="FirstName"><%: Model.FirstName%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.IsBlockedText) %>:
        </td>
        <td>
            <span id="IsBlockedText" 
            <% if (Model.AllowToEdit) { %>
                class="select_link"
            <%} %>>
            <%: Model.IsBlockedText%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Patronymic) %>:
        </td>
        <td>
            <span id="Patronymic"><%: Model.Patronymic%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.IsAdminText) %>:
        </td>
        <td>
            <span id="IsAdminText" <%--class="link"--%>><%: Model.IsAdminText%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.DisplayName) %>:
        </td>
        <td>
            <span id="DisplayName"><%: Model.DisplayName%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.RoleCount) %>:
        </td>
        <td>
            <span id="RoleCount"><%: Model.RoleCount%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.PostName) %>:
        </td>
        <td>
            <span id="PostName"><%: Model.PostName%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.TeamCount) %>:
        </td>
        <td>
            <span id="TeamCount"><%: Model.TeamCount%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Login) %>:
        </td>
        <td>
            <span id="Login"><%: Model.Login%></span>
        </td>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.PasswordHash) %>:
        </td>
        <td>
            <span id="PasswordHash"><%: Model.PasswordHash%></span>
            <span id='linkChangePassword' class="main_details_action" style="display:<%= Model.AllowToChangePassword  ? "inline" : "none" %>">[&nbsp;Изменить&nbsp;]</span>
            <span id='linkResetPassword' class="main_details_action" style="display:<%= Model.AllowToResetPassword  ? "inline" : "none" %>">[&nbsp;Изменить&nbsp;]</span>
        </td>
    </tr>
</table>

<div id="userChangePassword"></div>
<div id="userResetPassword"></div>
