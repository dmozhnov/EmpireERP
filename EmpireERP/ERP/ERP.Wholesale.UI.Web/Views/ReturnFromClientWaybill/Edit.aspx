<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill.ReturnFromClientWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ReturnFromClientWaybill_Edit.Init();

        function OnSuccessReturnFromClientReasonSave(ajaxContext) {
            ReturnFromClientWaybill_Edit.OnSuccessReturnFromClientReasonEdit(ajaxContext);
        }

        function OnBeginReturnFromClientWaybillEdit() {
            StartButtonProgress($("#btnSaveReturnFromClientWaybill"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ReturnFromClientWaybill", Model.Title, Model.Name, "/Help/GetHelp_ReturnFromClientWaybill_Edit")%>

    <% using (Ajax.BeginForm("Save", "ReturnFromClientWaybill", new AjaxOptions() { OnBegin = "OnBeginReturnFromClientWaybillEdit",
           OnSuccess = "ReturnFromClientWaybill_Edit.OnSuccessReturnFromClientWaybillEdit", OnFailure = "ReturnFromClientWaybill_Edit.OnFailReturnFromClientWaybillEdit" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%: Html.HiddenFor(model => model.Id) %>
        
        <%= Html.PageBoxTop(Model.Title)%>
    
        <div style="background: #fff; padding: 5px 0;">
            <div id="messageReturnFromClientWaybillEdit"></div>
                
            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="min-width: 100px">
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ReturnFromClientWaybill_Edit_Number")%>:
                    </td>
                    <td style="width: 50%; min-width: 420px;">                       
                        <%: Html.HiddenFor(model => model.AllowToGenerateNumber)%>
                        
                        <div id="rbIsAutoNumber_true_wrapper" style="text-align: left; float: left; margin-right: 10px;">
                            <%: Html.RadioButtonFor(model => model.IsAutoNumber, 1, new { id = "rbIsAutoNumber_true" })%>                            
                            <label for="rbIsAutoNumber_true"><%: Model.IsAutoNumber_true%></label>
                        </div>                        
                        <div id="rbIsAutoNumber_false_wrapper" style="text-align: left; float: left; margin-right: 5px; ">
                            <%: Html.RadioButtonFor(model => model.IsAutoNumber, 0, new { id = "rbIsAutoNumber_false" })%>                            
                            <label for="rbIsAutoNumber_false"><%: Model.IsAutoNumber_false%></label>
                        </div>
                        <div class="row_title" style="text-align: left; float: left">
                            <%: Html.TextBoxFor(model => model.Number, new { maxlength = "25", style = "width: 120px" } ) %>
                            <%: Html.ValidationMessageFor(model => model.Number) %>
                        </div>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.ClientName, "/Help/GetHelp_ReturnFromClientWaybill_Edit_ClientName")%>:
                    </td>
                    <td style="width: 50%">
                        <% if (Model.ClientId == "0") { %>
                            <span class="select_link" id="ClientName"><%: Model.ClientName %></span>
                        <% } else { %>
                            <%: Model.ClientName %>
                        <% } %>
                        <%: Html.HiddenFor(model => model.ClientId) %>
                        <%: Html.ValidationMessageFor(model => model.ClientId)%>
                    </td>                                        
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ReturnFromClientWaybill_Edit_Date")%>:
                    </td>
                    <td>
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: true)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.DealName, "/Help/GetHelp_ReturnFromClientWaybill_Edit_DealName")%>:
                    </td>
                    <td>
                        <% if (Model.DealId == "0") { %>
                            <span class="select_link no_auto_progress" id="DealName"><%: Model.DealName%></span>
                        <% } else { %>
                            <%: Model.DealName%>
                        <% } %>
                        <%: Html.HiddenFor(model => model.DealId)%>
                        <%: Html.ValidationMessageFor(model => model.DealId)%>
                    </td>
                </tr>
                <tr>                    
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_ReturnFromClientWaybill_Edit_Curator")%>:
                    </td>
                    <td>
                        <% if (Model.AllowToChangeCurator)
                           { %>
                            <span id="CuratorName" class="link"><%: Model.CuratorName%></span>
                        <% }
                           else
                           { %>
                            <%: Model.CuratorName%>
                        <% } %>
                        <%: Html.HiddenFor(model => model.CuratorId)%>
                        <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.TeamId, "/Help/GetHelp_ReturnFromClientWaybill_Edit_Team")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model=>model.TeamId, Model.TeamList, new { style="min-width:250px;" }, !Model.AllowToEditTeam) %>
                        <%: Html.ValidationMessageFor(model=>model.TeamId) %>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.ReturnFromClientReasonId, "/Help/GetHelp_ReturnFromClientWaybill_Edit_ReturnFromClientReason")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.ReturnFromClientReasonId, Model.ReturnFromClientReasonList, new { style = "min-width:100px;" })%>
                        <% if(Model.AllowToCreateReturnFromClientReason) { %>
                            <span class="edit_action" id='btnCreateReturnFromClientReason'>[ Добавить ]</span>
                        <% } %>
                        <%: Html.ValidationMessageFor(model => model.ReturnFromClientReasonId) %>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.AccountOrganizationId, "/Help/GetHelp_ReturnFromClientWaybill_Edit_AccountOrganization")%>:
                    </td>
                    <td>
                        <%: Model.AccountOrganizationName%>
                        <%: Html.HiddenFor(model => model.AccountOrganizationId)%>
                        <%: Html.ValidationMessageFor(model => model.AccountOrganizationId)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'></td>
                    <td></td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.ReceiptStorageId, "/Help/GetHelp_ReturnFromClientWaybill_Edit_ReceiptStorage")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.ReceiptStorageId, Model.ReceiptStorageList, new { style = "min-width:250px" }, !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.ReceiptStorageId)%>
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
            
            <br />
            <br />
            
            <div class="button_set">
                <input id="btnSaveReturnFromClientWaybill" type="submit" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>

        <%= Html.PageBoxBottom() %>
    <% } %>

    <div id="returnFromClientReasonEdit"></div>
    <div id="selector"></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
