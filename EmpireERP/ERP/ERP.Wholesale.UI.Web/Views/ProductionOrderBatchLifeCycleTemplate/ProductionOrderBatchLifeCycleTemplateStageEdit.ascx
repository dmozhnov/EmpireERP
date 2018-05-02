<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate.ProductionOrderBatchLifeCycleTemplateStageEditViewModel>" %>

<% using (Ajax.BeginForm("SaveStage", "ProductionOrderBatchLifeCycleTemplate", new AjaxOptions() { OnBegin = "OnBeginStageSave", OnFailure = "OnFailStageSave", OnSuccess = "OnSuccessStageSave" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.ProductionOrderBatchLifeCycleTemplateStageId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderBatchLifeCycleTemplateId)%>
    <%:Html.HiddenFor(model => model.Position)%>
    
    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrderBatchLifeCycleTemplate_Edit_ProductionOrderBatchLifeCycleTemplateStage") %></div>
    <div class="h_delim"></div>
    
    <div style="padding: 10px 10px 5px">    
        <div id="messageProductionOrderBatchLifeCycleTemplateStageEdit"></div>

        <table class="editor_table">
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Name)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.Name, new { maxlength = 200, size = 45 })%>
                    <%:Html.ValidationMessageFor(model => model.Name)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.Type)%>:</td>
                <td>
                    <%:Html.DropDownListFor(model => model.Type, Model.TypeList)%>
                    <%:Html.ValidationMessageFor(model => model.Type)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.PlannedDuration)%>:</td>
                <td>
                    <%:Html.TextBoxFor(model => model.PlannedDuration, new { maxlength = 5, size = 5 })%>&nbsp;дн.
                    <%:Html.ValidationMessageFor(model => model.PlannedDuration)%>
                </td>
            </tr>
            <tr>
                <td class="row_title"><%:Html.LabelFor(model => model.InWorkDays)%>:</td>
                <td>
                    <%:Html.YesNoToggleFor(x => x.InWorkDays)%>
                    <%:Html.ValidationMessageFor(model => model.InWorkDays)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <%= Html.SubmitButton("btnStageSave", "Сохранить")%>
            <input type="button" value="Закрыть" onclick="HideModal()" />     
        </div>
    </div>
<%} %>


