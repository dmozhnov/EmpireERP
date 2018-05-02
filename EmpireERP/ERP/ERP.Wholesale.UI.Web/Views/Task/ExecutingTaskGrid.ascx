<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Task_ExecutingTaskGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridExecutingTask", Model.HelpContentUrl)%>
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>