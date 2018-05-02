<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.TaskEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
         Task_Edit.Init();

         function OnBeginTaskSave() {
             StartButtonProgress($("#btnSaveTask"));
         }

         function OnFailTaskSave(ajaxContext) {
             Task_Edit.OnFailTaskSave(ajaxContext);
         }

         function OnSuccessTaskSave(ajaxContext) {
             Task_Edit.OnSuccessTaskSave(ajaxContext);
         }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("Task", Model.Title, "", "/Help/GetHelp_Task_Edit")%>

    <% using (Ajax.BeginForm("Save", "Task", new AjaxOptions() { OnBegin = "OnBeginTaskSave", OnSuccess = "OnSuccessTaskSave", OnFailure = "OnFailTaskSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%= Html.HiddenFor(model => model.Id) %>

        <%= Html.PageBoxTop(Model.Title)%>
            <div style="background: #fff; padding: 5px 0;">
                <div id="messageTaskEdit"></div>
        

                <div class="group_title">Параметры задачи</div>
                <div class="h_delim"></div>
                <br />

                <table class='editor_table'>
                    <tr>
                        <td class='row_title' style="min-width: 110px;">
                            <%: Html.HelpLabelFor(model => model.TaskPriorityId, "/Help/GetHelp_Task_Edit_TaskPriority")%>:
                        </td>
                        <td style="width: 50%; min-width: 150px">
                            <%: Html.DropDownListFor(model => model.TaskPriorityId, Model.TaskPriorityList, new { style = "min-width:110px;" })%>
                            <%: Html.ValidationMessageFor(model => model.TaskPriorityId)%>
                        </td>                        
                        <td class='row_title' style="min-width: 110px">
                            <%: Html.HelpLabelFor(model => model.CreatedBy, "/Help/GetHelp_Task_Edit_CreatedBy")%>:
                        </td>
                        <td style="width: 50%; min-width: 180px">
                            <%: Model.CreatedBy %>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.TaskTypeId, "/Help/GetHelp_Task_Edit_TaskType")%>:
                        </td>
                        <td>
                            <%: Html.DropDownListFor(model => model.TaskTypeId, Model.TaskTypeList, new { style = "min-width:110px;" })%>
                            <%: Html.ValidationMessageFor(model => model.TaskTypeId)%>
                        </td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ExecutedById, "/Help/GetHelp_Task_Edit_ExecuteBy")%>:
                        </td>
                        <td>
                            <span class="select_link" id="ExecutedBy"><%: Model.ExecutedByName %></span>
                            <%: Html.HiddenFor(model=>model.ExecutedById) %>
                            <%: Html.ValidationMessageFor(model => model.ExecutedById)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.TaskExecutionStateId, "/Help/GetHelp_Task_Edit_TaskExecutionState")%>:
                        </td>
                        <td>
                            <%: Html.DropDownListFor(model => model.TaskExecutionStateId, Model.TaskExecutionStateList, new { style = "min-width:110px;" })%>
                            <%: Html.ValidationMessageFor(model => model.TaskExecutionStateId)%>
                        </td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_Task_Edit_CreationDate")%>
                        </td>
                        <td>
                            <%: Model.CreationDate %>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'></td>
                        <td></td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.DeadLineDate, "/Help/GetHelp_Task_Edit_DeadLineDate")%>:
                        </td>
                        <td>
                            <%: Html.DatePickerFor(model => model.DeadLineDate) %>
                            <%: Html.TimePickerFor(model => model.DeadLineTime, new { size = 7, maxlength = 8 })%>
                            <%: Html.ValidationMessageFor(model => model.DeadLineDate) %>
                            <%: Html.ValidationMessageFor(model => model.DeadLineTime)%>
                        </td>
                    </tr>

                </table>

                <div class="group_title">Привязка задачи</div>
                <div class="h_delim"></div>
                <br />
                <table class='editor_table'>
                    <tr>
                        <td class='row_title' style="min-width: 110px">
                            <%: Html.HelpLabelFor(model => model.ContractorId, "/Help/GetHelp_Task_Edit_Contractor")%>:
                        </td>
                        <td style="width: 50%; min-width: 150px">
                            <span class="select_link" id="Contractor"><%: Model.ContractorName %></span>&nbsp;
                            <span class="main_details_action" id="ClearContractor"  <% if(Model.ContractorId == ""){ %>style="display:none;"<%} %>>[Сбросить]</span>
                            <%: Html.HiddenFor(model=>model.ContractorId) %>
                            <input type="hidden" id="ContractorType" value="<%: Model.ContractorType %>"/>
                        </td>
                        <td class='row_title' style="min-width: 110px">
                            <%: Html.HelpLabelFor(model => model.DealId, "/Help/GetHelp_Task_Edit_Deal")%>:
                        </td>
                        <td style="width: 50%; min-width: 200px">
                            <span <%if (Model.DealName != "---"){%>class="select_link"<%} %> id="Deal"><%: Model.DealName%></span>&nbsp;
                            <span class="main_details_action" <% if(Model.DealId == ""){ %>style="display: none;"<%} %> id="ClearDeal">[Сбросить]</span>
                            <%: Html.HiddenFor(model => model.DealId)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'>

                        </td>
                        <td>
                           
                        </td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProductionOrderId, "/Help/GetHelp_Task_Edit_ProductionOrder")%>:
                        </td>
                        <td>
                            <span <%if (Model.ProductionOrderName != "---"){%>class="select_link"<%} %> id="ProductionOrder"><%: Model.ProductionOrderName%></span>&nbsp;
                            <span class="main_details_action" id="ClearProductionOrder" <%if (Model.ProductionOrderId == ""){ %>style="display:none;"<%} %>>[Сбросить]</span>
                            <%: Html.HiddenFor(model => model.ProductionOrderId)%>
                        </td>
                    </tr>
                </table>

                <div class="group_title">Описание задачи</div>
                <div class="h_delim"></div>
                <br />
                <table class='editor_table'>
                    <tr>
                        <td class='row_title' style="min-width: 110px">
                           <%: Html.HelpLabelFor(model => model.Topic, "/Help/GetHelp_Task_Edit_Topic")%>:
                        </td>
                        <td style="width: 100%">
                           <%: Html.TextBoxFor(model => model.Topic, new { style = "width: 98%", maxlength = "200" })%>
                           <%: Html.ValidationMessageFor(model => model.Topic)%>
                        </td>
                    </tr>
                    <tr>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.Description, "/Help/GetHelp_Comment")%>:
                        </td>
                        <td>
                            <%: Html.CommentFor(model => model.Description, new { style = "width: 98%" }, rowsCount: 8, maxLength: 6000)%>                        
                            <%: Html.ValidationMessageFor(model => model.Description)%>
                        </td>
                    </tr>
                </table>
                
                <div class="button_set">
                    <input id="btnSaveTask" type="submit" value="Сохранить" />
                    <input type="button" id="btnBack" value="Назад" />
                </div>

            </div>
        <%= Html.PageBoxBottom()%>
    <%} %>

    <div id="executedBySelector"></div>
    <div id="contractorSelector"></div>
    <div id="dealSelector"></div>
    <div id="productionOrderSelector"></div>

</asp:Content>


