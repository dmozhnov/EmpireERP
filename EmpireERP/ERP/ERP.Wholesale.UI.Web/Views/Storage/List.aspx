<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Storage.StorageListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Места хранения
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Storage_List.Init();

        function OnSuccessStorageSave() {
            Storage_List.OnSuccessStorageSave();
        }
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <%= Html.PageTitle("Storage", "Места хранения", "", "/Help/GetHelp_Storage_List")%>
        
    <div id="messageStorageList"></div>

    <% Html.RenderPartial("StorageGrid", Model.GridData); %>
        
    <div id="storageEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
