<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.TaskExecutionEditViewModel>" %>

<script type="text/javascript">
    Task_TaskExecutionCreate.Init();

    function OnBeginTaskExecutionCreate(ajaxContext) {
        Task_TaskExecutionCreate.OnBeginTaskExecutionCreate(ajaxContext);
    }

    function OnSuccessTaskExecutionCreate(ajaxContext) {
        Task_TaskExecutionCreate.OnSuccessTaskExecutionCreate(ajaxContext);
    }

    function OnFailTaskExecutionCreate(ajaxContext) {
        Task_TaskExecutionCreate.OnFailTaskExecutionCreate(ajaxContext);
    }
</script>

<% using (Ajax.BeginForm("TaskExecutionSave", "Task", new AjaxOptions()
   {
       OnBegin = "OnBeginTaskExecutionCreate",
       OnSuccess = "OnSuccessTaskExecutionCreate",
       OnFailure = "OnFailTaskExecutionCreate"
   }))%>
<%{ %>
    <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">
            <%: Html.HiddenFor(model => model.ExecutionId)%>
            <%: Html.HiddenFor(model => model.TaskId)%>
            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="min-width: 110px">
                        <%: Html.LabelFor(model => model.Date)%>:
                    </td>
                    <td style="width:50%">
                        <%: Html.DatePickerFor(model => model.Date, null, !Model.AllowToChangeDate, !Model.AllowToChangeDate)%>
                        <%: Html.TimePickerFor(model => model.Time, new { size = 7, maxlength = 8 }, !Model.AllowToChangeDate, !Model.AllowToChangeDate)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                        <%: Html.ValidationMessageFor(model => model.Time)%>
                    </td>
                    <td class='row_title' style="min-width: 110px">
                        <%: Html.LabelFor(model => model.SpentTime_Hours)%>:
                    </td>
                    <td style="width:50%">
                        <%: Html.TextBoxFor(model => model.SpentTime_Hours, new { size = 4, maxlength = 4 })%> ч. <%: Html.TextBoxFor(model => model.SpentTime_Minutes, new { size = 2, maxlength = 2 })%> мин.
                        <%: Html.ValidationMessageFor(model => model.SpentTime_Hours)%>
                        <%: Html.ValidationMessageFor(model => model.SpentTime_Minutes)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.TaskExecutionStateId)%>:
                    </td>
                    <td>
                        <%:Html.DropDownListFor(model => model.TaskExecutionStateId, Model.TaskExecutionStateList)%>
                        <%: Html.ValidationMessageFor(model => model.TaskExecutionStateId)%>
                    </td>
                    <td class='row_title'>
                        <%: Html.LabelFor(model => model.CompletionPercentage)%>:
                    </td>
                    <td>
                        <%:Html.TextBoxFor(model => model.CompletionPercentage, new { size = 3, maxlength = 3 })%>%
                        <%:Html.ValidationMessageFor(model => model.CompletionPercentage)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.ResultDescription, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">
                        <%= Html.CommentFor(model => model.ResultDescription, rowsCount: 8, maxLength: 4000)%>
                        <%: Html.ValidationMessageFor(model => model.ResultDescription)%>
                    </td>
                </tr>
            </table>

            <div class="button_set">
                <input id="btnSaveTaskExecution" type="submit" value="Сохранить" />
                <input id="btnCancel" type="button" value="Отмена" />
            </div>
        </div>
    <%= Html.PageBoxBottom()%>
<%} %>

