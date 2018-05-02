<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.TaskDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Task_Details.Init();
        Task_Executions.Init();
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model=>model.BackURL) %>
    <%= Html.PageTitle("Task", Model.Title, "№" + Model.MainDetails.Id.ToString(), "/Help/GetHelp_Task_MainDetails")%>
    
    <div class="button_set">
        <%: Html.Button("btnEditTask", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteTask", "Удалить", Model.AllowToDelete, Model.AllowToDelete) %>        
        <input id="btnBackTo" type="button" value="Назад" />
    </div>
    
    <div id="messageTaskDetails"></div>
    <% Html.RenderPartial("MainDetails", Model.MainDetails); %>

    <div id="messageTaskExecutionCreateButton"></div>
    <div class="button_set">
        <%: Html.Button("btnCreateTaskExecution", "Добавить исполнение", Model.AllowToCreateTaskExecutuion, Model.AllowToCreateTaskExecutuion) %>
        <%: Html.Button("btnCompleteTask", "Завершить задачу", Model.AllowToCompleteTask, Model.AllowToCompleteTask) %>
    </div>

    <div id="messageTaskExecutionDetailsEdit"></div>
    <div id="taskExecutionCreate"></div>
    
    <%= Html.PageBoxTop("") %>
        <div id="tabPanel_menu_container">
            <div id="taskExecutionTab" class="tabPanel_menu_item  selected">
                <span>Исполнение</span>
            </div>
            <div id="taskHistoryTab" class="tabPanel_menu_item">
                <span>История</span>
            </div>
        </div>
                
        <div style="background: #fff; padding: 5px 0; border: 1px solid #ddd;">
            <div id="taskDetailsContainer">
               <% Html.RenderPartial("TaskExecutions", Model.TaskExecutions); %>
            </div>
        </div>
    
    <%= Html.PageBoxBottom() %>

</asp:Content>