<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate.ProductionOrderBatchLifeCycleTemplateEditViewModel>" %>

<script type="text/javascript">
    function OnFailProductionOrderBatchLifeCycleTemplateEdit(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderBatchLifeCycleTemplateStageEdit");
    }

    function OnBeginProductionOrderBatchLifeCycleTemplateEdit() {
        StartButtonProgress($("#btnProductionOrderBatchLifeCycleTemplateSave"));
    }
</script>

<% using (Ajax.BeginForm("Save", "ProductionOrderBatchLifeCycleTemplate", new AjaxOptions() { OnBegin = "OnBeginProductionOrderBatchLifeCycleTemplateEdit", OnFailure = "OnFailProductionOrderBatchLifeCycleTemplateEdit", OnSuccess = "OnSuccessProductionOrderBatchLifeCycleTemplateEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.Id) %>

    <div class='modal_title'><%:Model.Title %><%: Html.Help("/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_Edit") %></div>
    <div class='h_delim'></div>

    <div style='padding: 10px 10px 5px'>
        <div id='messageProductionOrderBatchLifeCycleTemplateStageEdit'></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title'><%:Html.LabelFor(model => model.Name) %>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.Name, new { style = "width: 250px;", maxlength = 200 }) %>
                    <%:Html.ValidationMessageFor(model => model.Name) %>
                </td>
            </tr>
        </table>

        <div class='button_set'>
            <input type="submit" id="btnProductionOrderBatchLifeCycleTemplateSave" value="Сохранить" />
            <input type="button" value="Закрыть" onclick='HideModal();'/>
        </div>
    </div>
<%} %>