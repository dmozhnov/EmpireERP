<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Storage.StorageEditViewModel>" %>

<script type="text/javascript">
    function OnBeginStorageSave() {
        StartButtonProgress($("#btnSaveStorage"));
    }
    
    function OnFailStorageSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageStorageEdit");
    } 
</script>

<% using (Ajax.BeginForm("Save", "Storage", new AjaxOptions() { OnSuccess = "OnSuccessStorageSave", OnFailure = "OnFailStorageSave", OnBegin = "OnBeginStorageSave" }))%>
<%{ %>
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Storage_Edit") %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px">
        <div id="messageStorageEdit"></div>        

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.LabelFor(model => model.Name)%>:
                    <%: Html.HiddenFor(model => model.Id)%>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.Name, new { style = "width:400px" })%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>                   
            </tr>            
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.StorageTypeId)%>:
                </td>
                <td>
                    <%: Html.DropDownListFor(x => x.StorageTypeId, Model.StorageTypeList, new { style = "width:200px" })%>                 
                    <%: Html.ValidationMessageFor(model => model.StorageTypeId)%>
                </td>                       
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                </td>
                <td>
                    <%: Html.CommentFor(model => model.Comment, new { style = "width:400px" }, rowsCount: 4)%>
                    <%: Html.ValidationMessageFor(model => model.Comment)%>
                </td>
            </tr>             
        </table>
    </div>

    <div class="button_set">
        <input id="btnSaveStorage" type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()" />        
    </div>
<%} %>


