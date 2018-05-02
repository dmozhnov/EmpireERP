<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Организационно-правовые формы
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessLegalFormSave() {
            LegalForm_List.OnSuccessLegalFormSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("LegalForm", "Организационно-правовые формы", "", "/Help/GetHelp_LegalForm_List")%>
       
    <div id="messageLegalFormList"></div>
    <% Html.RenderPartial("LegalFormGrid", Model.Data); %>     
   
    <div id="legalFormEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

