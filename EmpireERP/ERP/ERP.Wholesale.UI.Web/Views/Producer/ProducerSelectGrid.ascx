<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Producer_SelectGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridProducerSelect") %>
<%= Html.GridContent(Model, "/Producer/ShowProducerSelectGrid/")%>
