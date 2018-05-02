<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Storage.AccountOrganizationSelectList>" %>

<script type="text/javascript">
    Storage_AccountOrganizationSelectList.Init();
    
    function OnFailStorageAccountOrganizationAdd(ajaxContext) {
        Storage_AccountOrganizationSelectList.OnFailStorageAccountOrganizationAdd(ajaxContext);
    }

    function OnBeginStorageAccountOrganizationAdd() {
        StartButtonProgress($("#btnSaveAccountOrganization"));
    }
</script>

<%using (Ajax.BeginForm("AddAccountOrganization", "Storage", new AjaxOptions() { OnBegin = "OnBeginStorageAccountOrganizationAdd",
      OnSuccess = "OnSuccessStorageAccountOrganizationAdd", OnFailure = "OnFailStorageAccountOrganizationAdd" })) %>
<%{%>
    <%: Html.HiddenFor(model => model.StorageId) %>

    <div class="modal_title">Добавление связанной организации<%: Html.Help("/Help/GetHelp_Storage_Edit_AccountOrganizationSelectList") %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px">
        <div id="messageAccountOrganizationEdit"></div>    

        <table class="editor_table">
            <tr>
                <td><%: Html.LabelFor(model => model.AccountOrganizationList)%>:</td>
                <td>                                        
                    <%: Html.DropDownList("SelectedAccountOrganizationId", Model.AccountOrganizationList, new { style = "width:210px" })%>
                    <%: Html.ValidationMessageFor(model => model.SelectedAccountOrganizationId) %>                    
                </td>
            </tr>
        </table>    
    </div>
            
    <div class="button_set">
        <input id="btnSaveAccountOrganization" type="submit" value="Сохранить"/>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
    
<%}%>
