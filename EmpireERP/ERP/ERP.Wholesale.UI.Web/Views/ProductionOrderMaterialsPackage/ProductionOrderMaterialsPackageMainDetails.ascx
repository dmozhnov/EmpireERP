<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ProductionOrderMaterialsPackage.ProductionOrderMaterialsPackageMainDetailsViewModel>" %>

<%: Html.HiddenFor(model => model.AllowToViewProductionOrder) %>

<table class="main_details_table">
    <tr>
        <td class="row_title" style="min-width: 150px;">
            <%: Html.HelpLabelFor(model => model.ProductionOrder, "/Help/GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_ProductionOrder")%>:
        </td>
        <td style="width: 75%;">
            <a id="ProductionOrder"><%: Model.ProductionOrder %></a>
            <%: Html.HiddenFor(model => model.ProductionOrderId) %>
        </td>
        <td class="row_title" style="min-width: 100px;">
            <%: Html.HelpLabelFor(model => model.CreationDate, "/Help/GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_CreationDate")%>:
        </td>
        <td style="width: 25%;">
            <%: Model.CreationDate %>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.Name) %>:
        </td>
        <td>
            <%: Model.Name %>
        </td>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.LastChangeDate, "/Help/GetHelp_ProductionOrderMaterialsPackage_Details_MainDetails_LastChangeDate")%>:
        </td>
        <td>
            <span id="LastChangeDate"><%: Model.LastChangeDate %></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.Description) %>:
        </td>
        <td>
            <%: Model.Description %>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.DocumentCount)%>:
        </td>
        <td>
            <span id="DocumentCount"><%: Model.DocumentCount%></span>
        </td>
    </tr>
    <tr>
        <td class="row_title">
        </td>
        <td>
        </td>
        <td class="row_title">
            <%: Html.LabelFor(model => model.PakageSize) %>:
        </td>
        <td>
            <span id="PakageSize"><%: Model.PakageSize %></span> Мб
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.LabelFor(model => model.Comment) %>:
        </td>
        <td colspan="3">
            <%:Html.CommentFor(model => model.Comment, true) %>
        </td>
    </tr>
</table>