<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Storage.StorageDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Детали места хранения
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Storage_Details.Init();

        function OnSuccessStorageSave(result) {
            Storage_Details.OnSuccessStorageSave(result);
        }

        function OnSuccessStorageAccountOrganizationAdd(result) {
            Storage_Details.OnSuccessStorageAccountOrganizationAdd(result);
        }

        function OnSuccessStorageSectionSave(result) {
            Storage_Details.OnSuccessStorageSectionSave(result);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <input id="storage_id" type="hidden" value="<%: Model.MainDetails.Id %>" />
    <%: Html.HiddenFor(model => model.BackURL) %>
        
    <%= Html.PageTitle("Storage", "Детали места хранения", Model.MainDetails.Name, "/Help/GetHelp_Storage_Details")%>

    <div class="button_set">       
        <%: Html.Button("btnEditStorage", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteStorage", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>        
        <%: Html.Button("btnCreatePriceListByDefault", "Создать реестр цен", Model.AllowToCreateAccountingPriceList, Model.AllowToCreateAccountingPriceList)%>        
        <input id="btnBackToList" type="button" value="Назад" />
    </div>       
        
    <div id="messageStorageDetails"></div>
    <% Html.RenderPartial("StorageMainDetails", Model.MainDetails); %>
    
    <br />
    
    <div id="messageAccountOrganizationAdd"></div>
    <% Html.RenderPartial("StorageAccountOrganizationGrid", Model.AccountOrganizationsGrid); %>
       
    <div id="messageStorageSectionList"></div>
    <% Html.RenderPartial("StorageSectionGrid", Model.SectionsGrid); %>

    <div id="accountOrganizationSelectList"></div>
    <div id="storageSectionEdit"></div>
    <div id="storageEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
