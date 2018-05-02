<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Основания для возврата товара от клиента
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessReturnFromClientReasonSave() {
            ReturnFromClientReason_List.OnSuccessReturnFromClientReasonSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("ReturnFromClientReason", "Основания для возврата товара от клиента", "", "/Help/GetHelp_ReturnFromClientReason_List")%>
       
    <div id="messageReturnFromClientReasonList"></div>
    <% Html.RenderPartial("ReturnFromClientReasonGrid", Model.Data); %>     
   
    <div id="returnFromClientReasonEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

