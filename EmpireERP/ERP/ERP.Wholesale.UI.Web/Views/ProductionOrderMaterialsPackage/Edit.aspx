<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProductionOrderMaterialsPackage_Edit.Init();

        function OnSuccessMaterialsPackageEdit(ajaxContext) {
            ProductionOrderMaterialsPackage_Edit.OnSuccessMaterialsPackageEdit(ajaxContext)
        }

        function OnFailMaterialsPackageEdit(ajaxContext) {
            ProductionOrderMaterialsPackage_Edit.OnFailMaterialsPackageEdit(ajaxContext)
        }

        function OnBeginMaterialsPackageEdit() {
            ProductionOrderMaterialsPackage_Edit.OnBeginMaterialsPackageEdit()
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Ajax.BeginForm("Save", "ProductionOrderMaterialsPackage", new AjaxOptions() { OnBegin = "OnBeginMaterialsPackageEdit", OnSuccess = "OnSuccessMaterialsPackageEdit", OnFailure = "OnFailMaterialsPackageEdit" })) %>
<%{ %>
    
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.HiddenFor(model => model.BackURL)%>

    <%= Html.PageTitle("ProductionOrderMaterialsPackage", Model.Title, Model.Name, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit")%>

    <%= Html.PageBoxTop(Model.Title)%>
    <div style="padding: 10px 10px 0 0;background: White;">
        
        <div id="messageMaterialsPackageEdit"></div>
        <table class="editor_table">
            <tr>
                <td class="row_title" style="min-width: 150px;">
                    <%: Html.HelpLabelFor(model => model.ProductionOrder, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_ProductionOrder")%>:
                </td>
                <td style="width: 50%">
                    <span <% if (Model.AllowToChangeProductionOrder){ %>class="select_link" id="ProductionOrder" <% } %>><%: Model.ProductionOrder%></span>
                    <%: Html.HiddenFor(model => model.ProductionOrderId)%>
                    <%: Html.ValidationMessageFor(model => model.ProductionOrderId)%>
                </td>
                <td class="row_title" style="min-width: 150px;">
                    <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_CreationDate")%>:
                </td>
                <td style="width: 50%">
                    <%: Model.CreationDate%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_Name")%>:
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.Name, new { maxlength = 250, style = "width: 340px;" })%>
                    <%: Html.ValidationMessageFor(model => model.Name)%>
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.LastChangeDate, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_LastChangeDate")%>:
                </td>
                <td>
                    <%: Model.LastChangeDate%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Description, "/Help/GetHelp_ProductionOrderMaterialsPackage_Edit_Description")%>:
                </td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.Description, new { maxlength = 250, style = "width: 100%" })%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                </td>
                <td colspan="3">
                    <%: Html.CommentFor(model => model.Comment, new { style = "width: 100%;" })%>
                    <%:Html.ValidationMessageFor(model => model.Comment)%>
                </td>
            </tr>
        </table>

        <div class="button_set">
            <input type="submit" value="Сохранить" id="btnMaterialsPackageSave" />
            <input type="button" id="btnBack" value="Назад" />
        </div>

    </div>

    <%=Html.PageBoxBottom()%>

<%} %>

<div id="selectProductionOrder"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
