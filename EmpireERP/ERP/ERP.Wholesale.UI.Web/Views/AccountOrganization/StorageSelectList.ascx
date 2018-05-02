<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountOrganization.LinkedStorageListViewModel>" %>

<script type="text/javascript">
    AccountOrganization_StorageSelectList.Init();

    function OnFailStorageAdd(ajaxContext) {
        AccountOrganization_StorageSelectList.OnFailStorageAdd(ajaxContext);
    }

    function OnBeginStorageAdd() {
        StartButtonProgress($("#btnSaveLinkedStorage"));
    }
</script>

<%using (Ajax.BeginForm("StoragesList", "AccountOrganization", new AjaxOptions() { OnBegin = "OnBeginStorageAdd", 
      OnSuccess = "OnSuccessStorageAdd", OnFailure = "OnFailStorageAdd" })) %>
<%{%>
    <%: Html.HiddenFor(model => model.OrganizationId) %>
    
    <div class="modal_title">Добавление связанного места хранения <%: Html.Help("/Help/GetHelp_AccountOrganization_Details_StorageSelectList") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">        
        <div id="messageSelectStorage"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.StorageList) %>:
                </td>
                <td>
                    <%: Html.DropDownListFor(model => model.StorageId, Model.StorageList, new { style = "width:210px" })%>
                </td>
            </tr>
        </table>        
    </div>

    <div class="button_set">
        <input id="btnSaveLinkedStorage" type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()"/>
    </div>
<%}%>
