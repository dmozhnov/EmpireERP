<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Client.ClientEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Client_Edit.Init();

        function OnFailClientSave(ajaxContext) {
            Client_Edit.OnFailClientSave(ajaxContext);
        }

        function OnSuccessClientSave(ajaxContext) {
            Client_Edit.OnSuccessClientSave(ajaxContext);
        }

        function OnSuccessClientServiceProgramSave(ajaxContext) {
            Client_Edit.OnSuccessClientServiceProgramEdit(ajaxContext);
        }

        function OnSuccessClientTypeSave(ajaxContext) {
            Client_Edit.OnSuccessClientTypeSave(ajaxContext);
        }

        function OnSuccessClientRegionSave(ajaxContext) {
            Client_Edit.OnSuccessClientRegionSave(ajaxContext);
        }

        function OnBeginClientSave() {
            StartButtonProgress($("#btnSaveClient"));
        }    
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Ajax.BeginForm("Save", "Client", new AjaxOptions() { OnBegin = "OnBeginClientSave",
       OnFailure = "OnFailClientSave", OnSuccess = "OnSuccessClientSave" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.BackURL)%>
    <%:Html.HiddenFor(model => model.Id)%>

    <%=Html.PageTitle("Client", Model.Title, Model.Name, "/Help/GetHelp_Client_Edit")%>
    
    <%=Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 10px 5px 5px;">       
            
            <div id='messageClientEdit'></div>

            <table class='editor_table'>
                <tr>
                    <td class='row_title' style='width: 160px'><%:Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Client_Edit_Name")%>:</td>
                    <td>
                        <%:Html.TextBoxFor(model => model.Name, new { maxlength = 200, size = 60 })%>
                        <%:Html.ValidationMessageFor(model => model.Name)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%: Html.HelpLabelFor(model => model.FactualAddress, "/Help/GetHelp_Client_Edit_FactualAddress")%></td>
                    <td>
                        <%:Html.TextBoxFor(model => model.FactualAddress, new { maxlength = 250, size = 60 })%>
                        <%:Html.ValidationMessageFor(model => model.FactualAddress)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%: Html.HelpLabelFor(model => model.ContactPhone, "/Help/GetHelp_Client_Edit_ContactPhone")%></td>
                    <td>
                        <%:Html.TextBoxFor(model => model.ContactPhone, new { maxlength = 20, size = 60 })%>
                        <%:Html.ValidationMessageFor(model => model.ContactPhone)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.HelpLabelFor(model => model.TypeId, "/Help/GetHelp_Client_Edit_Type")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.TypeId, Model.TypeList)%>
                        <% if(Model.AllowToAddClientType) { %><span id='AddType' class="edit_action">[ Добавить ]</span> <% } %>
                        <%:Html.ValidationMessageFor(model => model.TypeId)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.HelpLabelFor(model => model.Rating, "/Help/GetHelp_Client_Edit_Rating")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Rating, Model.RatingList)%>
                        <%:Html.ValidationMessageFor(model => model.Rating)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.HelpLabelFor(model => model.Loyalty, "/Help/GetHelp_Client_Edit_Loyalty")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Loyalty, Model.LoyaltyList)%>
                        <%:Html.ValidationMessageFor(model => model.Loyalty)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.HelpLabelFor(model => model.ServiceProgramId, "/Help/GetHelp_Client_Edit_ServiceProgram")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.ServiceProgramId, Model.ServiceProgramList)%>
                        <% if(Model.AllowToAddClientServiceProgram) { %><span class="edit_action" id='AddServiceProgram'>[ Добавить ]</span> <% } %>
                        <%:Html.ValidationMessageFor(model => model.ServiceProgramId)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'><%:Html.HelpLabelFor(model => model.RegionId, "/Help/GetHelp_Client_Edit_Region")%>:</td>
                    <td>
                        <%:Html.DropDownListFor(model => model.RegionId, Model.RegionList)%>
                        <% if(Model.AllowToAddClientRegion) { %><span class="edit_action" id='AddRegion'>[ Добавить ]</span> <% } %>
                        <%:Html.ValidationMessageFor(model => model.RegionId)%>
                    </td>
                </tr> 
                <tr>
                    <td class='row_title'>
						<%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
					</td>
                    <td>
                        <%:Html.CommentFor(model => model.Comment, new { style = "width: 340px" }, rowsCount: 4)%>
                        <%:Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>              
            </table>

            <div class='button_set'>
                <input id="btnSaveClient" type="submit" value="Сохранить" />
                <input type="button" id='btnCancel' value="Назад" />
            </div>
        </div>

    <%=Html.PageBoxBottom()%>
<%} %>

<div id='regionEdit'></div>
<div id='clientServiceProgramEdit'></div>
<div id='clientTypeEdit'></div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
