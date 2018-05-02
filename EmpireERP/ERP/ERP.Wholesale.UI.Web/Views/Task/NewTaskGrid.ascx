<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<script type="text/javascript">
    Task_NewTaskGrid.Init();
</script>

<%= Html.GridHeader(Model.Title, "gridNewTask", Model.HelpContentUrl)%>

    <div class="grid_buttons">
        <%: Html.Button("btnCreateNewTask", "Новая задача", Model.ButtonPermissions["AllowToCreateNewTask"], Model.ButtonPermissions["AllowToCreateNewTask"])%>
    </div>

<%= Html.GridContent( Model, Model.GridPartialViewAction) %>