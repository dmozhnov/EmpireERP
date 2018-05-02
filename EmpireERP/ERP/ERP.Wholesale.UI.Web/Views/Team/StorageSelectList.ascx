<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Team.LinkedStorageListViewModel>" %>

<script type="text/javascript">
    Team_StorageSelectList.Init();

    function OnFailStorageAdd(ajaxContext) {
        Team_StorageSelectList.OnFailStorageAdd(ajaxContext);
    }

    function OnSuccessStorageAdd(ajaxContext) {
        Team_StorageSelectList.OnSuccessStorageAdd(ajaxContext);
    }

    function OnBeginStorageAdd() {
        Team_StorageSelectList.OnBeginStorageAdd();
    }   
</script>

<%using (Ajax.BeginForm("AddStorage", "Team", new AjaxOptions() { OnBegin = "OnBeginStorageAdd", OnSuccess = "OnSuccessStorageAdd", OnFailure = "OnFailStorageAdd" })) %>
<%{%>
    <%: Html.HiddenFor(model => model.TeamId) %>
    
    <div class="modal_title">Добавление связанного места хранения<%: Html.Help("/Help/GetHelp_Team_Select_StorageSelectList") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 10px 5px">        
        <div id="messageStorageSelectList"></div>

        <%List<SelectListItem> storageList = new List<SelectListItem>();
          foreach (var value in Model.StorageList)
              storageList.Add(new SelectListItem() { Text = value.Name, Value = value.Id.ToString() });
        %>

        <table class="editor_table">
            <tr>
                <td class="row_title">
                    <%: Html.LabelFor(model => model.StorageList) %>:
                </td>
                <td>
                    <%: Html.DropDownList("StorageId", storageList, "", new { style = "width:210px" })%>
                </td>
            </tr>
        </table>        
    </div>

    <div class="button_set">
        <input id="btnSaveLinkedStorage" type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()"/>
    </div>
<%}%>
