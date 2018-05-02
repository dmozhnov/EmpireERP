<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Task_CompletedTaskGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridCompletedTask", Model.HelpContentUrl)%>
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>