<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.WriteoffWaybill.WriteoffWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        WriteoffWaybill_Edit.Init();

        function OnBeginWriteoffWaybillEdit() {
            StartButtonProgress($("#btnWriteoffWaybill"));
        }

        function OnSuccessWriteoffWaybillEdit(ajaxContext) {
            WriteoffWaybill_Edit.OnSuccessWriteoffWaybillEdit(ajaxContext);
        }

        function OnSuccessWriteoffReasonSave(ajaxContext) {
            WriteoffWaybill_Edit.OnSuccessWriteoffReasonEdit(ajaxContext);
        }        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("WriteoffWaybill", Model.Title, Model.Name, "/Help/GetHelp_WriteoffWaybill_Edit")%>

    <% using (Ajax.BeginForm("Edit", "WriteoffWaybill", new AjaxOptions() { OnBegin = "OnBeginWriteoffWaybillEdit",
           OnSuccess = "OnSuccessWriteoffWaybillEdit", OnFailure = "WriteoffWaybill_Edit.OnFailWriteoffWaybillEdit" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>
    
        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">
    
            <div id="messageWriteoffWaybillEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 110px;">
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_WriteoffWaybill_Edit_Number")%>:
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
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_WriteoffWaybill_Edit_Date")%>:
                    </td>
                    <td style="width: 50%">
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: true)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.SenderStorageId, "/Help/GetHelp_WriteoffWaybill_Edit_SenderStorage")%>:
                    </td>
                    <td style="width: 50%">
                        <% bool isDisabled = (Model.Id != Guid.Empty || !Model.AllowToEdit); %>
                        <%: Html.DropDownListFor(model => model.SenderStorageId, Model.StorageList, isDisabled)%>
                        <%: Html.ValidationMessageFor(model => model.SenderStorageId) %>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.WriteoffReasonId, "/Help/GetHelp_WriteoffWaybill_Edit_WriteoffReason")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Html.DropDownListFor(model => model.WriteoffReasonId, Model.WriteoffReasonList, !Model.AllowToEdit)%>
                        <% if(Model.AllowToEdit && Model.AllowToAddReason) { %><span class="edit_action" id='btnAddWriteoffReason'>[ Добавить ]</span> <%} %>
                        <%: Html.ValidationMessageFor(model => model.WriteoffReasonId)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.SenderId, "/Help/GetHelp_WriteoffWaybill_Edit_Sender")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.SenderId, Model.SenderList, new { style = "min-width: 200px" }, isDisabled)%>
                        <%: Html.ValidationMessageFor(model => model.SenderId)%> 
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_WriteoffWaybill_Edit_Curator")%>:
                    </td>
                    <td>
                        <% if (Model.AllowToChangeCurator)
                           { %>
                            <span id="CuratorName" class="link"><%: Model.CuratorName%></span>
                        <%} 
                          else 
                          { %>
                            <%: Model.CuratorName%>
                        <%} %>
                        <%: Html.HiddenFor(model => model.CuratorId)%>
                        <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                </tr>  
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">                        
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>                        
                    </td>
                </tr>
            </table>

            <div class="button_set">                
                <%: Html.SubmitButton("btnWriteoffWaybill", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>             
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>

    <div id='writeoffReasonEdit'></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
