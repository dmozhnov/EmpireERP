<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Storage.StorageSectionEditViewModel>" %>

<script type="text/javascript">
    function OnFailStorageSectionSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageStorageSectionEdit");
    }

    function OnBeginStorageSectionSave() {
        StartButtonProgress($("#btnSaveStorageSection"));
    }
</script>

<% using (Ajax.BeginForm("SaveSection", "Storage", new AjaxOptions() { OnBegin = "OnBeginStorageSectionSave",
       OnSuccess = "OnSuccessStorageSectionSave", OnFailure = "OnFailStorageSectionSave" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.HiddenFor(model => model.StorageId)%>
    
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Storage_Edit_StorageSection") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">
        <div id="messageStorageSectionEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Name)%>:                    
                </td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:340px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>                   
            </tr>            
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.Square)%>:</td>
                <td style="width: 140px">
                    <%: Html.TextBoxFor(model => model.Square, new { style = "width:80px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Square)%>
                </td>
                <td class='row_title'><%: Html.LabelFor(model => model.Volume)%>:</td>
                <td style="width: 140px">
                    <%: Html.TextBoxFor(model => model.Volume, new { style = "width:80px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Volume)%>
                </td>
            </tr>          
        </table>
    </div>

    <div class="button_set">
        <%: Html.SubmitButton("btnSaveStorageSection", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>
