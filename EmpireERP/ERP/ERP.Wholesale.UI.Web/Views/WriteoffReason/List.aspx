<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Основания для списания
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessWriteoffReasonSave() {
            WriteoffReason_List.OnSuccessWriteoffReasonSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("WriteoffReason", "Основания для списания", "", "/Help/GetHelp_WriteoffReason_List")%>
       
    <div id="messageWriteoffReasonList"></div>
    <% Html.RenderPartial("WriteoffReasonGrid", Model.Data); %>     
   
    <div id="writeoffReasonEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

