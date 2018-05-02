<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Ставки НДС
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessValueAddedTaxSave() {
            ValueAddedTax_List.OnSuccessValueAddedTaxSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("ValueAddedTax", "Ставки НДС", "", "/Help/GetHelp_ValueAddedTax_List")%>
       
    <div id="messageValueAddedTaxList"></div>
    <% Html.RenderPartial("ValueAddedTaxGrid", Model.Data); %>     
   
    <div id="valueAddedTaxEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

