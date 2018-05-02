<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill.ChangeOwnerWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Добавление накладной смены собственника
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ChangeOwnerWaybill_Edit.Init();

        function OnBeginChangeOwnerWaybillEdit() {
            StartButtonProgress($("#btnSaveChangeOwnerWaybill"));
        }

        function OnSuccessChangeOwnerWaybillEdit(ajaxContext) {
            ChangeOwnerWaybill_Edit.OnSuccessChangeOwnerWaybillEdit(ajaxContext);
        }

        function OnFailChangeOwnerWaybillEdit(ajaxContext) {
            ChangeOwnerWaybill_Edit.OnFailChangeOwnerWaybillEdit(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%=Html.PageTitle("ChangeOwnerWaybill", Model.Title, Model.Name, "/Help/GetHelp_ChangeOwnerWaybill_Edit")%>

    <% using (Ajax.BeginForm("Save", "ChangeOwnerWaybill", new AjaxOptions() { OnBegin = "OnBeginChangeOwnerWaybillEdit",
           OnSuccess = "OnSuccessChangeOwnerWaybillEdit", OnFailure = "OnFailChangeOwnerWaybillEdit" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL)  %>

        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">
    
            <div id="messageChangeOwnerWaybillEdit"></div>
            
            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 160px;">
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Number")%>:
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
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Date")%>:
                    </td>
                    <td style="width: 50%">                                        
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: true)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title" style="min-width: 160px;">
                        <%: Html.HelpLabelFor(model => model.StorageId, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Storage")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Html.DropDownListFor(model => model.StorageId, Model.StorageList, !Model.AllowToEdit || !Model.IsNew)%>
                        <%: Html.ValidationMessageFor(model => model.StorageId)%>
                    </td>
                    <td class="row_title" style="min-width: 160px;">
                        <%: Html.HelpLabelFor(model => model.SenderId, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Sender")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Html.DropDownListFor(model => model.SenderId, Model.AccountOrganizationList, new { disabled = "disabled" }, !Model.AllowToEdit || !Model.IsNew)%>
                        <%: Html.ValidationMessageFor(model => model.SenderId)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.ValueAddedTaxId, "/Help/GetHelp_ChangeOwnerWaybill_Edit_ValueAddedTax")%>:
                    </td>
                    <td style="width: 50%">                                        
                        <%: Html.DropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList, !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.RecipientId, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Recipient")%>:
                    </td>
                    <td style="width: 50%">                                        
                        <%: Html.DropDownListFor(model => model.RecipientId, Model.AccountOrganizationList, new { disabled = "disabled" }, !Model.AllowToEdit || !Model.IsNew)%>
                        <%: Html.ValidationMessageFor(model => model.RecipientId)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_ChangeOwnerWaybill_Edit_Curator")%>:
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
                        <%: Html.ValidationMessageFor(model => model.CuratorId) %>
                    </td>
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                       <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">                        
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 99%;" }, !Model.AllowToEdit, rowsCount: 4) %> 
                        <%:Html.ValidationMessageFor(model => model.Comment)%>                       
                    </td>
                </tr>
            </table>            

            <div class="button_set">
                <%= Html.SubmitButton("btnSaveChangeOwnerWaybill", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>        
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>

        <%= Html.PageBoxBottom() %>
    <%} %>
    <div id="curatorSelector"></div>
</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
