<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.AccountingPriceList.AccountingPriceListAddStorageViewModel>" %>

<script type="text/javascript">
    AccountingPriceList_AddStorage.Init();

    function OnBeginStorageAdd() {
        StartButtonProgress($("#btnSaveStorage"));
    }

    function OnSuccessStorageAdd(ajaxContext) {
        AccountingPriceList_AddStorage.OnSuccessStorageAdd(ajaxContext);
    }

    function OnFailStorageAdd(ajaxContext) {
        AccountingPriceList_AddStorage.OnFailStorageAdd(ajaxContext);
    }

</script>

<%using (Ajax.BeginForm("StoragesList", "AccountingPriceList", new AjaxOptions() { OnBegin = "OnBeginStorageAdd",
      OnSuccess = "OnSuccessStorageAdd", OnFailure = "OnFailStorageAdd" })) %>
<%{%>
    <%: Html.HiddenFor(model => model.AccountingPriceListId) %>

    <div class="modal_title">Добавление места хранения</div>
    <div class="h_delim"></div>
        
    <div style="padding: 10px 10px 5px">
        <div id="messageAccountingPriceListStorageForm"></div>
        
        <table class='editor_table'>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.StorageList) %>:</td>
                <td>
                    <%: Html.DropDownListFor(x => x.StorageId, Model.StorageList, new { style = "width:210px" })%>
                    <%: Html.HiddenFor(model => model.AccountingPriceListId) %>
                </td>
            </tr>
        </table>
    </div>

    <div class="button_set">        
        <input id="btnSaveStorage" type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%}%>
