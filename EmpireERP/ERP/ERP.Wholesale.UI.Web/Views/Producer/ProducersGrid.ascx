<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Producer_ProducersGrid.Init();
</script>

<%= Html.GridHeader("Производители", "gridProducers", "/Help/GetHelp_Producer_List_ProducerGrid")%>
    <div class="grid_buttons">
        <%: Html.Button("btnCreateProducer", "Новый производитель", Model.ButtonPermissions["AllowToCreateProducer"], Model.ButtonPermissions["AllowToCreateProducer"])%>
    </div>
<%= Html.GridContent(Model, "/Producer/ShowProducersGrid/")%>