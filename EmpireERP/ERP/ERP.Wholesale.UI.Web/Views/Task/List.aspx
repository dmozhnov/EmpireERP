<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.TaskListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Задачи и мероприятия
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Task_List.Init();
        
        $("#btnCreateNewTask").live("click", function () {
            window.location = "/Task/Create?" + GetBackUrl(true);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%=Html.PageTitle("Task", "Задачи и мероприятия", "", "/Help/GetHelp_Task_List")%>

    <div id="messageTaskList"></div>
    <%=Html.GridFilterHelper("filterTask", Model.Filter, new List<string>() { "gridNewTask", "gridExecutingTask", "gridCompletedTask" })%>

    <div id="messageNewTask"></div>

    <% Html.RenderPartial("NewTaskGrid", Model.NewTaskGrid); %>
    
    <% Html.RenderPartial("ExecutingTaskGrid", Model.ExecutingTaskGrid); %>

    <% Html.RenderPartial("CompletedTaskGrid", Model.CompletedTaskGrid); %>

    <div id='createdByFilterSelector'></div>
    <div id='executedByFilterSelector'></div>
    <div id='contractorFilterSelector'></div>
    <div id='dealFilterSelector'></div>
    <div id='productionOrderFilterSelector'></div>
</asp:Content>