<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.MovementWaybill.MovementWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        MovementWaybill_Edit.Init();

        function OnBeginMovementWaybillEdit() {
            StartButtonProgress($("#btnSave"));
        }

        function OnSuccessMovementWaybillEdit(ajaxContext) {
            MovementWaybill_Edit.OnSuccessMovementWaybillEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%= Html.PageTitle("MovementWaybill", Model.Title, Model.Name, "/Help/GetHelp_MovementWaybill_Edit")%>

<% using (Ajax.BeginForm("Save", "MovementWaybill", new AjaxOptions() { OnBegin = "OnBeginMovementWaybillEdit",
       OnSuccess = "OnSuccessMovementWaybillEdit", OnFailure = "MovementWaybill_Edit.OnFailMovementWaybillEdit" })) %>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    <%: Html.HiddenFor(model => model.AllowToChangeValueAddedTax)%>
    
    <%= Html.PageBoxTop(Model.Title)%>
    <div style="background: #fff; padding: 5px 0;">
    
        <div id="messageMovementWaybillEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title" style="min-width: 160px;">
                    <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_MovementWaybill_Edit_Number")%>:
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
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_MovementWaybill_Edit_Date")%>:
                </td>
                <td style="width: 50%">                                        
                    <%= Html.DatePickerFor(model => model.Date, isDisabled: true)%>
                    <%: Html.ValidationMessageFor(model => model.Date)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.SenderStorageId, "/Help/GetHelp_MovementWaybill_Edit_SenderStorage")%>:
                </td>
                <td>
                    <%: Html.DropDownListFor(model => model.SenderStorageId, Model.SenderStorageList, new { style = "min-width: 200px" }, !Model.AllowToEditSenderAndSenderStorage)%>
                    <%: Html.ValidationMessageFor(model => model.SenderStorageId) %>
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.RecipientStorageId, "/Help/GetHelp_MovementWaybill_Edit_RecipientStorage")%>:
                </td>
                <td>                    
                    <%: Html.DropDownListFor(model => model.RecipientStorageId, Model.RecipientStorageList, new { style = "min-width: 200px" }, !Model.AllowToEditRecipientAndRecipientStorage)%>
                    <%: Html.ValidationMessageFor(model => model.RecipientStorageId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.SenderId, "/Help/GetHelp_MovementWaybill_Edit_Sender")%>:
                </td>
                <td>
                    <%: Html.DropDownListFor(model => model.SenderId, Model.SenderAccountOrganizationList, new { style = "min-width: 200px" }, !Model.AllowToEditSenderAndSenderStorage)%>
                    <%: Html.ValidationMessageFor(model => model.SenderId)%> 
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.RecipientId, "/Help/GetHelp_MovementWaybill_Edit_Recipient")%>:
                </td>
                <td>
                    <%: Html.DropDownListFor(model => model.RecipientId, Model.RecipientAccountOrganizationList, new { style = "min-width: 200px" }, !Model.AllowToEditRecipientAndRecipientStorage)%>
                    <%: Html.ValidationMessageFor(model => model.RecipientId)%>                 
                </td>
            </tr>    
            <tr>    
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.ValueAddedTaxId, "/Help/GetHelp_MovementWaybill_Edit_ValueAddedTax")%>:
                </td>
                <td>
                    <%: Html.ParamDropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList, null, "Укажите ставку НДС", !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>  
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_MovementWaybill_Edit_Curator")%>:
                </td>
                <td>
                    <% if (Model.AllowToChangeCurator)
                       { %>
                        <span id="CuratorName" class="link"><%: Model.CuratorName%></span>
                    <%}
                       else
                       { %>
                       <%: Model.CuratorName%>
                    <% } %>
                    <%: Html.HiddenFor(model => model.CuratorId)%>
                    <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                </td>
                <td colspan="3">
                    <%: Html.CommentFor(model => model.Comment, new { style = "width: 99%;" }, !Model.AllowToEdit, rowsCount: 4)%>                    
                    <%:Html.ValidationMessageFor(model => model.Comment)%>
                </td>
            </tr>
        </table>
        
        <div class="button_set">
            <input type="submit" id="btnSave" value="Сохранить" />
            <input type="button" id="btnBack" value="Назад" />
        </div>

    </div>
    <%= Html.PageBoxBottom() %>
<% } %>      
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

