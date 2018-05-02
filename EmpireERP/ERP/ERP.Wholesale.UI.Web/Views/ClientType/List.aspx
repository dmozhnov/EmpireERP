<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Типы клиентов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessClientTypeSave() {
            ClientType_List.OnSuccessClientTypeSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("ClientType", "Типы клиентов", "", "/Help/GetHelp_ClientType_List")%>
       
    <div id="messageClientTypeList"></div>
    <% Html.RenderPartial("ClientTypeGrid", Model.Data); %>     
   
    <div id="clientTypeEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

