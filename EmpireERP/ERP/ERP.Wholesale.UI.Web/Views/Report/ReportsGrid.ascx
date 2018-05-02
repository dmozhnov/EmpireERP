<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Report_ReportsGrid.Init();
</script>

<%= Html.GridHeader("Отчетные формы", "gridReports")%>    
<%= Html.GridContent(Model, Model.GridPartialViewAction)%>