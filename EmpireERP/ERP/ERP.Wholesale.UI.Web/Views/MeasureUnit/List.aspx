<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.MeasureUnit.MeasureUnitListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Единицы измерения
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessMeasureUnitSave() {
            MeasureUnit_List.OnSuccessMeasureUnitSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("MeasureUnit", "Единицы измерения", "", "/Help/GetHelp_MeasureUnit_List")%>
       
    <div id="messageMeasureUnitList"></div>
    <% Html.RenderPartial("MeasureUnitGrid", Model.Data); %>     
   
    <div id="measureUnitEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

