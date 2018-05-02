<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.User.UserEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        User_Edit.Init();

        function OnSuccessUserSave(ajaxContext) {
            User_Edit.OnSuccessUserSave(ajaxContext);
        }

        function OnBeginUserSave() {
            User_Edit.OnBeginUserSave();
        }

        function OnFailUserSave(ajaxContext) {
            User_Edit.OnFailUserSave(ajaxContext);
        }

        function OnSuccessEmployeePostSave(ajaxContext) {
            User_Edit.OnSuccessEmployeePostSave(ajaxContext)
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("User", Model.Title, Model.Name, "/Help/GetHelp_User_Edit")%>

    <% using (Ajax.BeginForm("Save", "User", new AjaxOptions() { OnBegin = "OnBeginUserSave", OnSuccess = "OnSuccessUserSave", OnFailure = "OnFailUserSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%: Html.HiddenFor(model => model.DisplayNameTemplate) %>

        <%= Html.PageBoxTop(Model.Title)%>

        <div style="background: #fff; padding: 5px 0;">

            <div id="messageUserEdit"></div>

            <div class="group_title">Общая информация</div>
            <div class="h_delim"></div>
            <br />

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 150px;">
                        <%: Html.HelpLabelFor(model => model.LastName, "/Help/GetHelp_User_Edit_LastName")%>:
                    </td>
                    <td style="width: 60%">
                        <%: Html.TextBoxFor(model => model.LastName, new { style="width: 200px", maxlength="100"})%>
                        <%: Html.ValidationMessageFor(model => model.LastName)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CreatedBy, "/Help/GetHelp_User_Edit_CreateBy")%>:
                    </td>
                    <td style="width: 40%">
                        <%: Model.CreatedBy%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.FirstName, "/Help/GetHelp_User_Edit_FirstName")%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.FirstName, new { style = "width: 200px", maxlength = "100" })%>
                        <%: Html.ValidationMessageFor(model => model.FirstName)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_User_Edit_CreationDate")%>:
                    </td>
                    <td>
                        <%: Model.CreationDate%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Patronymic, "/Help/GetHelp_User_Edit_Patronymic")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(model => model.Patronymic, new { style = "width: 200px", maxlength = "100" })%>
                        <%: Html.ValidationMessageFor(model => model.Patronymic)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.DisplayName, "/Help/GetHelp_User_Edit_DisplayName")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.DropDownListFor(model => model.DisplayName, new List<SelectListItem>(), new { style = "min-width: 200px" })%>
                        <%: Html.ValidationMessageFor(model => model.DisplayName)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.EmployeePostId, "/Help/GetHelp_User_Edit_EmployeePost")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.EmployeePostId, Model.EmployeePostList, new { style = "min-width: 200px" })%>
                        <span id="linkAddEmployeePost" class="edit_action">[ Добавить ]</span>
                        <%:Html.ValidationMessageFor(model => model.EmployeePostId)%>
                    </td>
                </tr>
                <% if (Model.Id == 0) { %>
                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.TeamId, "/Help/GetHelp_User_Edit_Team")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.TeamId, Model.TeamList, new { style = "min-width: 200px" })%>
                        <%:Html.ValidationMessageFor(model => model.TeamId)%>
                    </td>
                </tr>
                <% } %>
            </table>
            <br />

            <div class="group_title">Информация для доступа в систему</div>
            <div class="h_delim"></div>
            <br />

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 150px;">
                        <%: Html.HelpLabelFor(model => model.Login, "/Help/GetHelp_User_Edit_Login")%>:
                    </td>
                    <td style="width: 100%">
                        <%: Html.TextBoxFor(model => model.Login, new { style="width: 200px", maxlength="30"})%>
                        <%: Html.ValidationMessageFor(model => model.Login)%>
                        <%: Html.HiddenFor(model => model.LoginIsUnique) %>
                        <%: Html.ValidationMessageFor(model => model.LoginIsUnique)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Password, "/Help/GetHelp_User_Edit_Password")%>:
                    </td>
                    <td>
                        <% if (Model.Id == 0)
                           { %>
                        <%: Html.PasswordFor(model => model.Password, new { style = "width: 200px", maxlength = "30" })%>
                        <%}
                           else
                           { %>
                                <%: Html.TextBoxFor(model => model.Password, new { style = "width: 200px", maxlength = "30" }, true)%>
                        <%} %>
                        <%: Html.ValidationMessageFor(model => model.Password)%>
                    </td>
                </tr>
                <% if (Model.Id == 0)
                   { %>
                <tr>                    
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.PasswordConfirmation, "/Help/GetHelp_User_Edit_PasswordConfirmation")%>:
                    </td>
                    <td>
                        <%: Html.PasswordFor(model => model.PasswordConfirmation, new { style = "width: 200px", maxlength = "30" })%>
                        <%: Html.ValidationMessageFor(model => model.PasswordConfirmation)%>
                    </td>
                </tr>
                <%} %>                        
            </table>
            <br />

            <div class="button_set">
                <input type="submit" id="btnUserSave" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>

    <div id="employeePostEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
