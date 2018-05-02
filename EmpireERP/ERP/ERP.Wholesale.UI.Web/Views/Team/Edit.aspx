<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Team.TeamEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Team_Edit.Init();

        function OnSuccessTeamSave(ajaxContext) {
            Team_Edit.OnSuccessTeamSave(ajaxContext);
        }

        function OnFailTeamSave(ajaxContext) {
            Team_Edit.OnFailTeamSave(ajaxContext);
        }

        function OnBeginTeamSave() {
            Team_Edit.OnBeginTeamSave();
        }
        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Team", Model.Title, Model.Name, "/Help/GetHelp_Team_Edit")%>

    <% using (Ajax.BeginForm("Save", "Team", new AjaxOptions() { OnBegin = "OnBeginTeamSave", OnSuccess = "OnSuccessTeamSave", OnFailure = "OnFailTeamSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>

        <%= Html.PageBoxTop(Model.Title)%>
        
        <div style="background: #fff; padding: 5px 0;">

            <div id="messageTeamEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 150px;">
                        <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Team_Edit_Name")%>:
                    </td>
                    <td style="width: 60%">
                        <%: Html.TextBoxFor(model => model.Name, new { style="width: 200px", maxlength="100"})%>
                        <%: Html.ValidationMessageFor(model => model.Name)%>                        
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_Team_Edit_CreationDate")%>:
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
                <input type="submit" id="btnSaveTeam" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
