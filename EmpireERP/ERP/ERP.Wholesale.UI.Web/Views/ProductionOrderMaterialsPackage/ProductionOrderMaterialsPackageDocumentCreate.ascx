<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageDocumentEditViewModel>" %>

<script type="text/javascript" src="/Scripts/ajaxupload.3.5.js"></script>
<script type="text/javascript">
    ProductionOrderMaterialsPackage_MaterialsPackageDocumentCreate.Init();
</script>


<%:Html.HiddenFor(model => model.DocumentId)%>
<%:Html.HiddenFor(model => model.PackageId)%>


<div style="width: 520px; padding: 0 15px">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrderMaterialsPackageDocumentCreate") %></div>
    <div class="h_delim"></div>
    <br />

    <div "messageProductionOrderMaterialsPackageDocumentEdit"></div>
    <table class="editor_table">
        <tr>
            <td class="row_title" style="width: 80px;">
                <%:Html.LabelFor(model => model.FileName)%>:
            </td>
            <td>
                 <%:Html.TextBoxFor(model => model.FileName, new {@readonly = "True", style="width: 300px" }) %>
            </td>
            <td style="width: 100%">
                <span class="select_link" id="file_upload_button">Загрузить файл</span>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.Description)%>:
            </td>
            <td colspan="2">
                <%:Html.TextBoxFor(model => model.Description, new { maxlength="250", style="width: 400px"}) %>
            </td>
        </tr>
    </table>

    <div class="button_set">
        <input type="button" id="btnSave" value="Сохранить" class="disabled" disabled="disabled" />
        <input type="button" id="btnClose" value="Закрыть" onclick="HideModal()"/>
    </div>
</div>
    