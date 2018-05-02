<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Торговые марки
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessTrademarkSave() {
            Trademark_List.OnSuccessTrademarkSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("Trademark", "Торговые марки", "", "/Help/GetHelp_Trademark_List")%>
       
    <div id="messageTrademarkList"></div>
    <% Html.RenderPartial("TrademarkGrid", Model.Data); %>     
   
    <div id="trademarkEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

