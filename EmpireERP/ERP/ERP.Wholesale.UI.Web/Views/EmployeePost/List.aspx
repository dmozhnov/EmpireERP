<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionaryListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Должности пользователей
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function OnSuccessEmployeePostSave() {
            EmployeePost_List.OnSuccessEmployeePostSave();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">   
    <%= Html.PageTitle("EmployeePost", "Должности пользователей", "", "/Help/GetHelp_EmployeePost_List")%>
       
    <div id="messageEmployeePostList"></div>
    <% Html.RenderPartial("EmployeePostGrid", Model.Data); %>     
   
    <div id="employeePostEdit"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>

