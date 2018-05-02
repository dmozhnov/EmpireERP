<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageDocumentEditViewModel>" %>

<script type="text/javascript">
    function OnFailProductionOrderMaterialsPackageDocumentEdit(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderMaterialsPackageDocumentEdit");
    }
</script>

<% using (Ajax.BeginForm("ProductionOrderMaterialsPackageDocumentSave", "ProductionOrderMaterialsPackage", new AjaxOptions() { OnFailure = "OnFailProductionOrderMaterialsPackageDocumentEdit", OnSuccess = "OnSuccessProductionOrderMaterialsPackageDocumentEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.DocumentId)%>
    <%:Html.HiddenFor(model => model.PackageId)%>


<div style="width: 500px; padding: 0 15px">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrderMaterialsPackageDocumentEdit") %></div>
    <div class="h_delim"></div>
    <br />

    <div "messageProductionOrderMaterialsPackageDocumentEdit"></div>
    <table class="editor_table">
        <tr>
            <td class="row_title" style="width: 80px;">
                <%:Html.LabelFor(model => model.FileName)%>:
            </td>
            <td>
                 <span id="FileName"><%: Model.FileName %></span>
            </td>
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.Description)%>:
            </td>
            <td>
                <%:Html.TextBoxFor(model => model.Description, new { maxlength="250", style="width: 400px"}) %>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </td>
        </tr>
    </table>

    <div class="button_set">
        <input type="submit" value="Сохранить" />
        <input type="button" value="Закрыть" onclick="HideModal()"/>
    </div>
</div>
<%} %>
