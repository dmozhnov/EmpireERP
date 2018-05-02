<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchChangeStageViewModel>" %>

<script type="text/javascript">
    //ProductionOrder_ProductionOrderBatchChangeStage.Init();
</script>

<div style="width: 600px; padding: 0 10px 0;">
    <%:Html.HiddenFor(model => model.ProductionOrderId)%>
    <%:Html.HiddenFor(model => model.ProductionOrderBatchId)%>
    <%:Html.HiddenFor(model => model.CurrentStageId)%>

    <div class="modal_title"><%:Model.Title%><%: Html.Help("/Help/GetHelp_ProductionOrder_Edit_ProductionOrderBatchChangeStage") %></div>
    <br />

    <div id="messageProductionOrderBatchStageChange"></div>

    <table class="display_table">
    <tr>
        <td colspan="4">
            <div class="group_title bold">Текущий этап</div>
            <br />
        </td>
    </tr>
    <tr>
        <td class="row_title" style="width: 100px">
            <%:Html.LabelFor(model => model.CurrentStageName)%>:
        </td>
        <td>
            <span class="bold"><%:Model.CurrentStageName%></span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CurrentStageTypeName)%>:
        </td>
        <td>
            <%:Model.CurrentStageTypeName%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CurrentStagePlannedDuration)%>:
        </td>
        <td>
            <%:Model.CurrentStagePlannedDuration%>&nbsp;дн.&nbsp;&nbsp;||&nbsp;&nbsp;до&nbsp;<%:Model.CurrentStagePlannedEndDate%>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CurrentStageActualDuration)%>:
        </td>
        <td>
            <%:Model.CurrentStageActualDuration%>&nbsp;дн.
        </td>
    </tr>
    <tr>
        <td colspan="4">
        <div class="button_set">
            <%=Html.Button("btnMoveToPreviousStage", "Вернуть предыдущий", Model.AllowToMoveToPreviousStage, true, classString: "orange")%>
            <%=Html.Button("btnMoveToNextStage", "Перейти на следующий", Model.AllowToMoveToNextStage, true, classString: "green")%>
        </div>
        <br />

        <div class="group_title greentext bold">Следующий этап</div>
        <br />
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.NextStageName)%>:
        </td>
        <td>
            <span class="greentext bold"><%:Model.NextStageName%></span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.NextStageTypeName)%>:
        </td>
        <td>
            <%:Model.NextStageTypeName%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.NextStagePlannedDuration)%>:
        </td>
        <td>
            <%:Model.NextStagePlannedDuration%>&nbsp;дн.&nbsp;&nbsp;||&nbsp;&nbsp;до&nbsp;<%:Model.NextStagePlannedEndDate%>
        </td>
        <td class="row_title">
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td colspan="4">
        <br />

        <div class="group_title orangetext bold">Предыдущий этап</div>
        <br />
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PreviousStageName)%>:
        </td>
        <td>
            <span class="orangetext bold"><%:Model.PreviousStageName%></span>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PreviousStageTypeName)%>:
        </td>
        <td>
            <%:Model.PreviousStageTypeName%>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PreviousStagePlannedDuration)%>:
        </td>
        <td>
            <%:Model.PreviousStagePlannedDuration%>&nbsp;дн.&nbsp;&nbsp;||&nbsp;&nbsp;до&nbsp;<%:Model.PreviousStagePlannedEndDate%>
        </td>
        <td class="row_title">
            <%:Html.LabelFor(model => model.PreviousStageActualDuration)%>:
        </td>
        <td>
            <%:Model.PreviousStageActualDuration%>&nbsp;дн.&nbsp;&nbsp;||&nbsp;&nbsp;до&nbsp;<%:Model.PreviousStageActualEndDate%>
        </td>
    </tr>
    </table>
    <br />

    <div class="button_set">
        <%:Html.Button("btnMoveToUnsuccessfulClosingStage", "Перейти в «" + Model.UnsuccessfulClosingStageName + "»", Model.AllowToMoveToUnsuccessfulClosingStage, true, classString: "orange")%>
        <input type="button" value="Закрыть форму" onclick="HideModal()" />
    </div>
    <br />

</div>
