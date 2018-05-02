<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Provider.ProviderEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%:Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Provider_Edit.Init();

        function OnSuccessProviderTypeSave(ajaxContext) {
            Provider_Edit.OnSuccessProviderTypeEdit(ajaxContext);
        }

        function OnFailProviderSave(ajaxContext) {
            Provider_Edit.OnFailProviderSave(ajaxContext)
        }

        function OnSuccessProviderSave(ajaxContext) {
            Provider_Edit.OnSuccessProviderSave(ajaxContext);
        }

        function OnBeginProviderSave() {
            StartButtonProgress($("#btnSaveProvider"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Ajax.BeginForm("Save", "Provider", new AjaxOptions() { OnFailure = "OnFailProviderSave", OnSuccess = "OnSuccessProviderSave",
   OnBegin = "OnBeginProviderSave" }))%> 
<%{ %>
    <%:Html.HiddenFor(model => model.Id)%>
    <%:Html.HiddenFor(model => model.BackURL)%>
    
    <%=Html.PageTitle("Provider", Model.Title, Model.Name, "/Help/GetHelp_Provider_Edit")%>

    <%=Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 10px 5px 5px;">
            
            <div id="messageProviderEdit"></div>
            
            <table class="editor_table">
                <tr>
                    <td class="row_title" style="width: 120px"><%:Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Provider_Edit_Name")%>:</td>
                    <td>
                        <%:Html.TextBoxFor(model => model.Name, new { maxlength = 200, style = "width: 400px;" })%>
                        <%:Html.ValidationMessageFor(model => model.Name)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.Type, "/Help/GetHelp_Provider_Edit_Type")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Type, Model.TypeList)%>
                        <% if(Model.AllowToCreateProviderType) { %>
                            <span class="edit_action" id="btnAddProviderType">[ Добавить ]</span>
                        <%} %>
                        <%:Html.ValidationMessageFor(model => model.Type)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.Reliability, "/Help/GetHelp_Provider_Edit_Reliability")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Reliability, Model.ReliabilityList)%>
                        <%:Html.ValidationMessageFor(model => model.Reliability)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.Rating, "/Help/GetHelp_Provider_Edit_Rating")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Rating, Model.RatingList)%>
                        <%:Html.ValidationMessageFor(model => model.Rating)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title"><%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:</td>
                    <td>
                        <%:Html.CommentFor(model => model.Comment, new { style = "width: 400px;" }, rowsCount: 4)%>
                        <%:Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>
            </table>

             <div class="button_set">
                <input id="btnSaveProvider" type="submit" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
    <%=Html.PageBoxBottom()%>    
<%} %>

<div id="providerTypeEdit"></div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
