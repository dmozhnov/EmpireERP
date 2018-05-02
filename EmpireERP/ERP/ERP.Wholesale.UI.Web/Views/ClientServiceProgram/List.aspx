<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Программы обслуживания клиентов
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessClientServiceProgramSave() {
            ClientServiceProgram_List.OnSuccessClientServiceProgramSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("ClientServiceProgram", "Программы обслуживания клиентов", "", "/Help/GetHelp_ClientServiceProgram_List"
)%>
       
    <div id="messageClientServiceProgramList"></div>
    <% Html.RenderPartial("ClientServiceProgramGrid", Model.Data); %>     
   
    <div id="clientServiceProgramEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

