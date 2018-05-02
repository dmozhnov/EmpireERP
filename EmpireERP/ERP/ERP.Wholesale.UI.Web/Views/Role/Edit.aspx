<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Role.RoleEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#Name").focus();

            $("#btnBack").live("click", function () {
                window.location = $('#BackURL').val();
            });
        });

        function OnSuccessRoleSave(ajaxContext) {
            window.location = "/Role/Details?id=" + ajaxContext + "&backURL=/Role/List";
        }

        function OnBeginRoleSave() {
            StartButtonProgress($("#btnRoleSave"));
        }

        function OnFailRoleSave(ajaxContext) {
            ShowErrorMessage(ajaxContext.responseText, "messageRoleEdit");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Role", Model.Title, Model.Name, "/Help/GetHelp_Role_Edit")%>

    <% using (Ajax.BeginForm("Save", "Role", new AjaxOptions() { OnBegin = "OnBeginRoleSave", OnSuccess = "OnSuccessRoleSave", OnFailure = "OnFailRoleSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>

        <%= Html.PageBoxTop(Model.Title)%>
        
        <div style="background: #fff; padding: 5px 0;">

            <div id="messageRoleEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 150px;">
                        <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Role_Edit_Name")%>:
                    </td>
                    <td style="width: 60%">
                        <%: Html.TextBoxFor(model => model.Name, new { style="width: 200px", maxlength="100"})%>
                        <%: Html.ValidationMessageFor(model => model.Name)%>                        
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_Role_Edit_CreationDate")%>:
                    </td>
                    <td style="width: 40%">
                        <%: Model.CreationDate%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>
            </table>
            
            <div class="button_set">
                <input type="submit" id="btnRoleSave" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
