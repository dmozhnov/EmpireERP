<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.UI.ViewModels.Grid.GridData>" %>

<%= Html.GridHeader("Секции места хранения", "gridStorageSection", "/Help/GetHelp_Storage_Details_StorageSectionGrid")%>
    <div class="grid_buttons">          
        <%: Html.Button("btnCreateStorageSection", "Новая секция", Model.ButtonPermissions["AllowToCreateSection"], Model.ButtonPermissions["AllowToCreateSection"])%>
    </div>
<%= Html.GridContent(Model, "/Storage/ShowStorageSectionGrid/")%>
