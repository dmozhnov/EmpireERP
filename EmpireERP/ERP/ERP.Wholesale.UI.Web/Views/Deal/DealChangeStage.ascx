<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Deal.DealChangeStageViewModel>" %>

<script type="text/javascript">
    Deal_ChangeStage.Init();
</script>

<div style="width: 450px; padding: 0 10px 0;">
    <%:Html.HiddenFor(model => model.DealId)%>
    <%:Html.HiddenFor(model => model.CurrentStageId)%>

    <div class="modal_title"><%:Model.Title%></div>
    <br />

    <div id="messageDealStageChange"></div>

    <table class="display_table">
        <tr>
            <td colspan="2">
                <div class="group_title bold">Текущий этап сделки</div>
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
        </tr>
        <tr>
            <td class="row_title">
                <%:Html.LabelFor(model => model.CurrentStageStartDate)%>:
            </td>
            <td>
                <%:Model.CurrentStageStartDate%> &nbsp;||&nbsp; <%:Model.CurrentStageDuration%>&nbsp;дн.
            </td>
        </tr>
        <tr>
            <td colspan="2">
            <div class="button_set">
                <%:Html.Button("btnMoveToPreviousStage", "Вернуть предыдущий", Model.AllowToMoveToPreviousStage, true, classString: "orange")%>
                <%:Html.Button("btnMoveToNextStage", "Перейти на следующий", Model.AllowToMoveToNextStage, true, classString: "green")%>
            </div>
            <br />

            <div class="group_title greentext bold">Следующий этап сделки</div>
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
        </tr>
        <tr>
            <td colspan="2">
            <br />

            <div class="group_title orangetext bold">Предыдущий этап сделки</div>
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
        </tr>
    </table>

    <div class="button_set">
        <%:Html.Button("btnMoveToUnsuccessfulClosingStage", "На «" + Model.UnsuccessfulClosingStageName + "»", Model.AllowToMoveToUnsuccessfulClosingStage, true, classString: "orange")%>
        <%:Html.Button("btnMoveToDecisionMakerSearchStage", "На «" + Model.DecisionMakerSearchStageName + "»", Model.AllowToMoveToDecisionMakerSearchStage, true, classString: "orange")%>
    </div>
    
    <br />

    <div class="button_set">
        <input type="button" value="Закрыть форму" onclick="HideModal()" />
    </div>

</div>
